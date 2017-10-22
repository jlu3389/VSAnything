using System;

namespace SCLCoreCLR
{
	public interface IXmlSerialisable
	{
		void Read(XmlReadStream stream);

		void Write(XmlWriteStream stream);
	}
}
