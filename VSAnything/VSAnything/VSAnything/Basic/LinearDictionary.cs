using System;
using System.Collections.Generic;

namespace SCLCoreCLR
{
	public class LinearDictionary<TKey, TValue>
	{
		private struct Pair
		{
			public TKey m_Key;

			public TValue m_Value;
		}

		private List<LinearDictionary<TKey, TValue>.Pair> m_Pairs = new List<LinearDictionary<TKey, TValue>.Pair>();

		public TValue this[TKey key]
		{
			get
			{
				int num = 0;
				foreach (LinearDictionary<TKey, TValue>.Pair current in this.m_Pairs)
				{
					TKey key2 = current.m_Key;
					if (key2.Equals(key))
					{
						if (num != 0)
						{
							LinearDictionary<TKey, TValue>.Pair value = this.m_Pairs[0];
							this.m_Pairs[0] = current;
							this.m_Pairs[num] = value;
						}
						return current.m_Value;
					}
					num++;
				}
				return default(TValue);
			}
			set
			{
				LinearDictionary<TKey, TValue>.Pair item = default(LinearDictionary<TKey, TValue>.Pair);
				item.m_Key = key;
				item.m_Value = value;
				this.m_Pairs.Add(item);
			}
		}

		public void Clear()
		{
			this.m_Pairs.Clear();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			foreach (LinearDictionary<TKey, TValue>.Pair current in this.m_Pairs)
			{
				TKey key2 = current.m_Key;
				if (key2.Equals(key))
				{
					value = current.m_Value;
					return true;
				}
			}
			value = default(TValue);
			return false;
		}
	}
}
