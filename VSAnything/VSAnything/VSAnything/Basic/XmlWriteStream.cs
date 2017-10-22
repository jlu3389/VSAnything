using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;

namespace SCLCoreCLR
{
	public class XmlWriteStream
	{
		private XmlDocument m_Document;

		private XmlElement m_CurrentElement;

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

		public XmlWriteStream()
		{
			this.m_Document = new XmlDocument();
		}

		public XmlWriteStream(XmlDocument xml_doc) : this(xml_doc, null)
		{
		}

		public XmlWriteStream(XmlDocument xml_doc, XmlElement current_element)
		{
			this.m_Document = xml_doc;
			this.m_CurrentElement = current_element;
		}

		public bool Save(string path)
		{
			bool result;
			try
			{
				this.m_Document.Save(path);
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public void StartElement(string name)
		{
			XmlElement xmlElement = this.m_Document.CreateElement(name);
			if (this.m_CurrentElement != null)
			{
				this.m_CurrentElement.AppendChild(xmlElement);
			}
			else
			{
				this.m_Document.AppendChild(xmlElement);
			}
			this.m_CurrentElement = xmlElement;
		}

		public void EndElement()
		{
			this.m_CurrentElement = (this.m_CurrentElement.ParentNode as XmlElement);
		}

		public void Write<T>(string name, T value)
		{
			this.StartElement(name);
			this.m_CurrentElement.InnerText = ((value != null) ? value.ToString() : "");
			this.EndElement();
		}

		public void Write(string name, Color value)
		{
			this.StartElement(name);
			this.m_CurrentElement.InnerText = string.Concat(new string[]
			{
				value.A.ToString(),
				",",
				value.R.ToString(),
				",",
				value.G.ToString(),
				",",
				value.B.ToString()
			});
			this.EndElement();
		}

		public void Write(string name, Point value)
		{
			this.StartElement(name);
			this.m_CurrentElement.InnerText = value.X.ToString() + "," + value.Y.ToString();
			this.EndElement();
		}

		public void Write(string name, Size value)
		{
			this.StartElement(name);
			this.m_CurrentElement.InnerText = value.Width.ToString() + "," + value.Height.ToString();
			this.EndElement();
		}

		public void Write<T>(string name, List<T> list) where T : IXmlSerialisable, new()
		{
			this.StartElement(name);
			foreach (IXmlSerialisable xmlSerialisable in list)
			{
				this.StartElement(xmlSerialisable.GetType().Name);
				xmlSerialisable.Write(this);
				this.EndElement();
			}
			this.EndElement();
		}

		public void Write<T>(string name, LinkedList<T> list) where T : LinkedListNode, IXmlSerialisable, new()
		{
			this.StartElement(name);
			foreach (IXmlSerialisable xmlSerialisable in list)
			{
				this.StartElement(xmlSerialisable.GetType().Name);
				xmlSerialisable.Write(this);
				this.EndElement();
			}
			this.EndElement();
		}

		public void Write<T>(string name, System.Collections.Generic.LinkedList<T> list)
		{
			this.StartElement(name);
			foreach (T current in list)
			{
				this.Write<T>(current.GetType().Name, current);
			}
			this.EndElement();
		}

		public void Write(string name, List<bool> list)
		{
			this.StartElement(name);
			foreach (bool current in list)
			{
				this.Write<bool>("bool", current);
			}
			this.EndElement();
		}

		public void Write(string name, List<int> list)
		{
			this.StartElement(name);
			using (List<int>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					float value = (float)enumerator.Current;
					this.Write<float>("int", value);
				}
			}
			this.EndElement();
		}

		public void Write(string name, List<float> list)
		{
			this.StartElement(name);
			foreach (float current in list)
			{
				this.Write<float>("float", current);
			}
			this.EndElement();
		}

		public void Write(string name, List<double> list)
		{
			this.StartElement(name);
			foreach (double current in list)
			{
				this.Write<double>("double", current);
			}
			this.EndElement();
		}

		public void Write(string name, List<string> list)
		{
			this.StartElement(name);
			foreach (string current in list)
			{
				this.Write<string>("string", current);
			}
			this.EndElement();
		}

		public void Save(FileStream file_stream)
		{
			this.m_Document.Save(file_stream);
		}
	}
}
