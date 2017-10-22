using System;
using System.Collections;
using System.Collections.Generic;

namespace SCLCoreCLR
{
	public class Set<T> : IEnumerable<T>, IEnumerable
	{
		private Dictionary<T, bool> m_Dictionary = new Dictionary<T, bool>();

		public int Count
		{
			get
			{
				return this.m_Dictionary.Count;
			}
		}

		public Set()
		{
		}

		public Set(Set<T> other)
		{
			this.m_Dictionary = new Dictionary<T, bool>(other.m_Dictionary);
		}

		public Set(IEnumerable<T> other)
		{
			foreach (T current in other)
			{
				this.Add(current);
			}
		}

		public void Clear()
		{
			this.m_Dictionary.Clear();
		}

		public void Add(T value)
		{
			if (!this.Contains(value))
			{
				this.m_Dictionary[value] = true;
			}
		}

		public void Remove(T value)
		{
			if (this.Contains(value))
			{
				this.m_Dictionary.Remove(value);
			}
		}

		public bool Contains(T value)
		{
			return this.m_Dictionary.ContainsKey(value);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.m_Dictionary.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new Exception("non generic GetEnumerator called!");
		}

		public T[] ToArray()
		{
			T[] array = new T[this.m_Dictionary.Count];
			int num = 0;
			foreach (T current in this.m_Dictionary.Keys)
			{
				array[num++] = current;
			}
			return array;
		}

		public List<T> ToList()
		{
			List<T> list = new List<T>(this.m_Dictionary.Count);
			foreach (T current in this.m_Dictionary.Keys)
			{
				list.Add(current);
			}
			return list;
		}

		public T Remove()
		{
			using (Dictionary<T, bool>.KeyCollection.Enumerator enumerator = this.m_Dictionary.Keys.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					this.m_Dictionary.Remove(current);
					return current;
				}
			}
			return default(T);
		}

		public static Set<T>operator -(Set<T> s1, Set<T> s2)
		{
			Set<T> set = new Set<T>();
			foreach (T current in s1)
			{
				if (!s2.Contains(current))
				{
					set.Add(current);
				}
			}
			return set;
		}

		public static Set<T>operator +(Set<T> s1, List<T> s2)
		{
			Set<T> set = new Set<T>(s1);
			foreach (T current in s2)
			{
				set.Add(current);
			}
			return set;
		}

		public Set<T> GetIntersection(Set<T> other)
		{
			Set<T> set = new Set<T>();
			foreach (T current in other)
			{
				if (this.m_Dictionary.ContainsKey(current))
				{
					set.Add(current);
				}
			}
			return set;
		}
	}
}
