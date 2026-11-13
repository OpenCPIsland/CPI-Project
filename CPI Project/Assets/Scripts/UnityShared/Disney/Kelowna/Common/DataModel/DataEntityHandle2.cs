namespace Disney.Kelowna.Common.DataModel
{
    public class DataEntityHandle2
    {
        public static DataEntityHandle2 NullHandle = new DataEntityHandle2(null);

        internal string Id;

        public bool IsNull
        {
            get
            {
                return Id == null;
            }
        }

        public DataEntityHandle2(string id)
        {
            Id = id;
        }

        public static bool IsNullValue(DataEntityHandle2 handle)
        {
            return object.ReferenceEquals(null, handle) || handle.IsNull;
        }

        public static bool operator ==(DataEntityHandle2 a, DataEntityHandle2 b)
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

        public static bool operator !=(DataEntityHandle2 a, DataEntityHandle2 b)
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
            return equals((DataEntityHandle2)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null) ? Id.GetHashCode() : 0;
        }

        private bool equals(DataEntityHandle2 other)
        {
            return Id == other.Id;
        }

        public override string ToString()
        {
            return string.Format("[DataEntityHandle2: id={0}]", Id);
        }
    }
}
