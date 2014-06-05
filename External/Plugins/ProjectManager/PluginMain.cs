using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using ProjectManager.Actions;
using ProjectManager.Controls;
using ProjectManager.Controls.TreeView;
using WeifenLuo.WinFormsUI.Docking;
using WeifenLuo.WinFormsUI;
using ProjectManager.Helpers;
using ProjectManager.Projects;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore;
using PluginCore.Bridge;

namespace ProjectManager
{
    public static class ProjectManagerCommands
    {
        public const string NewProject = "ProjectManager.NewProject";
        public const string OpenProject = "ProjectManager.OpenProject";
        public const string SendProject = "ProjectManager.SendProject";
        public const string DebugProject = "ProjectManager.DebugProject";
        public const string LineEntryDialog = "ProjectManager.LineEntryDialog";
    }

    public static class ProjectManagerEvents
    {
        public const string Menu = "ProjectManager.Menu";
        public const string ToolBar = "ProjectManager.ToolBar";
        public const string Project = "ProjectManager.Project";
        public const string DebugProject = "ProjectManager.DebugProject";
        public const string RunCustomCommand = "ProjectManager.RunCustomCommand";
        public const string FileMapping = "ProjectManager.FileMapping";
        public const string TreeSelectionChanged = "ProjectManager.TreeSelectionChanged";
        public const string OpenVirtualFile = "ProjectManager.OpenVirtualFile";
        public const string CreateProject = "ProjectManager.CreateProject";
        public const string ProjectCreated = "ProjectManager.ProjectCreated";
        public const string FileMoved = "ProjectManager.FileMoved";
        public const string FilePasted = "ProjectManager.FilePasted";
        public const string UserRefreshTree = "ProjectManager.UserRefreshTree";
        public const string BeforeSave = "ProjectManager.BeforeSave";
    }

	public class PluginMain : IPlugin
	{
        const string pluginName = "ProjectManager";
        const string pluginAuth = "LuaDevelop Team";
        const string pluginGuid = "30018864-fadd-1122-b2a5-779832cbbf23";
        const string pluginHelp = "www.LuaDevelop.org/community/";
        private string pluginDesc = "Adds project management and building to LuaDevelop.";

        private FDMenus menus;
        private FileActions fileActions;
        private ProjectActions projectActions;
        private LuaDevelopActions LuaDevelopActions;
        private Queue<String> openFileQueue;
        private DockContent pluginPanel;
        private PluginUI pluginUI;
        private Image pluginImage;
        private Project activeProject;
        private OpenResourceForm projectResources;
        private bool listenToPathChange;

        private ProjectTreeView Tree { get { return pluginUI.Tree; } }
        public static IMainForm MainForm { get { return PluginBase.MainForm; } }
        public static ProjectManagerSettings Settings;

        const EventType eventMask = EventType.UIStarted | EventType.FileOpening
            | EventType.FileOpen | EventType.FileSave | EventType.FileSwitch | EventType.ProcessStart | EventType.ProcessEnd
            | EventType.ProcessArgs | EventType.Command | EventType.Keys | EventType.ApplySettings;

        #region Load/Save Settings

        static string SettingsDir { get { return Path.Combine(PathHelper.DataDir, pluginName); } }
        static string SettingsPath { get { return Path.Combine(SettingsDir, "Settings.ldb"); } }
        static string FDBuildHints { get { return Path.Combine(SettingsDir, "FDBuildHints.txt"); } }

        public void LoadSettings()
        {
            Settings = new ProjectManagerSettings();
            if (!Directory.Exists(SettingsDir)) Directory.CreateDirectory(SettingsDir);
            if (!File.Exists(SettingsPath)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(SettingsPath, Settings);
                Settings = (ProjectManagerSettings)obj;
                PatchSettings();
            }
            // set manually to avoid dependency in FDBuild
            FileInspector.ExecutableFileTypes = Settings.ExecutableFileTypes;
            Settings.Changed += SettingChanged;
        }

