using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/ProxyComponent")]
	public class ProxyComponent : Component
	{
		[SerializeField]
		public Component targetComponment;

		public void Awake()
		{
			if (targetComponment != null)
			{
				targetComponment.MoveToComponent(this);
			}
		}
	}
}
