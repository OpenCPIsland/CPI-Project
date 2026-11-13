using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems.Utility.Collections
{
	public class Array2D<T>
	{
		private List<List<T>> m_data = null;

		public int Width
		{
			get
			{
				if (m_data != null)
				{
					return m_data.Count;
				}
				return 0;
			}
		}

		public int Height
		{
			get
			{
				if (m_data != null && m_data[0] != null)
				{
					return m_data[0].Count;
				}
				return 0;
			}
		}

		public Array2D(int width, int height, T initialValue)
		{
			m_data = new List<List<T>>(width);
			for (int i = 0; i < width; i++)
			{
				m_data.Add(new List<T>(height));
				for (int j = 0; j < height; j++)
				{
					m_data[i].Add(initialValue);
				}
			}
		}

		public void SetAt(int x, int y, T value)
		{
			if (m_data != null)
			{
				m_data[x][y] = value;
			}
		}

		public T GetAt(int x, int y)
		{
			return m_data[x][y];
		}

		public void SetAll(T value)
		{
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					SetAt(i, j, value);
				}
			}
		}

		public void RemoveRow(int y)
		{
			for (int i = 0; i < Width; i++)
			{
				m_data[i].RemoveAt(y);
			}
		}

		public void RemoveColumn(int x)
		{
			m_data.RemoveAt(x);
		}

		public void InsertRow(int y, T initialValue)
		{
			for (int i = 0; i < Width; i++)
			{
				m_data[i].Insert(y, initialValue);
			}
		}

		public void InsertColumn(int x, T initialValue)
		{
			List<T> list = new List<T>(Height);
			for (int i = 0; i < Height; i++)
			{
				list[i] = initialValue;
			}
			m_data.Insert(x, list);
		}
	}
}