        private void PatchSettings()
        {
            // remove 'obj' from the excluded directory names - now /obj a hidden directory
            if (Settings.ExcludedDirectories.Length > 0 && Settings.ExcludedDirectories[0] == "obj")
            {
                List<String> ex = new List<string>(Settings.ExcludedDirectories);
                ex.RemoveAt(0);
                Settings.ExcludedDirectories = ex.ToArray();
                this.SaveSettings();
            }
            // add new filtered types if user has old settings
            if (Array.IndexOf<string>(Settings.FilteredDirectoryNames, "git") < 0)
            {
                List<String> fdn = new List<string>(Settings.FilteredDirectoryNames);
                fdn.Add("git");
                fdn.Add("hg");
                Settings.FilteredDirectoryNames = fdn.ToArray();
                this.SaveSettings();
            }
        }

        public void SaveSettings()
        {
            Settings.Changed -= SettingChanged;
            ObjectSerializer.Serialize(SettingsPath, Settings);
        }

        #endregion

        #region Plugin MetaData

        public Int32 Api { get { return 1; } }
        public string Name { get { return pluginName; } }
        public string Guid { get { return pluginGuid; } }
        public string Author { get { return pluginAuth; } }
        public string Description { get { return pluginDesc; } }
        public string Help { get { return pluginHelp; } }
        [Browsable(false)] // explicit implementation so we can reuse the "Settings" var name
        object IPlugin.Settings { get { return Settings; } }
		
		#endregion
		
		#region Initialize/Dispose
		
		public void Initialize()
		{
            LoadSettings();
            pluginImage = MainForm.FindImage("100");
            pluginDesc = TextHelper.GetString("Info.Description");
            openFileQueue = new Queue<String>();

            Icons.Initialize(MainForm);
            EventManager.AddEventHandler(this, eventMask);

            #region Actions and Event Listeners

            menus = new FDMenus(MainForm);
            menus.ProjectMenu.ProjectItemsEnabled = false;
            menus.View.Click += delegate { OpenPanel(); };
            menus.DebugProject.Click += delegate { DebugProject(); };

            menus.ProjectMenu.NewProject.Click += delegate { NewProject(); };
            menus.ProjectMenu.OpenProject.Click += delegate { OpenProject(); };
            menus.ProjectMenu.CloseProject.Click += delegate { CloseProject(false); };
            menus.ProjectMenu.OpenResource.Click += delegate { OpenResource(); };
            menus.ProjectMenu.DebugProject.Click += delegate { DebugProject(); };
            menus.ProjectMenu.Properties.Click += delegate { OpenProjectProperties(); };
            menus.RecentProjects.ProjectSelected += delegate(string projectPath) { OpenProjectSilent(projectPath); };

            LuaDevelopActions = new LuaDevelopActions(MainForm);

            fileActions = new FileActions(MainForm,LuaDevelopActions);
            fileActions.OpenFile += OpenFile;
            fileActions.FileDeleted += FileDeleted;
            fileActions.FileMoved += FileMoved;
            fileActions.FileCopied += FilePasted;

            projectActions = new ProjectActions(pluginUI);

            pluginUI = new PluginUI(this, menus, fileActions, projectActions);
            pluginUI.NewProject += delegate { NewProject(); };
            pluginUI.OpenProject += delegate { OpenProject(); };
            pluginUI.Rename += fileActions.Rename;
            pluginUI.TreeBar.ShowHidden.Click += delegate { ToggleShowHidden(); };
            pluginUI.TreeBar.Synchronize.Click += delegate { TreeSyncToCurrentFile(); };
            pluginUI.TreeBar.ProjectProperties.Click += delegate { OpenProjectProperties(); };
            pluginUI.TreeBar.RefreshSelected.Click += delegate { TreeRefreshSelectedNode(); };

            pluginUI.Menu.Open.Click += delegate { TreeOpenItems(); };
            pluginUI.Menu.Execute.Click += delegate { TreeExecuteItems(); };
            pluginUI.Menu.Browse.Click += delegate { TreeBrowseItem(); };
            pluginUI.Menu.Cut.Click += delegate { TreeCutItems(); };
            pluginUI.Menu.Copy.Click += delegate { TreeCopyItems(); };
            pluginUI.Menu.Paste.Click += delegate { TreePasteItems(); };
            pluginUI.Menu.Delete.Click += delegate { TreeDeleteItems(); };
            pluginUI.Menu.HideItem.Click += delegate { TreeHideItems(); };
            pluginUI.Menu.ShowHidden.Click += delegate { ToggleShowHidden(); };
            pluginUI.Menu.AddFileFromTemplate += TreeAddFileFromTemplate;
            pluginUI.Menu.AddNewFolder.Click += delegate { TreeAddFolder(); };
            pluginUI.Menu.AddExistingFile.Click += delegate { TreeAddExistingFile(); };
            pluginUI.Menu.DebugProject.Click += delegate { DebugProject(); };
            pluginUI.Menu.CloseProject.Click += delegate { CloseProject(false); };
            pluginUI.Menu.Properties.Click += delegate { OpenProjectProperties(); };
            pluginUI.Menu.ShellMenu.Click += delegate { TreeShowShellMenu(); };
            pluginUI.Menu.CommandPrompt.Click += delegate { TreeShowCommandPrompt(); };
            pluginUI.Menu.FindAndReplace.Click += delegate { FindAndReplace(); };
            pluginUI.Menu.FindInFiles.Click += delegate { FindInFiles(); };
            pluginUI.Menu.Opening += new CancelEventHandler(this.MenuOpening);

            Tree.MovePath += fileActions.Move;
            Tree.CopyPath += fileActions.Copy;
            Tree.DoubleClick += delegate { TreeDoubleClick(); };

            #endregion

            pluginPanel = MainForm.CreateDockablePanel(pluginUI, Guid, Icons.Project.Img, DockState.DockRight);
        }

