using UnityEngine;

namespace Tweaker.UI
{
	public interface ITweakerConsoleController
	{
		Tweaker Tweaker
		{
			get;
		}

		TweakerTree Tree
		{
			get;
		}

		ITweakerSerializer Serializer
		{
			get;
		}

		BaseNode CurrentInspectorNode
		{
			get;
		}

		void ShowInspector(BaseNode nodeToInspect);

		void DestroyObject(GameObject go);
	}
}
