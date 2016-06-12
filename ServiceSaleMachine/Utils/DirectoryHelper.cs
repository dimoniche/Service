using System;
using System.Collections.Generic;
using System.IO;

namespace ServiceSaleMachine
{
	public static class DirectoryHelper
	{
		public static bool TryCreate(string path)
		{
			try
			{
				Directory.CreateDirectory(path);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool TryDelete(string path, bool recursive = false)
		{
			try
			{
				Directory.Delete(path, recursive);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static string[] GetDirectoriesSafe(string path, string pattern, SearchOption option)
		{
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException();

			List<string> result = new List<string>();
			try
			{
				string[] dirs = !string.IsNullOrWhiteSpace(pattern) ? Directory.GetDirectories(path, pattern) : Directory.GetDirectories(path);
				result.AddRange(dirs);
				if (option == SearchOption.AllDirectories)
					foreach (string dir in dirs)
						result.AddRange(GetDirectoriesSafe(dir, pattern, option));
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch
			{
			}
			return result.ToArray();
		}

		public static string[] GetFilesSafe(string path, string pattern, SearchOption option)
		{
			if (Directory.Exists(path))
			{
				List<string> dirs = new List<string> {path};
				if (option == SearchOption.AllDirectories)
				{
					dirs.AddRange(GetDirectoriesSafe(path, null, option));
				}

				List<string> result = new List<string>();
				foreach (string dir in dirs)
				{
					try
					{
						string[] files = !string.IsNullOrWhiteSpace(pattern) ? Directory.GetFiles(dir, pattern) : Directory.GetFiles(dir);
						result.AddRange(files);
					}
					catch (UnauthorizedAccessException)
					{
					}
					catch
					{
					}
				}
				return result.ToArray();
			}
			throw new DirectoryNotFoundException();
		}
	}
}