        void TargetBuildSelector_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // leave target build input field to apply
                PluginBase.MainForm.CurrentDocument.Activate();
        }
		
		public void Dispose()
		{
            // we have to fiddle this a little since we only get once change to save our settings!
            // (further saves will be ignored by FD design)
            Project project = activeProject; 
            string lastProject = (project != null) ? project.ProjectPath : "";
            CloseProject(true);
            Settings.LastProject = lastProject;
            SaveSettings();
		}
		
        #endregion

        #region Plugin Events

        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
		{
            TextEvent te = e as TextEvent;
            DataEvent de = e as DataEvent;
            switch (e.Type)
            {
                case EventType.UIStarted:
                    // for some reason we have to do this on the next message loop for the tree
                    // state to be restored properly.
                    pluginUI.BeginInvoke((MethodInvoker)delegate 
                    { 
                        BroadcastMenuInfo(); 
                        BroadcastToolBarInfo(); 
                        OpenLastProject(); 
                    });
                    break;

                case EventType.FileOpening:
                    // if this is a project file, we can handle it ourselves
                    if (FileInspector.IsProject(te.Value))
                    {
                        te.Handled = true;
                        OpenProjectSilent(te.Value);
                    }
                    break;

                case EventType.FileOpen:
                    SetDocumentIcon(MainForm.CurrentDocument);
                    OpenNextFile(); // it's safe to open any other files on the queue
                    break;

                case EventType.FileSave:
                    TabColors.UpdateTabColors(Settings);
                    break;

                case EventType.FileSwitch:
                    TabColors.UpdateTabColors(Settings);
                    break;

                case EventType.ProcessStart:
                    break;

                case EventType.ProcessEnd:
                    break;

                case EventType.ApplySettings:
                    TabColors.UpdateTabColors(Settings);
                    break;

                case EventType.Command:
                    if (de.Action.StartsWith("ProjectManager."))
                    if (de.Action == ProjectManagerCommands.NewProject)
                    {
                        NewProject();
                        e.Handled = true;
                    }
                    else if (de.Action == ProjectManagerCommands.OpenProject)
                    {
                        if (de.Data != null && File.Exists((string)de.Data))
                        {
                            OpenProjectSilent((string)de.Data);
                        }
                        else OpenProject();
                        e.Handled = true;
                    }
                    else if (de.Action == ProjectManagerCommands.SendProject)
                    {
                        BroadcastProjectInfo(activeProject);
                        e.Handled = true;
                    }
                    else if (de.Action == ProjectManagerCommands.LineEntryDialog)
                    {
                        Hashtable info = (Hashtable)de.Data;
                        LineEntryDialog askName = new LineEntryDialog((string)info["title"], (string)info["label"], (string)info["suggestion"]);
                        DialogResult choice = askName.ShowDialog();
                        if (choice == DialogResult.OK && askName.Line.Trim().Length > 0 && askName.Line.Trim() != (string)info["suggestion"])
                        {
                            info["suggestion"] = askName.Line.Trim();
                        }
                        if (choice == DialogResult.OK)
                        {
                            e.Handled = true;
                        }
                    }
                    break;

                case EventType.Keys:
                    e.Handled = HandleKeyEvent(e as KeyEvent);
                    break;
            }
        }

