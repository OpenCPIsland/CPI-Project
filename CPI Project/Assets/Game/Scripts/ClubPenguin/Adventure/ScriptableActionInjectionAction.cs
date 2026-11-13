using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using HutongGames.PlayMaker;
using System.Collections.Generic;

namespace ClubPenguin.Adventure
{
	public abstract class ScriptableActionInjectionAction<T> : FsmStateAction
	{
		public ScriptableAction InjectibleAction;

		public FsmString DataKey;

		public T DataObject;

		public override void OnEnter()
		{
			if (InjectibleAction is IDataInjection)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(DataKey.Value, getDataObject());
				(InjectibleAction as IDataInjection).InjectData(dictionary);
			}
			Finish();
		}

		protected virtual object getDataObject()
		{
			return DataObject;
		}
	}
}
