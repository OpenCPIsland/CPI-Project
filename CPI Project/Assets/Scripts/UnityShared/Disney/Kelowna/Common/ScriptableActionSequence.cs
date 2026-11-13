using Disney.LaunchPadFramework;
using System.Collections;
using System.Collections.Generic;

namespace Disney.Kelowna.Common
{
	public class ScriptableActionSequence : ScriptableAction, IDataInjection
	{
		public ScriptableAction[] Actions;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			for (int i = 0; i < Actions.Length; i++)
			{
				while (player != null && player.IsPaused)
				{
					yield return null;
				}
				IEnumerator enumerator = Actions[i].Execute(player);
				while (enumerator.MoveNext())
				{
					while (player != null && player.IsPaused)
					{
						yield return null;
					}
					yield return enumerator.Current;
				}
			}
		}

		public void InjectData(Dictionary<string, object> injectedData)
		{
			int num = Actions.Length;
			for (int i = 0; i < num; i++)
			{
				if (Actions[i] is IDataInjection)
				{
					(Actions[i] as IDataInjection).InjectData(injectedData);
				}
			}
		}
	}
}
