using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace yeetong_VideoGateway
{
    static class Program
    {

        [DllImport("User32.dll")]
        //This function puts the thread that created the specified window into the foreground and activates the window.
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            int flag = 0;
            Process[] processList = System.Diagnostics.Process.GetProcesses();
            foreach (Process p in processList)
            {
                if (p.ProcessName == "yeetong_VideoGateway")
                {
                    flag++;
                }
            }
            if (flag > 1)
            {
                MessageBox.Show("程序已经在运行了");
                System.Environment.Exit(0);
            }
            bool createdNew;
            string appName;
            appName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            appName = appName.Replace(Path.DirectorySeparatorChar, '_'); using (Mutex mutex = new Mutex(true, appName, out createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                else
                {
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
            }
        }
    }
}
