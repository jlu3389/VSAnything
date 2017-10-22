using System;

namespace SCLCoreCLR
{
	public class Wildcard
	{
		private string m_Pattern;

		private string m_PatternLowercase;

		private static char[] m_LowercaseTable;

		public string Pattern
		{
			get
			{
				return this.m_Pattern;
			}
		}

		static Wildcard()
		{
			Wildcard.m_LowercaseTable = new char[255];
			for (int i = 0; i < 255; i++)
			{
				Wildcard.m_LowercaseTable[i] = (char)((i >= 65 && i <= 90) ? (i + 97 - 65) : i);
			}
		}

		private static char MakeLowercase(char c)
		{
			if ((ulong)c >= (ulong)((long)Wildcard.m_LowercaseTable.Length))
			{
				return c;
			}
			return Wildcard.m_LowercaseTable[(int)c];
		}

		public static bool Match(string value, string pattern)
		{
			return new Wildcard(pattern).IsMatch(value);
		}

		public Wildcard(string pattern)
		{
			this.m_Pattern = pattern;
			this.m_PatternLowercase = pattern.ToLower();
		}

		public bool IsMatch(string str)
		{
			if (this.m_Pattern.Length == 0)
			{
				return false;
			}
			int num = 0;
			int num2 = 0;
			int length = this.m_Pattern.Length;
			int length2 = str.Length;
			while (true)
			{
				bool flag = false;
				if (this.m_Pattern[num] == '*')
				{
					flag = true;
					do
					{
						num++;
					}
					while (num < length && this.m_Pattern[num] == '*');
				}
				int num3;
				while (true)
				{
					IL_57:
					num3 = 0;
					while (num + num3 < length && this.m_Pattern[num + num3] != '*')
					{
						int num4 = num2 + num3;
						if (num4 == length2)
						{
							return false;
						}
						if (Wildcard.MakeLowercase(str[num4]) != this.m_PatternLowercase[num + num3])
						{
							if (num4 == length2)
							{
								return false;
							}
							if (this.m_Pattern[num + num3] != '?' || str[num4] == '.')
							{
								if (!flag)
								{
									return false;
								}
								num2++;
								if (num4 == length2)
								{
									return false;
								}
								goto IL_57;
							}
						}
						num3++;
					}
					if (num + num3 < length && this.m_Pattern[num + num3] == '*')
					{
						break;
					}
					if (num2 + num3 == length2)
					{
						return true;
					}
					if (num3 != 0 && this.m_Pattern[num + num3 - 1] == '*')
					{
						return true;
					}
					if (!flag)
					{
						return false;
					}
					num2++;
				}
				num2 += num3;
				num += num3;
			}
			return false;
		}
	}
}
