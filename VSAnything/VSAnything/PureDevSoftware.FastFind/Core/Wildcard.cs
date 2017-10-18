using System;

namespace Company.VSAnything
{
	public class Wildcard
	{
		private string m_Pattern;

		public static bool Match(string value, string pattern)
		{
			int start = -1;
			int end = -1;
			return Wildcard.Match(value, pattern, ref start, ref end);
		}

		public static bool Match(string value, string pattern, ref int start, ref int end)
		{
			return new Wildcard(pattern).IsMatch(value, ref start, ref end);
		}

		public Wildcard(string pattern)
		{
			this.m_Pattern = pattern;
		}

		public bool IsMatch(string str, ref int start, ref int end)
		{
			if (this.m_Pattern.Length == 0)
			{
				return false;
			}
			int pindex = 0;
			int sindex = 0;
			int pattern_len = this.m_Pattern.Length;
			int str_len = str.Length;
			start = -1;
			while (true)
			{
				bool star = false;
				if (this.m_Pattern[pindex] == '*')
				{
					star = true;
					do
					{
						pindex++;
					}
					while (pindex < pattern_len && this.m_Pattern[pindex] == '*');
				}
				end = sindex;
				int i;
				while (true)
				{
					IL_5D:
					i = 0;
					while (pindex + i < pattern_len && this.m_Pattern[pindex + i] != '*')
					{
						int si = sindex + i;
						if (si == str_len)
						{
							return false;
						}
						if (str[si] != this.m_Pattern[pindex + i])
						{
							if (si == str_len)
							{
								return false;
							}
							if (this.m_Pattern[pindex + i] != '?' || str[si] == '.')
							{
								if (!star)
								{
									return false;
								}
								sindex++;
								if (si == str_len)
								{
									return false;
								}
								goto IL_5D;
							}
						}
						i++;
					}
					if (start == -1)
					{
						start = sindex;
					}
					if (pindex + i < pattern_len && this.m_Pattern[pindex + i] == '*')
					{
						break;
					}
					if (sindex + i == str_len)
					{
						goto Block_15;
					}
					if (i != 0 && this.m_Pattern[pindex + i - 1] == '*')
					{
						return true;
					}
					if (!star)
					{
						return false;
					}
					sindex++;
				}
				sindex += i;
				pindex += i;
				if (start == -1)
				{
					start = sindex;
				}
			}
			return false;
			Block_15:
			if (end <= start)
			{
				end = str_len;
			}
			return true;
		}
	}
}
