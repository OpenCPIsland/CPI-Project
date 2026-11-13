// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Snap Vector3 coordinates to grid points.")]
	public class Vector3SnapToGrid : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector3 Variable to snap.")]
		public FsmVector3 vector3Variable;

		[Tooltip("Grid Size.")]
		public FsmFloat gridSize;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
		{
			vector3Variable = null;
            gridSize = new FsmFloat { Value = 1f };
        }

        public override void OnEnter()
		{
            DoSnapToGrid();

            if (!everyFrame)
                Finish();
		}

		public override void OnUpdate()
		{
            DoSnapToGrid();
        }

        private void DoSnapToGrid()
        {
            if (gridSize.Value < 0.001f) return;

            var v3 = vector3Variable.Value;
            var grid = gridSize.Value;

            v3 /= grid;
            v3.Set(Mathf.Round(v3.x), Mathf.Round(v3.y), Mathf.Round(v3.z));
            vector3Variable.Value = v3 * grid;
        }
	}
}

