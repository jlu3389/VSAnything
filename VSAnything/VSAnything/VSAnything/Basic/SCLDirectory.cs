using System;
using System.Collections.Generic;
using System.IO;

namespace SCLCoreCLR
{
	public class SCLDirectory
	{
		private static void GetFilesRecursive(string path, Wildcard wildcard, List<string> files)
		{
			string[] array = Directory.GetFiles(path);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (wildcard.IsMatch(text))
				{
					files.Add(Path.Combine(path, text));
				}
			}
			array = Directory.GetDirectories(path);
			for (int i = 0; i < array.Length; i++)
			{
				SCLDirectory.GetFilesRecursive(array[i], wildcard, files);
			}
		}

		public static string[] GetFilesRecursive(string path)
		{
			return SCLDirectory.GetFilesRecursive(path, "*");
		}

		public static string[] GetFilesRecursive(string path, string wildcard)
		{
			List<string> list = new List<string>();
			SCLDirectory.GetFilesRecursive(path, new Wildcard(wildcard), list);
			return list.ToArray();
		}
	}
}
