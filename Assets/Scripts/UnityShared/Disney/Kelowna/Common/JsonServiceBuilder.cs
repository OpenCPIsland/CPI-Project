namespace Disney.Kelowna.Common
{
	public class JsonServiceBuilder
	{
		public static JsonService Build(bool prettyPrint = false)
		{
			LitJsonService litJsonService = new LitJsonService();
			litJsonService.PrettyPrint = prettyPrint;
			return litJsonService;
		}
	}
}