        private bool HandleKeyEvent(KeyEvent ke)
        {
            if (activeProject == null) return false;
            // Handle tree-level simple shortcuts like copy/paste/del
            else if (Tree.Focused && !pluginUI.IsEditingLabel && ke != null)
            {
                if (ke.Value == (Keys.Control | Keys.C) && pluginUI.Menu.Contains(pluginUI.Menu.Copy)) TreeCopyItems();
                else if (ke.Value == (Keys.Control | Keys.X) && pluginUI.Menu.Contains(pluginUI.Menu.Cut)) TreeCutItems();
                else if (ke.Value == (Keys.Control | Keys.V) && pluginUI.Menu.Contains(pluginUI.Menu.Paste)) TreePasteItems();
                else if (ke.Value == Keys.Delete && pluginUI.Menu.Contains(pluginUI.Menu.Delete)) TreeDeleteItems();
                else if (ke.Value == Keys.Enter && pluginUI.Menu.Contains(pluginUI.Menu.Open)) TreeOpenItems();
                else return false;
            }
            else return false;
            return true;
        }
		
		#endregion

        #region Custom Methods

        bool RestoreProjectSession(Project project)
        {
            if (project == null || !Settings.UseProjectSessions) return false;
            String hash = HashCalculator.CalculateSHA1(project.ProjectPath.ToLower());
            String sessionDir = Path.Combine(SettingsDir, "Sessions");
            String sessionFile = Path.Combine(sessionDir, hash + ".ldb");
            if (File.Exists(sessionFile))
            {
                PluginBase.MainForm.CallCommand("RestoreSession", sessionFile);
                return true;
            }
            return false;
        }

        void SaveProjectSession()
        {
            Project project = Tree.Projects.Count > 0 ? Tree.Projects[0] : null; // TODO we need a main project/solution

            if (project == null || !Settings.UseProjectSessions) return;
            String hash = HashCalculator.CalculateSHA1(project.ProjectPath.ToLower());
            String sessionDir = Path.Combine(SettingsDir, "Sessions");
            String sessionFile = Path.Combine(sessionDir, hash + ".ldb");
            if (!Directory.Exists(sessionDir)) Directory.CreateDirectory(sessionDir);
            PluginBase.MainForm.CallCommand("SaveSession", sessionFile);
        }

        void SetProject(Project project, Boolean stealFocus, Boolean internalOpening)
        {
            if (Tree.Projects.Contains(project)) return;
            if (activeProject != null) CloseProject(true);

            // configure
            var prefs = PluginMain.Settings.GetPrefs(project);
            project.UpdateVars(true);

            SetActiveProject(project);

            // events
            project.BeforeSave += new BeforeSaveHandler(ProjectBeforeSave);
            listenToPathChange = true;

            // activate
            if (!internalOpening) RestoreProjectSession(project);

            if (stealFocus)
            {
                OpenPanel();
                pluginUI.Focus();
            }
            TabColors.UpdateTabColors(Settings);
        }

        private void SetActiveProject(Project project)
        {
            activeProject = project;

            // init
            Environment.CurrentDirectory = project.Directory;
            Settings.LastProject = project.ProjectPath;
            Settings.Language = project.Language;

            // notify
            PluginBase.CurrentProject = project;
            PluginBase.MainForm.RefreshUI();

            BroadcastProjectInfo(project);

            // ui
            pluginUI.SetProject(project);
            menus.SetProject(project); // TODO this should reflect the "solution"
            pluginUI.NotifyIssues();
        }

