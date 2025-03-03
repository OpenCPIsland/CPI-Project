using DevonLocalization.Core;
using Disney.MobileNetwork;

namespace SmoothieSmash
{
	public class mg_ss_ScoreSurvival : mg_ss_Score
	{
		public override string CustomText
		{
			get
			{
				if (Service.IsSet<Localizer>())
				{
					return Service.Get<Localizer>().GetTokenTranslation("Activity.MiniGames.Time");
				}
				return "Time:";
			}
		}

		public override int CustomValue
		{
			get
			{
				return (int)m_logic.GameTime;
			}
		}

		public mg_ss_ScoreSurvival(mg_ss_GameLogic p_logic)
			: base(p_logic)
		{
		}
	}
}
