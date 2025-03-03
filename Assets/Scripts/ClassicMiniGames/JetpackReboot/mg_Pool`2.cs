using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
    public class mg_Pool<T, U> : MonoBehaviour where T : class where U : mg_ICreator<T>
    {
        protected Stack<T> m_collection;

        protected U m_creator;

        protected virtual void Awake()
        {
            m_collection = new Stack<T>();
        }

        protected virtual void Start()
        {
            Assert.NotNull(m_creator, "Pools require a craetor, make sure you call Init with a valid mg_ICreator");
        }

        public virtual void Init(U _creator)
        {
            m_creator = _creator;
        }

        public virtual T Fetch()
        {
            T val = null;
            if (m_collection.Count == 0)
            {
                return m_creator.Create();
            }
            return m_collection.Pop();
        }

        public virtual void Return(T _objectToAddToPool)
        {
            Assert.NotNull(_objectToAddToPool, "Can't add null to a pool");
            m_collection.Push(_objectToAddToPool);
        }
    }
}