        void SetProject(Project project, Boolean stealFocus)
        {
            SetProject(project, stealFocus, false);
        }
        void SetProject(Project project)
        {
            SetProject(project, true, false);
        }

        void CloseProject(bool internalClosing)
        {
            Project project = Tree.Projects.Count > 0 ? Tree.Projects[0] : null; // TODO we need a main project/solution
            if (project == null) return; // already closed
            listenToPathChange = false;

            // save project prefs
            ProjectPreferences prefs = Settings.GetPrefs(project);
            prefs.ExpandedPaths = Tree.ExpandedPaths;
            
            if (!PluginBase.MainForm.ClosingEntirely) SaveProjectSession();

            activeProject = null;
            if (projectResources != null)
            {
                projectResources.Close();
                projectResources = null;
            }

            if (!internalClosing)
            {
                pluginUI.SetProject(null);
                Settings.LastProject = "";
                menus.DisabledForBuild = true;
                
                PluginBase.CurrentSolution = null;
                PluginBase.CurrentProject = null;
                PluginBase.CurrentSDK = null;
                PluginBase.MainForm.RefreshUI();

                BroadcastProjectInfo(null);
            }
            TabColors.UpdateTabColors(Settings);
        }
        
        public void OpenPanel()
        {
            this.pluginPanel.Show();
        }

        public void OpenLastProject()
        {
            // try to open the last opened project
            string lastProject = Settings.LastProject;
            if (lastProject != null && lastProject != "" && File.Exists(lastProject))
            {
                SetProject(projectActions.OpenProjectSilent(lastProject), false, true);
            }
        }

        void OpenProjectProperties()
        {
            Project project = activeProject;
            using (PropertiesDialog dialog = project.CreatePropertiesDialog())
            {
                project.UpdateVars(false);
                dialog.SetProject(project);
                dialog.ShowDialog(pluginUI);

                if (dialog.PropertiesChanged)
                {
                    project.UpdateVars(true);
                    BroadcastProjectInfo(project);
                    project.Save();
                    menus.ProjectChanged(project);
                }
            }
        }

