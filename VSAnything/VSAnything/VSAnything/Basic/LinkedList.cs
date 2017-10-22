using System;
using System.Collections;
using System.Collections.Generic;

namespace SCLCoreCLR
{
	public class LinkedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : LinkedListNode
	{
		private class Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private LinkedList<T> m_List;

			private T m_Current;

			object IEnumerator.Current
			{
				get
				{
					return this.m_Current;
				}
			}

			public T Current
			{
				get
				{
					return this.m_Current;
				}
			}

			public Enumerator(LinkedList<T> list)
			{
				this.m_List = list;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				this.m_Current = ((this.m_Current != null) ? ((T)((object)this.m_Current.m_Next)) : this.m_List.Head);
				return this.m_Current != null;
			}

			public void Reset()
			{
				this.m_Current = default(T);
			}
		}

		private T m_Head;

		private T m_Tail;

		private int m_Count;

		private bool m_ReadOnly;

		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.m_ReadOnly;
			}
			set
			{
				this.m_ReadOnly = value;
			}
		}

		public T Head
		{
			get
			{
				return this.m_Head;
			}
		}

		public T Tail
		{
			get
			{
				return this.m_Tail;
			}
		}

		public void AddFirst(T node)
		{
			if (this.m_Head != null)
			{
				this.m_Head.m_Prev = node;
			}
			if (this.m_Tail == null)
			{
				this.m_Tail = node;
			}
			node.m_Next = this.m_Head;
			this.m_Head = node;
			this.m_Count++;
		}

		public void AddLast(T node)
		{
			if (this.m_Head == null)
			{
				this.m_Head = node;
			}
			if (this.m_Tail != null)
			{
				this.m_Tail.m_Next = node;
			}
			node.m_Prev = this.m_Tail;
			this.m_Tail = node;
			this.m_Count++;
		}

		public void Add(T item)
		{
			this.AddLast(item);
		}

		public void Clear()
		{
			this.m_Head = (this.m_Tail = default(T));
			this.m_Count = 0;
		}

		public bool Contains(T item)
		{
			for (LinkedListNode linkedListNode = this.m_Head; linkedListNode != null; linkedListNode = linkedListNode.m_Next)
			{
				if (linkedListNode == item)
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			T t = this.m_Head;
			int num = 0;
			while (t != null)
			{
				array[num + arrayIndex] = t;
				t = (T)((object)t.m_Next);
			}
		}

		public bool Remove(T item)
		{
			if (item.m_Prev == null)
			{
				this.m_Head = (T)((object)item.m_Next);
			}
			else
			{
				item.m_Prev.m_Next = item.m_Next;
			}
			if (item.m_Next == null)
			{
				this.m_Tail = (T)((object)item.m_Prev);
			}
			else
			{
				item.m_Next.m_Prev = item.m_Prev;
			}
			this.m_Count--;
			item.m_Prev = (item.m_Next = null);
			return true;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new LinkedList<T>.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new Exception("non generic GetEnumerator called!");
		}

		public void InsertAfter(T node, T prev_node)
		{
			if (prev_node == null)
			{
				this.AddFirst(node);
			}
			else if (prev_node.m_Next == null)
			{
				this.AddLast(node);
			}
			else
			{
				node.m_Prev = prev_node;
				node.m_Next = prev_node.m_Next;
				prev_node.m_Next.m_Prev = node;
				prev_node.m_Next = node;
			}
			this.m_Count++;
		}

		public void InsertBefore(T node, T next_node)
		{
			if (next_node == null)
			{
				this.AddLast(node);
			}
			else if (next_node.m_Prev == null)
			{
				this.AddFirst(node);
			}
			else
			{
				node.m_Next = next_node;
				node.m_Prev = next_node.m_Prev;
				next_node.m_Prev.m_Next = node;
				next_node.m_Prev = node;
			}
			this.m_Count++;
		}
	}
}
