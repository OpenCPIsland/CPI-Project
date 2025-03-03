namespace ZenFulcrum.EmbeddedBrowser
{
	public interface IPendingPromise2<PromisedT2> : IRejectable2
	{
		void Resolve(PromisedT2 value);
	}
	public interface IPendingPromise2 : IRejectable2
	{
		void Resolve();
	}
}
