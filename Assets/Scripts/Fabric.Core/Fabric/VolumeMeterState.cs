using System;

namespace Fabric
{
	[Serializable]
	public class VolumeMeterState
	{
		[Serializable]
		public class stSpeakers
		{
			public float[] mChannels = new float[2];

			public float mRMS;

			public void Clear()
			{
				for (int i = 0; i < 2; i++)
				{
					mChannels[i] = 0f;
				}
				mRMS = 0f;
			}
		}

		public int mHistoryIndex;

		public stSpeakers[] mHistory = new stSpeakers[5];

		public stSpeakers mPeaks = new stSpeakers();

		public float mRMS;
	}
}
