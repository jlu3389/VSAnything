using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;

namespace Company.VSAnything
{
	internal class SourceHeaderToggleCmd
	{
		private DTE m_DTE;

		private SolutionFiles m_SolutionFiles;

		private const int m_MaxParentDepth = 2;

		private const int m_MaxChildDepth = 2;

		private static Dictionary<string, string> m_HeaderPathMap = new Dictionary<string, string>();

		private static string[] m_SourceExt = new string[]
		{
			".cpp",
			".c"
		};

		private static string[] m_HeaderExt = new string[]
		{
			".hpp",
			".h",
			".inl"
		};

		public void Initialise(DTE dte, SolutionFiles solution_files, OleMenuCommandService mcs)
		{
			this.m_DTE = dte;
			this.m_SolutionFiles = solution_files;
			if (mcs != null)
			{
                CommandID menuCommandID = new CommandID(GuidList.guidVSAnythingCmdSet, 258);
				MenuCommand menuItem = new MenuCommand(new EventHandler(this.MenuItemCallback), menuCommandID);
				mcs.AddCommand(menuItem);
			}
		}

		private void MenuItemCallback(object sender, EventArgs e)
		{
			string path = this.m_DTE.GetActiveDocumentFilename();
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			string dir = Path.GetDirectoryName(path);
			string filename = Path.GetFileName(path);
			string new_path = SourceHeaderToggleCmd.GetOppositeFile(this.m_DTE, this.m_SolutionFiles, dir, filename);
			if (File.Exists(new_path))
			{
				this.m_DTE.OpenFile(new_path);
				return;
			}
			MessageBox.Show(null, "Unable to find header for " + path, "FastFind Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		private static bool IsOpposite(string file1, string file2)
		{
			string arg_0E_0 = Path.GetFileNameWithoutExtension(file1);
			string f2 = Path.GetFileNameWithoutExtension(file2);
			if (arg_0E_0 != f2)
			{
				return false;
			}
			string e = Path.GetExtension(file1);
			string e2 = Path.GetExtension(file2);
			if (SourceHeaderToggleCmd.IsSource(e))
			{
				return SourceHeaderToggleCmd.IsHeader(e2);
			}
			return SourceHeaderToggleCmd.IsSource(e2) && SourceHeaderToggleCmd.IsHeader(e);
		}

		private static bool IsHeader(string ext)
		{
			string[] headerExt = SourceHeaderToggleCmd.m_HeaderExt;
			for (int i = 0; i < headerExt.Length; i++)
			{
				string e = headerExt[i];
				if (ext == e)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsSource(string ext)
		{
			string[] sourceExt = SourceHeaderToggleCmd.m_SourceExt;
			for (int i = 0; i < sourceExt.Length; i++)
			{
				string e = sourceExt[i];
				if (ext == e)
				{
					return true;
				}
			}
			return false;
		}

		private static string GetOppositeFile(DTE dte, SolutionFiles solution_files, string dir, string filename)
		{
			string opposite_file = SourceHeaderToggleCmd.GetOppositeFile(dir, filename, new HashSet<string>(), 0);
			if (File.Exists(opposite_file))
			{
				return opposite_file;
			}
			IEnumerable<string> arg_25_0 = solution_files.Files;
			string filename_lwr = filename.ToLower();
			foreach (string sln_file in arg_25_0)
			{
				if (SourceHeaderToggleCmd.IsOpposite(Path.GetFileName(sln_file).ToLower(), filename_lwr))
				{
					string result = sln_file;
					return result;
				}
			}
			string path = Path.Combine(dir, filename).ToLower();
			string new_path = null;
			if (SourceHeaderToggleCmd.m_HeaderPathMap.TryGetValue(path, out new_path) && File.Exists(new_path))
			{
				return new_path;
			}
			HashSet<string> visited_paths = new HashSet<string>();
			new_path = SourceHeaderToggleCmd.GetOppositeFile(dir, filename, visited_paths, 0);
			int i = 0;
			while (new_path == null && i < 2 && dir != null)
			{
				dir = Path.GetDirectoryName(dir);
				if (dir != null)
				{
					new_path = SourceHeaderToggleCmd.GetOppositeFile(dir, filename, visited_paths, 0);
				}
				i++;
			}
			SourceHeaderToggleCmd.m_HeaderPathMap[path] = new_path;
			return new_path;
		}

		private static string GetOppositeFile(string dir, string filename, HashSet<string> visited_paths, int depth)
		{
			string normalised_dir = dir.ToLower();
			if (visited_paths.Contains(normalised_dir))
			{
				return null;
			}
			visited_paths.Add(normalised_dir);
			string ext = Path.GetExtension(filename).ToLower();
			if (Array.IndexOf<string>(SourceHeaderToggleCmd.m_SourceExt, ext) != -1)
			{
				string[] array = SourceHeaderToggleCmd.m_HeaderExt;
				for (int i = 0; i < array.Length; i++)
				{
					string new_ext = array[i];
					string new_path = Path.Combine(dir, Path.ChangeExtension(filename, new_ext));
					if (File.Exists(new_path))
					{
						return new_path;
					}
				}
			}
			else
			{
				if (Array.IndexOf<string>(SourceHeaderToggleCmd.m_HeaderExt, ext) == -1)
				{
					return null;
				}
				string[] array = SourceHeaderToggleCmd.m_SourceExt;
				for (int i = 0; i < array.Length; i++)
				{
					string new_ext2 = array[i];
					string new_path2 = Path.Combine(dir, Path.ChangeExtension(filename, new_ext2));
					if (File.Exists(new_path2))
					{
						return new_path2;
					}
				}
			}
			if (depth < 2)
			{
				try
				{
					string[] array = Directory.GetDirectories(dir);
					for (int i = 0; i < array.Length; i++)
					{
						string sub_dir = array[i];
						string new_path3 = SourceHeaderToggleCmd.GetOppositeFile(Path.Combine(dir, sub_dir), filename, visited_paths, depth + 1);
						if (new_path3 != null)
						{
							return new_path3;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			return null;
		}
	}
}
