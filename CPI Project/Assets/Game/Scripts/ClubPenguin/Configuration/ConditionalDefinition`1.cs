namespace ClubPenguin.Configuration
{
	public abstract class ConditionalDefinition<T> : ConditionalDefinition
	{
		public T DefaultValue;

		public abstract ConditionalTier<T>[] Tiers
		{
			get;
		}

		public override ConditionalProperty GenerateProperty()
		{
			return new ConditionalProperty<T>(this);
		}
	}
}
