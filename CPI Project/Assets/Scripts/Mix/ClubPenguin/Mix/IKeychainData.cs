using System;

namespace ClubPenguin.Mix
{
	public interface IKeychainData
	{
		event Action OnKeyGenWithExistingDBError;
	}
}
