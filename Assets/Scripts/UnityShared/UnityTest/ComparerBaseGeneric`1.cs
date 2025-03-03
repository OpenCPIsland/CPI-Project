using System;

namespace UnityTest
{


    [Serializable]
    public abstract class ComparerBaseGeneric<T1, T2> : ComparerBase
    {
        public T2 constantValueGeneric = default(T2);

        public override object ConstValue
        {
            get
            {
                return constantValueGeneric;
            }
            set
            {
                constantValueGeneric = (T2)value;
            }
        }

        protected override bool UseCache
        {
            get
            {
                return true;
            }
        }

        public override object GetDefaultConstValue()
        {
            return default(T2);
        }

        private static bool IsValueType(Type type)
        {
            return type.IsValueType;
        }

        protected override bool Compare(object a, object b)
        {
            Type typeFromHandle = typeof(T2);
            if (b == null && IsValueType(typeFromHandle))
            {
                throw new ArgumentException("Null was passed to a value-type argument");
            }
            return Compare((T1)a, (T2)b);
        }

        protected abstract bool Compare(T1 a, T2 b);

        public override Type[] GetAccepatbleTypesForA()
        {
            return new Type[1]
            {
                typeof(T1)
            };
        }

        public override Type[] GetAccepatbleTypesForB()
        {
            return new Type[1]
            {
                typeof(T2)
            };
        }
    }
    [Serializable]
    public abstract class ComparerBaseGeneric<T> : ComparerBaseGeneric<T, T>
    {
    }
}
