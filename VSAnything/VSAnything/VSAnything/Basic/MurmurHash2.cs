using System;

namespace SCLCoreCLR
{
	public class MurmurHash2
	{
		public static uint Hash(string str)
		{
			return MurmurHash2.Hash(MurmurHash2.GetBytes(str), 3314489979u);
		}

		private static byte[] GetBytes(string str)
		{
			byte[] array = new byte[str.Length * 2];
			Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
			return array;
		}

		private static uint Hash(byte[] data, uint seed)
		{
			int i = data.Length;
			if (i == 0)
			{
				return 0u;
			}
			uint num = seed ^ (uint)i;
			int num2 = 0;
			while (i >= 4)
			{
				uint num3 = BitConverter.ToUInt32(data, num2);
				num3 *= 1540483477u;
				num3 ^= num3 >> 24;
				num3 *= 1540483477u;
				num *= 1540483477u;
				num ^= num3;
				num2 += 4;
				i -= 4;
			}
			switch (i)
			{
			case 1:
				num ^= (uint)data[num2];
				num *= 1540483477u;
				break;
			case 2:
				num ^= (uint)BitConverter.ToUInt16(data, num2);
				num *= 1540483477u;
				break;
			case 3:
				num ^= (uint)BitConverter.ToUInt16(data, num2);
				num ^= (uint)((uint)data[num2 + 2] << 16);
				num *= 1540483477u;
				break;
			}
			num ^= num >> 13;
			num *= 1540483477u;
			return num ^ num >> 15;
		}
	}
}
