using DevonLocalization.Core;
using Disney.MobileNetwork;

namespace SmoothieSmash
{
	public class mg_ss_ScoreNormal : mg_ss_Score
	{
		public int OrdersCompleted;

		public override string CustomText
		{
			get
			{
				if (Service.IsSet<Localizer>())
				{
					return Service.Get<Localizer>().GetTokenTranslation("Activity.MiniGames.Recipe");
				}
				return "Recipes Completed:";
			}
		}

		public override int CustomValue
		{
			get
			{
				return OrdersCompleted;
			}
		}

		public mg_ss_ScoreNormal(mg_ss_GameLogic p_logic)
			: base(p_logic)
		{
		}
	}
}
