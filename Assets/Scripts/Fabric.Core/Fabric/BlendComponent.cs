using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/BlendComponent")]
	public class BlendComponent : Component
	{
		private bool finishedPlayingOncePerFrame;

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents = false)
		{
			if (CheckMIDI(zComponentInstance))
			{
				finishedPlayingOncePerFrame = false;
				base.PlayInternal(zComponentInstance, target, curve, dontPlayComponents);
			}
		}

		internal override void OnFinishPlaying(double time)
		{
			if (_notifyParentComponent == NotifyParentType.AllComponentsHaveFinished)
			{
				int num = 0;
				for (int i = 0; i < _componentsArray.Length; i++)
				{
					if (_componentsArray[i].IsComponentActive())
					{
						num++;
					}
				}
				if (num <= 1)
				{
					base.OnFinishPlaying(time);
				}
			}
			else if (!finishedPlayingOncePerFrame)
			{
				base.OnFinishPlaying(time);
				finishedPlayingOncePerFrame = true;
			}
		}
	}
}
