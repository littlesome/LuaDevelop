using PluginCore.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Aga.Controls.Tree;
using System.Runtime.InteropServices;
using PluginCore;
using LuaDebugger.Properties;
using PluginCore.Localization;

namespace LuaDebugger.Controls
{
    public partial class AttachProcess : SmartForm
    {
        class ProcessNode : Node, IComparable<ProcessNode>
        {
            public int id;
            public string name;
            public string title;
            public int parentId = -1;

            public override string Text { get { return name; } }
            public int PID { get { return id; } }
            public string Name { get { return name; } }
            public string Title { get { return title; } }

            public int CompareTo(ProcessNode other)
            {
                return name.CompareTo(other.name);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PROCESS_BASIC_INFORMATION
        {
            public int ExitStatus;
            public int PebBaseAddress;
            public int AffinityMask;
            public int BasePriority;
            public int UniqueProcessId;
            public int InheritedFromUniqueProcessId;
        }

        [DllImport("ntdll.dll")]
        static extern int NtQueryInformationProcess(
            IntPtr ProcessHandle,
            int ProcessInformationClass,
            out PROCESS_BASIC_INFORMATION ProcessInformation,
            int ProcessInformationLength,
            out int ReturnLength
            );

        [DllImport("shell32.dll")]
        static extern int ExtractIconEx(
            string lpszFile,
            int nIconIndex,
            IntPtr[] phIconLarge, 
            IntPtr[] phIconSmall,
            int nIcons);

        [DllImport("user32.dll")]
        static extern int DestroyIcon(IntPtr hIcon);

        [DllImport("kernel32.dll")]
        private static extern bool QueryFullProcessImageName(
            IntPtr hprocess, 
            int dwFlags,
            StringBuilder lpExeName, 
            out int size);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(
            uint dwDesiredAccess,
            bool bInheritHandle, 
            int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        private Dictionary<string, Image> IconMap = new Dictionary<string,Image>();
        private Image ExeIcon = Resource.ExeIcon;

        public AttachProcess()
        {
            this.Font = PluginBase.Settings.DefaultFont;
            this.FormGuid = "8eed2732-f428-4c66-ba6f-cf308945c7e6";

            InitializeComponent();

            Text = TextHelper.GetString("Title.AttachToProcess");

            btnOK.Text = TextHelper.GetString("Label.Attach");
            btnCancel.Text = TextHelper.GetString("Label.Close");
            btnRefresh.Text = TextHelper.GetString("Label.Refresh");

            RefreshProcessTree();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshProcessTree();
        }

        private void RefreshProcessTree()
        {
            Dictionary<int, ProcessNode> dict = new Dictionary<int, ProcessNode>();

            foreach (Process p in Process.GetProcesses())
            {
                ProcessNode node;
                ProcessNode parentNode;
                
                if (dict.ContainsKey(p.Id))
                {
                    node = dict[p.Id];
                }
                else
                {
                    node = new ProcessNode();
                    dict[p.Id] = node;
                }

                node.id = p.Id;
                node.title = p.MainWindowTitle;
                node.name = p.ProcessName;

                try
                {
                    Icon icon = ExtractIcon(GetExecutablePath(p));
                    if (icon != null)
                        node.Image = icon.ToBitmap();
                    else
                        node.Image = ExeIcon;
                }
                catch
                {
                    node.Image = ExeIcon;
                }

                try
                {
                    PROCESS_BASIC_INFORMATION pbi;
                    int size;

                    NtQueryInformationProcess(p.Handle, 0, out pbi, Marshal.SizeOf(typeof(PROCESS_BASIC_INFORMATION)), out size);

                    int parentId = pbi.InheritedFromUniqueProcessId;

                    if (Process.GetProcessById(parentId) != null)
                    {
                        node.parentId = parentId;

                        if (dict.ContainsKey(parentId))
                        {
                            parentNode = dict[parentId];
                        }
                        else
                        {
                            parentNode = new ProcessNode();
                            dict[parentId] = parentNode;
                        }

                        parentNode.Nodes.Add(node);
                    }
                }
                catch
                {
                }
            }

            List<ProcessNode> nodes = new List<ProcessNode>();

            foreach (KeyValuePair<int, ProcessNode> kv in dict)
            {
                if (kv.Value.parentId == -1)
                {
                    nodes.Add(kv.Value);
                }
            }

            nodes.Sort();

            TreeModel model = new TreeModel();
            foreach (Node n in nodes)
                model.Nodes.Add(n);
            _tree.Model = model;
            _tree.ExpandAll();
        }

        private Icon ExtractIcon(string file)
        {
            IntPtr[] hIconEx = new IntPtr[1] {IntPtr.Zero};

            ExtractIconEx(file, 0, null, hIconEx, 1);

            if (hIconEx[0] != IntPtr.Zero)
            {
                Icon icon = (Icon)Icon.FromHandle(hIconEx[0]).Clone();
                DestroyIcon(hIconEx[0]);

                return icon;
            }

            return null;
        }

        private static string GetExecutablePath(Process process)
        {
            //If running on Vista or later use the new function
            if (Environment.OSVersion.Version.Major >= 6)
            {
                return GetExecutablePathAboveVista(process.Id);
            }

            return process.MainModule.FileName;
        }

        private static string GetExecutablePathAboveVista(int processId)
        {
            var buffer = new StringBuilder(1024);
            IntPtr hprocess = OpenProcess(0x00000400, false, processId);
            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    int size = buffer.Capacity;
                    if (QueryFullProcessImageName(hprocess, 0, buffer, out size))
                    {
                        return buffer.ToString();
                    }
                }
                finally
                {
                    CloseHandle(hprocess);
                }
            }
            throw new Exception("Can not get executable path for process " + processId.ToString());
        }

        private void AttachSelectedProcess()
        {
            if (_tree.SelectedNode != null)
            {
                ProcessNode node = _tree.SelectedNode.Tag as ProcessNode;

                if (node != null)
                {
                    PluginMain.debugManager.AttachToProcess((uint)node.PID);
                }

                Close();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            AttachSelectedProcess();
        }

        private void _tree_DoubleClick(object sender, EventArgs e)
        {
            AttachSelectedProcess();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
