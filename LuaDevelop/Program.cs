using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using LuaDevelop;

namespace LuaDevelop
{
    static class Program
    {
        [STAThread]
        static void Main(String[] arguments)
        {
            if (SingleInstanceApp.AlreadyExists)
            {
                Boolean reUse = Array.IndexOf(arguments, "-reuse") > -1;
                if (!MultiInstanceMode || reUse) SingleInstanceApp.NotifyExistingInstance(arguments);
                else RunLuaDevelopWithErrorHandling(arguments, false);
            }
            else RunLuaDevelopWithErrorHandling(arguments, true);
        }

        /// <summary>
        /// Run LuaDevelop and catch any unhandled exceptions.
        /// </summary>
        static void RunLuaDevelopWithErrorHandling(String[] arguments, Boolean isFirst)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm.IsFirst = isFirst;
            MainForm.Arguments = arguments;
            MainForm mainForm = new MainForm();
            SingleInstanceApp.NewInstanceMessage += delegate(Object sender, Object message)
            {
                MainForm.Arguments = message as String[];
                mainForm.ProcessParameters(message as String[]);
            };
            //try
            {
                SingleInstanceApp.Initialize();
                Application.Run(mainForm);
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show("There was an unexpected problem while running LuaDevelop: " + ex.Message, "Error");
            //}
            //finally
            {
                SingleInstanceApp.Close();
            }
        }

        /// <summary>
        /// Checks if we should run in multi instance mode.
        /// </summary>
        public static Boolean MultiInstanceMode
        {
            get 
            {
                String file = Path.Combine(PathHelper.AppDir, ".multi");
                return File.Exists(file);
            }
        }

    }
    
}
