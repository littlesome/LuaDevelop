using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Projects;
using ScintillaNet;
using PluginCore;
using PluginCore.Helpers;
using System.Text;
using Decoda;
using System.Xml;

namespace LuaDebugger
{
    public delegate void StateChangedEventHandler(object sender, DebuggerState state);

	public enum DebuggerState
	{
		Initializing,
		Stopped,
		Starting,
		Running,
		Pausing,
		PauseHalt,
		BreakHalt,
		ExceptionHalt
	}

    public class Location
    {
        public uint index;
        public uint line;

        public Location(uint i, uint l)
        {
            index = i;
            line = l;
        }

        public override int GetHashCode()
        {
            return (int)(line + index);
        }

        public static bool operator ==(Location l, Location r)
        {
            return object.Equals(l, r);
        }

        public static bool operator !=(Location l, Location r)
        {
            return !object.Equals(l, r);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            Location rhs = (Location)obj;

            return rhs.index == this.index && rhs.line == this.line;
        }
    }

    public struct StackFrameInfo
    {
        public string file;
        public string function;
        public uint line;
        public bool available;
    }

    public class Variable
    {
        private XmlNode mNode;
        private string mName;

        public Variable(XmlNode node, string name)
        {
            mNode = node;
            mName = name;
        }

        public string Name
        {
            get { return mName; }
        }
        public string Type
        {
            get { return mNode.Name; }
        }

        public bool HasChild
        {
            get { return mNode.Name == "values" || mNode.Name == "table"; }
        }

        public List<Variable> Memebrs
        {
            get 
            {
                List<Variable> ret = new List<Variable>();

                if (mNode.Name == "values")
                {
                    int index = 1;
                    XmlNode node = mNode.FirstChild;
                    while (node != null)
                    {
                        Variable var = new Variable(node, index.ToString());

                        ret.Add(var);

                        node = node.NextSibling;
                        index++;
                    }
                }
                else if (mNode.Name == "table")
                {
                    XmlNodeList elements = mNode.SelectNodes("element");
                    for (int i = 0; i < elements.Count; i++)
                    {
                        XmlNode e = elements[i];

                        string dummy;
                        XmlNode ek = e.SelectSingleNode("key").FirstChild;
                        XmlNode ev = e.SelectSingleNode("data").FirstChild;

                        string k = Parse(ek, out dummy);

                        Variable var = new Variable(ev, k);
                        ret.Add(var);
                    }
                }

                return ret;
            }
        }

        override public string ToString()
        {
            string type;

            return Parse(mNode, out type);
        }

        private string Parse(XmlNode node, out string type)
        {
            type = "";

            string text = "";

            switch (node.Name)
            {
                case "error":
                    text = node.FirstChild.Value;
                    break;
                case "value":
                    {
                        text = node.SelectSingleNode("data").FirstChild.Value;
                        type = node.SelectSingleNode("type").FirstChild.Value;
                    }
                    break;
                case "function":
                    {
                        text = "function";
                        type = "function";
                    }
                    break;
                case "values":
                    {
                        XmlNode e = node.FirstChild;
                        while (e != null)
                        {
                            if (text.Length > 0)
                            {
                                text += ", ";
                            }

                            string dummy;
                            text += Parse(node, out dummy);

                            e = e.NextSibling;
                        }
                    }
                    break;
                case "table":
                    {
                        XmlNodeList elements = node.SelectNodes("element");
                        for (int i = 0; i < elements.Count && i <= 4; i++)
                        {
                            XmlNode e = elements[i];

                            if (i == 4)
                                text += "...";
                            else
                            {
                                string dummy;
                                XmlNode ek = e.SelectSingleNode("key").FirstChild;
                                XmlNode ev = e.SelectSingleNode("data").FirstChild;

                                string k = Parse(ek, out dummy);
                                string v = Parse(ev, out dummy);

                                text += k + "=" + v + " ";
                            }
                        }
                    }
                    break;
            }

            return text;
        }
    }

	public class DebuggerManager : IEvtHandler
    {
        public event StateChangedEventHandler StateChangedEvent;

