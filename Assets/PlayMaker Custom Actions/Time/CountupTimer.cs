// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.  
// License: Attribution 4.0 International(CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Author : Deek

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time)]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=15458.0")]
	[Tooltip("Counts up as long as in current state and ends when leaving the current state.")]
	public class CountupTimer : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Time since state start.")]
		public FsmFloat storeTime;
		[Tooltip("Count up TimeScale independent.")]
		public FsmBool realTime;
		[Tooltip("Pauses the timer when true. Resumes when false again.")]
		public FsmBool pause;
		[Tooltip("Setting this to true sets the timer back to 0 while counting up. Will set itself back to false once it has reset.")]
		public FsmBool reset;
		[Tooltip("Sets the timer back to 0 when entering this state. Otherwise continues where left of.")]
		public FsmBool restartOnEnter;

		private float startTime;
		private float timer;

		public override void Reset()
		{
			storeTime = 0f;
			realTime = false;
			pause = false;
			reset = false;
			restartOnEnter = true;
		}

		public override void OnEnter()
		{
			startTime = FsmTime.RealtimeSinceStartup;
			if(restartOnEnter.Value)
			{
				timer = 0f;
			}
		}

		public override void OnUpdate()
		{
			if(pause.Value)
			{
				return;
			}

			if(reset.Value)
			{
				timer = 0f;
				reset.Value = false;
			}

			if(realTime.Value)
			{
				timer = FsmTime.RealtimeSinceStartup - startTime;
			} else
			{
				timer += Time.deltaTime;
			}
			storeTime.Value = timer;
		}
	}
}
