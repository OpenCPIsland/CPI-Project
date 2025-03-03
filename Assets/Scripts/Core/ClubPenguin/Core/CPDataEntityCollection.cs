using Disney.Kelowna.Common.DataModel;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Core
{
	public interface CPDataEntityCollection : DataEntityCollection
	{
		DataEntityHandle LocalPlayerHandle
		{
			get;
		}

		long LocalPlayerSessionId
		{
			get;
		}

		void ResetLocalPlayerHandle();

		bool IsLocalPlayer(long sessionId);

		bool IsLocalPlayerMember();

		IList<Transform> GetRemotePlayers();

		DataEntityHandle[] GetRemotePlayerHandles();

		void ClearZoneScope();
	}
}
