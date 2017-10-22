using SCLCoreCLR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Company.VSAnything
{
	internal class Utils
	{
		private static bool m_ShownNormalisingPathMessageBox;

		public static bool IsLowercase(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (c >= 'A' && c <= 'Z')
				{
					return false;
				}
			}
			return true;
		}

		public static bool StrStr(string str, string pattern)
		{
			return pattern.Length == 0 || str.Contains(pattern);
		}

		public static bool ListsMatch(List<string> a, List<string> b)
		{
			if (a.Count != b.Count)
			{
				return false;
			}
			int count = a.Count;
			for (int i = 0; i < count; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}

		public static List<string> FilterFileListByExt(List<string> files, List<string> ext_list)
		{
			List<string> filtered_files = new List<string>(files.Count);
			foreach (string file in files)
			{
				if (ext_list.Contains(Path.GetExtension(file).ToLower()))
				{
					filtered_files.Add(file);
				}
			}
			return filtered_files;
		}

		public static Color Mul(Color colour, float f)
		{
			return Color.FromArgb(Math.Min((int)((float)colour.R * f), 255), Math.Min((int)((float)colour.G * f), 255), Math.Min((int)((float)colour.B * f), 255));
		}

		private static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * t;
		}

		public static Color Lerp(Color c0, Color c1, float t)
		{
			return Color.FromArgb((int)Utils.Lerp((float)c0.R, (float)c1.R, t), (int)Utils.Lerp((float)c0.G, (float)c1.G, t), (int)Utils.Lerp((float)c0.B, (float)c1.B, t));
		}

		public static Color ModifyColour(Color colour, int offset)
		{
			int brightness = (int)(0.299 * (double)colour.R + 0.587 * (double)colour.G + 0.114 * (double)colour.B);
			if (brightness < 128)
			{
				brightness = Math.Min(brightness + offset, 255);
			}
			else
			{
				brightness = Math.Max(0, brightness - offset);
			}
			return Color.FromArgb(brightness, brightness, brightness);
		}

		public static List<string> ToLower(List<string> strings)
		{
			List<string> strings_lwr = new List<string>();
			foreach (string str in strings)
			{
				strings_lwr.Add(str.ToLower());
			}
			return strings_lwr;
		}

		public static Set<string> ToLower(Set<string> strings)
		{
			Set<string> strings_lwr = new Set<string>();
			foreach (string str in strings)
			{
				strings_lwr.Add(str.ToLower());
			}
			return strings_lwr;
		}

		public static void LogExceptionQuiet(Exception e)
		{
			Log.WriteLine("FastFind Exception:\n\n" + e.Message + "\n\n" + e.StackTrace);
		}

		public static void LogException(Exception e)
		{
			string expr_1B = "FastFind Exception:\n\n" + e.Message + "\n\n" + e.StackTrace;
			Log.WriteLine(expr_1B);
			MessageBox.Show(expr_1B, "FastFind ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		public static string NormalisePath(string path)
		{
			return Utils.GetFullPath(path);
		}

		public static string GetFullPath(string path)
		{
			try
			{
				return Path.GetFullPath(path).ToLower();
			}
			catch (Exception e)
			{
				string message = "Error getting full path for " + path + "\n" + e.Message;
				Log.WriteLine(message);
				if (!Utils.m_ShownNormalisingPathMessageBox)
				{
					Utils.m_ShownNormalisingPathMessageBox = true;
					MessageBox.Show(message, "FastFind ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
			return path;
		}

		public static void Read(List<string> list, BinaryReader reader)
		{
			list.Clear();
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				string item = Utils.Intern(reader.ReadString());
				list.Add(item);
			}
		}

		public static string Intern(string value)
		{
			return string.Intern(value);
		}

		public static void Write(List<string> list, BinaryWriter writer)
		{
			writer.Write(list.Count);
			foreach (string item in list)
			{
				writer.Write(item);
			}
		}

		public static void CheckNormalised(string path)
		{
		}

		public static bool IsSolutionFile(string path)
		{
			return Path.GetExtension(path).ToLower() == ".sln";
		}

		public static int ToInt(string value, int old_value)
		{
			int result;
			try
			{
				result = Convert.ToInt32(value);
			}
			catch (Exception arg_09_0)
			{
				Utils.LogExceptionQuiet(arg_09_0);
				result = old_value;
			}
			return result;
		}

		public static int GetStringWidth(string str, int len, Graphics graphics, Font font, StringFormat in_string_format)
		{
			if (len == 0)
			{
				return 0;
			}
			if (len > 1024)
			{
				return -1;
			}
			int result;
			try
			{
				StringFormat string_format = new StringFormat(in_string_format);
				string_format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
				CharacterRange[] char_ranges = new CharacterRange[]
				{
					new CharacterRange(0, len)
				};
				string_format.SetMeasurableCharacterRanges(char_ranges);
				Rectangle layout_rect = new Rectangle(0, 0, 2147483647, 2147483647);
				result = (int)graphics.MeasureCharacterRanges(str, font, layout_rect, string_format)[0].GetBounds(graphics).Width;
			}
			catch (Exception arg_7D_0)
			{
				Utils.LogExceptionQuiet(arg_7D_0);
				result = 0;
			}
			return result;
		}

		public static string GetWord(string text, int index)
		{
			if (index >= text.Length || !Utils.IsWordChar(text[index]))
			{
				return "";
			}
			int start = index;
			int end = index;
			while (start > 0)
			{
				if (!Utils.IsWordChar(text[start - 1]))
				{
					break;
				}
				start--;
			}
			while (end < text.Length && Utils.IsWordChar(text[end]))
			{
				end++;
			}
			return text.Substring(start, end - start);
		}

		private static bool IsWordChar(char c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
		}

		public static string NormalisePathAndLowerCase(string path)
		{
			return Path.GetFullPath(path).ToLower();
		}

		public static void Swap<T>(ref T value1, ref T value2)
		{
			T temp = value1;
			value1 = value2;
			value2 = temp;
		}

		public static string GetFileNameSafe(string path)
		{
			try
			{
				return Path.GetFileName(path);
			}
			catch (Exception)
			{
			}
			return "";
		}
	}
}
