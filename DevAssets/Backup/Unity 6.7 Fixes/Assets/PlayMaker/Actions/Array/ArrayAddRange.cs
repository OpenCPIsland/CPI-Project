// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Add multiple items to the end of an array.\nNOTE: There is a bug in this action when resizing Variables. It will be fixed in the next update.")]
	public class ArrayAddRange : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;
		
		[RequiredField]
		[MatchElementType("array")]
		[Tooltip("The items to add to the array.")]
		public FsmVar[] variables;
		
		public override void Reset()
		{
			array = null;
			variables = new FsmVar[2];
		}

		public override void OnEnter()
		{
			DoAddRange();
			
			Finish();
		}
		
		private void DoAddRange()
		{
			int count = variables.Length;

			if (count>0)
			{
				array.Resize(array.Length+count);

				foreach(FsmVar _var in variables)
				{
                    _var.UpdateValue();
					array.Set(array.Length-count,_var.GetValue());
					count--;
				}
			}

		}
		
		
	}
}
