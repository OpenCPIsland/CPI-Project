using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Scene;

namespace ClubPenguin.Igloo
{
	public static class IglooEvents
	{
		public struct ReloadLayout
		{
			public readonly SceneLayout NewLayout;

			public ReloadLayout(SceneLayout newLayout)
			{
				NewLayout = newLayout;
			}
		}

		public struct ChangeZone
		{
			public readonly ZoneId ZoneId;

			public ChangeZone(ZoneId zoneId)
			{
				ZoneId = zoneId;
			}
		}

		public struct CreateIgloo
		{
			public readonly bool Success;

			public CreateIgloo(bool success)
			{
				Success = success;
			}
		}

		public struct PlayIglooSound
		{
			public readonly string Sound;

			public PlayIglooSound(string sound)
			{
				Sound = sound;
			}
		}
	}
}
