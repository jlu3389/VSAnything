using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;

namespace SCLCoreCLR
{
	public class XmlReadStream
	{
		private XmlDocument m_Document;

		private XmlElement m_CurrentElement;

		public int Count
		{
			get
			{
				return this.m_CurrentElement.ChildNodes.Count;
			}
		}

		public XmlDocument XmlDocument
		{
			get
			{
				return this.m_Document;
			}
		}

		public XmlElement CurrentElement
		{
			get
			{
				return this.m_CurrentElement;
			}
		}

		public string CurrentValue
		{
			get
			{
				return this.CurrentElement.InnerText;
			}
		}

		public XmlReadStream()
		{
			this.m_Document = new XmlDocument();
		}

		public XmlReadStream(XmlDocument xml_document) : this(xml_document, null)
		{
		}

		public XmlReadStream(XmlDocument xml_document, XmlElement current_element)
		{
			this.m_Document = xml_document;
			this.m_CurrentElement = current_element;
		}

		public void Load(FileStream file_stream)
		{
			this.m_Document.Load(file_stream);
		}

		public bool Load(string path)
		{
			if (!File.Exists(path))
			{
				return false;
			}
			FileStream fileStream = null;
			bool result;
			try
			{
				fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
				this.Load(fileStream);
				fileStream.Close();
				result = true;
			}
			catch (Exception arg_2F_0)
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				Log.WriteLine(arg_2F_0.Message);
				result = false;
			}
			return result;
		}

		public bool StartElement(string name)
		{
			if (this.m_CurrentElement != null)
			{
                IEnumerator enumerator = this.m_CurrentElement.ChildNodes.GetEnumerator();
				{
					while (enumerator.MoveNext())
					{
						XmlElement xmlElement = ((XmlNode)enumerator.Current) as XmlElement;
						if (xmlElement != null && xmlElement.Name == name)
						{
							this.m_CurrentElement = xmlElement;
							return true;
						}
					}
					return false;
				}
			}
			if (this.m_Document.ChildNodes.Count > 0 && this.m_Document.FirstChild.Name == name)
			{
				this.m_CurrentElement = (XmlElement)this.m_Document.FirstChild;
				return true;
			}
			return false;
		}

