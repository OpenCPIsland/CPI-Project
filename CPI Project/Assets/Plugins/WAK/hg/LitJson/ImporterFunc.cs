namespace hg.LitJson
{
	internal delegate object ImporterFunc(object input);
	public delegate TValue ImporterFunc<TJson, TValue>(TJson input);
}
