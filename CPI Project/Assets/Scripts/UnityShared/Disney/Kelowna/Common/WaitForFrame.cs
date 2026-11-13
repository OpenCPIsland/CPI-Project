using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class WaitForFrame : CoroutineReturn
	{
		private int startFrame;

		private int targetFrame;

		public override bool Finished
		{
			get
			{
				return Time.frameCount >= targetFrame;
			}
		}

		public WaitForFrame(int frameDelay)
		{
			startFrame = Time.frameCount;
			targetFrame = startFrame + frameDelay;
		}
	}
}
