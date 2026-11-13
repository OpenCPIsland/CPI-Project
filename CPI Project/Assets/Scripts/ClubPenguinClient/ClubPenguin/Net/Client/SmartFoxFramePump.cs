using UnityEngine;
using System;

namespace ClubPenguin.Net.Client
{
    public class SmartFoxFramePump : MonoBehaviour
    {
        private static SmartFoxFramePump _instance;

        private Action _onTick;

        public static void Register(Action tickAction)
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SmartFoxFramePump");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<SmartFoxFramePump>();
            }

            _instance._onTick = tickAction;
        }

        void Update()
        {
           _onTick?.Invoke();
        }
    }
}
