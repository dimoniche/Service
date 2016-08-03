using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ServiceSaleMachine
{
    public static class CommonHelper
    {
        /// <summary>
        /// Определяет, допускается ли приведение объекта к указанному типу
        /// </summary>
        public static bool IsAssignableTo(this object sourceObject, Type destinationType)
        {
            return IsAssignableTo(sourceObject.GetType(), destinationType);
        }

        /// <summary>
        /// Определяет, допускается ли приведение первого типа ко второму
        /// </summary>
        public static bool IsAssignableTo(this Type sourceType, Type destinationType)
        {
            return destinationType.IsAssignableFrom(sourceType);
        }

        /// <summary>
        /// Возвращает значение атрибута DescriptionAttribute для указанного объекта, если оно задано, в противном случае возвращает результат метода ToString()
        /// </summary>
        public static string GetDescription<T>(this T first)
        {
            FieldInfo fieldInfo = first.GetType().GetField(first.ToString());
            if (fieldInfo != null)
            {
                DescriptionAttribute attr = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).OfType<DescriptionAttribute>().FirstOrDefault();
                if (attr != null)
                    return attr.Description;
            }
            return first.ToString();
        }

        /// <summary>
        /// Возвращает массив всех значений в указанном типе перечисления
        /// </summary>
        public static T[] GetEnumValues<T>() where T : struct
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        /// <summary>
        /// Возвращает массив простых значений в указанном типе перечисления
        /// </summary>
        public static HashSet<T> GetEnumFlagValues<T>() where T : struct
        {
            T[] allEnumValues = (T[])Enum.GetValues(typeof(T));

            HashSet<T> result = new HashSet<T>();

            foreach (T enumValue in allEnumValues)
            {
                int iValue = (int)Convert.ToInt32(enumValue);

                if (iValue != 0)
                {
                    if (!result.Any(c => (iValue & Convert.ToInt32(c)) == Convert.ToInt32(c)))
                    {
                        result.Add(enumValue);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Возвращает массив простых значений в указанном значении перечисления
        /// </summary>
        public static HashSet<T> GetEnumUsedFlagValues<T>(T val) where T : struct
        {
            HashSet<T> flags = GetEnumFlagValues<T>();
            flags.RemoveWhere(c => (Convert.ToInt32(val) & Convert.ToInt32(c)) != Convert.ToInt32(c));
            return flags;
        }

        /// <summary>
        /// Создает список HashSet(T) из объекта IEnumerable(T)
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection)
        {
            return new HashSet<T>(collection);
        }

        /// <summary>
        /// Создает список HashSet(T) из объекта ICollection
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this ICollection collection)
        {
            HashSet<T> result = new HashSet<T>();
            foreach (T item in collection)
            {
                result.Add(item);
            }
            return result;
        }

        public static void AddRange<T>(this HashSet<T> hasSet, IEnumerable<T> collection)
        {
            foreach (T item in collection)
                hasSet.Add(item);
        }

        /// <summary>
        /// Переводи строку HEX-формата в байты
        /// </summary>
        public static byte[] GetBytesFromHexString(string inHexString)
        {
            byte[] bytes = new byte[inHexString.Length / 2];
            for (int i = 0; i < inHexString.Length; i += 2)
            {
                try
                {
                    bytes[i / 2] = (byte)int.Parse(inHexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    return null;
                }
            }
            return bytes;
        }

        public static object TryParse<T>(string str, bool throwOnError = true)
        {
            try
            {
                return Enum.Parse(typeof(T), str);
            }
            catch
            {
                if (throwOnError) throw new InvalidCastException("Не удалось найти определение для значения \"" + str + "\" перечисления \"" + typeof(T).Name + "\".");
                return null;
            }
        }

        public static int GetVersionFieldCount(this Version version)
        {
            if (version.Revision > 0) return 4;
            else if (version.Build > 0) return 3;
            else if (version.Minor > 0) return 2;
            else return 1;
        }

        public static string GetVersionText(this Version version, int minFieldCount)
        {
            return version.ToString(Math.Max(GetVersionFieldCount(version), minFieldCount));
        }

        /// <summary>
        /// Сравнение без учета количества задействованных полей
        /// </summary>
        public static int VspCompareTo(this Version ver1, Version ver2)
        {
            if (ver1 == null || ver2 == null) throw new ArgumentNullException();
            Version tempVer1 = new Version(ver1.Major == -1 ? 0 : ver1.Major, ver1.Minor == -1 ? 0 : ver1.Minor, ver1.Build == -1 ? 0 : ver1.Build, ver1.Revision == -1 ? 0 : ver1.Revision);
            Version tempVer2 = new Version(ver2.Major == -1 ? 0 : ver2.Major, ver2.Minor == -1 ? 0 : ver2.Minor, ver2.Build == -1 ? 0 : ver2.Build, ver2.Revision == -1 ? 0 : ver2.Revision);
            return tempVer1.CompareTo(tempVer2);
        }

        public static string GetDebugInformation(this Exception ex)
        {
            string str = (string.IsNullOrWhiteSpace(ex.Message) ? "<null>" : ex.Message) + Environment.NewLine + Environment.NewLine;
            str += "Тип Исключения: " + ex.GetType() + Environment.NewLine;
            str += "Трассировка стека:" + Environment.NewLine + (string.IsNullOrWhiteSpace(ex.StackTrace) ? "<null>" : ex.StackTrace);
            if (ex.InnerException != null)
            {
                str += Environment.NewLine + "----------" + Environment.NewLine;
                str += ex.InnerException.GetDebugInformation();
            }
            return str;
        }

        /// <summary>
        /// Считывает необходимое количество байт с указанной позиции
        /// </summary>
        public static int ReadAt(this Stream stream, long position, byte[] buffer, int offset, int count)
        {
            if (stream.Position != position) stream.Position = position;
            return stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Считывает необходимое количество байт с указанной позиции
        /// </summary>
        public static byte[] ReadAt(this Stream stream, long position, int count)
        {
            if (stream.Position != position) stream.Position = position;

            int resultCount = (stream.Position + count <= stream.Length) ? count : (int)(stream.Length - stream.Position);
            byte[] result = new byte[resultCount];
            stream.Read(result, 0, resultCount);
            return result;
        }

        /// <summary>
        /// Возвращает время, истекшее с момента загрузки системы (в миллисекундах)
        /// </summary>
        public static uint GetTickCount()
        {
            return (uint)Environment.TickCount;
        }

        /// <summary>
        /// Возвращает время между двумя замерами истекших с момента загрузки системы времен (в миллисекундах)
        /// </summary>
        public static uint GetTickCountDiff(uint lastTickCount)
        {
            return (uint)Environment.TickCount - lastTickCount;
        }

        public static DateTime Limit(this DateTime val, string format)
        {
            return DateTime.ParseExact(val.ToString(format, CultureInfo.InvariantCulture), format, CultureInfo.InvariantCulture);
        }

        public static bool GetAllowAddSpan(this DateTime dateTime, TimeSpan span)
        {
            return DateTime.MinValue - dateTime < span && span < DateTime.MaxValue - dateTime;
        }

        public static decimal GetTenScale(int scale)
        {
            decimal result = 1;

            if (scale != 0)
            {
                int absScale = Math.Abs(scale);

                StringBuilder b = new StringBuilder();
                b.Append('1');

                if (absScale > 0)
                    b.Append('0', absScale);

                if (scale >= 0) result *= int.Parse(b.ToString(), CultureInfo.InvariantCulture);
                else result /= int.Parse(b.ToString(), CultureInfo.InvariantCulture);
            }

            return result;
        }
    }
}
