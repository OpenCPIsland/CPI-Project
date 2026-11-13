using DisneyMobile.CoreUnitySystems.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	[RequireComponent(typeof(Transition))]
	public class InitializerComponent : MonoBehaviour
	{
		[SerializeField]
		protected List<string> mInitActionList = new List<string>();

		protected EventDispatcher mEventDispatcher = null;

		protected InitManager mInitManager = null;

		public InitManager InitManager
		{
			get
			{
				if (mInitManager == null)
				{
					mInitManager = new InitManager(EventDispatcher);
					return mInitManager;
				}
				return mInitManager;
			}
		}

		public EventDispatcher EventDispatcher
		{
			get
			{
				if (mEventDispatcher == null)
				{
					mEventDispatcher = new EventDispatcher();
				}
				return mEventDispatcher;
			}
		}

		public void RemoveMissingInitActions()
		{
			mInitActionList.RemoveAll(delegate(string initAction)
			{
				Type typeInAllAssemblies = ReflectionHelper.GetTypeInAllAssemblies(initAction);
				return typeInAllAssemblies == null || initAction == null;
			});
		}

		public List<string> GetInitActions()
		{
			return mInitActionList;
		}

		public void AddInitAction(Type initActionType)
		{
			mInitActionList.AddIfUnique(initActionType.ToString());
		}

		public void RemoveInitAction(Type initActionType)
		{
			mInitActionList.Remove(initActionType.ToString());
		}

		public void Initialize()
		{
			AddInitActions();
			EventDispatcher.AddListener<InitCompleteEvent>(OnInitComplete);
			InitManager.Process(BaseGameController.Instance.Configurator);
		}

		protected void AddInitActions()
		{
			InitManager.Clear();
			for (int i = 0; i < mInitActionList.Count; i++)
			{
				object obj = Activator.CreateInstance(ReflectionHelper.GetTypeInAllAssemblies(mInitActionList[i]));
				InitManager.AddInitAction(obj as InitAction);
			}
		}

		protected virtual bool OnInitComplete(InitCompleteEvent evt)
		{
			Logger.LogDebug(this, "Initialization complete in InitializerComponent", Logger.TagFlags.INIT);
			mInitManager = null;
			mEventDispatcher = null;
			InitManager.Clear();
			return false;
		}
	}
}
