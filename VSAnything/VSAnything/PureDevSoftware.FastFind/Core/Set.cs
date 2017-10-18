using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Company.VSAnything
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
			foreach (T value in other)
			{
				this.Add(value);
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
			int index = 0;
			foreach (T value in this.m_Dictionary.Keys)
			{
				array[index++] = value;
			}
			return array;
		}

		public List<T> ToList()
		{
			List<T> list = new List<T>(this.m_Dictionary.Count);
			foreach (T value in this.m_Dictionary.Keys)
			{
				list.Add(value);
			}
			return list;
		}

		public T Remove()
		{
			using (Dictionary<T, bool>.KeyCollection.Enumerator enumerator = this.m_Dictionary.Keys.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					T value = enumerator.Current;
					this.m_Dictionary.Remove(value);
					return value;
				}
			}
			return default(T);
		}

		public static Set<T>operator -(Set<T> s1, Set<T> s2)
		{
			Set<T> result = new Set<T>();
			foreach (T value in s1)
			{
				if (!s2.Contains(value))
				{
					result.Add(value);
				}
			}
			return result;
		}

		public static Set<T>operator +(Set<T> s1, List<T> s2)
		{
			Set<T> s3 = new Set<T>(s1);
			foreach (T v in s2)
			{
				s3.Add(v);
			}
			return s3;
		}

		public Set<T> GetIntersection(Set<T> other)
		{
			Set<T> intersection = new Set<T>();
			foreach (T value in other)
			{
				if (this.m_Dictionary.ContainsKey(value))
				{
					intersection.Add(value);
				}
			}
			return intersection;
		}

		public static bool Match(Set<T> set1, Set<T> set2)
		{
			return set1.m_Dictionary.Keys.SequenceEqual(set2.m_Dictionary.Keys);
		}
	}
}
