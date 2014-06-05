using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using ProjectManager.Projects;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ProjectManager.Controls.TreeView
{
    public class ProjectNode : WatcherNode
    {
        bool isActive;

        public ProjectNode(Project project) : base(project.Directory)
        {
            this.project = project;
            isDraggable = false;
            isRenamable = false;
        }

        public override void Refresh(bool recursive)
        {
            base.Refresh(recursive);
            FontStyle style = isActive ? FontStyle.Bold : FontStyle.Regular;
            NodeFont = new System.Drawing.Font(PluginCore.PluginBase.Settings.DefaultFont, FontStyle.Bold);
            Text = ProjectRef.Name + " (" + ProjectRef.Language.ToUpper() + ")";
            ImageIndex = Icons.Project.Index;
            SelectedImageIndex = ImageIndex;
            Expand();
            NotifyRefresh();
        }

        public Project ProjectRef
        {
            get { return project; }
        }

        public bool IsActive 
        {
            get { return isActive; }
            set 
            {
                if (isActive == value) return;
                isActive = value;
                FontStyle style = isActive ? FontStyle.Bold : FontStyle.Regular;
                NodeFont = new System.Drawing.Font(PluginCore.PluginBase.Settings.DefaultFont, style);
                Text = Text; // Reset text to update the font
            }
        }
    }
}
