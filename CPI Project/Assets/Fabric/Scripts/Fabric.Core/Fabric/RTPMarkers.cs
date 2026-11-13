using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class RTPMarkers
	{
		[SerializeField]
		public List<RTPMarker> _markers = new List<RTPMarker>();

		[NonSerialized]
		public RTPMarker _keyOffMarker;

		public RTPMarker GetMarker(string label)
		{
			for (int i = 0; i < _markers.Count; i++)
			{
				if (_markers[i]._label == label)
				{
					return _markers[i];
				}
			}
			return null;
		}

		public bool AddMarker(string label, float position)
		{
			if (GetMarker(label) != null)
			{
				return false;
			}
			_markers.Add(new RTPMarker(label, position));
			return true;
		}

		public bool RemoveMarker(string label)
		{
			RTPMarker marker = GetMarker(label);
			if (marker != null)
			{
				_markers.Remove(marker);
				return true;
			}
			return false;
		}

		public void RemoveMarker(RTPMarker rtpMarker)
		{
			_markers.Remove(rtpMarker);
		}

		public void KeyOffMarker()
		{
			if (_keyOffMarker != null)
			{
				_keyOffMarker._keyOff = true;
				_keyOffMarker = null;
			}
		}

		public bool IsMarkerKeyOff()
		{
			if (_keyOffMarker == null)
			{
				return false;
			}
			return true;
		}

		public void Reset()
		{
			for (int i = 0; i < _markers.Count; i++)
			{
				RTPMarker rTPMarker = _markers[i];
				rTPMarker._keyOff = false;
			}
		}

		public bool Update(float value, float direction)
		{
			for (int i = 0; i < _markers.Count; i++)
			{
				RTPMarker rTPMarker = _markers[i];
				if (rTPMarker._keyOffEnabled)
				{
					if (direction >= 0f && value >= rTPMarker._value && !rTPMarker._keyOff)
					{
						_keyOffMarker = rTPMarker;
						return true;
					}
					if (direction < 0f && value <= rTPMarker._value && !rTPMarker._keyOff)
					{
						_keyOffMarker = rTPMarker;
						return true;
					}
				}
			}
			return false;
		}
	}
}
