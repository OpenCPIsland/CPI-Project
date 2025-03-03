namespace Disney.Kelowna.Common.DataModel
{
	public class DataEntityEvents
	{
		public struct EntityAddedEvent
		{
			public readonly DataEntityHandle EntityHandle;

			public EntityAddedEvent(DataEntityHandle entityHandle)
			{
				EntityHandle = entityHandle;
			}
		}

		public struct EntityRemovedEvent
		{
			public readonly DataEntityHandle EntityHandle;

			public EntityRemovedEvent(DataEntityHandle entityHandle)
			{
				EntityHandle = entityHandle;
			}
		}

		public struct ComponentAddedEvent<T>
		{
			public readonly DataEntityHandle Handle;

			public readonly T Component;

			public ComponentAddedEvent(DataEntityHandle handle, T component)
			{
				Handle = handle;
				Component = component;
			}
		}

		public struct ComponentRemovedEvent
		{
			public readonly DataEntityHandle Handle;

			public readonly BaseData Component;

			public ComponentRemovedEvent(DataEntityHandle handle, BaseData component)
			{
				Handle = handle;
				Component = component;
			}
		}
	}
}
