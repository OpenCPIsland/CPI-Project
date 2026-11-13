using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Image))]
	public class MulticoloredListElement : MonoBehaviour
	{
		private MulticoloredList multicoloredList;

		private void Start()
		{
			if (multicoloredList == null)
			{
				multicoloredList = GetComponentInParent<MulticoloredList>();
				if (multicoloredList == null)
				{
					throw new MissingReferenceException("MulticoloredListElements must be a child of a MulticoloredList");
				}
			}
			multicoloredList.Refresh();
		}

		private void OnEnable()
		{
			if (multicoloredList != null)
			{
				multicoloredList.Refresh();
			}
		}

		private void OnDisable()
		{
			if (multicoloredList != null)
			{
				multicoloredList.Refresh();
			}
		}
	}
}
