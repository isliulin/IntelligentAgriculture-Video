using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.ServiceModel;

namespace yeetong_SpecialEquipmentServer
{
    partial class RoutedProtocol : ServiceBase
    {
        ServiceHost Host;
        /// <summary>
        /// 
        /// </summary>
        public RoutedProtocol()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                if (Host == null)
                {
                    Host = new ServiceHost(typeof(WcfServiceload.Service1));

                    Host.Open();
                }
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("服务开启异常", ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                if (Host != null)
                {
                    Host.Close();
                }
            }
            catch (Exception ex) {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("服务关闭异常", ex.Message);
            }
        }
    }
}
