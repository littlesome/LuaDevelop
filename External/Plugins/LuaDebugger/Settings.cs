using System;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Runtime.CompilerServices;
using PluginCore.Localization;
using LayoutManager.Controls;

namespace LuaDebugger
{
    public delegate void PathChangedEventHandler(String path);
	
	[Serializable]
	[DefaultProperty("Path")]
	public class Folder
	{
		private String m_Value;

		public Folder()
		{
			m_Value = "";
		}

		public Folder(String value)
		{
			m_Value = value;
		}

		public override String ToString()
		{
			return m_Value;
		}

		[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
		public String Path
		{
			get { return m_Value; }
			set { m_Value = value; }
		}
	}

    [Serializable]
    public class Settings
    {
        private Boolean m_SaveBreakPoints = true;
        private Boolean m_DisablePanelsAutoshow = false;
        private Boolean m_VerboseOutput = false;
		private String m_SwitchToLayout = null;

        [DisplayName("Save Breakpoints")]
        [LocalizedCategory("LuaDebugger.Category.Misc")]
        [LocalizedDescription("LuaDebugger.Description.SaveBreakPoints")]
		[DefaultValue(true)]
		public bool SaveBreakPoints
		{
			get { return m_SaveBreakPoints; }
			set { m_SaveBreakPoints = value; }
        }

        [DisplayName("Disable Panel Auto Show")]
        [LocalizedCategory("LuaDebugger.Category.Misc")]
        [LocalizedDescription("LuaDebugger.Description.DisablePanelsAutoshow")]
        [DefaultValue(false)]
        public bool DisablePanelsAutoshow
        {
            get { return m_DisablePanelsAutoshow; }
            set { m_DisablePanelsAutoshow = value; }
        }

		[DisplayName("Switch To Layout On Debugger Start")]
		[LocalizedCategory("LuaDebugger.Category.Misc")]
		[LocalizedDescription("LuaDebugger.Description.SwitchToLayout")]
		[DefaultValue(null)]
		[Editor(typeof(LayoutSelectorEditor), typeof(UITypeEditor))]
		public String SwitchToLayout
		{
			get { return m_SwitchToLayout; }
			set { m_SwitchToLayout = value; }
		}

		[DisplayName("Verbose Output")]
        [LocalizedCategory("LuaDebugger.Category.Misc")]
        [LocalizedDescription("LuaDebugger.Description.VerboseOutput")]
        [DefaultValue(false)]
        public bool VerboseOutput
        {
            get { return m_VerboseOutput; }
            set { m_VerboseOutput = value; }
        }
    }
}
