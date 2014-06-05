using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PluginCore.Controls;
using ProjectManager.Projects;
using PluginCore.Localization;
using System.IO;
using PluginCore;

namespace ProjectManager.Controls
{
    public partial class PropertiesDialog : SmartForm
    {
        private Project project;
        private Boolean propertiesChanged;

        public PropertiesDialog()
        {
            this.Font = PluginBase.Settings.DefaultFont;
            this.FormGuid = "45a003fd-66ff-4554-aefa-a5c5043c7c24";
            InitializeComponent();

            btnOK.Text = TextHelper.GetString("Label.OK");
            btnCancel.Text = TextHelper.GetString("Label.Cancel");
            labelCommand.Text = TextHelper.GetString("Label.Command");
            labelArgument.Text = TextHelper.GetString("Label.Argument");
            labelWorkingDir.Text = TextHelper.GetString("Label.WorkingDir");
            labelSymbolPath.Text = TextHelper.GetString("Label.SymbolPath");
        }

        protected Project BaseProject { get { return project; } }

        public void SetProject(Project project)
        {
            this.project = project;
            BuildDisplay();
        }

        public bool PropertiesChanged
        {
            get { return propertiesChanged; }
            protected set { propertiesChanged = value; }
        }

        protected virtual void BuildDisplay()
        {
            this.Text = " " + project.Name + " (" + project.Language.ToUpper() + ") " + TextHelper.GetString("Info.Properties");

            textCommand.Text = project.StartupCommand;
            textArgument.Text = project.StartupArgument;
            textWorkingDir.Text = project.WorkingDir;
            textSymbolPath.Text = project.SymbolPath;
        }

        private void btnCommand_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "exe files (*.exe)|*.exe|all files (*.*)|*.*";
            dialog.Multiselect = false;
            dialog.InitialDirectory = project.Directory;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                textCommand.Text = dialog.FileName;
                textWorkingDir.Text = Path.GetDirectoryName(dialog.FileName);
            }
        }

        private void btnWorkingDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = textWorkingDir.Text;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                textWorkingDir.Text = dialog.SelectedPath;
            }
        }

        private void btnSymbolPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = textSymbolPath.Text;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                textSymbolPath.Text = dialog.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            project.StartupCommand = textCommand.Text;
            project.StartupArgument = textArgument.Text;
            project.WorkingDir = textWorkingDir.Text;
            project.SymbolPath = textSymbolPath.Text;

            propertiesChanged = true;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
