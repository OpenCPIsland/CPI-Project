namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class ModerateTextRequest : BaseUserRequest
	{
		public string Text;

		public string Language;

		public long? ChatThreadId;

		public string ModerationPolicy;
	}
}
