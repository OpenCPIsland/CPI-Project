using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the 2d Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetPosition2d : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Use a stored Vector2 position, and/or set individual axis below.")]
		public FsmVector2 vector;
		
        [Tooltip("Set the X position.")]
		public FsmFloat x;

        [Tooltip("Set the Y position.")]
		public FsmFloat y;

		[Tooltip("Use local or world space.")]
		public Space space;
		
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;	

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			// default axis to variable dropdown with None selected.
			x = new FsmFloat { UseVariable = true };
			y = new FsmFloat { UseVariable = true };
			space = Space.Self;
			everyFrame = false;
			lateUpdate = false;
		}

		public override void OnPreprocess()
		{
			if(lateUpdate)
			{
				Fsm.HandleLateUpdate = true;
			}
		}

		public override void OnEnter()
		{
			if (!everyFrame && !lateUpdate)
			{
				DoSetPosition();
				Finish();
			}		
		}

		public override void OnUpdate()
		{
			if (!lateUpdate)
			{
				DoSetPosition();
			}
		}

		public override void OnLateUpdate()
		{
			if (lateUpdate)
			{
				DoSetPosition();
			}

			if (!everyFrame)
			{
				Finish();
			}
		}

        private void DoSetPosition()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}
			
			// init position
			
			Vector2 position;

			if (vector.IsNone)
			{
				position = space == Space.World ? go.transform.position : go.transform.localPosition;
			}
			else
			{
				position = vector.Value;
			}
			
			// override any axis

			if (!x.IsNone) position.x = x.Value;
			if (!y.IsNone) position.y = y.Value;

			// apply
			
			if (space == Space.World)
			{
				go.transform.position = position;
			}
			else
			{
				go.transform.localPosition = position;
			}
		}


	}
}
