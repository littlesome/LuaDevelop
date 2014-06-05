using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;

namespace ProjectManager.Projects
{
    public delegate void ChangedHandler(Project project);
    public delegate void ProjectUpdatingHandler(Project project);
    public delegate bool BeforeSaveHandler(Project project, string fileName);

    public abstract class Project : PluginCore.IProject
	{
        string path; // full path to this project, including filename

        HiddenPathCollection hiddenPaths;
        internal Dictionary<string, string> storage;

		public bool ShowHiddenPaths;
        
        public string StartupCommand;
        public string StartupArgument;
        public string WorkingDir;
        public string SymbolPath;

        public event BeforeSaveHandler BeforeSave;
        public event ProjectUpdatingHandler ProjectUpdating;

		public Project(string path)
		{
			this.path = path;

			hiddenPaths = new HiddenPathCollection();
            storage = new Dictionary<string, string>();
		}

        public abstract string Language { get; }
        public virtual bool ReadOnly { get { return false; } }
        public abstract string DefaultSearchFilter { get; }

        public abstract void Save();
        public abstract void SaveAs(string fileName);

        protected bool AllowedSaving(string fileName)
        {
            if (ReadOnly && fileName == ProjectPath) return false;
            if (BeforeSave != null) return BeforeSave(this, fileName);
            else return true;
        }

        public virtual ProjectManager.Controls.PropertiesDialog CreatePropertiesDialog()
        {
            return new ProjectManager.Controls.PropertiesDialog();
        }

		#region Simple Properties

		public string ProjectPath { get { return path; } }
        public virtual string Name { get { return Path.GetFileNameWithoutExtension(path); } }
		public string Directory { get { return Path.GetDirectoryName(path); } }
		
		// we only provide getters for these to preserve the original pointer
		public HiddenPathCollection HiddenPaths { get { return hiddenPaths; } }
        public Dictionary<string, string> Storage { get { return storage; } }

		#endregion

		#region Project Methods

		// all the Set/Is methods expect absolute paths (as opposed to the way they're
		// actually stored)

		public void SetPathHidden(string path, bool isHidden)
		{
			path = GetRelativePath(path);

			if (isHidden)
			{
				hiddenPaths.Add(path);				
			}
			else hiddenPaths.Remove(path);
		}

		public bool IsPathHidden(string path)
		{
			return hiddenPaths.IsHidden(GetRelativePath(path));
		}
		
		/// <summary>
		/// Call this when you delete a path so we can remove all our references to it
		/// </summary>
		public void NotifyPathsDeleted(string path)
		{
			path = GetRelativePath(path);
			hiddenPaths.Remove(path);
		}

        /// <summary>
        /// Return text to "Insert Into Document"
        /// </summary>
        public virtual string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            // to be implemented
            return null;
        }

		#endregion

		#region Path Helpers

        public String[] GetHiddenPaths()
        {
            return this.hiddenPaths.ToArray();
        }

		public string GetRelativePath(string path)
		{
			return ProjectPaths.GetRelativePath(this.Directory, path);
		}

        public void UpdateVars(bool silent)
        {
            if (!silent && ProjectUpdating != null) ProjectUpdating(this);
        }

		public string GetAbsolutePath(string path)
		{
            path = Environment.ExpandEnvironmentVariables(path);

            return ProjectPaths.GetAbsolutePath(this.Directory, path);
		}

        /// <summary>
        /// Replace accented characters and remove whitespace
        /// </summary>
        public static String RemoveDiacritics(String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

		#endregion

    }
}
