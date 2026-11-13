using System;
using System.Collections.Generic;

namespace Fabric
{
	public sealed class ShuffleBagCollection<T>
	{
		private List<T> m_data = new List<T>();

		private int m_cursor = -1;

		private T m_current = default(T);

		private Random m_generator
		{
			get
			{
				return Generic._random;
			}
		}

		public T Current
		{
			get
			{
				return m_current;
			}
		}

		public int Capacity
		{
			get
			{
				return m_data.Capacity;
			}
		}

		public int Size
		{
			get
			{
				return m_data.Count;
			}
		}

		public void Add(T item)
		{
			Add(item, 1);
		}

		public void Add(T item, int quantity)
		{
			for (int i = 0; i < quantity; i++)
			{
				m_data.Add(item);
			}
			m_cursor = m_data.Count - 1;
		}

		public T Next()
		{
			if (m_cursor < 1)
			{
				m_cursor = m_data.Count - 1;
				m_current = m_data[0];
				return m_current;
			}
			int index = m_generator.Next(m_cursor);
			m_current = m_data[index];
			m_data[index] = m_data[m_cursor];
			m_data[m_cursor] = m_current;
			m_cursor--;
			return m_current;
		}

		public void TrimExcess()
		{
			m_data.TrimExcess();
		}
	}
}
