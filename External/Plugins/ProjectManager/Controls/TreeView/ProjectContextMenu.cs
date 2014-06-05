using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using ProjectManager.Projects;
using PluginCore.Localization;
using PluginCore.Helpers;
using PluginCore;

namespace ProjectManager.Controls.TreeView
{
    public delegate void FileAddHandler(string templatePath, bool noName);

    /// <summary>
    /// Provides a smart context menu for a ProjectTreeView.
    /// </summary>
    public class ProjectContextMenu : ContextMenuStrip
    {
        Project project;
        ProjectTreeView projectTree;
        static Image newFolderImg = Icons.Overlay(Icons.Folder.Img, Icons.BulletAdd.Img, 5, -3);
        public ToolStripMenuItem AddMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Add"));
        public ToolStripMenuItem AddNewFolder = new ToolStripMenuItem(TextHelper.GetString("Label.NewFolder"), newFolderImg);
        public ToolStripMenuItem AddExistingFile = new ToolStripMenuItem(TextHelper.GetString("Label.ExistingFile"), Icons.SilkPage.Img);
        public ToolStripMenuItem Open = new ToolStripMenuItem(TextHelper.GetString("Label.Open"), Icons.OpenFile.Img);
        public ToolStripMenuItem Execute = new ToolStripMenuItem(TextHelper.GetString("Label.Execute"));
        public ToolStripMenuItem Browse = new ToolStripMenuItem(TextHelper.GetString("Label.BrowseDirectory"), Icons.Browse.Img);
        public ToolStripMenuItem Cut = new ToolStripMenuItem(TextHelper.GetString("Label.Cut"), Icons.Cut.Img);
        public ToolStripMenuItem Copy = new ToolStripMenuItem(TextHelper.GetString("Label.Copy"), Icons.Copy.Img);
        public ToolStripMenuItem Paste = new ToolStripMenuItem(TextHelper.GetString("Label.Paste"), Icons.Paste.Img);
        public ToolStripMenuItem Delete = new ToolStripMenuItem(TextHelper.GetString("Label.Delete"), Icons.Delete.Img);
        public ToolStripMenuItem Rename = new ToolStripMenuItem(TextHelper.GetString("Label.Rename"));
        public ToolStripMenuItem HideItem = new ToolStripMenuItem(TextHelper.GetString("Label.HideFile"));
        public ToolStripMenuItem ShowHidden = new ToolStripMenuItem(TextHelper.GetString("Label.ShowHiddenItems"), Icons.HiddenItems.Img);
        public ToolStripMenuItem AddLibrary = new ToolStripMenuItem(TextHelper.GetString("Label.AddToLibrary"));
        public ToolStripMenuItem DebugProject = new ToolStripMenuItem(TextHelper.GetString("Label.DebugProject"), Icons.GreenCheck.Img);
        public ToolStripMenuItem CloseProject = new ToolStripMenuItem(TextHelper.GetString("Label.CloseProject"));
        public ToolStripMenuItem Properties = new ToolStripMenuItem(TextHelper.GetString("Label.Properties"), Icons.Options.Img);
        public ToolStripMenuItem ShellMenu = new ToolStripMenuItem(TextHelper.GetString("Label.ShellMenu"));
        public ToolStripMenuItem FindAndReplace = new ToolStripMenuItem(TextHelper.GetString("Label.FindHere"), Icons.FindAndReplace.Img);
        public ToolStripMenuItem FindInFiles = new ToolStripMenuItem(TextHelper.GetString("Label.FindHere"), Icons.FindInFiles.Img);
        public ToolStripMenuItem CommandPrompt = new ToolStripMenuItem(TextHelper.GetString("LuaDevelop.Label.CommandPrompt"), Icons.CommandPrompt.Img);
        public event FileAddHandler AddFileFromTemplate;

        public ProjectContextMenu()
        {
            this.Renderer = new DockPanelStripRenderer();
            this.Font = PluginCore.PluginBase.Settings.DefaultFont;
            this.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
        }

        public ProjectTreeView ProjectTree
        {
            get { return projectTree; }
            set { projectTree = value; }
        }

        public Boolean Contains(ToolStripMenuItem item)
        {
            return item != null && item.Enabled && findItem(item, Items);
        }

        private bool findItem(ToolStripMenuItem item, ToolStripItemCollection items)
        {
            foreach (ToolStripItem i in items)
                if (i == item) return true;
                else if (i is ToolStripMenuItem)
                {
                    ToolStripMenuItem mi = i as ToolStripMenuItem;
                    if (mi.DropDown.Items.Count > 0 && findItem(item, mi.DropDown.Items)) return true;
                }
            return false;
        }

