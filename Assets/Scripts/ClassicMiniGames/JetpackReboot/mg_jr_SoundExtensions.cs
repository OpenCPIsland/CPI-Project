using NUnit.Framework;

namespace JetpackReboot
{
	public static class mg_jr_SoundExtensions
	{
		public static string ClipName(this mg_jr_Sound _sound)
		{
			string result = "";
			switch (_sound)
			{
			case mg_jr_Sound.COIN_COLLECT_LOOP:
				result = "mg_jr_sfx_CoinCollectLoop";
				break;
			case mg_jr_Sound.JETPACK_LOOP:
				result = "mg_jr_sfx_JetPackLoop";
				break;
			case mg_jr_Sound.ROBOT_HELI_LOOP:
				result = "mg_jr_sfx_RobotHeliLoop";
				break;
			case mg_jr_Sound.UI_POINTS_COUNTER_LOOP:
				result = "mg_jr_sfx_UIPointsCounterLoop";
				break;
			case mg_jr_Sound.PLAYER_TURBO_MODE_LOOP:
				result = "mg_jr_sfx_PlayerTurboModeLoop";
				break;
			case mg_jr_Sound.BOSS_KLUTZY_LOOP:
				result = "mg_jr_sfx_BossKlutzyFlyLoop";
				break;
			case mg_jr_Sound.BOSS_HERBERT_FLY_LOOP:
				result = "mg_jr_sfx_BossHerbertFlyLoop";
				break;
			case mg_jr_Sound.GARY_INTRO_JETPACK_LOOP:
				result = "mg_jr_sfx_UIGaryJetpackLoop";
				break;
			case mg_jr_Sound.JETPACK_LOOP_END:
				result = "mg_jr_sfx_JetPackLoopEnd";
				break;
			case mg_jr_Sound.PICKUP_COIN:
				result = "mg_jr_sfx_PickupCoin";
				break;
			case mg_jr_Sound.STORM_CLOUD:
				result = "mg_jr_sfx_StormCloud";
				break;
			case mg_jr_Sound.WHALE:
				result = "mg_jr_sfx_Whale";
				break;
			case mg_jr_Sound.OBSTACLE_EXPLODE_01:
				result = "mg_jr_sfx_ObjectExplode01";
				break;
			case mg_jr_Sound.OBSTACLE_EXPLODE_02:
				result = "mg_jr_sfx_ObjectExplode02";
				break;
			case mg_jr_Sound.FIRE_GREEN_CANNON:
				result = "mg_jr_sfx_CannonGreenFire";
				break;
			case mg_jr_Sound.FIRE_RED_CANNON:
				result = "mg_jr_sfx_CannonFireRed";
				break;
			case mg_jr_Sound.MENU_MUSIC_AMBIENT:
				result = "mg_jr_MenuMusicAmbient";
				break;
			case mg_jr_Sound.THEME_SONG_AMBIENT:
				result = "mg_jr_ThemeSongAmbient";
				break;
			case mg_jr_Sound.TURBO_MODE:
				result = "mg_jr_TurboMode";
				break;
			case mg_jr_Sound.PICKUP_COIN_10:
				result = "mg_jr_sfx_PickupCoin10";
				break;
			case mg_jr_Sound.PLAYER_DEATH:
				result = "mg_jr_sfx_PlayerDeath";
				break;
			case mg_jr_Sound.PLAYER_EXPLODE:
				result = "mg_jr_sfx_PlayerExplode";
				break;
			case mg_jr_Sound.ROBOT_HELI_EXPLODE:
				result = "mg_jr_sfx_RobotHeliExplode";
				break;
			case mg_jr_Sound.ROBOT_HELI_PICKUP:
				result = "mg_jr_sfx_RobotHeliPickup";
				break;
			case mg_jr_Sound.UI_GAMEOVER_SCREEN:
				result = "mg_jr_sfx_UIGameOverScreen";
				break;
			case mg_jr_Sound.UI_GARYTALK_LOOP:
				result = "mg_jr_sfx_UIGaryTalkLoop";
				break;
			case mg_jr_Sound.UI_GARYTEXT_POPUP:
				result = "mg_jr_sfx_UIGaryTextPopUp";
				break;
			case mg_jr_Sound.UI_SELECT:
				result = "mg_jr_sfx_UISelect";
				break;
			case mg_jr_Sound.UI_TURBO_MODE_END:
				result = "mg_jr_sfx_UITurboModeEnd";
				break;
			case mg_jr_Sound.UI_TURBO_MODE_START:
				result = "mg_jr_sfx_UITurboModeStart";
				break;
			case mg_jr_Sound.UI_JETPACK_FULL:
				result = "mg_jr_sfx_UIJetpackFull";
				break;
			case mg_jr_Sound.UI_NOTIFICATION:
				result = "mg_jr_sfx_UIGoalsScreenAppear";
				break;
			case mg_jr_Sound.PICKUP_JETPACK:
				result = "mg_jr_sfx_PickupJetpack";
				break;
			case mg_jr_Sound.PLAYER_JETPACK_TURBO_START:
				result = "mg_jr_sfx_PlayerTurboModeStart";
				break;
			case mg_jr_Sound.BOSS_PROTOBOT_RL:
				result = "mg_jr_sfx_BossProbotByRL";
				break;
			case mg_jr_Sound.BOSS_PROTOBOT_LR:
				result = "mg_jr_sfx_BossProbotByLR";
				break;
			case mg_jr_Sound.BOSS_ALERT:
				result = "mg_jr_sfx_UIAlarmBoss";
				break;
			case mg_jr_Sound.BOSS_KLUTZY_LAUGH:
				result = "mg_jr_sfx_BossKlutzyLaugh";
				break;
			case mg_jr_Sound.BOSS_HERBERT_LAUGH:
				result = "mg_jr_sfx_BossHerbertLaugh";
				break;
			case mg_jr_Sound.BOSS_HERBERT_FIRE:
				result = "mg_jr_sfx_BossHerbertFire";
				break;
			case mg_jr_Sound.GARY_INTRO_FLY_IN:
				result = "mg_jr_sfx_UIGaryFlyIn";
				break;
			case mg_jr_Sound.GARY_INTRO_FLY_OUT:
				result = "mg_jr_sfx_UIGaryFlyAway";
				break;
			case mg_jr_Sound.GOAL_FLY_BY:
				result = "mg_jr_sfx_UIJetpackFlyby";
				break;
			default:
				Assert.IsTrue(false, string.Concat("No case for _sound '", _sound, "'"));
				break;
			}
			return result;
		}
	}
}
