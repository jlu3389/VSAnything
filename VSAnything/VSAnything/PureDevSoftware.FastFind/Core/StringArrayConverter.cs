using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Company.VSAnything
{
	internal class StringArrayConverter : TypeConverter
	{
		private const char m_Delimiter = ' ';

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string[]) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string str = value as string;
			if (str == null)
			{
				return base.ConvertFrom(context, culture, value);
			}
			List<string> str_list = new List<string>();
			string[] array = str.Split(new char[]
			{
				' '
			});
			for (int i = 0; i < array.Length; i++)
			{
				string s_trim = array[i].Trim();
				if (s_trim != "")
				{
					str_list.Add(s_trim);
				}
			}
			return str_list.ToArray();
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			string[] str_array = value as string[];
			if (destinationType != typeof(string) || str_array == null)
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			string str = "";
			string[] array = str_array;
			for (int i = 0; i < array.Length; i++)
			{
				string s = array[i];
				str = str + s + " ";
			}
			return str.Trim();
		}
	}
}
