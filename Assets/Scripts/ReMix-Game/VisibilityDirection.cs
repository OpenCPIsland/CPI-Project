using UnityEngine;

public class VisibilityDirection : MonoBehaviour
{
	private Transform transformRef;

	public Transform TransformRef
	{
		get
		{
			if (transformRef == null)
			{
				transformRef = base.transform;
			}
			return transformRef;
		}
	}
}
