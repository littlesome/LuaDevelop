using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ProjectManager.Helpers;
using ProjectManager.Projects;
using PluginCore.Utilities;

namespace ProjectManager.Controls.TreeView
{
    public delegate FileNode FileNodeFactory(string filePath);
    public delegate void FileNodeRefresh(FileNode node);

	/// <summary>
	/// Represents a file on disk.
	/// </summary>
	public class FileNode : GenericNode
	{
        static public readonly Dictionary<string, FileNodeFactory> FileAssociations 
            = new Dictionary<string, FileNodeFactory>();

        static public event FileNodeRefresh OnFileNodeRefresh;

		protected FileNode(string filePath) : base(filePath)
		{
			isDraggable = true;
			isRenamable = true;
		}

		/// <summary>
		/// Creates the correct type of FileNode based on the file name.
		/// </summary>
		public static FileNode Create(string filePath, Project project)
		{
            string ext = Path.GetExtension(filePath).ToLower();

             if (FileAssociations.ContainsKey(ext)) // custom nodes building
                return FileAssociations[ext](filePath);
            else
                return new FileNode(filePath);
		}

		public override void Refresh(bool recursive)
		{
			base.Refresh(recursive);

            string path = BackingPath;
            string ext = Path.GetExtension(path).ToLower();

            if (project.IsPathHidden(path))
                ImageIndex = Icons.HiddenFile.Index;
            else
                ImageIndex = Icons.GetImageForFile(path).Index;
			SelectedImageIndex = ImageIndex;

			Text = Path.GetFileName(path);

            Color color = PluginCore.PluginBase.MainForm.GetThemeColor("ProjectTreeView.ForeColor");
            if (color != Color.Empty) ForeColorRequest = color;
            else ForeColorRequest = SystemColors.ControlText;

            // hook for plugins
            if (OnFileNodeRefresh != null) OnFileNodeRefresh(this);
		}
	}
}
