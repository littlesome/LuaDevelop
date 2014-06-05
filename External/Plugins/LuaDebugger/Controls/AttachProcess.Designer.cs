namespace LuaDebugger.Controls
{
    partial class AttachProcess
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this._tree = new Aga.Controls.Tree.TreeViewAdv();
            this.NameColumn = new Aga.Controls.Tree.TreeColumn();
            this.PIDColumn = new Aga.Controls.Tree.TreeColumn();
            this.TitleColumn = new Aga.Controls.Tree.TreeColumn();
            this.nodeIcon1 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox2 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox3 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(409, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 23);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(312, 227);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 23);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // _tree
            // 
            this._tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tree.BackColor = System.Drawing.SystemColors.Window;
            this._tree.Columns.Add(this.NameColumn);
            this._tree.Columns.Add(this.PIDColumn);
            this._tree.Columns.Add(this.TitleColumn);
            this._tree.DefaultToolTipProvider = null;
            this._tree.DragDropMarkColor = System.Drawing.Color.Black;
            this._tree.FullRowSelect = true;
            this._tree.GridLineStyle = ((Aga.Controls.Tree.GridLineStyle)((Aga.Controls.Tree.GridLineStyle.Horizontal | Aga.Controls.Tree.GridLineStyle.Vertical)));
            this._tree.LineColor = System.Drawing.SystemColors.ControlDark;
            this._tree.Location = new System.Drawing.Point(12, 12);
            this._tree.Model = null;
            this._tree.Name = "_tree";
            this._tree.NodeControls.Add(this.nodeIcon1);
            this._tree.NodeControls.Add(this.nodeTextBox1);
            this._tree.NodeControls.Add(this.nodeTextBox2);
            this._tree.NodeControls.Add(this.nodeTextBox3);
            this._tree.RowHeight = 20;
            this._tree.SelectedNode = null;
            this._tree.Size = new System.Drawing.Size(487, 199);
            this._tree.TabIndex = 20;
            this._tree.UseColumns = true;
            this._tree.DoubleClick += new System.EventHandler(this._tree_DoubleClick);
            // 
            // NameColumn
            // 
            this.NameColumn.Header = "Name";
            this.NameColumn.MinColumnWidth = 120;
            this.NameColumn.Sortable = true;
            this.NameColumn.SortOrder = System.Windows.Forms.SortOrder.None;
            this.NameColumn.TooltipText = null;
            this.NameColumn.Width = 250;
            // 
            // PIDColumn
            // 
            this.PIDColumn.Header = "PID";
            this.PIDColumn.MinColumnWidth = 50;
            this.PIDColumn.Sortable = true;
            this.PIDColumn.SortOrder = System.Windows.Forms.SortOrder.None;
            this.PIDColumn.TooltipText = null;
            // 
            // TitleColumn
            // 
            this.TitleColumn.Header = "Title";
            this.TitleColumn.MinColumnWidth = 100;
            this.TitleColumn.Sortable = true;
            this.TitleColumn.SortOrder = System.Windows.Forms.SortOrder.None;
            this.TitleColumn.TooltipText = null;
            this.TitleColumn.Width = 180;
            // 
            // nodeIcon1
            // 
            this.nodeIcon1.DataPropertyName = "Image";
            this.nodeIcon1.LeftMargin = 1;
            this.nodeIcon1.ParentColumn = this.NameColumn;
            this.nodeIcon1.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "Name";
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = this.NameColumn;
            // 
            // nodeTextBox2
            // 
            this.nodeTextBox2.DataPropertyName = "PID";
            this.nodeTextBox2.IncrementalSearchEnabled = true;
            this.nodeTextBox2.LeftMargin = 3;
            this.nodeTextBox2.ParentColumn = this.PIDColumn;
            // 
            // nodeTextBox3
            // 
            this.nodeTextBox3.DataPropertyName = "Title";
            this.nodeTextBox3.IncrementalSearchEnabled = true;
            this.nodeTextBox3.LeftMargin = 3;
            this.nodeTextBox3.ParentColumn = this.TitleColumn;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefresh.Location = new System.Drawing.Point(12, 227);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 23);
            this.btnRefresh.TabIndex = 21;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // AttachProcess
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(511, 262);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this._tree);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "AttachProcess";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AttachProcess";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private Aga.Controls.Tree.TreeViewAdv _tree;
        private Aga.Controls.Tree.TreeColumn NameColumn;
        private Aga.Controls.Tree.TreeColumn PIDColumn;
        private Aga.Controls.Tree.TreeColumn TitleColumn;
        private System.Windows.Forms.Button btnRefresh;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox2;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox3;
    }
}