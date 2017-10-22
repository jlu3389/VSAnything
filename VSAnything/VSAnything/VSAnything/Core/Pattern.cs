using System;

namespace Company.VSAnything
{
	internal struct Pattern
	{
		public enum Operator
		{
			AND,
			OR,
			AND_NOT
		}

		public string m_Pattern;

		public Pattern.Operator m_Operator;

		public bool m_UseWildcard;
	}
}