        internal Project currentProject;
		private Dictionary<String, uint> m_PathMap = new Dictionary<String, uint>();
        private int m_CurrentFrame = 0;
        private DebugFrontend debugger = DebugFrontend.Get();
        private uint m_CurrentVM = 0;
        private Location m_CurrentLocation;
        private Dictionary<uint, String> vms = new Dictionary<uint,string>();
        private BreakPointManager breakPointManager = PluginMain.breakPointManager;
        private bool isAttached;

        public DebuggerManager()
        {
            debugger.SetEventHandler(this);
        }

        #region Startup

        /// <summary>
        /// 
        /// </summary>
        internal bool Start()
        {
            IProject project = PluginBase.CurrentProject;
            if (project == null) return false;
            currentProject = project as Project;

            if (debugger.Start(currentProject.StartupCommand, currentProject.StartupArgument, currentProject.WorkingDir, currentProject.SymbolPath, true, false))
            {
                isAttached = false;
                DebugStarted();
            }

            return true;
        }
        public bool AttachToProcess(uint pid)
        {
            IProject project = PluginBase.CurrentProject;
            if (project == null) return false;
            currentProject = project as Project;

            if (debugger.Attach(pid, ""))
            {
                isAttached = true;
                DebugStarted();
            }

            return true;
        }

        private void DebugStarted()
        {
            UpdateMenuState(DebuggerState.Running);

            NotifyEvent ne = new NotifyEvent(EventType.ProcessStart);
            EventManager.DispatchEvent(this, ne);

            PluginBase.MainForm.ProgressBar.Visible = false;
            PluginBase.MainForm.ProgressLabel.Visible = false;
            
            if (PluginMain.settingObject.SwitchToLayout != null)
            {
                // save current state
                String oldLayoutFile = Path.Combine(Path.Combine(PathHelper.DataDir, "LuaDebugger"), "oldlayout.ldl");
                PluginBase.MainForm.DockPanel.SaveAsXml(oldLayoutFile);
                PluginBase.MainForm.CallCommand("RestoreLayout", PluginMain.settingObject.SwitchToLayout);
            }
            else if (!PluginMain.settingObject.DisablePanelsAutoshow)
            {
                PanelsHelper.watchPanel.Show();
                PanelsHelper.pluginPanel.Show();
                PanelsHelper.virtualMachinesPanel.Show();
                PanelsHelper.breakPointPanel.Show();
                PanelsHelper.stackframePanel.Show();
            }
        }

        #endregion

        #region Properties

        public string CurrentFullFilePath
        { 
            get
            {
                if (m_CurrentLocation != null)
                {
                    DebugFrontend.Script script = debugger.GetScript(m_CurrentLocation.index);

                    if (script != null)
                    {
                        return GetLocalPath(script.name);
                    }
                }

                return null;
            }
        }

        public Location CurrentLocation
        {
            get { return m_CurrentLocation; }
            set 
            {
                if (m_CurrentLocation != value)
                {
                    if (m_CurrentLocation != null)
                    {
                        ResetCurrentLocation();
                    }
                    m_CurrentLocation = value;
                    if (m_CurrentLocation != null)
                    {
                        GotoCurrentLocation(true);
                    }
                }
            }
        }

        public int CurrentFrame
        {
            get { return m_CurrentFrame; }
            set
            {
                m_CurrentFrame = value;
                UpdateLocalsUI();
            }
        }

        public uint CurrentVM
        {
            get { return m_CurrentVM; }
            set
            {
                if (m_CurrentVM != value)
                {
                    m_CurrentVM = value;
                    UpdateLocalsUI();
                }
            }
        }

        public bool IsDebuggerStarted
        {
            get { return debugger.GetState() != DebugFrontend.State.State_Inactive; }
        }

        public bool IsDebuggerSuspended
        {
            get { return debugger.GetState() == DebugFrontend.State.State_Broken; }
        }

        #endregion

		#region LayoutHelpers

