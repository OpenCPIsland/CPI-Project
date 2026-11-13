using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.Task
{
	[Serializable]
	public class TaskDefinitionContentKey : TypedAssetContentKey<TaskDefinition>
	{
		public TaskDefinitionContentKey(string key)
			: base(key)
		{
		}
	}
}
