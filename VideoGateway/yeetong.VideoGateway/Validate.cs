using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ToolAPI;
using System.Management;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Data; 

namespace yeetong_VideoGateway
{
   public class ValidateC
    {
       public static bool IsActivation = false;
       public static string Mac = "";
       public static string Projectid = "";
       public static string ProjectName = "";
       public static string ActivationCode = "";
       public static string KEYT = "*Gy9";
       static ValidateC()
       {
           try
           {
               string isActivationTemp = INIOperate.IniReadValue("Identity", "isActivation", AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
               string activationCodeTemp = INIOperate.IniReadValue("Identity", "activationCode", AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
               string macTemp = INIOperate.IniReadValue("Identity", "mac", AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
               string projectidTemp = INIOperate.IniReadValue("Identity", "projectid", AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
               string projectNametemp = INIOperate.IniReadValue("Identity", "projectName", AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
               Mac = CryptoTool.Decrypt_DES(macTemp, KEYT);
               Projectid = CryptoTool.Decrypt_DES(projectidTemp, KEYT);
               ActivationCode = CryptoTool.Decrypt_DES(activationCodeTemp, KEYT);
               ProjectName = CryptoTool.Decrypt_DES(projectNametemp, KEYT);
               if(string.IsNullOrEmpty(isActivationTemp))
               {
                   MessageBox.Show("配置文件丢失，请联系相关技术人员");
                   System.Environment.Exit(0);
               }
               else
               {
                   if (bool.TryParse(CryptoTool.Decrypt_DES(isActivationTemp, KEYT), out IsActivation))
                   {
                       //已经激活
                       if(IsActivation)
                       {
                           if(Mac!= GetMacAddressByNetworkInformation())
                           {
                               MessageBox.Show("权限认证错误");
                               ActivationCode acc = new ActivationCode(false);
                               acc.ShowDialog();
                           }
                       }
                       else//尚未激活
                       {
                           //激活码无效 重新填写
                           ActivationCode acc = new ActivationCode(false);
                           acc.ShowDialog();
                       }
                   }
                    else
                    {//激活码无效 重新填写
                        ActivationCode acc = new ActivationCode(false);
                        acc.ShowDialog();
                    }
               }
           }
           catch (Exception)
           {

           }
       }

       public  static void ValidateCInit() { }

        #region 获取MAC地址
       static public string GetMacAddressByNetworkInformation()
       {
           string key = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
           string macAddress = string.Empty;
           try
           {
               NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
               foreach (NetworkInterface adapter in nics)
               {
                   if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                       && adapter.GetPhysicalAddress().ToString().Length != 0)
                   {
                       string fRegistryKey = key + adapter.Id + "\\Connection";
                       RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                       if (rk != null)
                       {
                           string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                           int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                           if (fPnpInstanceID.Length > 3 &&
                               fPnpInstanceID.Substring(0, 3) == "PCI")
                           {
                               macAddress = adapter.GetPhysicalAddress().ToString();
                               for (int i = 1; i < 6; i++)
                               {
                                   macAddress = macAddress.Insert(3 * i - 1, ":");
                               }
                               break;
                           }
                       }

                   }
               }
           }
           catch (Exception ex)
           {
               //这里写异常的处理  
           }
           return macAddress;
       }
        #endregion
    }
}
