using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using LuaDebugger.Properties;
using PluginCore.Localization;
using PluginCore;
using PluginCore.Managers;
using LuaDebugger.Controls;

namespace LuaDebugger
{
    internal class MenusHelper
    {
        static public ImageList imageList;
        private ToolStripItem[] m_ToolStripButtons;
        private ToolStripSeparator m_ToolStripSeparator;
		private ToolStripButton StartContinueButton, PauseButton, StopButton, CurrentButton, StepButton, NextButton;
        private ToolStripMenuItem AttachToProcessMenu, StartContinueMenu, PauseMenu, StopMenu, CurrentMenu, StepMenu, NextMenu, ToggleBreakPointMenu, ToggleBreakPointEnableMenu, DeleteAllBreakPointsMenu, DisableAllBreakPointsMenu, EnableAllBreakPointsMenu;
        private DebuggerState CurrentState = DebuggerState.Initializing;
        private List<ToolStripItem> debugItems;
        private Settings settingObject;

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public MenusHelper(Image pluginImage, DebuggerManager debugManager, Settings settings)
        {
            settingObject = settings;

            imageList = new ImageList();
            imageList.Images.Add("Attach", PluginBase.MainForm.FindImage("127"));
            imageList.Images.Add("StartContinue", Resource.StartContinue);
			imageList.Images.Add("Pause", Resource.Pause);
			imageList.Images.Add("Stop", Resource.Stop);
			imageList.Images.Add("Current", Resource.Current);
			imageList.Images.Add("Step", Resource.Step);
			imageList.Images.Add("Next", Resource.Next);

            ToolStripMenuItem tempItem;
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewBreakpointsPanel"), pluginImage, new EventHandler(this.OpenBreakPointPanel));
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowBreakpoints", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewLocalVariablesPanel"), pluginImage, new EventHandler(this.OpenLocalVariablesPanel));
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowLocalVariables", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewStackframePanel"), pluginImage, new EventHandler(this.OpenStackframePanel));
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowStackframe", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewWatchPanel"), pluginImage, new EventHandler(this.OpenWatchPanel));
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowWatch", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            viewMenu.DropDownItems.Add(tempItem);
			tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewVMsPanel"), pluginImage, new EventHandler(this.OpenVMsPanel));
			PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowThreads", tempItem);
			viewMenu.DropDownItems.Add(tempItem);

            // Menu           
            ToolStripMenuItem debugMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("DebugMenu");
            if (debugMenu == null)
            {
                debugMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Debug"));
                ToolStripMenuItem insertMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("InsertMenu");
                Int32 idx = PluginBase.MainForm.MenuStrip.Items.IndexOf(insertMenu);
                if (idx < 0) idx = PluginBase.MainForm.MenuStrip.Items.Count - 1;
                PluginBase.MainForm.MenuStrip.Items.Insert(idx, debugMenu);
            }

            AttachToProcessMenu = new ToolStripMenuItem(TextHelper.GetString("Label.AttachToProcess"), imageList.Images["Attach"], new EventHandler(AttachToProcess_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.AttachToProcess", AttachToProcessMenu);

            StartContinueMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Start"), imageList.Images["StartContinue"], new EventHandler(StartContinue_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Start", StartContinueMenu);
            PauseMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Pause"), imageList.Images["Pause"], new EventHandler(debugManager.Pause_Click), Keys.Control | Keys.Shift | Keys.F5);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Pause", PauseMenu);
            StopMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Stop"), imageList.Images["Stop"], new EventHandler(debugManager.Stop_Click), Keys.Shift | Keys.F5);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Stop", StopMenu);
            CurrentMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Current"), imageList.Images["Current"], new EventHandler(debugManager.Current_Click), Keys.Shift | Keys.F10);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Current", CurrentMenu);
            StepMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Step"), imageList.Images["Step"], new EventHandler(debugManager.Step_Click), Keys.F11);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.StepInto", StepMenu);
            NextMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Next"), imageList.Images["Next"], new EventHandler(debugManager.Next_Click), Keys.F10);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.StepOver", NextMenu);

            ToggleBreakPointMenu = new ToolStripMenuItem(TextHelper.GetString("Label.ToggleBreakpoint"), null, new EventHandler(ScintillaHelper.ToggleBreakPoint_Click), Keys.F9);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.ToggleBreakpoint", ToggleBreakPointMenu);
            DeleteAllBreakPointsMenu = new ToolStripMenuItem(TextHelper.GetString("Label.DeleteAllBreakpoints"), null, new EventHandler(ScintillaHelper.DeleteAllBreakPoints_Click), Keys.Control | Keys.Shift | Keys.F9);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.DeleteAllBreakpoints", DeleteAllBreakPointsMenu);
            ToggleBreakPointEnableMenu = new ToolStripMenuItem(TextHelper.GetString("Label.ToggleBreakpointEnabled"), null, new EventHandler(ScintillaHelper.ToggleBreakPointEnable_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.ToggleBreakpointEnabled", ToggleBreakPointEnableMenu);
            DisableAllBreakPointsMenu = new ToolStripMenuItem(TextHelper.GetString("Label.DisableAllBreakpoints"), null, new EventHandler(ScintillaHelper.DisableAllBreakPoints_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.DisableAllBreakpoints", DisableAllBreakPointsMenu);
            EnableAllBreakPointsMenu = new ToolStripMenuItem(TextHelper.GetString("Label.EnableAllBreakpoints"), null, new EventHandler(ScintillaHelper.EnableAllBreakPoints_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.EnableAllBreakpoints", EnableAllBreakPointsMenu);

            debugItems = new List<ToolStripItem>(new ToolStripItem[]
			{
                AttachToProcessMenu, new ToolStripSeparator(),
				StartContinueMenu, PauseMenu, StopMenu, new ToolStripSeparator(),
				CurrentMenu, StepMenu, NextMenu, new ToolStripSeparator(),
				ToggleBreakPointMenu, DeleteAllBreakPointsMenu, ToggleBreakPointEnableMenu ,DisableAllBreakPointsMenu, EnableAllBreakPointsMenu
            });

            debugMenu.DropDownItems.AddRange(debugItems.ToArray());

            // ToolStrip
            m_ToolStripSeparator = new ToolStripSeparator();
            m_ToolStripSeparator.Margin = new Padding(1, 0, 0, 0);
            StartContinueButton = new ToolStripButton(TextHelper.GetString("Label.Start"), imageList.Images["StartContinue"], new EventHandler(StartContinue_Click));
			StartContinueButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			PauseButton = new ToolStripButton(TextHelper.GetString("Label.Pause"), imageList.Images["Pause"], new EventHandler(debugManager.Pause_Click));
			PauseButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			StopButton = new ToolStripButton(TextHelper.GetString("Label.Stop"), imageList.Images["Stop"], new EventHandler(debugManager.Stop_Click));
			StopButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			CurrentButton = new ToolStripButton(TextHelper.GetString("Label.Current"), imageList.Images["Current"], new EventHandler(debugManager.Current_Click));
			CurrentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			StepButton = new ToolStripButton(TextHelper.GetString("Label.Step"), imageList.Images["Step"], new EventHandler(debugManager.Step_Click));
            StepButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			NextButton = new ToolStripButton(TextHelper.GetString("Label.Next"), imageList.Images["Next"], new EventHandler(debugManager.Next_Click));
            NextButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            m_ToolStripButtons = new ToolStripItem[] { m_ToolStripSeparator, StartContinueButton, PauseButton, StopButton, new ToolStripSeparator(), CurrentButton, StepButton, NextButton };
            
            // Events
            PluginMain.debugManager.StateChangedEvent += UpdateMenuState;
        }

        public void AddToolStripItems()
        {
            ToolStrip toolStrip = PluginBase.MainForm.ToolStrip;
            toolStrip.Items.AddRange(m_ToolStripButtons);
        }

        public void OpenLocalVariablesPanel(Object sender, System.EventArgs e)
        {
            PanelsHelper.pluginPanel.Show();
        }

        public void OpenBreakPointPanel(Object sender, System.EventArgs e)
        {
            PanelsHelper.breakPointPanel.Show();
        }

        public void OpenStackframePanel(Object sender, System.EventArgs e)
        {
            PanelsHelper.stackframePanel.Show();
        }

        public void OpenWatchPanel(Object sender, System.EventArgs e)
        {
            PanelsHelper.watchPanel.Show();
        }

        public void OpenVMsPanel(Object sender, System.EventArgs e)
		{
            PanelsHelper.virtualMachinesPanel.Show();
		}

        /// <summary>
        /// 
        /// </summary>
        void StartContinue_Click(Object sender, EventArgs e)
        {
            if (PluginMain.debugManager.IsDebuggerStarted)
            {
                PluginMain.debugManager.Continue_Click(sender, e);
            }
            else PluginMain.debugManager.Start(/*false*/);
		}

        /// <summary>
        /// 
        /// </summary>
        void AttachToProcess_Click(Object sender, EventArgs e)
        {
            AttachProcess dialog = new AttachProcess();
            dialog.ShowDialog();
        }

		#region Menus State Management

		/// <summary>
        /// 
        /// </summary>
        public void UpdateMenuState(object sender)
        {
            UpdateMenuState(sender, CurrentState);
        }
        public void UpdateMenuState(object sender, DebuggerState state)
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
                {
                    UpdateMenuState(sender, state);
                });
                return;
            }
            Boolean hasChanged = CurrentState != state;
            CurrentState = state; // Set current now...
			if (state == DebuggerState.Initializing || state == DebuggerState.Stopped)
			{
                StartContinueButton.Text = StartContinueMenu.Text = TextHelper.GetString("Label.Continue");
			}
			else StartContinueButton.Text = StartContinueMenu.Text = TextHelper.GetString("Label.Continue");
            //
			StopButton.Enabled = StopMenu.Enabled = (state != DebuggerState.Initializing && state != DebuggerState.Stopped);
            PauseButton.Enabled = PauseMenu.Enabled = (state == DebuggerState.Running);
            //
			if (state == DebuggerState.Initializing || state == DebuggerState.Stopped)
			{
                StartContinueButton.Enabled = StartContinueMenu.Enabled = false;
			}
            else if (state == DebuggerState.BreakHalt || state == DebuggerState.ExceptionHalt || state == DebuggerState.PauseHalt)
            {
                StartContinueButton.Enabled = StartContinueMenu.Enabled = true;
            }
			else StartContinueButton.Enabled = StartContinueMenu.Enabled = false;
            //
            Boolean enabled = (state == DebuggerState.BreakHalt || state == DebuggerState.PauseHalt);
            CurrentButton.Enabled = CurrentMenu.Enabled = enabled;
            NextButton.Enabled = NextMenu.Enabled = enabled;
            StepButton.Enabled = StepMenu.Enabled = enabled;
			if (state == DebuggerState.Running)
			{
				PanelsHelper.pluginUI.TreeControl.Clear();
				PanelsHelper.stackframeUI.ClearItem();
			}
            enabled = /*(state != DebuggerState.Running) &&*/ GetLanguageIsValid();
            ToggleBreakPointMenu.Enabled = ToggleBreakPointEnableMenu.Enabled = enabled;
            DeleteAllBreakPointsMenu.Enabled = DisableAllBreakPointsMenu.Enabled = enabled;
            EnableAllBreakPointsMenu.Enabled = PanelsHelper.breakPointUI.Enabled = enabled;
			AttachToProcessMenu.Enabled = (state == DebuggerState.Initializing || state == DebuggerState.Stopped);
            // Notify plugins of main states when state changes...
            if (hasChanged && (state == DebuggerState.Running || state == DebuggerState.Stopped))
            {
                DataEvent de = new DataEvent(EventType.Command, "LuaDebugger." + state.ToString(), null);
                EventManager.DispatchEvent(this, de);
            }
            PluginBase.MainForm.RefreshUI();
        }

        /// <summary>
        /// Gets if the language is valid for debugging
        /// </summary>
        private Boolean GetLanguageIsValid()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable)
            {
                String ext = Path.GetExtension(document.FileName);
                String lang = document.SciControl.ConfigurationLanguage;
                return lang == "lua";
            }
            else return false;
        }

        #endregion

    }

}