		/// <summary>
		/// Check if old layout is sotred and restores it. It also deletes this temporary layout file.
		/// </summary>
		public void RestoreOldLayout()
		{
			String oldLayoutFile = Path.Combine(Path.Combine(PathHelper.DataDir, "LuaDebugger"), "oldlayout.ldl");
			if (File.Exists(oldLayoutFile))
			{
				PluginBase.MainForm.CallCommand("RestoreLayout", oldLayoutFile);
				File.Delete(oldLayoutFile);
			}
		}

		#endregion

        #region Control

        /// <summary>
        /// 
        /// </summary>
        public void Cleanup()
        {
			m_PathMap.Clear();
			debugger.Shutdown();
            vms.Clear();
        }

        public Variable EvaluateCurrent(string expression)
        {
            DebugFrontend.StackFrame frame = debugger.GetStackFrame((uint)m_CurrentFrame);

            if (frame != null)
            {
                if ((int)frame.stackLevel != -1)
                {
                    string str = debugger.Evaluate(m_CurrentVM, expression, frame.stackLevel);

                    if (str.Length > 0)
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(str);

                        return new Variable(xml.DocumentElement, expression);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateMenuState(DebuggerState state)
        {
            if (StateChangedEvent != null) StateChangedEvent(this, state);
        }

        #endregion

		#region Decoda Events

		/// <summary>
        /// 
        /// </summary>
        /// 
        override public void AddPendingEvent(DebugEvent e)
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                DebugEvent clone = e.Clone(); 
                
                (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
                {
                    AddPendingEvent(clone);
                });
                return;
            }

            ProcessEvent(e);
        }

        private void ProcessEvent(DebugEvent e)
        {
            switch (e.GetEventId())
            {
                case EventId.EventId_CreateVM:
                    OnCreateVM(e);
                    break;
                case EventId.EventId_DestroyVM:
                    OnDestroyVM(e);
                    break;
                case EventId.EventId_LoadScript:
                    OnLoadScript(e);
                    break;
                case EventId.EventId_Break:
                    OnBreak(e);
                    break;
                case EventId.EventId_SetBreakpoint:
                    OnSetBreakpoint(e);
                    break;
                case EventId.EventId_Exception:
                    OnException(e);
                    break;
                case EventId.EventId_LoadError:
                    OnLoadError(e);
                    break;
                case EventId.EventId_Message:
                    OnMessage(e);
                    break;
                case EventId.EventId_SessionEnd:
                    OnSessionEnd(e);
                    break;
                case EventId.EventId_NameVM:
                    OnNameVM(e);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnCreateVM(DebugEvent e)
        {
            uint vm = e.GetVm();

            vms.Add(vm, vm.ToString("X"));

            UpdateVirtualMachinesUI();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroyVM(DebugEvent e)
        {
            uint vm = e.GetVm();

            vms.Remove(vm);

            UpdateVirtualMachinesUI();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnNameVM(DebugEvent e)
        {
            uint vm = e.GetVm();

            vms[vm] = e.GetMessageStr();

            UpdateVirtualMachinesUI();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnLoadScript(DebugEvent e)
        {
            // Sync up the breakpoints for this file.

            uint scriptIndex = e.GetScriptIndex();

            DebugFrontend.Script script = debugger.GetScript(scriptIndex);

            string fullPath = GetLocalPath(script.name);

            if (fullPath != null)
                m_PathMap[fullPath] = scriptIndex;

            foreach (BreakPointInfo info in breakPointManager.BreakPoints)
            {
                if (info.FileFullPath == fullPath && info.Line > 0 && info.IsEnabled && !info.IsDeleted)
                { 
                    debugger.ToggleBreakpoint(e.GetVm(), scriptIndex, (uint)info.Line);
                }
            }

            // Tell the backend we're done processing this script for loading.
            debugger.DoneLoadingScript(e.GetVm());
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnSetBreakpoint(DebugEvent e)
		{
		}

        /// <summary>
        /// 
        /// </summary>
        private void OnBreak(DebugEvent e)
        {
            m_CurrentVM = e.GetVm();

            UpdateUI(DebuggerState.BreakHalt);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnException(DebugEvent e)
        {
            TraceManager.AddAsync(e.GetMessageStr(), (int)TraceType.Error);

            UpdateUI(DebuggerState.ExceptionHalt);

            DialogResult result = MessageBox.Show(e.GetMessageStr(), "Exception", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation);

            if (result == DialogResult.Ignore)
            {
                debugger.Continue(m_CurrentVM);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnLoadError(DebugEvent e)
        {
            TraceManager.AddAsync(e.GetMessageStr(), (int)TraceType.Error);

            UpdateUI(DebuggerState.ExceptionHalt);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnSessionEnd(DebugEvent e)
		{
            Cleanup();

			UpdateMenuState(DebuggerState.Stopped);

            NotifyEvent ne = new NotifyEvent(EventType.ProcessEnd);
            EventManager.DispatchEvent(this, ne);

			if (PluginMain.settingObject.SwitchToLayout != null)
			{
				RestoreOldLayout();
			}
            else if (!PluginMain.settingObject.DisablePanelsAutoshow)
            {
                PanelsHelper.watchPanel.Hide();
                PanelsHelper.pluginPanel.Hide();
                PanelsHelper.virtualMachinesPanel.Hide();
                PanelsHelper.breakPointPanel.Hide();
                PanelsHelper.stackframePanel.Hide();
            }

			PanelsHelper.pluginUI.TreeControl.Nodes.Clear();
			PanelsHelper.stackframeUI.ClearItem();
			PanelsHelper.watchUI.Clear();
            PanelsHelper.virtualMachinesUI.ClearItem();
			PluginMain.breakPointManager.ResetAll();
            PluginBase.MainForm.ProgressBar.Visible = false;
            PluginBase.MainForm.ProgressLabel.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMessage(DebugEvent e)
        {
            MessageType type = e.GetMessageType();
            int state = (int)TraceType.Info;

            if (type == MessageType.MessageType_Warning)
            {
                state = (int)TraceType.Warning;
            }
            else if (type == MessageType.MessageType_Error)
            {
                state = (int)TraceType.Error;
            }

            TraceManager.AddAsync(e.GetMessageStr(), state);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateUI(DebuggerState state)
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					UpdateUI(state);
				});
				return;
			}

            try
            {
                if (debugger.GetNumStackFrames() > 0)
                {
                    DebugFrontend.StackFrame frame = debugger.GetStackFrame(0);

                    CurrentLocation = new Location(frame.scriptIndex, frame.line);
                }
                else
                    CurrentLocation = null;

                UpdateStackUI();
                UpdateLocalsUI();
                UpdateMenuState(state);
				UpdateVirtualMachinesUI();

                (PluginBase.MainForm as Form).Activate();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError("Internal Debugger Exception", ex);
            }
		}

        /// <summary>
        /// 
        /// </summary>
        private void UpdateStackUI()
		{
			m_CurrentFrame = 0;

            uint num = debugger.GetNumStackFrames();

            if (num > 0)
            {
                StackFrameInfo[] frames = new StackFrameInfo[num];

                for (uint i = 0; i < debugger.GetNumStackFrames(); i++)
                {
                    DebugFrontend.StackFrame frame = debugger.GetStackFrame(i);
                    DebugFrontend.Script script = debugger.GetScript(frame.scriptIndex);

                    if (script != null)
                    {
                        frames[i].file = script.name;
                        frames[i].available = true;
                    }
                    else
                    {
                        frames[i].file = "";
                        frames[i].available = false;
                    }

                    frames[i].function = frame.function;
                    frames[i].line = frame.line;
                }
                
                PanelsHelper.stackframeUI.AddFrames(frames);
            }
            else
                PanelsHelper.stackframeUI.ClearItem();
		}

        /// <summary>
        /// 
        /// </summary>
		private void UpdateLocalsUI()
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					UpdateLocalsUI();
				});
				return;
			}

            if (debugger.GetNumStackFrames() > (uint)m_CurrentFrame)
            {
                DebugFrontend.StackFrame frame = debugger.GetStackFrame((uint)m_CurrentFrame);

                if (frame != null)
                    CurrentLocation = new Location(frame.scriptIndex, frame.line);
            }
            else
                CurrentLocation = null;

			PanelsHelper.watchUI.UpdateElements();
		}

		private void UpdateVirtualMachinesUI()
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					UpdateVirtualMachinesUI();
				});
				return;
			}

            PanelsHelper.virtualMachinesUI.SetThreads(vms);
		}

        public string GetLocalPath(string file)
        {
            string fileFullPath = currentProject.GetAbsolutePath(file);

            if (File.Exists(fileFullPath))
            {
                return fileFullPath;
            }

            return null;
        }

        public void ToggleBreakpoint(string fullPath, uint line)
        {
            if (m_PathMap.ContainsKey(fullPath))
            {
                uint scriptIndex = m_PathMap[fullPath];

                debugger.ToggleBreakpoint(m_CurrentVM, scriptIndex, line);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetCurrentLocation()
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					ResetCurrentLocation();
				});
				return;
			}

