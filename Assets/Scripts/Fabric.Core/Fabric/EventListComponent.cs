using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Events/EventListComponent")]
	public class EventListComponent : MonoBehaviour
	{
		[SerializeField]
		public List<string> _eventList = new List<string>();

		public void Start()
		{
			if (EventManager.Instance != null)
			{
				EventManager.Instance.AddEventNames(_eventList.ToArray());
			}
		}

		public void OnDestroy()
		{
			if (EventManager.Instance != null)
			{
				EventManager.Instance.RemoveEventNames(_eventList.ToArray());
			}
		}

		public void OnEnable()
		{
			if (EventManager.Instance != null)
			{
				EventManager.Instance.AddEventNames(_eventList.ToArray());
			}
		}

		public void OnDisable()
		{
			if (EventManager.Instance != null)
			{
				EventManager.Instance.RemoveEventNames(_eventList.ToArray());
			}
		}

		public void AddEventNamesFromFile(string filename)
		{
			StreamReader streamReader = new StreamReader(filename, Encoding.Default);
			if (streamReader == null)
			{
				return;
			}
			string text;
			do
			{
				text = streamReader.ReadLine();
				if (text != null)
				{
					int num = _eventList.IndexOf(text);
					if (num < 0)
					{
						_eventList.Add(text);
					}
				}
			}
			while (text != null);
			streamReader.Close();
		}

		public void ExportEventNamesToFile(string filename)
		{
			StreamWriter streamWriter = new StreamWriter(filename);
			if (streamWriter != null)
			{
				for (int i = 0; i < _eventList.Count; i++)
				{
					streamWriter.WriteLine(_eventList[i]);
				}
				streamWriter.Close();
			}
		}

		public void RemoveEventNamesFromFile(string filename)
		{
			StreamReader streamReader = new StreamReader(filename, Encoding.Default);
			if (streamReader == null)
			{
				return;
			}
			string text;
			do
			{
				text = streamReader.ReadLine();
				if (text != null)
				{
					int num = _eventList.IndexOf(text);
					if (num >= 0)
					{
						_eventList.RemoveAt(num);
					}
				}
			}
			while (text != null);
			streamReader.Close();
		}
	}
}
