// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the Cosine.")]
	public class GetCosine : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The angle. Note: Check Deg To Rad if the angle is expressed in degrees.")]
		public FsmFloat angle;
		
		[Tooltip("Check if the angle is expressed in degrees.")]
		public FsmBool DegToRad;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The angle cosine.")]
		public FsmFloat result;
		
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			angle = null;
			DegToRad = true;
			everyFrame = false;
			result = null;
		}

		public override void OnEnter()
		{
			DoCosine();
			
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCosine();
		}
		
		void DoCosine()
		{
			float _angle = angle.Value;
			if (DegToRad.Value)
			{
				_angle = _angle*Mathf.Deg2Rad;
			}
			result.Value = Mathf.Cos(_angle);
		}
	}
}