		public bool StartElement(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				return false;
			}
			this.m_CurrentElement = (XmlElement)this.m_CurrentElement.ChildNodes[index];
			return true;
		}

		public void EndElement()
		{
			this.m_CurrentElement = (this.m_CurrentElement.ParentNode as XmlElement);
		}

		public bool Read(string name, ref bool value)
		{
			if (this.StartElement(name))
			{
				value = Convert.ToBoolean(this.m_CurrentElement.InnerText);
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read(string name, ref int value)
		{
			if (this.StartElement(name))
			{
				value = Convert.ToInt32(this.m_CurrentElement.InnerText);
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read(string name, ref long value)
		{
			if (this.StartElement(name))
			{
				value = Convert.ToInt64(this.m_CurrentElement.InnerText);
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read(string name, ref ulong value)
		{
			if (this.StartElement(name))
			{
				value = Convert.ToUInt64(this.m_CurrentElement.InnerText);
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read(string name, ref float value)
		{
			if (this.StartElement(name))
			{
				value = Convert.ToSingle(this.m_CurrentElement.InnerText);
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read(string name, ref double value)
		{
			if (this.StartElement(name))
			{
				value = Convert.ToDouble(this.m_CurrentElement.InnerText);
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read(string name, ref string value)
		{
			if (this.StartElement(name))
			{
				value = this.m_CurrentElement.InnerText;
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read(string name, ref DateTime value)
		{
			if (this.StartElement(name))
			{
				value = DateTime.Parse(this.m_CurrentElement.InnerText);
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read<T>(string name, ref T value) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			if (this.StartElement(name))
			{
				value = (T)((object)Enum.Parse(typeof(T), this.m_CurrentElement.InnerText));
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read(string name, ref Color value)
		{
			if (this.StartElement(name))
			{
				try
				{
					string[] array = this.m_CurrentElement.InnerText.Split(new char[]
					{
						','
					});
					value = Color.FromArgb(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]));
				}
				catch (Exception arg_52_0)
				{
					Log.WriteLine(arg_52_0.Message);
					value = Color.Black;
				}
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read(string name, ref Point value)
		{
			if (this.StartElement(name))
			{
				try
				{
					string[] array = this.m_CurrentElement.InnerText.Split(new char[]
					{
						','
					});
					value = new Point(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
				}
				catch (Exception arg_42_0)
				{
					Log.WriteLine(arg_42_0.Message);
					value = new Point(0, 0);
				}
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool Read(string name, ref Size value)
		{
			if (this.StartElement(name))
			{
				try
				{
					string[] array = this.m_CurrentElement.InnerText.Split(new char[]
					{
						','
					});
					value = new Size(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
				}
				catch (Exception arg_42_0)
				{
					Log.WriteLine(arg_42_0.Message);
					value = new Size(0, 0);
				}
				this.EndElement();
				return true;
			}
			return false;
		}

		public bool ReadEnum<T>(string name, ref T value)
		{
			if (this.StartElement(name))
			{
				try
				{
					value = (T)((object)Enum.Parse(typeof(T), this.m_CurrentElement.InnerText));
				}
				catch (Exception)
				{
					return false;
				}
				this.EndElement();
				return true;
			}
			return false;
		}

		public void Read<T>(string name, ref List<T> list) where T : IXmlSerialisable, new()
		{
			if (this.StartElement(name))
			{
				list = new List<T>(this.Count);
				for (int i = 0; i < this.Count; i++)
				{
					this.StartElement(i);
					T item = (T)((object)typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]));
					item.Read(this);
					list.Add(item);
					this.EndElement();
				}
				this.EndElement();
			}
		}

		public void Read<T>(string name, ref LinkedList<T> list) where T : LinkedListNode, IXmlSerialisable, new()
		{
			if (this.StartElement(name))
			{
				list = new LinkedList<T>();
				for (int i = 0; i < this.Count; i++)
				{
					this.StartElement(i);
					T t = (T)((object)typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]));
					t.Read(this);
					list.AddLast(t);
					this.EndElement();
				}
				this.EndElement();
			}
		}

		public void Read(string name, ref System.Collections.Generic.LinkedList<string> list)
		{
			if (this.StartElement(name))
			{
				list = new System.Collections.Generic.LinkedList<string>();
				for (int i = 0; i < this.Count; i++)
				{
					string value = "";
					this.Read(i, ref value);
					list.AddLast(value);
				}
				this.EndElement();
			}
		}

		public void Read(string name, ref List<bool> list)
		{
			if (this.StartElement(name))
			{
				list = new List<bool>(this.Count);
				for (int i = 0; i < this.Count; i++)
				{
					bool item = false;
					this.Read(i, ref item);
					list.Add(item);
				}
				this.EndElement();
			}
		}

		public void Read(string name, ref List<int> list)
		{
			if (this.StartElement(name))
			{
				list = new List<int>(this.Count);
				for (int i = 0; i < this.Count; i++)
				{
					int item = 0;
					this.Read(i, ref item);
					list.Add(item);
				}
				this.EndElement();
			}
		}

		public void Read(string name, ref List<float> list)
		{
			if (this.StartElement(name))
			{
				list = new List<float>(this.Count);
				for (int i = 0; i < this.Count; i++)
				{
					float item = 0f;
					this.Read(i, ref item);
					list.Add(item);
				}
				this.EndElement();
			}
		}

		public void Read(string name, ref List<double> list)
		{
			if (this.StartElement(name))
			{
				list = new List<double>(this.Count);
				for (int i = 0; i < this.Count; i++)
				{
					double item = 0.0;
					this.Read(i, ref item);
					list.Add(item);
				}
				this.EndElement();
			}
		}

		public bool Read(string name, ref List<string> list)
		{
			if (this.StartElement(name))
			{
				list = new List<string>(this.Count);
				for (int i = 0; i < this.Count; i++)
				{
					string item = "";
					this.Read(i, ref item);
					list.Add(item);
				}
				this.EndElement();
				return true;
			}
			return false;
		}

		public void Read(int index, ref bool value)
		{
			this.StartElement(index);
			value = Convert.ToBoolean(this.m_CurrentElement.InnerText);
			this.EndElement();
		}

		public void Read(int index, ref int value)
		{
			this.StartElement(index);
			value = Convert.ToInt32(this.m_CurrentElement.InnerText);
			this.EndElement();
		}

		public void Read(int index, ref long value)
		{
			this.StartElement(index);
			value = Convert.ToInt64(this.m_CurrentElement.InnerText);
			this.EndElement();
		}

		public void Read(int index, ref float value)
		{
			this.StartElement(index);
			value = Convert.ToSingle(this.m_CurrentElement.InnerText);
			this.EndElement();
		}

		public void Read(int index, ref double value)
		{
			this.StartElement(index);
			value = Convert.ToDouble(this.m_CurrentElement.InnerText);
			this.EndElement();
		}

		public void Read(int index, ref string value)
		{
			this.StartElement(index);
			value = this.m_CurrentElement.InnerText;
			this.EndElement();
		}

		public void Read(int index, ref Color value)
		{
			this.StartElement(index);
			try
			{
				string[] array = this.m_CurrentElement.InnerText.Split(new char[]
				{
					','
				});
				value = Color.FromArgb(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]));
			}
			catch (Exception arg_51_0)
			{
				Log.WriteLine(arg_51_0.Message);
				value = Color.Black;
			}
			this.EndElement();
		}

		public void ReadEnum<T>(int index, ref T value)
		{
			this.StartElement(index);
			value = (T)((object)Enum.Parse(typeof(T), this.m_CurrentElement.InnerText));
			this.EndElement();
		}
	}
}
