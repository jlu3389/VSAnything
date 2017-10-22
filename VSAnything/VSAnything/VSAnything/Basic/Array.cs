using System;
using System.Collections;
using System.Collections.Generic;

namespace SCLCoreCLR
{
	public class Array<T> : IEnumerable<T>, IEnumerable
	{
		public sealed class Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private int m_Index = -1;

			private Array<T> m_Array;

			public T Current
			{
				get
				{
					return this.m_Array[this.m_Index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.m_Array[this.m_Index];
				}
			}

			public Enumerator(Array<T> array)
			{
				this.m_Array = array;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public bool MoveNext()
			{
				if (this.m_Index == this.m_Array.Count - 1)
				{
					return false;
				}
				this.m_Index++;
				return true;
			}

			public void Dispose()
			{
				this.m_Array = null;
			}
		}

		private int m_Capacity;

		private int m_Count;

		private T[] m_Array;

		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		public T this[int index]
		{
			get
			{
				return this.m_Array[index];
			}
			set
			{
				this.m_Array[index] = value;
			}
		}

		public Array()
		{
		}

		public Array(Array<T> other)
		{
			this.m_Capacity = other.m_Count;
			this.m_Count = other.m_Count;
			this.m_Array = new T[this.m_Count];
			for (int i = 0; i < this.Count; i++)
			{
				this.m_Array[i] = other.m_Array[i];
			}
		}

		public void Add(T value)
		{
			if (this.m_Count == this.m_Capacity)
			{
				this.Grow();
			}
			T[] arg_2C_0 = this.m_Array;
			int count = this.m_Count;
			this.m_Count = count + 1;
			arg_2C_0[count] = value;
		}

		private void Grow()
		{
			this.m_Capacity = ((this.m_Capacity != 0) ? (2 * this.m_Capacity) : 1);
			T[] array = new T[this.m_Capacity];
			for (int i = 0; i < this.m_Count; i++)
			{
				array[i] = this.m_Array[i];
			}
			this.m_Array = array;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Array<T>.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Array<T>.Enumerator(this);
		}

		public void Clear()
		{
			this.m_Count = 0;
		}
	}
}
