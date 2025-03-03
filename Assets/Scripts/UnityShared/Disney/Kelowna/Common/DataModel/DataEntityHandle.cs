namespace Disney.Kelowna.Common.DataModel
{
	public class DataEntityHandle
	{
		public static DataEntityHandle NullHandle = new DataEntityHandle(null);

		internal string Id;

		public bool IsNull
		{
			get
			{
				return Id == null;
			}
		}

		public DataEntityHandle(string id)
		{
			Id = id;
		}

		public static bool IsNullValue(DataEntityHandle handle)
		{
			return object.ReferenceEquals(null, handle) || handle.IsNull;
		}

		public static bool operator ==(DataEntityHandle a, DataEntityHandle b)
		{
			if (object.ReferenceEquals(null, a))
			{
				return false;
			}
			if (object.ReferenceEquals(null, b))
			{
				return false;
			}
			return a.equals(b);
		}

		public static bool operator !=(DataEntityHandle a, DataEntityHandle b)
		{
			if (object.ReferenceEquals(null, a))
			{
				return false;
			}
			if (object.ReferenceEquals(null, b))
			{
				return false;
			}
			return !a.equals(b);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return equals((DataEntityHandle)obj);
		}

		public override int GetHashCode()
		{
			return (Id != null) ? Id.GetHashCode() : 0;
		}

		private bool equals(DataEntityHandle other)
		{
			return Id == other.Id;
		}

		public override string ToString()
		{
			return string.Format("[DataEntityHandle: id={0}]", Id);
		}
	}
}
