using System;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Controls;
using LuaDebugger.Controls;
using ScintillaNet;
using PluginCore;

namespace LuaDebugger
{
    class LiveDataTip
    {
        private DataTipForm m_ToolTip;
		private MouseMessageFilter m_MouseMessageFilter;

		public LiveDataTip()
		{
			m_ToolTip = new DataTipForm();
			m_ToolTip.Dock = DockStyle.Fill;
			m_ToolTip.Visible = false;
			m_MouseMessageFilter = new MouseMessageFilter();
			m_MouseMessageFilter.AddControls(new Control[] { m_ToolTip, m_ToolTip.DataTree });
			m_MouseMessageFilter.MouseDownEvent += new MouseDownEventHandler(MouseMessageFilter_MouseDownEvent);
			m_MouseMessageFilter.KeyDownEvent += new EventHandler(MouseMessageFilter_KeyDownEvent);
			Application.AddMessageFilter(m_MouseMessageFilter);
			UITools.Manager.OnMouseHover += new UITools.MouseHoverHandler(Manager_OnMouseHover);
		}

		public void Show(Point point, Variable variable, String path)
		{
			m_ToolTip.Location = point;
			m_ToolTip.SetVariable(variable, path);
			m_ToolTip.Visible = true;
			m_ToolTip.Location = point;
			m_ToolTip.BringToFront();
			m_ToolTip.Focus();
		}

		public void Hide()
		{
			m_ToolTip.Visible = false;
		}

		private void MouseMessageFilter_MouseDownEvent(MouseButtons button, Point e)
		{
			if (m_ToolTip.Visible &&
				!m_ToolTip.DataTree.Tree.ContextMenuStrip.Visible &&
				!m_ToolTip.DataTree.Viewer.Visible)
			{
				Hide();
			}
		}

		private void MouseMessageFilter_KeyDownEvent(object sender, EventArgs e)
		{
			if (m_ToolTip.Visible &&
				!m_ToolTip.DataTree.Tree.ContextMenuStrip.Visible &&
				!m_ToolTip.DataTree.Viewer.Visible)
			{
				Hide();
			}
		}

		private void Manager_OnMouseHover(ScintillaControl sci, Int32 position)
		{
			DebuggerManager debugManager = PluginMain.debugManager;
			if (!PluginBase.MainForm.EditorMenu.Visible && debugManager.IsDebuggerStarted && debugManager.IsDebuggerSuspended)
			{
                string currentFilePath = debugManager.CurrentFullFilePath;
                if (currentFilePath == null || currentFilePath != PluginBase.MainForm.CurrentDocument.FileName)
                {
                    return;
                }

				Point dataTipPoint = Control.MousePosition;
				Rectangle rect = new Rectangle(m_ToolTip.Location, m_ToolTip.Size);
				if (m_ToolTip.Visible && rect.Contains(dataTipPoint))
				{
					return;
				}

                String leftword = GetWordAtPosition(sci, position);

				if (leftword != null && leftword != String.Empty)
				{
					try
					{
                        Variable var = debugManager.EvaluateCurrent(leftword);

                        if (var != null)
                        {
                            Show(dataTipPoint, var, leftword);
                        }
					}
					catch (Exception){}
				}
			}
		}

        private bool IsIdentifierChar(string characterClass, char c)
        {
            return characterClass.IndexOf(c) >= 0;
        }

		private String GetWordAtPosition(ScintillaControl sci, Int32 position)
		{
            if (position < 0)
                return null;

            string characterClass = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;

            int line = sci.LineFromPosition(position);
            int seek = sci.MBSafeCharPosition(position) - sci.MBSafeCharPosition(sci.PositionFromLine(line));

            string text = sci.GetLine(line);

            if (seek < 0 || seek >= text.Length)
                return null;

            if (!IsIdentifierChar(characterClass, text[seek]) && text[seek] != '.')
            {
                return null;
            }

            // Search from the seek point to the left until we hit a non-alphanumeric which isn't a "."
            // "." must be handled specially so that expressions like player.health are easy to evaluate. 
            int start = seek;

            while (start > 0 && (IsIdentifierChar(characterClass, text[start - 1]) || text[start - 1] == '.'))
            {
                --start;
            }

            // Search from the seek point to the right until we hit a non-alphanumeric
            int end = seek;

            while (end + 1 < text.Length && IsIdentifierChar(characterClass, text[end + 1]))
            {
                ++end;
            }

            return text.Substring(start, end - start + 1);
		}
    }

    public delegate void MouseDownEventHandler(MouseButtons button, Point e);
    public class MouseMessageFilter : IMessageFilter
    {
        public event MouseDownEventHandler MouseDownEvent = null;
        public event EventHandler KeyDownEvent = null;
		private Control[] m_ControlList;

        public void AddControls(Control[] controls)
        {
            m_ControlList = controls;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == Win32.WM_KEYDOWN && m.WParam.ToInt32() == Win32.VK_ESCAPE)
            {
                if (KeyDownEvent != null)
                {
                    KeyDownEvent(null, null);
                }
            }
            if (m.Msg == Win32.WM_LBUTTONDOWN)
            {
				Control target = Control.FromHandle(m.HWnd);
				foreach (Control c in m_ControlList)
                {
					if (c == target || c.Contains(target))
					{
						return false;
					}
                }
				if (MouseDownEvent != null)
				{
					MouseDownEvent(MouseButtons.Left, new Point(m.LParam.ToInt32()));
				}
            }
            return false;
        }

    }

}
