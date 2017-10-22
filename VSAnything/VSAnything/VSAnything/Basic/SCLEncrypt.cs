using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SCLCoreCLR
{
	public class SCLEncrypt
	{
		private static byte[] Encrypt(byte[] input_data, byte[] key, byte[] iv)
		{
			MemoryStream arg_19_0 = new MemoryStream();
			Rijndael rijndael = Rijndael.Create();
			rijndael.Key = key;
			rijndael.IV = iv;
			CryptoStream expr_26 = new CryptoStream(arg_19_0, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
			expr_26.Write(input_data, 0, input_data.Length);
			expr_26.Close();
			return arg_19_0.ToArray();
		}

		private static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
		{
			MemoryStream arg_19_0 = new MemoryStream();
			Rijndael rijndael = Rijndael.Create();
			rijndael.Key = key;
			rijndael.IV = iv;
			CryptoStream expr_26 = new CryptoStream(arg_19_0, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
			expr_26.Write(cipherData, 0, cipherData.Length);
			expr_26.Close();
			return arg_19_0.ToArray();
		}

		public static string Encrypt(string text, string password)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(text);
			PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, new byte[]
			{
				73,
				118,
				97,
				110,
				32,
				77,
				101,
				100,
				118,
				101,
				100,
				101,
				118
			});
			return SCLEncrypt.ToString(SCLEncrypt.Encrypt(bytes, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16)));
		}

		public static string Decrypt(string cipher_text, string password)
		{
			byte[] cipherData = SCLEncrypt.ToByteArray(cipher_text);
			PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, new byte[]
			{
				73,
				118,
				97,
				110,
				32,
				77,
				101,
				100,
				118,
				101,
				100,
				101,
				118
			});
			byte[] bytes = SCLEncrypt.Decrypt(cipherData, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));
			return Encoding.Unicode.GetString(bytes);
		}

		private static string ToString(byte[] bytes)
		{
			string text = "";
			for (int i = 0; i < bytes.Length; i++)
			{
				byte b = bytes[i];
				text += b.ToString("X2");
			}
			return text;
		}

		private static byte[] ToByteArray(string text)
		{
			byte[] array = new byte[text.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				string s = text.Substring(2 * i, 2);
				array[i] = byte.Parse(s, NumberStyles.HexNumber);
			}
			return array;
		}
	}
}
