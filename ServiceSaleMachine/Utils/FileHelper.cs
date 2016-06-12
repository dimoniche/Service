using System.IO;
using System.Linq;

namespace ServiceSaleMachine
{
	public static class FileHelper
	{
		private static char[] FileNameReplaceChars = null;

		static FileHelper()
		{
			FileNameReplaceChars = Path.GetInvalidFileNameChars();
		}

		public static string GetCorrectFileName(string fileName)
		{
			if (fileName == null) return null;

			return FileNameReplaceChars.Aggregate(fileName, (current, t) => current.Replace(t, '_'));
		}

		public static bool TryDelete(string fileName)
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
	}
}
