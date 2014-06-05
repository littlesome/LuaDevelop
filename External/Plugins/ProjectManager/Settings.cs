using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using ProjectManager.Projects;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore;
using ProjectManager.Controls;

namespace ProjectManager
{
    public delegate void SettingChangeHandler(string setting);

    [Serializable]
    public class ProjectManagerSettings
    {
        public event SettingChangeHandler Changed;

        List<ProjectPreferences> projectPrefList = new List<ProjectPreferences>();
        List<string> recentProjects = new List<string>();
        int maxRecentProjects = 10;
        string lastProject = string.Empty;
        bool createProjectDirectory = false;
        bool useProjectSessions = false;
        string newProjectDefaultDirectory = string.Empty;

        // These are string arrays because they are only edited by the propertygrid (which deals with them nicely)
        string[] excludedFileTypes = new string[] { ".p", ".abc", ".bak", ".tmp" };
        string[] excludedDirectories = new string[] { ".svn", "_svn", ".cvs", "_cvs", "cvs", "_sgbak", ".git", ".hg" };
        string[] executableFileTypes = new string[] { ".exe", ".lnk", ".fla", ".doc", ".pps", ".psd", ".png", ".jpg", ".gif", ".xls", ".docproj", ".ttf", ".otf", ".wav", ".mp3", ".ppt", ".pptx", ".docx", ".xlsx", ".ai", ".pdf", ".zip", ".rar" };
        string[] filteredDirectoryNames = new string[] { "src", "source", "sources", "as", "as2", "as3", "actionscript", "flash", "classes", "trunk", "svn", "git", "hg", "..", "." };

        HighlightType tabHighlightType = HighlightType.ExternalFiles;

        #region Properties
        [Browsable(false)]
        public List<ProjectPreferences> ProjectPrefs
        {
            get { return projectPrefList; }
            set { projectPrefList = value; }
        }

        [Browsable(false)]
        public string LastProject
        {
            get { return lastProject; }
            set { lastProject = value; FireChanged("LastProject"); }
        }

        [Browsable(false)]
        public List<string> RecentProjects
        {
            get { return recentProjects; }
        }

        [Browsable(false)]
        public bool CreateProjectDirectory
        {
            get { return createProjectDirectory; }
            set { createProjectDirectory = value; FireChanged("CreateProjectDirectory"); }
        }

        [Browsable(false)]
        public String NewProjectDefaultDirectory
        {
            get { return newProjectDefaultDirectory; }
            set { newProjectDefaultDirectory = value; FireChanged("NewProjectDefaultDirectory"); }
        }

        [DisplayName("Use Project Sessions")]
        [LocalizedDescription("ProjectManager.Description.UseProjectSessions")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(false)]
        public Boolean UseProjectSessions
        {
            get { return useProjectSessions; }
            set { useProjectSessions = value; }
        }

        [DisplayName("Maximum Recent Projects")]
        [LocalizedDescription("ProjectManager.Description.MaxRecentProjects")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(10)]
        public Int32 MaxRecentProjects
		{
            get { return maxRecentProjects; }
            set { maxRecentProjects = value; FireChanged("MaxRecentProjects"); }
        }

        [DisplayName("Excluded File Types")]
        [LocalizedDescription("ProjectManager.Description.ExcludedFileTypes")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] ExcludedFileTypes
		{
            get { return excludedFileTypes; }
            set { excludedFileTypes = value; FireChanged("ExcludedFileTypes"); }
		}

        [DisplayName("Executable File Types")]
        [LocalizedDescription("ProjectManager.Description.ExecutableFileTypes")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] ExecutableFileTypes
        {
            get { return executableFileTypes; }
            set { executableFileTypes = value; FireChanged("ExecutableFileTypes"); }
        }

        [DisplayName("Excluded Directories")]
        [LocalizedDescription("ProjectManager.Description.ExcludedDirectories")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] ExcludedDirectories
		{
            get { return excludedDirectories; }
            set { excludedDirectories = value; FireChanged("ExcludedDirectories"); }
		}

        [DisplayName("Filtered Directory Names")]
        [LocalizedDescription("ProjectManager.Description.FilteredDirectoryNames")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] FilteredDirectoryNames
        {
            get { return filteredDirectoryNames; }
            set { filteredDirectoryNames = value; FireChanged("FilteredDirectoryNames"); }
        }

        [DisplayName("Tab Highlight Type")]
        [LocalizedDescription("ProjectManager.Description.TabHighlightType")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(HighlightType.ExternalFiles)]
        public HighlightType TabHighlightType
        {
            get { return tabHighlightType; }
            set { tabHighlightType = value; }
        }

        #endregion

        [Browsable(false)]
        public string Language;

        /// <summary>
        /// Returns the preferences object for the given project
        /// and creates a new instance if necessary.
        /// </summary>
        public ProjectPreferences GetPrefs(Project project)
        {
            foreach (ProjectPreferences prefs in projectPrefList)
                if (prefs.ProjectPath == project.ProjectPath)
                    return prefs;

            // ok, we haven't seen this project before.  let's take this opportunity
            // to clean out any prefs for projects that don't exist anymore
            CleanOldPrefs();

            ProjectPreferences newPrefs = new ProjectPreferences(project.ProjectPath);
            projectPrefList.Add(newPrefs);
            return newPrefs;
        }

        private void CleanOldPrefs()
        {
            for (int i = 0; i < projectPrefList.Count; i++)
                if (!File.Exists(projectPrefList[i].ProjectPath))
                    projectPrefList.RemoveAt(i--); // search this index again
        }

        private void FireChanged(string setting)
        {
            if (Changed != null)
                Changed(setting);
        }
    }

    [Serializable]
    public class ProjectPreferences
    {
        public Boolean DebugMode;
        public List<String> ExpandedPaths;
        public String ProjectPath;
        public String TargetBuild;

        public ProjectPreferences()
        {
            this.ExpandedPaths = new List<String>();
        }
        public ProjectPreferences(String projectPath) : this()
        {
            this.ProjectPath = projectPath;
        }
    }
}
