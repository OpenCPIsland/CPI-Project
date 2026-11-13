using System;
using UnityEngine;

namespace Fabric
{
	[ExecuteInEditMode]
	[AddComponentMenu("Fabric/Components/GroupComponent")]
	public class GroupComponent : Component
	{
		private bool _solo;

		[HideInInspector]
		[SerializeField]
		public bool _showInMixerView = true;

		[HideInInspector]
		[SerializeField]
		public string _targetGroupComponentPath;

		[NonSerialized]
		[HideInInspector]
		public bool _isRegisteredWithMainHierarchy;

		[SerializeField]
		[HideInInspector]
		public bool _ignoreUnloadUnusedAssets = true;

		[NonSerialized]
		[HideInInspector]
		public int _registeredWithMainRefCount;

		[NonSerialized]
		[HideInInspector]
		public static bool _createProxy = true;

		[SerializeField]
		public EventEditor _eventEditor;

		private bool finishedPlayingOncePerFrame;

		public bool Solo
		{
			get
			{
				return _solo;
			}
			set
			{
				_solo = value;
			}
		}

		public bool IsFabricHierarchyPresent()
		{
			if (base.transform.parent != null && (base.transform.parent.gameObject.GetComponent<FabricManager>() != null || base.transform.parent.gameObject.GetComponent<Component>() != null))
			{
				return true;
			}
			return false;
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents = false)
		{
			if (CheckMIDI(zComponentInstance))
			{
				finishedPlayingOncePerFrame = false;
				base.PlayInternal(zComponentInstance, target, curve, dontPlayComponents);
			}
		}

		internal override void OnFinishPlaying(double time)
		{
			if (_notifyParentComponent == NotifyParentType.AllComponentsHaveFinished)
			{
				int num = 0;
				for (int i = 0; i < _componentsArray.Length; i++)
				{
					if (_componentsArray[i].IsComponentActive())
					{
						num++;
					}
				}
				if (num <= 1)
				{
					base.OnFinishPlaying(time);
				}
			}
			else if (!finishedPlayingOncePerFrame)
			{
				base.OnFinishPlaying(time);
				finishedPlayingOncePerFrame = true;
			}
		}

		public void IncRef()
		{
			_registeredWithMainRefCount++;
		}

		public void DecRef()
		{
			_registeredWithMainRefCount--;
		}

		public void Awake()
		{
			RegisterWithMainHierarchy();
		}

		private void OnEnable()
		{
			if (Application.isEditor && !Application.isPlaying)
			{
				UnregisterWithMainHierarchy();
				RegisterWithMainHierarchy();
			}
		}

		private void Update()
		{
			if (_eventEditor != null)
			{
				_eventEditor.Update();
			}
		}

		private void OnDestroy()
		{
			if (!_quitting && !IsFabricHierarchyPresent() && !(FabricManager.Instance == null))
			{
				UnregisterWithMainHierarchy();
				Destroy();
			}
		}

		public void RegisterWithMainHierarchy()
		{
			if (!Component._initializationInProgress && !(FabricManager.Instance == null) && !IsFabricHierarchyPresent() && !_isInstance)
			{
				if (_eventEditor != null)
				{
					_eventEditor.Initialise();
				}
				_isRegisteredWithMainHierarchy = FabricManager.Instance.RegisterGroupComponent(this, _targetGroupComponentPath, _createProxy);
			}
		}

		public void UnregisterWithMainHierarchy()
		{
			if (_isRegisteredWithMainHierarchy)
			{
				if (_eventEditor != null)
				{
					_eventEditor.Shutdown();
				}
				_isRegisteredWithMainHierarchy = FabricManager.Instance.UnregisterGroupComponent(this);
				if (!_ignoreUnloadUnusedAssets)
				{
					Resources.UnloadUnusedAssets();
				}
			}
		}
	}
}
