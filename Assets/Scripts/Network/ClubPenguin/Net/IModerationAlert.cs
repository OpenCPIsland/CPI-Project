using Disney.Mix.SDK;

namespace ClubPenguin.Net
{
	public interface IModerationAlert
	{
		bool IsCritical
		{
			get;
		}

		string Text
		{
			get;
		}

		IAlert MixAlert
		{
			get;
		}
	}
}
