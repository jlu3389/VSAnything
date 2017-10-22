using System;
using System.Collections.Generic;
using System.Linq;

namespace SCLCoreCLR
{
	public class WildcardExpression
	{
		private enum Operator
		{
			And,
			Or
		}

		private WildcardExpression m_Exp1;

		private WildcardExpression.Operator m_Operator;

		private WildcardExpression m_Exp2;

		private Wildcard m_Wildcard;

		public static bool Match(string value, string expression, ref string error)
		{
			return WildcardExpression.Match(value, expression, false, ref error);
		}

		public static bool Match(string value, string expression, bool add_asterisks, ref string error)
		{
			return WildcardExpression.Parse(expression, add_asterisks, ref error).IsMatch(value);
		}

		private static bool IsWhiteSpace(char c)
		{
			return c == ' ' || c == '\t' || c == '\r' || c == '\n';
		}

		private static List<string> ParseTokens(string value)
		{
			List<string> list = new List<string>();
			int i = 0;
			string text = "";
			while (i < value.Length)
			{
				char c = value[i++];
				if (c == '"')
				{
					string item = WildcardExpression.ParseString(value, ref i);
					list.Add(item);
				}
				else if (c == ' ')
				{
					if (text != "")
					{
						list.Add(text);
					}
					text = "";
				}
				else if (c == '(')
				{
					if (text != "")
					{
						list.Add(text);
					}
					text = "";
					list.Add("(");
				}
				else if (c == ')')
				{
					if (text != "")
					{
						list.Add(text);
					}
					text = "";
					list.Add(")");
				}
				else
				{
					text += c.ToString();
				}
			}
			if (text != "")
			{
				list.Add(text);
			}
			return list;
		}

		private static string ParseString(string value, ref int index)
		{
			string text = "";
			while (index < value.Length)
			{
				int num = index;
				index = num + 1;
				char c = value[num];
				if (c == '"')
				{
					return text;
				}
				text += c.ToString();
			}
			return null;
		}

		public static WildcardExpression Parse(string value, ref string error)
		{
			return WildcardExpression.Parse(value, false, ref error);
		}

		public static WildcardExpression Parse(string value, bool add_asterisks, ref string error)
		{
			List<string> arg_13_0 = WildcardExpression.ParseTokens(value);
			WildcardExpression wildcardExpression = new WildcardExpression();
			int num = 0;
			if (!WildcardExpression.ParseExpression(arg_13_0, wildcardExpression, ref num, add_asterisks, ref error))
			{
				error = "error parsing string, defaulting to initial value";
				wildcardExpression.m_Wildcard = new Wildcard(value);
			}
			return wildcardExpression;
		}

		private static string AddAsterisks(string value)
		{
			if (!value.Contains('*'))
			{
				return "*" + value + "*";
			}
			return value;
		}

		private static bool ParseExpression(List<string> tokens, WildcardExpression expression, ref int index, bool add_asterisks, ref string error)
		{
			if (index == tokens.Count)
			{
				return true;
			}
			int num = index;
			index = num + 1;
			string text = tokens[num];
			string a = text.ToLower();
			if (index == tokens.Count || tokens[index] == ")")
			{
				if (index != tokens.Count)
				{
					index++;
				}
				if (add_asterisks)
				{
					text = WildcardExpression.AddAsterisks(text);
				}
				expression.m_Wildcard = new Wildcard(text);
				return true;
			}
			WildcardExpression wildcardExpression = new WildcardExpression();
			if (text == "(")
			{
				if (!WildcardExpression.ParseExpression(tokens, wildcardExpression, ref index, add_asterisks, ref error))
				{
					return false;
				}
			}
			else
			{
				if (text == "&&" || text == "||" || a == "and" || a == "or")
				{
					error = "Unexpected conditional token";
					return false;
				}
				if (add_asterisks)
				{
					text = WildcardExpression.AddAsterisks(text);
				}
				wildcardExpression.m_Wildcard = new Wildcard(WildcardExpression.AddAsterisks(text));
			}
			expression.m_Exp1 = wildcardExpression;
			if (index == tokens.Count)
			{
				return true;
			}
			num = index;
			index = num + 1;
			text = tokens[num];
			if (text == ")")
			{
				return true;
			}
			string a2 = text.ToLower();
			if (!(a2 == "&&") && !(a2 == "and"))
			{
				if (!(a2 == "||") && !(a2 == "or"))
				{
					error = "Unexpected token " + text;
					return false;
				}
				expression.m_Operator = WildcardExpression.Operator.Or;
			}
			else
			{
				expression.m_Operator = WildcardExpression.Operator.And;
			}
			WildcardExpression wildcardExpression2 = new WildcardExpression();
			if (!WildcardExpression.ParseExpression(tokens, wildcardExpression2, ref index, add_asterisks, ref error))
			{
				return false;
			}
			expression.m_Exp2 = wildcardExpression2;
			return true;
		}

		public bool IsMatch(string value)
		{
			if (this.m_Wildcard != null)
			{
				return this.m_Wildcard.IsMatch(value);
			}
			bool flag = this.m_Exp1.IsMatch(value);
			return (flag && this.m_Operator == WildcardExpression.Operator.Or) || ((flag || this.m_Operator != WildcardExpression.Operator.And) && (this.m_Exp2 == null || this.m_Exp2.IsMatch(value)));
		}

		public bool IsMatch(ICollection<string> values)
		{
			if (this.m_Wildcard != null)
			{
				foreach (string current in values)
				{
					if (this.m_Wildcard.IsMatch(current))
					{
						return true;
					}
				}
				return false;
			}
			bool flag = this.m_Exp1.IsMatch(values);
			return (flag && this.m_Operator == WildcardExpression.Operator.Or) || ((flag || this.m_Operator != WildcardExpression.Operator.And) && (this.m_Exp2 == null || this.m_Exp2.IsMatch(values)));
		}
	}
}