        #region File Templates

        private ToolStripItem[] GetAddFileTemplates()
        {
            List<ToolStripItem> items = new List<ToolStripItem>();
            // the custom project file templates are in a subdirectory named after the project class name
            string customDir = Path.Combine(ProjectPaths.FileTemplatesDirectory, project.GetType().Name);
            if (Directory.Exists(customDir))
            {
                foreach (string file in Directory.GetFiles(customDir, "*.ldt"))
                {
                    items.Add(GetCustomAddFile(file));
                }
                List<string> excludedDirs = new List<string>(PluginMain.Settings.ExcludedDirectories);
                foreach (string dir in Directory.GetDirectories(customDir))
                {
                    // don't copy like .svn and stuff
                    if (excludedDirs.Contains(Path.GetFileName(dir).ToLower())) continue;
                    items.Add(GetCustomAddFileDirectory(dir));
                }
            }
            if (items.Count > 0) items.Add(new ToolStripSeparator());
            // get all the generic FD templates
            foreach (string file in Directory.GetFiles(PathHelper.TemplateDir, "*.ldt"))
            {
                string name = Path.GetFileNameWithoutExtension(file).ToLower();
                if (name != "as2" && name != "as3" && name != "haxe") items.Add(GetGenericAddFile(file));
            }
            return items.ToArray();
        }

        private ToolStripMenuItem GetCustomAddFileDirectory(string customDir)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();
            List<string> excludedDirs = new List<string>(PluginMain.Settings.ExcludedDirectories);
            foreach (string dir in Directory.GetDirectories(customDir))
            {
                string dirName = Path.GetFileName(dir);
                // don't copy like .svn and stuff
                if (excludedDirs.Contains(dirName.ToLower())) continue;
                items.Add(GetCustomAddFileDirectory(dir));
            }
            foreach (string file in Directory.GetFiles(customDir, "*.ldt"))
            {
                items.Add(GetCustomAddFile(file));
            }
            string[] dirNames = customDir.Split('\\');
            return new ToolStripMenuItem((string)dirNames.GetValue(dirNames.Length - 1), null, items.ToArray());
        }

        private ToolStripMenuItem GetCustomAddFile(string file)
        {
            string actualFile = Path.GetFileNameWithoutExtension(file); // strip .fdt
            string actualName = Path.GetFileNameWithoutExtension(actualFile); // strip ext
            Image image = Icons.GetImageForFile(actualFile).Img;
            image = Icons.Overlay(image, Icons.BulletAdd.Img, 5, 4);
            String label = TextHelper.GetString("Label.New");
            ToolStripMenuItem item = new ToolStripMenuItem("New " + actualName + "...", image);
            item.Click += delegate
            {
                if (AddFileFromTemplate != null) AddFileFromTemplate(file, false);
            };
            return item;
        }

        private ToolStripMenuItem GetGenericAddFile(string file)
        {
            string ext = string.Empty;
            string actual = Path.GetFileNameWithoutExtension(file);
            bool isDoubleExt = Path.GetFileName(file).Split('.').Length > 2;
            if (isDoubleExt) // Double extension...
            {
                actual = Path.GetFileNameWithoutExtension(actual);
                ext = Path.GetExtension(actual);
            }
            else ext = "." + actual;
            Image image = Icons.GetImageForFile(ext).Img;
            image = Icons.Overlay(image, Icons.BulletAdd.Img, 5, 4);
            String nlabel = TextHelper.GetString("Label.New");
            String flabel = TextHelper.GetString("Label.File");
            ToolStripMenuItem item = new ToolStripMenuItem(nlabel + " " + actual + " " + flabel + "...", image);
            item.Click += delegate
            {
                if (AddFileFromTemplate != null) AddFileFromTemplate(file, true);
            };
            return item;
        }

        #endregion

        #region Configure

