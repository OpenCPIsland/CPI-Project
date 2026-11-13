using System.Text;

namespace ClubPenguin.Participation
{
	public class ParticipationRequestOutcome
	{
		public readonly ParticipationRequest Request;

		public readonly ParticipationState State;

		public readonly string WinnerName;

		public readonly bool RequestAccepted;

		public ParticipationRequestOutcome(ParticipationRequest request, ParticipationState state, string gameObjectName, bool accepted)
		{
			Request = request;
			State = state;
			WinnerName = gameObjectName;
			RequestAccepted = accepted;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Request).Append(State).Append(WinnerName)
				.Append(RequestAccepted);
			return stringBuilder.ToString();
		}
	}
}
