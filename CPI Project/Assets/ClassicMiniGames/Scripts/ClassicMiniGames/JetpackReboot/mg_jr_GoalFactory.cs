using DevonLocalization.Core;
using Disney.MobileNetwork;
using NUnit.Framework;

namespace JetpackReboot
{
	public class mg_jr_GoalFactory
	{
		public mg_jr_Goal MakeGoal(mg_jr_Goal.GoalType _type, mg_jr_Goal.GoalDuration _duration)
		{
			Localizer localizer = null;
			if (Service.IsSet<Localizer>())
			{
				localizer = Service.Get<Localizer>();
			}
			mg_jr_Goal result = null;
			bool flag = false;
			int[] array = new int[5];
			int[] array2 = new int[5];
			string pluralDescription = "";
			string singularDescription = "";
			switch (_duration)
			{
			case mg_jr_Goal.GoalDuration.SINGLE_RUN:
				switch (_type)
				{
				case mg_jr_Goal.GoalType.DISTANCE_TRAVELLED:
					array = new int[5]
					{
						150,
						250,
						500,
						750,
						1000
					};
					array2 = new int[5]
					{
						10,
						20,
						30,
						50,
						100
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal1Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal1");
					}
					else
					{
						pluralDescription = "Fly {0}m in one run";
						singularDescription = "Fly {0}m in one run";
					}
					flag = true;
					break;
				case mg_jr_Goal.GoalType.COLLECT_COINS:
					array = new int[5]
					{
						50,
						100,
						200,
						300,
						400
					};
					array2 = new int[5]
					{
						10,
						20,
						30,
						50,
						100
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal2Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal2");
					}
					else
					{
						pluralDescription = "Collect {0} Coins in one run";
						singularDescription = "Collect {0} Coin in one run";
					}
					flag = true;
					break;
				case mg_jr_Goal.GoalType.USE_TURBO:
					array = new int[5]
					{
						1,
						2,
						3,
						4,
						5
					};
					array2 = new int[5]
					{
						10,
						20,
						30,
						60,
						100
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal3Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal3");
					}
					else
					{
						pluralDescription = "Activate Turbo {0} times in one run";
						singularDescription = "Activate Turbo {0} time in one run";
					}
					flag = true;
					break;
				case mg_jr_Goal.GoalType.DESTROY_OBSTACLES:
					array = new int[5]
					{
						5,
						10,
						15,
						20,
						30
					};
					array2 = new int[5]
					{
						10,
						20,
						30,
						50,
						100
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal4Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal4");
					}
					else
					{
						pluralDescription = "Destroy {0} obstacles in Turbo in one run";
						singularDescription = "Destroy {0} obstacle in Turbo in one run";
					}
					flag = true;
					break;
				default:
					Assert.IsTrue(false, "No such goal");
					break;
				case mg_jr_Goal.GoalType.PLAY_GAMES:
				case mg_jr_Goal.GoalType.COLLECT_ROBOTS:
				case mg_jr_Goal.GoalType.LOSE_ROBOTS:
					break;
				}
				break;
			case mg_jr_Goal.GoalDuration.MULTIPLE_RUNS:
				switch (_type)
				{
				case mg_jr_Goal.GoalType.DISTANCE_TRAVELLED:
					array = new int[5]
					{
						2500,
						5000,
						10000,
						20000,
						50000
					};
					array2 = new int[5]
					{
						25,
						50,
						100,
						200,
						500
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal5Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal5 ");
					}
					else
					{
						pluralDescription = "Fly {0}m";
						singularDescription = "Fly {0}m";
					}
					flag = true;
					break;
				case mg_jr_Goal.GoalType.COLLECT_COINS:
					array = new int[5]
					{
						1000,
						2500,
						5000,
						7500,
						15000
					};
					array2 = new int[5]
					{
						35,
						75,
						150,
						250,
						350
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal6Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal6");
					}
					else
					{
						pluralDescription = "Collect {0} Coins";
						singularDescription = "Collect {0} Coin";
					}
					flag = true;
					break;
				case mg_jr_Goal.GoalType.USE_TURBO:
					array = new int[5]
					{
						5,
						10,
						25,
						50,
						100
					};
					array2 = new int[5]
					{
						25,
						50,
						100,
						200,
						300
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal7Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal7");
					}
					else
					{
						pluralDescription = "Activate Turbo {0} times";
						singularDescription = "Activate Turbo {0} time";
					}
					flag = true;
					break;
				case mg_jr_Goal.GoalType.DESTROY_OBSTACLES:
					array = new int[5]
					{
						50,
						100,
						200,
						350,
						600
					};
					array2 = new int[5]
					{
						25,
						50,
						100,
						200,
						300
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal8Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal8");
					}
					else
					{
						pluralDescription = "Destroy {0} obstacles in Turbo";
						singularDescription = "Destroy {0} obstacle in Turbo";
					}
					flag = true;
					break;
				case mg_jr_Goal.GoalType.PLAY_GAMES:
					array = new int[5]
					{
						5,
						10,
						25,
						50,
						100
					};
					array2 = new int[5]
					{
						25,
						50,
						100,
						200,
						250
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal9Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal9");
					}
					else
					{
						pluralDescription = "Play {0} runs";
						singularDescription = "Play {0} run";
					}
					flag = true;
					break;
				case mg_jr_Goal.GoalType.COLLECT_ROBOTS:
					array = new int[5]
					{
						5,
						10,
						25,
						50,
						75
					};
					array2 = new int[5]
					{
						25,
						50,
						100,
						200,
						250
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal10Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal10");
					}
					else
					{
						pluralDescription = "Collect {0} Robot Penguins";
						singularDescription = "Collect {0} Robot Penguin";
					}
					flag = true;
					break;
				case mg_jr_Goal.GoalType.LOSE_ROBOTS:
					array = new int[5]
					{
						5,
						10,
						20,
						50,
						75
					};
					array2 = new int[5]
					{
						25,
						50,
						100,
						200,
						250
					};
					if (localizer != null)
					{
						pluralDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal11Plural");
						singularDescription = localizer.GetTokenTranslation("Activity.MiniGames.Goal11");
					}
					else
					{
						pluralDescription = "Destroy {0} Robot Penguins";
						singularDescription = "Destroy {0} Robot Penguin";
					}
					flag = true;
					break;
				default:
					Assert.IsTrue(false, "No such goal");
					break;
				}
				break;
			default:
				Assert.IsTrue(false, "No such duration");
				break;
			}
			Assert.AreEqual(array.Length, 5, "Goals must have " + 5 + " level objectives");
			Assert.AreEqual(array2.Length, 5, "Goals must have " + 5 + " level rewards");
			if (flag)
			{
				result = new mg_jr_Goal(_type, _duration, array, array2, pluralDescription, singularDescription);
			}
			return result;
		}
	}
}
