using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	public class CPResponse
	{
		public List<SignedResponse<WebServiceEvent>> wsEvents;
	}
}
