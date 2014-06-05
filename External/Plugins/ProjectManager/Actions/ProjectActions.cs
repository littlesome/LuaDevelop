using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Localization;
using ProjectManager.Controls;
using ProjectManager.Helpers;
using ProjectManager.Projects;
using PluginCore.Helpers;
using ICSharpCode.SharpZipLib.Zip;
using ProjectManager.Controls.TreeView;
using System.Text.RegularExpressions;

namespace ProjectManager.Actions
{
	public delegate void ProjectModifiedHandler(string[] paths);

	/// <summary>
	/// Provides high-level actions for working with Project files.
	/// </summary>
	public class ProjectActions
	{
        IWin32Window owner; // for dialogs

		public event ProjectModifiedHandler ProjectModified;

		public ProjectActions(IWin32Window owner)
		{
			this.owner = owner;
		}

        #region New/Open Project

        public Project NewProject()
        {
            NewProjectDialog dialog = new NewProjectDialog();
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                try
                {
                    LuaDevelopActions.CheckAuthorName();
                    ProjectCreator creator = new ProjectCreator();
                    Project created = creator.CreateProject(dialog.TemplateDirectory, dialog.ProjectLocation, dialog.ProjectName);
                    PatchProject(created);
                    return created;
                }
                catch (Exception exception)
                {
                    string msg = TextHelper.GetString("Info.CouldNotCreateProject");
                    ErrorManager.ShowInfo(msg + " " + exception.Message);
                }
            }

            return null;
        }

        public Project OpenProject()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = " " + TextHelper.GetString("Title.OpenProjectDialog");
            dialog.Filter = ProjectCreator.GetProjectFilters();

            if (dialog.ShowDialog(owner) == DialogResult.OK)
                return OpenProjectSilent(dialog.FileName);
            else
                return null;
        }

        public Project OpenProjectSilent(string path)
        {
            try 
            {
                String physical = PathHelper.GetPhysicalPathName(path);
                Project loaded = ProjectLoader.Load(physical);
                PatchProject(loaded);
                return loaded;
            }
            catch (Exception exception)
            {
                string msg = TextHelper.GetString("Info.CouldNotOpenProject");
                ErrorManager.ShowInfo(msg + " " + exception.Message);
                return null;
            }
        }

        private void PatchProject(Project project)
        {
            if (project == null) return;
            if (!project.HiddenPaths.IsHidden("obj"))
                project.HiddenPaths.Add("obj");
        }

        private string ExtractPackagedProject(string packagePath)
        {
            using (FileStream fs = new FileStream(packagePath, FileMode.Open, FileAccess.Read))
            using (ZipFile zFile = new ZipFile(fs))
            {
                if (zFile.GetEntry(".actionscriptProperties") != null)
                {
                    using (FolderBrowserDialog saveDialog = new FolderBrowserDialog())
                    {
                        saveDialog.ShowNewFolderButton = true;
                        saveDialog.Description = TextHelper.GetString("Title.ImportPackagedProject");

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            foreach (ZipEntry entry in zFile)
                            {
                                Int32 size = 4095;
                                Byte[] data = new Byte[4095];
                                string newPath = Path.Combine(saveDialog.SelectedPath, entry.Name.Replace('/', '\\'));

                                if (entry.IsFile)
                                {
                                    Stream zip = zFile.GetInputStream(entry);
                                    String ext = Path.GetExtension(newPath);
                                    String dirPath = Path.GetDirectoryName(newPath);
                                    if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                                    FileStream extracted = new FileStream(newPath, FileMode.Create);
                                    while (true)
                                    {
                                        size = zip.Read(data, 0, data.Length);
                                        if (size > 0) extracted.Write(data, 0, size);
                                        else break;
                                    }
                                    extracted.Close();
                                    extracted.Dispose();
                                }
                                else
                                {
                                    Directory.CreateDirectory(newPath);
                                }
                            }
                        }

                        return Path.Combine(saveDialog.SelectedPath, ".actionScriptProperties");
                    }
                }
            }

            return string.Empty;
        }

        #endregion

		#region Project File Reference Updating

		public void RemoveAllReferences(Project project, string path)
		{
		}

		public void MoveReferences(Project project, string fromPath, string toPath)
		{
		}

		#endregion

		#region Working with Project Files

        public void InsertFile(IMainForm mainForm, Project project, string path, GenericNode node)
		{
            if (!mainForm.CurrentDocument.IsEditable) return;
            string nodeType = (node != null) ? node.GetType().ToString() : null;
            string textToInsert = project.GetInsertFileText(mainForm.CurrentDocument.FileName, path, null, nodeType);
            if (textToInsert == null) return;
            if (mainForm.CurrentDocument.IsEditable)
            {
                mainForm.CurrentDocument.SciControl.AddText(textToInsert.Length, textToInsert);
                mainForm.CurrentDocument.Activate();
            }
            else
            {
                string msg = TextHelper.GetString("Info.EmbedNeedsOpenDocument");
                ErrorManager.ShowInfo(msg);
            }
		}

		public void ToggleShowHidden(Project project)
		{
			project.ShowHiddenPaths = !project.ShowHiddenPaths;
			project.Save();
			OnProjectModified(null);
		}

		public void ToggleHidden(Project project, string[] paths)
		{
			foreach (string path in paths)
			{
				bool isHidden = project.IsPathHidden(path);
				project.SetPathHidden(path, !isHidden);
			}
			project.Save();

			OnProjectModified(null);
		}

		#endregion

		private void OnProjectModified(string[] paths)
		{
			if (ProjectModified != null)
				ProjectModified(paths);
		}
    }
}
