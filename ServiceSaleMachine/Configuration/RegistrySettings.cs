using System;
using System.Linq;
using Microsoft.Win32;

namespace AirVitamin
{
    public class RegistrySettings
    {
        public const string RegistryPathUninstall = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\AirVitamin";

        public string InstallPath { get; private set; }
        //public VspInstallType InstallType { get; private set; }
        public LogMessageTypeEnum LogLevel { get; set; }
        public Guid? InstanceId { get; private set; }

        public RegistrySettings()
        {
            LogLevel = LogMessageTypeEnum.Error;
        }

        public bool Load()
        {
            try
            {
                using (RegistryKey regLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (RegistryKey regUninstall = regLocalMachine.OpenSubKey(RegistryPathUninstall))
                    {
                        if (regUninstall != null)
                        {
                            InstallPath = (string)regUninstall.GetValue("InstallLocation", null);
                            //InstallType = (VspInstallType)Enum.Parse(typeof(VspInstallType), (string)regUninstall.GetValue("InstallType", null));
                            //LogLevel = (LogMessageTypeEnum)Enum.Parse(typeof(LogMessageTypeEnum), (string)regUninstall.GetValue("Settings: LogLevel", LogMessageTypeEnum.Error.ToString()));

                            return true;
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Save()
        {
            try
            {
                using (RegistryKey regLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (RegistryKey regUninstall = regLocalMachine.OpenSubKey(RegistryPathUninstall, true))
                    {
                        if (regUninstall != null)
                        {
                            regUninstall.SetValue("Settings: LogLevel", LogLevel.ToString());
                        }
                    }
                }
                return true;
            }
            catch
            {
            }
            return false;
        }
    }
}