using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using ProjectManager.Controls;
using PluginCore.Localization;
using PluginCore;
using PluginCore.Utilities;
using System.Collections.Generic;
using ProjectManager.Projects;
using PluginCore.Helpers;

namespace ProjectManager.Controls
{
	public class FDMenus
	{
        public ToolStripMenuItem View;
        public ToolStripButton DebugProject;
        public RecentProjectsMenu RecentProjects;
		public ProjectMenu ProjectMenu;

		public FDMenus(IMainForm mainForm)
		{
            // modify the file menu
            ToolStripMenuItem fileMenu = (ToolStripMenuItem)mainForm.FindMenuItem("FileMenu");
            RecentProjects = new RecentProjectsMenu();
            fileMenu.DropDownItems.Insert(5, RecentProjects);

            // modify the view menu
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)mainForm.FindMenuItem("ViewMenu");
            View = new ToolStripMenuItem(TextHelper.GetString("Label.MainMenuItem"));
			View.Image = Icons.Project.Img;
			viewMenu.DropDownItems.Add(View);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowProject", View);

			ProjectMenu = new ProjectMenu();

            MenuStrip mainMenu = mainForm.MenuStrip;
            mainMenu.Items.Insert(5, ProjectMenu);

            ToolStrip toolBar = mainForm.ToolStrip;
			toolBar.Items.Add(new ToolStripSeparator());

            toolBar.Items.Add(RecentProjects.ToolbarSelector);

            DebugProject = new ToolStripButton(Icons.GreenCheck.Img);
            DebugProject.Name = "DebugProject";
            DebugProject.ToolTipText = TextHelper.GetString("Label.DebugProject").Replace("&", "");
            toolBar.Items.Add(DebugProject);
        }

        public bool DisabledForBuild
        {
            get { return !DebugProject.Enabled; }
            set
            {
                DebugProject.Enabled = ProjectMenu.AllItemsEnabled = !value;
            }
        }

        public void SetProject(Project project)
        {
            RecentProjects.AddOpenedProject(project.ProjectPath);
            ProjectMenu.ProjectItemsEnabled = true;
            DebugProject.Enabled = true;
            ProjectChanged(project); 
        }

        public void ProjectChanged(Project project)
        {
            
        }
    }

	/// <summary>
	/// The "Project" menu for FD's main menu
	/// </summary>
	public class ProjectMenu : ToolStripMenuItem
	{
		public ToolStripMenuItem NewProject;
        public ToolStripMenuItem OpenProject;
        public ToolStripMenuItem CloseProject;
        public ToolStripMenuItem OpenResource;
        public ToolStripMenuItem DebugProject;
        public ToolStripMenuItem Properties;

        private List<ToolStripItem> AllItems;

		public ProjectMenu()
		{
            AllItems = new List<ToolStripItem>();

            NewProject = new ToolStripMenuItem(TextHelper.GetString("Label.NewProject"));
			NewProject.Image = Icons.NewProject.Img;
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.NewProject", NewProject);

            OpenProject = new ToolStripMenuItem(TextHelper.GetString("Label.OpenProject"));
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.OpenProject", OpenProject);

            CloseProject = new ToolStripMenuItem(TextHelper.GetString("Label.CloseProject"));
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.CloseProject", CloseProject);
            AllItems.Add(CloseProject);

            OpenResource = new ToolStripMenuItem(TextHelper.GetString("Label.OpenResource"));
            OpenResource.Image = PluginBase.MainForm.FindImage("209");
            OpenResource.ShortcutKeys = Keys.Control | Keys.R;
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.OpenResource", OpenResource);
            AllItems.Add(OpenResource);

            DebugProject = new ToolStripMenuItem(TextHelper.GetString("Label.DebugProject"));
            DebugProject.Image = Icons.GreenCheck.Img;
            DebugProject.ShortcutKeys = Keys.F5;
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.DebugProject", DebugProject);
            AllItems.Add(DebugProject);

            Properties = new ToolStripMenuItem(TextHelper.GetString("Label.Properties"));
			Properties.Image = Icons.Options.Img;
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.Properties", Properties);
            AllItems.Add(Properties);

            base.Text = TextHelper.GetString("Label.Project");
            base.DropDownItems.Add(NewProject);
            base.DropDownItems.Add(OpenProject);
            base.DropDownItems.Add(CloseProject);
            base.DropDownItems.Add(new ToolStripSeparator());
            base.DropDownItems.Add(OpenResource);
            base.DropDownItems.Add(new ToolStripSeparator());
            base.DropDownItems.Add(DebugProject);
            base.DropDownItems.Add(new ToolStripSeparator());
            base.DropDownItems.Add(Properties);
		}

		public bool ProjectItemsEnabled
		{
			set
			{
				CloseProject.Enabled = value;
                DebugProject.Enabled = value;
				Properties.Enabled = value;
                OpenResource.Enabled = value;
			}
		}

        public bool AllItemsEnabled
        {
            set
            {
                foreach (ToolStripItem item in DropDownItems)
                {
                    // Toggle items only if it's our creation
                    if (AllItems.Contains(item)) item.Enabled = value;
                }
            }
        }

	}

}
