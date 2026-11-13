using System;
using UnityEngine;

namespace Fabric
{
	public class AudioSpline : MonoBehaviour
	{
		public CRSpline _CRSpline = new CRSpline();

		public AudioSplineSource _audioSplineSource;

		public float _time = 0.5f;

		public float _resolution = 0.01f;

		public Color _splineColor = Color.white;

		private AudioListener _audioListener;

		private void Start()
		{
			_CRSpline.BakeSpline(_resolution);
		}

		public void Initialise()
		{
			if (_CRSpline.pts == null || _CRSpline.pts.Length < 4)
			{
				_CRSpline.pts = new AudioSplinePoint[4];
				float num = 0f;
				for (int i = 0; i < 4; i++)
				{
					_CRSpline.pts[i] = CreateAudioSplinePoint();
					Vector3 position = _CRSpline.pts[i].transform.position;
					position.x = num;
					_CRSpline.pts[i].transform.position = position;
					num += 5f;
				}
				_CRSpline.RefreshPointNames();
				AddAudioSource();
			}
		}

		public void UpdateSplinePoints()
		{
			AudioSplinePoint[] componentsInChildren = base.gameObject.GetComponentsInChildren<AudioSplinePoint>();
			_CRSpline.pts = componentsInChildren;
			Array.Sort(componentsInChildren, (AudioSplinePoint x, AudioSplinePoint y) => string.Compare(x.name, y.name));
		}

		private AudioSplinePoint CreateAudioSplinePoint()
		{
			GameObject gameObject = new GameObject("AudioSplinePoint");
			gameObject.transform.parent = base.gameObject.transform;
			return gameObject.AddComponent<AudioSplinePoint>();
		}

		public void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			float num = 0.25f;
			Gizmos.DrawCube(base.transform.position, new Vector3(num, num, num));
			_CRSpline.GizmoDraw(_time, _splineColor, base.gameObject);
			if ((bool)_audioListener)
			{
				float t = 0f;
				Vector3 nearestPointToListener = _CRSpline.GetNearestPointToListener(_audioListener.transform.position, ref t, base.gameObject);
				Gizmos.DrawLine(nearestPointToListener, _audioListener.transform.position);
			}
		}

		private void Update()
		{
			float t = 0f;
			if (_audioListener == null)
			{
				if (FabricManager.Instance != null)
				{
					_audioListener = FabricManager.Instance._audioListener;
				}
				return;
			}
			Vector3 nearestPointToListener = _CRSpline.GetNearestPointToListener(_audioListener.transform.position, ref t, base.gameObject);
			if (_audioSplineSource != null)
			{
				_audioSplineSource.gameObject.transform.position = nearestPointToListener;
				_audioSplineSource.UpdateWithNormaliseTime(t);
			}
			_CRSpline.UpdateEventTriggers(nearestPointToListener);
		}

		public void AddAudioSource()
		{
			GameObject gameObject = new GameObject("AudioSplineSource");
			gameObject.transform.parent = base.gameObject.transform;
			_audioSplineSource = gameObject.AddComponent<AudioSplineSource>();
			_audioSplineSource.gameObject.AddComponent<EventTrigger>();
		}

		public void AddPoint(int index)
		{
			if (index >= 0)
			{
				GameObject gameObject = new GameObject("AudioSplinePoint");
				gameObject.transform.parent = base.gameObject.transform;
				AudioSplinePoint point = gameObject.AddComponent<AudioSplinePoint>();
				_CRSpline.AddPoint(index, point);
			}
		}

		public void RemovePoint(int index)
		{
			if (index >= 0)
			{
				UnityEngine.Object.DestroyImmediate(_CRSpline.pts[index].gameObject);
				_CRSpline.RemovePoint(index);
			}
		}
	}
}
