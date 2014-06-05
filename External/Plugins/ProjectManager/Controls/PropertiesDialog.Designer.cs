namespace ProjectManager.Controls
{
    partial class PropertiesDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSymbolPath = new System.Windows.Forms.Button();
            this.btnWorkingDir = new System.Windows.Forms.Button();
            this.btnCommand = new System.Windows.Forms.Button();
            this.textSymbolPath = new System.Windows.Forms.TextBox();
            this.labelSymbolPath = new System.Windows.Forms.Label();
            this.textWorkingDir = new System.Windows.Forms.TextBox();
            this.labelWorkingDir = new System.Windows.Forms.Label();
            this.textArgument = new System.Windows.Forms.TextBox();
            this.labelArgument = new System.Windows.Forms.Label();
            this.textCommand = new System.Windows.Forms.TextBox();
            this.labelCommand = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // btnSymbolPath
            // 
            this.btnSymbolPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSymbolPath.Location = new System.Drawing.Point(366, 147);
            this.btnSymbolPath.Name = "btnSymbolPath";
            this.btnSymbolPath.Size = new System.Drawing.Size(36, 21);
            this.btnSymbolPath.TabIndex = 28;
            this.btnSymbolPath.Text = "...";
            this.btnSymbolPath.UseVisualStyleBackColor = true;
            this.btnSymbolPath.Click += new System.EventHandler(this.btnSymbolPath_Click);
            // 
            // btnWorkingDir
            // 
            this.btnWorkingDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWorkingDir.Location = new System.Drawing.Point(366, 108);
            this.btnWorkingDir.Name = "btnWorkingDir";
            this.btnWorkingDir.Size = new System.Drawing.Size(36, 21);
            this.btnWorkingDir.TabIndex = 27;
            this.btnWorkingDir.Text = "...";
            this.btnWorkingDir.UseVisualStyleBackColor = true;
            this.btnWorkingDir.Click += new System.EventHandler(this.btnWorkingDir_Click);
            // 
            // btnCommand
            // 
            this.btnCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommand.Location = new System.Drawing.Point(366, 33);
            this.btnCommand.Name = "btnCommand";
            this.btnCommand.Size = new System.Drawing.Size(36, 21);
            this.btnCommand.TabIndex = 26;
            this.btnCommand.Text = "...";
            this.btnCommand.UseVisualStyleBackColor = true;
            this.btnCommand.Click += new System.EventHandler(this.btnCommand_Click);
            // 
            // textSymbolPath
            // 
            this.textSymbolPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textSymbolPath.Location = new System.Drawing.Point(93, 147);
            this.textSymbolPath.Name = "textSymbolPath";
            this.textSymbolPath.Size = new System.Drawing.Size(266, 21);
            this.textSymbolPath.TabIndex = 25;
            // 
            // labelSymbolPath
            // 
            this.labelSymbolPath.AutoSize = true;
            this.labelSymbolPath.Location = new System.Drawing.Point(21, 150);
            this.labelSymbolPath.Name = "labelSymbolPath";
            this.labelSymbolPath.Size = new System.Drawing.Size(71, 12);
            this.labelSymbolPath.TabIndex = 24;
            this.labelSymbolPath.Text = "SymbolPath:";
            // 
            // textWorkingDir
            // 
            this.textWorkingDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textWorkingDir.Location = new System.Drawing.Point(93, 109);
            this.textWorkingDir.Name = "textWorkingDir";
            this.textWorkingDir.Size = new System.Drawing.Size(266, 21);
            this.textWorkingDir.TabIndex = 23;
            // 
            // labelWorkingDir
            // 
            this.labelWorkingDir.AutoSize = true;
            this.labelWorkingDir.Location = new System.Drawing.Point(21, 112);
            this.labelWorkingDir.Name = "labelWorkingDir";
            this.labelWorkingDir.Size = new System.Drawing.Size(71, 12);
            this.labelWorkingDir.TabIndex = 22;
            this.labelWorkingDir.Text = "WorkingDir:";
            // 
            // textArgument
            // 
            this.textArgument.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textArgument.Location = new System.Drawing.Point(93, 71);
            this.textArgument.Name = "textArgument";
            this.textArgument.Size = new System.Drawing.Size(309, 21);
            this.textArgument.TabIndex = 21;
            // 
            // labelArgument
            // 
            this.labelArgument.AutoSize = true;
            this.labelArgument.Location = new System.Drawing.Point(21, 74);
            this.labelArgument.Name = "labelArgument";
            this.labelArgument.Size = new System.Drawing.Size(59, 12);
            this.labelArgument.TabIndex = 20;
            this.labelArgument.Text = "Argument:";
            // 
            // textCommand
            // 
            this.textCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textCommand.Location = new System.Drawing.Point(93, 33);
            this.textCommand.Name = "textCommand";
            this.textCommand.Size = new System.Drawing.Size(266, 21);
            this.textCommand.TabIndex = 19;
            // 
            // labelCommand
            // 
            this.labelCommand.AutoSize = true;
            this.labelCommand.Location = new System.Drawing.Point(21, 36);
            this.labelCommand.Name = "labelCommand";
            this.labelCommand.Size = new System.Drawing.Size(53, 12);
            this.labelCommand.TabIndex = 18;
            this.labelCommand.Text = "Command:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(325, 212);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(228, 212);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 23);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(402, 180);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            // 
            // PropertiesDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(426, 247);
            this.Controls.Add(this.btnSymbolPath);
            this.Controls.Add(this.btnWorkingDir);
            this.Controls.Add(this.btnCommand);
            this.Controls.Add(this.textSymbolPath);
            this.Controls.Add(this.labelSymbolPath);
            this.Controls.Add(this.textWorkingDir);
            this.Controls.Add(this.labelWorkingDir);
            this.Controls.Add(this.textArgument);
            this.Controls.Add(this.labelArgument);
            this.Controls.Add(this.textCommand);
            this.Controls.Add(this.labelCommand);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(442, 285);
            this.Name = "PropertiesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PropertiesDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSymbolPath;
        private System.Windows.Forms.Button btnWorkingDir;
        private System.Windows.Forms.Button btnCommand;
        private System.Windows.Forms.TextBox textSymbolPath;
        private System.Windows.Forms.Label labelSymbolPath;
        private System.Windows.Forms.TextBox textWorkingDir;
        private System.Windows.Forms.Label labelWorkingDir;
        private System.Windows.Forms.TextBox textArgument;
        private System.Windows.Forms.Label labelArgument;
        private System.Windows.Forms.TextBox textCommand;
        private System.Windows.Forms.Label labelCommand;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}