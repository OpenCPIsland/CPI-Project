using DevonLocalization.Core;
using Disney.Mix.SDK;
using Disney.MobileNetwork;

namespace ClubPenguin.Net
{
	public class Alert : IModerationAlert
	{
		private IAlert mixAlert;

		private bool isCritical;

		public bool IsCritical
		{
			get
			{
				return isCritical;
			}
		}

		public string Text
		{
			get
			{
				return Service.Get<Localizer>().GetTokenTranslation(string.Concat("Account.WarningText.", MixAlert.Type, mixAlert.Level));
			}
		}

		public IAlert MixAlert
		{
			get
			{
				return mixAlert;
			}
		}

		public Alert(IAlert mixAlert, bool isCritical)
		{
			this.mixAlert = mixAlert;
			this.isCritical = isCritical;
		}
	}
}