        public void OpenFile(string path)
        {
            if (FileInspector.ShouldUseShellExecute(path)) ShellOpenFile(path);
            else if (path.IndexOf("::") > 0)
            {
                DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.OpenVirtualFile, path);
                EventManager.DispatchEvent(this, de);
            }
            else MainForm.OpenEditableDocument(path);
        }

        private void SetDocumentIcon(ITabbedDocument doc)
        {
            Bitmap bitmap = null;

            // try to open with the same icon that the treeview is using
            if (doc.FileName != null)
            {
                if (Tree.NodeMap.ContainsKey(doc.FileName))
                    bitmap = Tree.ImageList.Images[Tree.NodeMap[doc.FileName].ImageIndex] as Bitmap;
                else
                    bitmap = Icons.GetImageForFile(doc.FileName).Img as Bitmap;
            }
            if (bitmap != null)
            {
                doc.UseCustomIcon = true;
                doc.Icon = Icon.FromHandle(bitmap.GetHicon());
            }
        }

        #endregion

        #region Event Handlers

        private bool ProjectBeforeSave(Project project, string fileName)
        {
            DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.BeforeSave, fileName);
            EventManager.DispatchEvent(project, de);
            return !de.Handled; // saving handled or not allowed
        }

        private void ProjectClasspathsChanged(Project project)
        {
            if (!listenToPathChange) return;
            listenToPathChange = false;
            pluginUI.NotifyIssues();
            Tree.RebuildTree();
            listenToPathChange = true;
        }

        private void NewProject()
        {
            Project project = projectActions.NewProject();
            if (project != null) SetProject(project);
        }

        private void OpenProject()
        {
            Project project = projectActions.OpenProject();
            if (project != null) SetProject(project);
        }

        private void OpenProjectSilent(string projectPath)
        {
            Project project = projectActions.OpenProjectSilent(projectPath);
            if (project != null) SetProject(project);
        }

        void DebugProject()
        {
            Project project = activeProject;
            DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.DebugProject, "");
            EventManager.DispatchEvent(this, de);
        }

        private void FileDeleted(string path)
        {
            PluginCore.Managers.DocumentManager.CloseDocuments(path);
            Project project = Tree.ProjectOf(path);
            if (project != null)
            {
                projectActions.RemoveAllReferences(project, path);
                project.Save();
            }
            pluginUI.WatchParentOf(path);
        }

        private void FileMoved(string fromPath, string toPath)
        {
            Project project = Tree.ProjectOf(fromPath);
            Project projectTo = Tree.ProjectOf(toPath);

            PluginCore.Managers.DocumentManager.MoveDocuments(fromPath, toPath);
            if (project != null)
            {
                projectActions.MoveReferences(project, fromPath, toPath);
                project.Save();
            }
            pluginUI.WatchParentOf(fromPath);
            pluginUI.WatchParentOf(toPath);

            Hashtable data = new Hashtable();
            data["fromPath"] = fromPath;
            data["toPath"] = toPath;
            DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.FileMoved, data);
            EventManager.DispatchEvent(this, de);
        }

        private void FilePasted(string fromPath, string toPath)
        {
            Hashtable data = new Hashtable();
            data["fromPath"] = fromPath;
            data["toPath"] = toPath;
            DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.FilePasted, data);
            EventManager.DispatchEvent(this, de);
        }

        public void PropertiesClick(object sender, EventArgs e)
        {
            OpenProjectProperties();
        }

        private void SettingChanged(string setting)
        {
            if (setting == "ExcludedFileTypes" || setting == "ExcludedDirectories" || setting == "ShowProjectClasspaths" || setting == "ShowGlobalClasspaths" || setting == "GlobalClasspath")
            {
                Tree.RebuildTree();
            }
            else if (setting == "ExecutableFileTypes")
            {
                FileInspector.ExecutableFileTypes = Settings.ExecutableFileTypes;
            }
        }

        #endregion

        #region Event Broadcasting

        public void BroadcastMenuInfo()
        {
            DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.Menu, this.menus.ProjectMenu);
            EventManager.DispatchEvent(this, de);
        }

        public void BroadcastToolBarInfo()
        {
            DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.ToolBar, this.pluginUI.TreeBar);
            EventManager.DispatchEvent(this, de);
        }

        public void BroadcastProjectInfo(Project project)
        {
            DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.Project, project);
            EventManager.DispatchEvent(this, de);
        }

        #endregion

        #region Project Tree Event Handling

        private void MenuOpening(Object sender, CancelEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                this.TreeShowShellMenu();
            }
        }

        private void TreeDoubleClick()
        {
            if (pluginUI.Menu.Contains(pluginUI.Menu.Open)) 
                TreeOpenItems();
        }

        private void TreeOpenItems()
        {
            foreach (string path in Tree.SelectedPaths)
            {
                openFileQueue.Enqueue(path);
            }
            OpenNextFile();
        }

        private void OpenNextFile()
        {
            if (openFileQueue.Count > 0)
            {
                String file = openFileQueue.Dequeue() as String;
                if (File.Exists(file)) OpenFile(file);
                if (file.IndexOf("::") > 0 && File.Exists(file.Substring(0, file.IndexOf("::")))) // virtual files
                {
                    OpenFile(file);
                }
            }
        }

        private void TreeExecuteItems()
        {
            foreach (string path in Tree.SelectedPaths)
                ShellOpenFile(path);
        }

        private void ShellOpenFile(string path)
        {
            if (BridgeManager.Active && BridgeManager.IsRemote(path) && !BridgeManager.AlwaysOpenLocal(path))
            {
                BridgeManager.RemoteOpen(path);
                return;
            }
            ProcessStartInfo psi = new ProcessStartInfo(path);
            psi.WorkingDirectory = Path.GetDirectoryName(path);
            ProcessHelper.StartAsync(psi);
        }

        private void TreeInsertItem()
        {
            // special behavior if this is a fake export node inside a SWF file
            string path =  Tree.SelectedPath;
            Project project = Tree.ProjectOf(path) ?? Tree.ProjectOf(Tree.SelectedNode);
            if (project != null)
                projectActions.InsertFile(MainForm, project, path, null);
            // TODO better handling / report invalid action
        }

        private void TreeBrowseItem()
        {
            string path = Tree.SelectedPath;
            DataEvent de = new DataEvent(EventType.Command, "FileExplorer.Explore", path);
            EventManager.DispatchEvent(this, de);
        }

        private void TreeCutItems()
        {
            fileActions.CutToClipboard(Tree.SelectedPaths);
        }

        private void TreeCopyItems()
        {
            fileActions.CopyToClipboard(Tree.SelectedPaths);
        }

        private void TreePasteItems()
        {
            fileActions.PasteFromClipboard(Tree.SelectedPath);
        }

        private void TreeDeleteItems()
        {
            fileActions.Delete(Tree.SelectedPaths);
        }

        private void TreeAddFileFromTemplate(string templatePath, bool noName)
        {
            Project project = Tree.ProjectOf(Tree.SelectedNode);
            if (project != null)
                fileActions.AddFileFromTemplate(project, Tree.SelectedPath, templatePath, noName);
        }

        private void TreeAddFolder()
        {
            fileActions.AddFolder(Tree.SelectedPath);
        }

        private void TreeAddExistingFile()
        {
            fileActions.AddExistingFile(Tree.SelectedPath);
        }

        private void TreeHideItems()
        {
            Project project = Tree.ProjectOf(Tree.SelectedNode);
            if (project != null)
                projectActions.ToggleHidden(project, Tree.SelectedPaths);
        }

        public void ToggleShowHidden()
        {
            Project project = activeProject; // TODO apply to all projects
            projectActions.ToggleShowHidden(project);
            pluginUI.ShowHiddenPaths(project.ShowHiddenPaths);
        }

        public void TreeRefreshSelectedNode()
        {
            DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.UserRefreshTree, Tree);
            EventManager.DispatchEvent(this, de);

            Tree.RefreshTree();
        }

        /// <summary>
        /// Shows the command prompt
        /// </summary>
        private void TreeShowCommandPrompt()
        {
            ProcessStartInfo cmdPrompt = new ProcessStartInfo();
            cmdPrompt.FileName = "cmd.exe";
            cmdPrompt.WorkingDirectory = Tree.SelectedPath;
            Process.Start(cmdPrompt);
        }

        /// <summary>
        /// Shows the explorer shell menu
        /// </summary>
        private void TreeShowShellMenu()
        {
            String parentDir = null;
            ShellContextMenu scm = new ShellContextMenu();
            List<FileInfo> selectedPathsAndFiles = new List<FileInfo>();
            for (Int32 i = 0; i < Tree.SelectedPaths.Length; i++)
            {
                String path = Tree.SelectedPaths[i];
                // only select files in the same directory
                if (parentDir == null) parentDir = Path.GetDirectoryName(path);
                else if (Path.GetDirectoryName(path) != parentDir) continue;
                selectedPathsAndFiles.Add(new FileInfo(path));
            }
            this.pluginUI.Menu.Hide(); /* Hide default menu */
            Point location = new Point(this.pluginUI.Menu.Bounds.Left, this.pluginUI.Menu.Bounds.Top);
            scm.ShowContextMenu(selectedPathsAndFiles.ToArray(), location);
        }

        private void FindAndReplace()
        {
            String path = Tree.SelectedPath;
            if (path != null && File.Exists(path))
            {
                PluginBase.MainForm.CallCommand("FindAndReplaceFrom", path);
            }
        }

        private void FindInFiles()
        {
            String path = Tree.SelectedPath;
            if (path != null && Directory.Exists(path))
            {
                PluginBase.MainForm.CallCommand("FindAndReplaceInFilesFrom", path);
            }
        }
        
        private void TreeSyncToCurrentFile()
        {
            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            if (doc != null && doc.IsEditable && !doc.IsUntitled)
            {
                Tree.Select(doc.FileName);
                Tree.SelectedNode.EnsureVisible();
            }
        }

        private void OpenResource()
        {
            if (PluginBase.CurrentProject != null)
            {
                if (projectResources == null) projectResources = new OpenResourceForm(this);
                projectResources.ShowDialog(pluginUI);
            }
        }

        #endregion

	}

}
