using Disney.Kelowna.Common;

namespace ClubPenguin.CellPhone
{
	public interface ICellPhoneScheduledActivityDefinition
	{
		DateUnityWrapper GetStartingDate();

		DateUnityWrapper GetEndingDate();
	}
}
