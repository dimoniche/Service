﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace ServiceSaleMachine
{
    public class Globals
    {
        public const string GuidFormat = "D";
        public const string LogExtention = "log";
        public const long GcMinMemoryBlock = 1024 * 1024;

        // Режим отладки
        public static bool IsDebug { get; private set; }

        public static Version DatabaseVersion { get; private set; }
        public static Version ProductVersion { get; set; }
        public static CultureInfo CultureRu { get; private set; }
        public static ClientConfiguration ClientConfiguration { get; set; }
        public static RegistrySettings RegistrySettings { get; private set; }

        // Сообщения об ошибках
        public const string ErrorMessageRegistryDontRead = "Не удалось прочитать реестр.";
        public const string ErrorMessageBadInstallPath = "Не удалось определить путь установки.";
        public const string ErrorMessageBadInstanceId = "Не удалось определить идентификатор установки.";
        public const string ErrorMessageBadServerConfiguration = "Неверные настройки службы.";
        public const string ErrorMessagePortDontOpened = "Не удалось открыть порт {0} для прослушивания.";
        public const string ErrorMessageAccessRight = "Отказано в доступе.";

        static Globals()
        {
#if DEBUG
            IsDebug = true;
#endif
            DatabaseVersion = new Version("1.0");
            ProductVersion = (Version)Assembly.GetExecutingAssembly().GetName().Version.Clone();
            CultureRu = new CultureInfo("ru-RU");
            RegistrySettings = new RegistrySettings();
            ClientConfiguration = new ClientConfiguration();
        }

        public static string GetProcessFileName(bool extension)
        {
            string processPath = Process.GetCurrentProcess().MainModule.FileName;

            if (IsDebug)
            {
                // Удаляем постфикс ведущего процесса, если он есть
                int postFixIndex = processPath.IndexOf(".vshost", StringComparison.InvariantCultureIgnoreCase);
                if (postFixIndex != -1)
                {
                    processPath = processPath.Remove(postFixIndex, ".vshost".Length);
                }
            }

            if (extension)
                return Path.GetFileName(processPath);
            else
                return Path.GetFileNameWithoutExtension(processPath);
        }

        /// <summary>
        /// Возвращает полный путь
        /// </summary>
        public static string GetPath(PathEnum vspPath)
        {
            return GetPath(vspPath, RegistrySettings.InstallPath);
        }

        /// <summary>
        /// Возвращает полный путь, относительно указанного корневого
        /// </summary>
        public static string GetPath(PathEnum vspPath, string rootPath)
        {
            return (vspPath != PathEnum.Root) ? Utils.GetNormalisedFullPath(rootPath, true) + GetRelativePath(vspPath) : Utils.GetNormalisedFullPath(rootPath, false);
        }

        /// <summary>
        /// Возвращает относительный путь
        /// </summary>
        public static string GetRelativePath(PathEnum vspPath)
        {
            switch (vspPath)
            {
                case PathEnum.Bin:
                    return "Bin";

                case PathEnum.Logs:
                    return "Logs";

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Возвращает разницу относительных путей
        /// </summary>
        public static string GetDifferenceOfPaths(PathEnum longPath, PathEnum shortPath)
        {
            string strLong = GetRelativePath(longPath);
            string strShort = GetRelativePath(shortPath) + "\\";

            if (!strLong.StartsWith(strShort)) throw new InvalidOperationException();

            return strLong.Remove(0, strShort.Length);
        }
    }
}