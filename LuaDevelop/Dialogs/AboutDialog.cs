using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Localization;
using LuaDevelop.Helpers;
using PluginCore.Helpers;
using PluginCore;

namespace LuaDevelop.Dialogs
{
    public class AboutDialog : Form
    {
        private LinkLabel linkLabelGithub;
        private Label versionLabel;

		public AboutDialog()
		{
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
		}

		#region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
		public void InitializeComponent() 
        {
            this.versionLabel = new System.Windows.Forms.Label();
            this.linkLabelGithub = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(12, 17);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(89, 12);
            this.versionLabel.TabIndex = 0;
            this.versionLabel.Text = "LuaDevelop 1.0";
            // 
            // linkLabelGithub
            // 
            this.linkLabelGithub.AutoSize = true;
            this.linkLabelGithub.Location = new System.Drawing.Point(12, 45);
            this.linkLabelGithub.Name = "linkLabelGithub";
            this.linkLabelGithub.Size = new System.Drawing.Size(41, 12);
            this.linkLabelGithub.TabIndex = 1;
            this.linkLabelGithub.TabStop = true;
            this.linkLabelGithub.Text = "GitHub";
            this.linkLabelGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGithub_LinkClicked);
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 74);
            this.Controls.Add(this.linkLabelGithub);
            this.Controls.Add(this.versionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " About LuaDevelop";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DialogKeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		#region Methods And Event Handlers

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            this.Text = " " + TextHelper.GetString("Title.AboutDialog");
            this.versionLabel.Font = new Font(this.Font, FontStyle.Bold);
            this.versionLabel.Text = Application.ProductName;
        }

        /// <summary>
        /// Closes the about dialog
        /// </summary>
        private void DialogKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

		/// <summary>
		/// Closes the about dialog
		/// </summary>
        private void DialogCloseClick(Object sender, EventArgs e)
		{
			this.Close();
		}

        /// <summary>
        /// Shows the about dialog
        /// </summary>
        public static new void Show()
        {
            AboutDialog aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog();
        }

		#endregion

        private void linkLabelGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/littlesome/LuaDevelop");
        }

	}
	
}
