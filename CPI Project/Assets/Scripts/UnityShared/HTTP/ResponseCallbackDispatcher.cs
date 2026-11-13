using System.Collections;
using UnityEngine;

namespace HTTP
{
	public class ResponseCallbackDispatcher : MonoBehaviour
	{
		private static ResponseCallbackDispatcher singleton = null;

		private static GameObject singletonGameObject = null;

		private static object singletonLock = new object();

		public Queue requests = Queue.Synchronized(new Queue());

		public static ResponseCallbackDispatcher Singleton
		{
			get
			{
				return singleton;
			}
		}

		public static void Init()
		{
			if (!(singleton != null))
			{
				lock (singletonLock)
				{
					if (!(singleton != null))
					{
						singletonGameObject = new GameObject();
						singleton = singletonGameObject.AddComponent<ResponseCallbackDispatcher>();
						singletonGameObject.name = "HTTPResponseCallbackDispatcher";
					}
				}
			}
		}

		public void Update()
		{
			while (requests.Count > 0)
			{
				Request request = (Request)requests.Dequeue();
				request.completedCallback(request);
			}
		}
	}
}
