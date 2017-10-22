using System;
using System.Drawing;
using System.IO;

namespace SCLCoreCLR
{
	public class Misc
	{
		public static int Clamp(int value, int min, int max)
		{
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		public static long Clamp(long value, long min, long max)
		{
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		public static double Clamp(double value, double min, double max)
		{
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		public static bool HexStringToULong(string str, ref ulong value)
		{
			str = str.ToLower();
			ulong num = 0uL;
			string text = str;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				num <<= 4;
				ulong num2;
				if (c >= '0' && c <= '9')
				{
					num2 = (ulong)((long)(c - '0'));
				}
				else
				{
					if (c < 'a' || c > 'f')
					{
						return false;
					}
					num2 = (ulong)(10L + (long)(c - 'a'));
				}
				num |= num2;
			}
			value = num;
			return true;
		}

		public static Point Sub(Point p1, Point p2)
		{
			return new Point(p1.X - p2.X, p1.Y - p2.Y);
		}

		public static int Convert(string value)
		{
			int result;
			try
			{
				result = System.Convert.ToInt32(value);
			}
			catch (Exception arg_09_0)
			{
				Log.WriteLine(arg_09_0.Message);
				result = 0;
			}
			return result;
		}

		public static string ReplaceExt(string path, string new_ext)
		{
			string arg_0E_0 = Path.GetDirectoryName(path);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			return Path.Combine(arg_0E_0, fileNameWithoutExtension) + new_ext;
		}

		private static bool IsLowercase(string str)
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

		private static bool PathStartsWith(string path, string root_path)
		{
			if (path.StartsWith(root_path))
			{
				int length = root_path.Length;
				return path.Length == length || path[length] == Path.DirectorySeparatorChar;
			}
			return false;
		}

		public static string GetRelativePath(string root_path, string filename)
		{
			string result;
			try
			{
				string text = root_path.ToLower().TrimEnd(new char[]
				{
					Path.DirectorySeparatorChar
				});
				string path = Path.GetDirectoryName(filename).ToLower();
				string text2 = "";
				while (text != null)
				{
					if (Misc.PathStartsWith(path, text))
					{
						int num = text.Length;
						if (text[text.Length - 1] != Path.DirectorySeparatorChar)
						{
							num++;
						}
						text2 += filename.Substring(num);
						result = text2;
						return result;
					}
					text2 += "..\\";
					text = Path.GetDirectoryName(text);
				}
				result = filename;
			}
			catch (Exception ex)
			{
				Log.WriteLine(string.Concat(new string[]
				{
					"GetRelativePath error: ",
					ex.Message,
					"\nroot_path: ",
					root_path,
					" filename: ",
					filename
				}));
				result = filename;
			}
			return result;
		}

		public static Color ColorFromHSV(double h, double S, double V)
		{
			double num;
			for (num = h; num < 0.0; num += 360.0)
			{
			}
			while (num >= 360.0)
			{
				num -= 360.0;
			}
			double num4;
			double num3;
			double num2;
			if (V <= 0.0)
			{
				num2 = (num3 = (num4 = 0.0));
			}
			else if (S <= 0.0)
			{
				num4 = V;
				num2 = V;
				num3 = V;
			}
			else
			{
				double expr_77 = num / 60.0;
				int num5 = (int)Math.Floor(expr_77);
				double num6 = expr_77 - (double)num5;
				double num7 = V * (1.0 - S);
				double num8 = V * (1.0 - S * num6);
				double num9 = V * (1.0 - S * (1.0 - num6));
				switch (num5)
				{
				case -1:
					num3 = V;
					num2 = num7;
					num4 = num8;
					break;
				case 0:
					num3 = V;
					num2 = num9;
					num4 = num7;
					break;
				case 1:
					num3 = num8;
					num2 = V;
					num4 = num7;
					break;
				case 2:
					num3 = num7;
					num2 = V;
					num4 = num9;
					break;
				case 3:
					num3 = num7;
					num2 = num8;
					num4 = V;
					break;
				case 4:
					num3 = num9;
					num2 = num7;
					num4 = V;
					break;
				case 5:
					num3 = V;
					num2 = num7;
					num4 = num8;
					break;
				case 6:
					num3 = V;
					num2 = num9;
					num4 = num7;
					break;
				default:
					num4 = V;
					num2 = V;
					num3 = V;
					break;
				}
			}
			int arg_191_0 = Misc.Clamp((int)(num3 * 255.0), 0, 255);
			int green = Misc.Clamp((int)(num2 * 255.0), 0, 255);
			int blue = Misc.Clamp((int)(num4 * 255.0), 0, 255);
			return Color.FromArgb(arg_191_0, green, blue);
		}
	}
}
