using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	public interface IContentInterruption
	{
		event Action OnReturn;

		event Action OnContinue;

		void Show(Transform parentTransform = null);
	}
}
