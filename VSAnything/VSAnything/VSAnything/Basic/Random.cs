using System;

namespace SCLCoreCLR
{
	public class Random
	{
		private const uint m_ArraySize = 624u;

		private uint[] MT = new uint[624];

		private uint m_Index;

		public Random() : this(1571)
		{
		}

		public Random(int seed) : this((uint)seed)
		{
		}

		public Random(uint seed)
		{
			this.SetSeed(seed);
		}

		public void SetSeed(int seed)
		{
			this.SetSeed((uint)seed);
		}

		public void SetSeed(uint seed)
		{
			this.MT[0] = seed;
			for (uint num = 1u; num < 624u; num += 1u)
			{
				this.MT[(int)num] = 1812433253u * (this.MT[(int)(num - 1u)] ^ this.MT[(int)(num - 1u)] >> 30) + num;
			}
		}

		public uint Generate()
		{
			if (this.m_Index == 0u)
			{
				this.GenerateNumbers();
			}
			uint expr_1B = this.MT[(int)this.m_Index];
			uint expr_20 = expr_1B ^ expr_1B >> 11;
			uint expr_2A = expr_20 ^ (expr_20 << 7 & 2636928640u);
			uint expr_35 = expr_2A ^ (expr_2A << 15 & 4022730752u);
			uint arg_4E_0 = expr_35 ^ expr_35 >> 18;
			this.m_Index = (this.m_Index + 1u) % 624u;
			return arg_4E_0;
		}

		public int GeneratePositiveInt()
		{
			return Math.Abs((int)this.Generate());
		}

		public float GenerateFloat()
		{
			return (float)(this.Generate() / 4294967295.0);
		}

		private void GenerateNumbers()
		{
			for (uint num = 0u; num < 624u; num += 1u)
			{
				uint num2 = (this.MT[(int)num] & 2147483648u) | (this.MT[(int)((num + 1u) % 624u)] & 2147483647u);
				this.MT[(int)num] = (this.MT[(int)((num + 397u) % 624u)] ^ num2 >> 1);
				if ((num2 & 1u) == 1u)
				{
					this.MT[(int)num] = (this.MT[(int)num] ^ 2567483615u);
				}
			}
		}
	}
}