            ScintillaControl sci;
            String localPath = CurrentFullFilePath;
            if (localPath != null)
            {
                sci = ScintillaHelper.GetScintillaControl(localPath);
                        
                if (sci != null)
                {
                    Int32 i = ScintillaHelper.GetScintillaControlIndex(sci);
                    if (i != -1)
                    {
                        int line = (int)CurrentLocation.line;
                                
                        sci.MarkerDelete(line, ScintillaHelper.markerCurrentLine);
                    }
                }
            }
		}

        /// <summary>
        /// 
        /// </summary>
        private void GotoCurrentLocation(bool bSetMarker)
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					GotoCurrentLocation(bSetMarker);
				});
				return;
			}

            ScintillaControl sci;
            String localPath = CurrentFullFilePath;
            if (localPath != null)
            {
                sci = ScintillaHelper.GetScintillaControl(localPath);
                if (sci == null)
                {
                    PluginBase.MainForm.OpenEditableDocument(localPath);
                    sci = ScintillaHelper.GetScintillaControl(localPath);
                }
                if (sci != null)
                {
                    Int32 i = ScintillaHelper.GetScintillaControlIndex(sci);
                    if (i != -1)
                    {
                        PluginBase.MainForm.Documents[i].Activate();
                        int line = (int)CurrentLocation.line;
                        sci.GotoLine(line);

                        if (bSetMarker)
                        {
                            sci.MarkerAdd(line, ScintillaHelper.markerCurrentLine);
                        }
                    }
                }
            }
		}

        #endregion

        #region MenuItem Event Handling

        /// <summary>
        /// 
        /// </summary>
        internal void Stop_Click(Object sender, EventArgs e)
        {
            PluginMain.liveDataTip.Hide();
            CurrentLocation = null;
            debugger.Stop(!isAttached);
        }

		/// <summary>
		/// 
		/// </summary>
		internal void Current_Click(Object sender, EventArgs e)
		{
            if (IsDebuggerStarted && IsDebuggerSuspended)
            {
                GotoCurrentLocation(false);
            }
		}

		/// <summary>
        /// 
        /// </summary>
        internal void Next_Click(Object sender, EventArgs e)
        {
            CurrentLocation = null;
            debugger.StepOver(m_CurrentVM);
			UpdateMenuState(DebuggerState.Running);
		}

        /// <summary>
        /// 
        /// </summary>
        internal void Step_Click(Object sender, EventArgs e)
        {
            CurrentLocation = null;
            debugger.StepInto(m_CurrentVM);
			UpdateMenuState(DebuggerState.Running);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Continue_Click(Object sender, EventArgs e)
        {
            try
            {
                CurrentLocation = null;
                debugger.Continue(m_CurrentVM);
				UpdateMenuState(DebuggerState.Running);
				UpdateVirtualMachinesUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Pause_Click(Object sender, EventArgs e)
        {
            CurrentLocation = null;
            debugger.Break(m_CurrentVM);
        }

        #endregion

    }

}
