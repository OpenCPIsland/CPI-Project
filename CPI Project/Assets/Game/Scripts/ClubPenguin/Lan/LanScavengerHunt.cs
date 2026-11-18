#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClubPenguin.Net.Lan
{
	public static class LanMotion
	{
		public const double MaxJump = 20;

		public const double MaxSpeed = 30;

		public const long SpeedSpanMs = 1000;

		public static double Dist(float[] a, float[] b)
		{
			double dx = a[0] - b[0];
			double dy = a[1] - b[1];
			double dz = a[2] - b[2];
			return Math.Sqrt(dx * dx + dy * dy + dz * dz);
		}

		public static float[] Copy(float[] p)
		{
			return new float[3]
			{
				p[0],
				p[1],
				p[2]
			};
		}
	}

	public sealed class LanScavengerHunt : LanPartyGameSession
	{
		private sealed class Sample
		{
			public float[] P;

			public long T;
		}

		private const int SetRole = 0;

		private const int SetGameState = 1;

		private const int AddItem = 2;

		private const int RemoveItem = 3;

		private const int RoleHider = 0;

		private const int RoleFinder = 1;

		private const int StateHiding = 1;

		private const int StateFinding = 2;

		private const int StateSettling = 3;

		private const int Action1 = 7;

		private const int Action2 = 8;

		private const string HiderProp = "ScavengerHuntHiderServerAdded";

		private const string FinderProp = "ScavengerHuntFinderServerAdded";

		private const double PropSettleSeconds = 1;

		private const double FinishDelaySeconds = 1;

		private const int MaxSamples = 50;

		private const double MaxDropStep = 3;

		private const double MinMarbleGap = 1;

		private const double PickupRadius = 6;

		private LanPartyGamePlayer hider;

		private LanPartyGamePlayer finder;

		private int state;

		private int nextItemId = 1;

		private readonly Dictionary<int, float[]> marbles = new Dictionary<int, float[]>();

		private int hidden;

		private int found;

		private readonly Dictionary<LanPartyGamePlayer, float[]> lastPos = new Dictionary<LanPartyGamePlayer, float[]>();

		private readonly HashSet<LanPartyGamePlayer> jumped = new HashSet<LanPartyGamePlayer>();

		private readonly Dictionary<LanPartyGamePlayer, List<Sample>> samples = new Dictionary<LanPartyGamePlayer, List<Sample>>();

		private List<KeyValuePair<LanPlayer, int>> decided;

		public LanScavengerHunt(LanPartyGames host, int id, LanPartyGameDef def, List<LanPlayer> players, List<LanSlot> slots)
			: base(host, id, def, players, slots)
		{
		}

		public override string GamePropFor(LanPartyGamePlayer player)
		{
			if (player == hider)
			{
				return HiderProp;
			}
			return (player == finder) ? FinderProp : null;
		}

		public override void OnStart()
		{
			int h = Host.Server.Random.Next(2);
			hider = Players[h];
			finder = Players[1 - h];
			foreach (LanPartyGamePlayer p in Players)
			{
				float[] at = p.Player.LastPosition;
				if (at != null && at.Length == 3)
				{
					lastPos[p] = LanMotion.Copy(at);
				}
			}
			SendMessageTo(hider, SetRole, "{\"RoleId\":" + RoleHider + "}");
			SendMessageTo(finder, SetRole, "{\"RoleId\":" + RoleFinder + "}");
			Host.SetProp(hider.Player, HiderProp);
			Timer(Seconds("IntroTimeInSeconds", 5.0), delegate
			{
				state = StateHiding;
				SendMessage(SetGameState, "{\"GameStateId\":" + StateHiding + "}");
				Timer(Seconds("HideTimeInSeconds", 25.0) + Seconds("HideBufferTimeInSeconds", 3.0), StartFinding);
			});
		}

		private void StartFinding()
		{
			state = StateSettling;
			Host.SetProp(finder.Player, FinderProp);
			Timer(PropSettleSeconds, delegate
			{
				state = StateFinding;
				SendMessage(SetGameState, "{\"GameStateId\":" + StateFinding + "}");
				Timer(Seconds("FindTimeInSeconds", 55.0) + Seconds("FindBufferTimeInSeconds", 3.0), Finish);
			});
		}

		public override void OnPlayerMoved(LanPartyGamePlayer player, float[] position)
		{
			float[] before;
			lastPos.TryGetValue(player, out before);
			long now = Loop.NowMs;
			List<Sample> hist;
			if (!samples.TryGetValue(player, out hist))
			{
				hist = new List<Sample>();
				samples[player] = hist;
			}
			Sample s = new Sample();
			s.P = LanMotion.Copy(position);
			s.T = now;
			hist.Add(s);
			while (hist.Count > 1 && now - hist[0].T > LanMotion.SpeedSpanMs * 2)
			{
				hist.RemoveAt(0);
			}
			bool flooding = hist.Count > MaxSamples;
			if (flooding)
			{
				hist.RemoveRange(0, hist.Count - MaxSamples);
			}
			double along = 0.0;
			double maxSpeed = 0.0;
			for (int i = hist.Count - 2; i >= 0; i--)
			{
				along += LanMotion.Dist(hist[i].P, hist[i + 1].P);
				maxSpeed = Math.Max(maxSpeed, along * 1000.0 / (double)Math.Max(now - hist[i].T, LanMotion.SpeedSpanMs));
				if (now - hist[i].T > LanMotion.SpeedSpanMs)
				{
					break;
				}
			}
			double jump = (before != null) ? LanMotion.Dist(position, before) : 0.0;
			if (flooding || jump > LanMotion.MaxJump || maxSpeed > LanMotion.MaxSpeed)
			{
				jumped.Add(player);
			}
			lastPos[player] = LanMotion.Copy(position);
		}

		public override void OnAction(LanPartyGamePlayer player, int action, float[] position)
		{
			if (player != hider || (action != Action1 && action != Action2) || state != StateHiding || jumped.Contains(player))
			{
				return;
			}
			int max = (int)Seconds("MaxHiddenItems", 3.0);
			if (marbles.Count >= max)
			{
				return;
			}
			float[] previous;
			if (lastPos.TryGetValue(player, out previous) && LanMotion.Dist(position, previous) > MaxDropStep)
			{
				return;
			}
			if (marbles.Values.Any((float[] m) => LanMotion.Dist(position, m) < MinMarbleGap))
			{
				return;
			}
			int id = nextItemId++;
			marbles[id] = LanMotion.Copy(position);
			hidden++;
			SendMessage(AddItem, "{\"ItemId\":" + id + ",\"Pos\":{\"x\":" + F(position[0]) + ",\"y\":" + F(position[1]) + ",\"z\":" + F(position[2]) + "}}");
		}

		private static string F(float f)
		{
			return ((double)f).ToString("R", CultureInfo.InvariantCulture);
		}

		public override void OnMessage(LanPartyGamePlayer player, int type, object message)
		{
			if (type != RemoveItem || player != finder || state != StateFinding || decided != null || jumped.Contains(player))
			{
				return;
			}
			long id;
			if (!LanJson.TryLong(LanJson.Get(message, "ItemId"), out id) || id < 0 || id > int.MaxValue)
			{
				return;
			}
			float[] at;
			if (!marbles.TryGetValue((int)id, out at))
			{
				return;
			}
			float[] here;
			if (!lastPos.TryGetValue(player, out here) || LanMotion.Dist(here, at) > PickupRadius)
			{
				return;
			}
			marbles.Remove((int)id);
			found++;
			SendMessage(RemoveItem, "{\"ItemId\":" + id + "}");
			if (marbles.Count == 0)
			{
				decided = PlacementsFor(finder, hider);
				Timer(FinishDelaySeconds, delegate
				{
					EndWith(decided);
				});
			}
		}

		private static List<KeyValuePair<LanPlayer, int>> PlacementsFor(LanPartyGamePlayer win, LanPartyGamePlayer lose)
		{
			return Placements(Place(win.Player, 0), Place(lose.Player, 1));
		}

		private void Finish()
		{
			bool finderWins = found >= hidden;
			EndWith(decided ?? (finderWins ? PlacementsFor(finder, hider) : PlacementsFor(hider, finder)));
		}

		public override void OnPlayerQuit(LanPartyGamePlayer player)
		{
			if (decided != null)
			{
				EndWith(decided);
				return;
			}
			LanPartyGamePlayer stayer = Players.FirstOrDefault((LanPartyGamePlayer p) => p != player && !p.Left);
			List<KeyValuePair<LanPlayer, int>> placements = new List<KeyValuePair<LanPlayer, int>>();
			if (stayer != null)
			{
				placements.Add(Place(stayer.Player, 0));
			}
			placements.Add(Place(player.Player, 1));
			EndWith(placements, new HashSet<LanPlayer>
			{
				player.Player
			});
		}

		public override void OnPlayerLeft(LanPartyGamePlayer player)
		{
			if (decided != null)
			{
				EndWith(decided.Where((KeyValuePair<LanPlayer, int> kv) => kv.Key != player.Player).ToList());
				return;
			}
			LanPartyGamePlayer stayer = Players.FirstOrDefault((LanPartyGamePlayer p) => p != player && !p.Left);
			EndWith((stayer != null) ? Placements(Place(stayer.Player, 0)) : new List<KeyValuePair<LanPlayer, int>>());
		}
	}
}
#endif
