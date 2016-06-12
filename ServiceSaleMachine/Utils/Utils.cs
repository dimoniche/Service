using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace ServiceSaleMachine
{
    internal class Utils
    {
        public const string RegServiceStartModeArgument = "StartMode";

        public static bool SafelyDeleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryOpenFileForWrite(string filePath)
        {
            try
            {
                using (FileStream file = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Возвращает имя папки из указанного пути, без обратного слеша в конце
        /// </summary>
        public static string GetNormalisedFullPath(string path, bool useEndBackSlash)
        {
            return NormalizePath(Path.GetFullPath(path), useEndBackSlash);
        }

        public static string NormalizePath(string path, bool useEndBackSlash)
        {
            StringBuilder builder = new StringBuilder(Path.GetFullPath(path));
            if (builder.Length > 0 && builder[builder.Length - 1] == '\\')
            {
                if (!useEndBackSlash)
                    builder.Length--;
            }
            else if (useEndBackSlash)
            {
                builder.Append('\\');
            }
            return builder.ToString();
        }

        public static bool RegService(RegServiceActionEnum action, string fileName)
        {
            return RegService(action, (object)fileName, null);
        }

        public static bool RegService(RegServiceActionEnum action, string fileName, ServiceStartMode startMode)
        {
            return RegService(action, (object)fileName, startMode);
        }

        public static bool RegService(RegServiceActionEnum action, Assembly assembly)
        {
            return RegService(action, (object)assembly, null);
        }

        public static bool RegService(RegServiceActionEnum action, Assembly assembly, ServiceStartMode startMode)
        {
            return RegService(action, (object)assembly, startMode);
        }

        private static bool RegService(RegServiceActionEnum action, object module, ServiceStartMode? startMode = null)
        {
            try
            {
                string logFileName;
                string stateFileName;

                using (AssemblyInstaller installer = new AssemblyInstaller())
                {
                    if (module is Assembly)
                    {
                        installer.Assembly = (Assembly)module;
                    }
                    else if (module is string)
                    {
                        installer.Path = (string)module;
                    }
                    else throw new InvalidCastException("module");

                    string stateDirName = Utils.GetNormalisedFullPath(installer.Path, false);
                    logFileName = stateDirName + "\\" + Path.GetFileNameWithoutExtension(installer.Path) + ".InstallLog";
                    stateFileName = stateDirName + "\\" + Path.GetFileNameWithoutExtension(installer.Path) + ".InstallState";

                    List<string> commandLine = new List<string>();
                    commandLine.Add("/LogToConsole=false");
                    if (action == RegServiceActionEnum.Reg && startMode != null)
                        commandLine.Add(string.Format("/{0}={1}", RegServiceStartModeArgument, startMode.Value.ToString()));
                    commandLine.Add("/InstallStateDir=" + stateDirName);
                    commandLine.Add("/LogFile=" + logFileName);
                    installer.CommandLine = commandLine.ToArray();
                    installer.UseNewContext = true;

                    Hashtable savedState = new Hashtable();

                    try
                    {
                        if (action == RegServiceActionEnum.Unreg)
                        {
                            installer.Uninstall(savedState);
                        }
                        else
                        {
                            installer.Install(savedState);
                            installer.Commit(savedState);
                        }
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(savedState);
                        }
                        catch
                        {
                        }
                        throw;
                    }
                }

                // Если не было ошибок, то удаляем файлы журналов
                if (!string.IsNullOrWhiteSpace(logFileName))
                    SafelyDeleteFile(logFileName);
                if (!string.IsNullOrWhiteSpace(stateFileName))
                    SafelyDeleteFile(stateFileName);

                return true;
            }
            catch
            {
            }
            return false;
        }

        public static Mutex CreateSingleMutex(string mutexName)
        {
            Mutex mutex = null;
            try
            {
                bool createdNew;
                Mutex tempMutex = new Mutex(true, mutexName, out createdNew);
                if (createdNew)
                    mutex = tempMutex;
            }
            catch
            {
            }
            return mutex;
        }
    }
}