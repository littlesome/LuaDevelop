using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PluginCore;

namespace LuaDebugger.Controls
{
	public class WatchUI : DockPanelControl
	{
		private DataTreeControl treeControl;
		private List<String> watches;

		public WatchUI()
		{
			watches = new List<string>();
			treeControl = new DataTreeControl(true);
			this.treeControl.Tree.BorderStyle = BorderStyle.None;
			this.treeControl.Resize += new EventHandler(this.TreeControlResize);
			this.treeControl.Tree.Font = PluginBase.Settings.DefaultFont;
			this.treeControl.Dock = DockStyle.Fill;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.treeControl);
            this.AllowDrop = true;
            this.DragDrop += new DragEventHandler(WatchUI_DragDrop);
            this.DragEnter += new DragEventHandler(WatchUI_DragEnter);
		}

        void WatchUI_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        void WatchUI_DragDrop(object sender, DragEventArgs e)
        {
            string str = e.Data.GetData(DataFormats.Text) as string;

            AddElement(str);
        }

		private void TreeControlResize(Object sender, EventArgs e)
		{
			Int32 w = this.treeControl.Width / 2;
			this.treeControl.Tree.Columns[0].Width = w;
			this.treeControl.Tree.Columns[1].Width = w - 8;
		}

		public void AddElement(String item)
		{
			if (watches.Contains(item)) return;
			watches.Add(item);
			UpdateElements();
		}
		
		public void RemoveElement(string item)
		{
			watches.Remove(item);
			UpdateElements();
		}

		public void RemoveElement(int itemN)
		{
			if (itemN<watches.Count) RemoveElement(watches[itemN]);
		}

		public void Clear()
		{
			watches.Clear();
			UpdateElements();
		}

		public void UpdateElements()
		{
			treeControl.Tree.BeginUpdate();
			treeControl.Nodes.Clear();

            DebuggerManager debugManager = PluginMain.debugManager;
			
            foreach (String item in watches)
			{
				DataNode node = new DataNode(item); // todo, introduce new Node types.
				try
				{
                    Variable var = debugManager.EvaluateCurrent(item);
                    if (var != null)
                    {
                        node = new DataNode(var);
                    }
				}
				catch { }
				node.Text = item;
				treeControl.AddNode(node);
			}
			treeControl.Tree.EndUpdate();
			treeControl.Enabled = true;
		}

	}
}
