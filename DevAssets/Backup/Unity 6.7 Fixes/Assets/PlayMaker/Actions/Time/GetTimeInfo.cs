// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Gets various useful Time measurements.")]
	public class GetTimeInfo : FsmStateAction
	{
		public enum TimeInfo
		{
			DeltaTime,
			TimeScale,
			SmoothDeltaTime,
			TimeInCurrentState,
			TimeSinceStartup,
			TimeSinceLevelLoad,
			RealTimeSinceStartup,
			RealTimeInCurrentState
		}
		
        [Tooltip("Info to get.")]
		public TimeInfo getInfo;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the time info in a float variable.")]
		public FsmFloat storeValue;
		
        [Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			getInfo = TimeInfo.TimeSinceLevelLoad;
			storeValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetTimeInfo();
			
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetTimeInfo();
		}
		
		void DoGetTimeInfo()
		{
			switch (getInfo) 
			{
			
			case TimeInfo.DeltaTime:
				storeValue.Value = Time.deltaTime;
				break;
				
			case TimeInfo.TimeScale:
				storeValue.Value = Time.timeScale;
				break;
				
			case TimeInfo.SmoothDeltaTime:
				storeValue.Value = Time.smoothDeltaTime;
				break;
				
			case TimeInfo.TimeInCurrentState:
				storeValue.Value = State.StateTime;
				break;
			
			case TimeInfo.TimeSinceStartup:
				storeValue.Value = Time.time;
				break;
				
			case TimeInfo.TimeSinceLevelLoad:
				storeValue.Value = Time.timeSinceLevelLoad;
				break;
				
			case TimeInfo.RealTimeSinceStartup:
				storeValue.Value = FsmTime.RealtimeSinceStartup;
				break;
			
			case TimeInfo.RealTimeInCurrentState:
				storeValue.Value = FsmTime.RealtimeSinceStartup - State.RealStartTime;
				break;
				
			default:
				storeValue.Value = 0f;
				break;
			}
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return string.Format("GetTimeInfo{0} {1} > {2}", ActionHelpers.colon, getInfo, ActionHelpers.GetValueLabel(storeValue));
	    }
#endif

	}
}