        /// <summary>
        /// Configure ourself to be a menu relevant to the given Project with the
        /// given selected treeview nodes.
        /// </summary>
        public void Configure(ArrayList nodes, Project inProject)
        {
            base.Items.Clear();
            project = inProject;

            MergableMenu menu = new MergableMenu();
            foreach (GenericNode node in nodes)
            {
                MergableMenu newMenu = new MergableMenu();
                AddItems(newMenu, node);
                menu = (menu.Count > 0) ? menu.Combine(newMenu) : newMenu;
            }
            menu.Apply(this.Items);

            // deal with special menu items that can't be applied to multiple paths
            bool singleFile = (nodes.Count == 1);
            AddMenu.Enabled = singleFile;
            Rename.Enabled = singleFile;
            Paste.Enabled = singleFile;

            // deal with naming the "Hide" button correctly
            if (nodes.Count > 1 || nodes.Count == 0) HideItem.Text = TextHelper.GetString("Label.HideItems");
            else HideItem.Text = (nodes[0] is DirectoryNode) ? TextHelper.GetString("Label.HideFolder") : TextHelper.GetString("Label.HideFile");

            // deal with shortcuts
            AssignShortcuts();
            if (this.Items.Contains(AddMenu) && AddMenu.Enabled) BuildAddMenu();
        }

        protected override void OnOpening(CancelEventArgs e)
        {
            // only enable paste if there is filedrop data in the clipboard
            if (!Clipboard.GetDataObject().GetDataPresent(DataFormats.FileDrop)) Paste.Enabled = false;
            base.OnOpening(e);
        }

        public void AssignShortcuts()
        {
            Rename.ShortcutKeys = (Rename.Enabled) ? Keys.F2 : Keys.None;
        }

        #endregion

        #region Add Menu Items

        private void BuildAddMenu()
        {
            AddMenu.DropDownItems.Clear();
            AddMenu.DropDownItems.AddRange(GetAddFileTemplates());
            AddMenu.DropDownItems.Add(new ToolStripSeparator());
            AddMenu.DropDownItems.Add(AddNewFolder);
            AddMenu.DropDownItems.Add(new ToolStripSeparator());
            AddMenu.DropDownItems.Add(AddExistingFile);
        }

        private void AddItems(MergableMenu menu, GenericNode node)
        {
            string path = node.BackingPath;
            if (node.IsInvalid)
            {
                return;
            }
            if (node is ProjectNode) AddProjectItems(menu);
            else if (node is DirectoryNode) AddFolderItems(menu, path);
            else if (node is FileNode) AddGenericFileItems(menu, path);
        }

        private void AddProjectItems(MergableMenu menu)
        {
            bool showHidden = project.ShowHiddenPaths;
            menu.Add(DebugProject, 0);
            menu.Add(CloseProject, 0);
            menu.Add(AddMenu, 1);
            menu.Add(Browse, 1);
            menu.Add(FindInFiles, 1);
            menu.Add(CommandPrompt, 1);
            menu.Add(ShellMenu, 1);
            menu.Add(Paste, 2);
            menu.Add(ShowHidden, 3, showHidden);
            menu.Add(Properties, 4);
        }

        private void AddClasspathItems(MergableMenu menu, string path)
        {
            menu.Add(AddMenu, 0);
            menu.Add(Browse, 0);
            menu.Add(FindInFiles, 0);
            menu.Add(CommandPrompt, 0);
            menu.Add(ShellMenu, 0);
            menu.Add(Paste, 1);
            AddHideItems(menu, path, 3);
        }

        private void AddFolderItems(MergableMenu menu, string path)
        {
            menu.Add(AddMenu, 0);
            menu.Add(Browse, 0);
            menu.Add(FindInFiles, 0);
            menu.Add(CommandPrompt, 0);
            menu.Add(ShellMenu, 0);
            AddFileItems(menu, path, true);
        }

        private void AddFileItems(MergableMenu menu, string path, bool addPaste)
        {
            menu.Add(Cut, 1);
            menu.Add(Copy, 1);
            if (addPaste) menu.Add(Paste, 1);
            menu.Add(Delete, 1);
            menu.Add(Rename, 1);
            AddHideItems(menu, path, 3);
        }

        private void AddHideItems(MergableMenu menu, string path, int group)
        {
            bool hidden = project.IsPathHidden(path);
            bool showHidden = project.ShowHiddenPaths;
            menu.Add(ShowHidden, group, showHidden);
            menu.Add(HideItem, group, hidden);
        }

        private void AddFileItems(MergableMenu menu, string path)
        {
            AddFileItems(menu, path, true);
        }

        private void AddGenericFileItems(MergableMenu menu, string path)
        {
            menu.Add(Open, 0);
            menu.Add(Execute, 0);
            menu.Add(FindAndReplace, 0);
            menu.Add(ShellMenu, 0);
            AddFileItems(menu, path);
        }

        #endregion

    }

}
