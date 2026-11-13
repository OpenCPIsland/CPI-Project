using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using HutongGames.PlayMaker;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class SetMaterialPropertyAction : FsmStateAction
	{
		[RequiredField]
		public FsmMaterial TintMaterial;

		public FsmString TintMaterialProperty;

		public FsmAnimationCurve AnimCurve;

		public FsmColor TintColor;

		public bool WaitForFinish = true;

		private GameObject componentObject;

		private SetMaterialProperty setMaterialProperty;

		public override void Reset()
		{
			TintMaterial = new FsmMaterial
			{
				UseVariable = false
			};
			TintMaterialProperty = new FsmString
			{
				UseVariable = false
			};
			AnimCurve = new FsmAnimationCurve();
			TintColor = new FsmColor
			{
				UseVariable = false
			};
			WaitForFinish = true;
		}

		public override void OnEnter()
		{
			if (!TintMaterial.IsNone)
			{
				if (TintMaterial.Value.HasProperty(TintMaterialProperty.Value))
				{
					if (!TintColor.IsNone)
					{
						componentObject = new GameObject("SetMaterialPropertyObject");
						setMaterialProperty = componentObject.AddComponent<SetMaterialProperty>();
						if (WaitForFinish)
						{
							setMaterialProperty.OnComplete += onSetMaterialPropertyComplete;
						}
						setMaterialProperty.ChangeProperty(TintMaterial.Value, TintMaterialProperty.Value, TintColor.Value, AnimCurve.curve);
					}
					else
					{
						Disney.LaunchPadFramework.Log.LogError(this, string.Format("Error: {0} has no tint color set in the FSM", base.Owner.gameObject.GetPath()));
					}
				}
				else
				{
					Disney.LaunchPadFramework.Log.LogError(this, string.Format("Error: {0} has tint material {1} has no property named '{2}'", base.Owner.gameObject.GetPath(), TintMaterial.Name, TintMaterialProperty.Value));
				}
			}
			else
			{
				Disney.LaunchPadFramework.Log.LogError(this, string.Format("Error: {0} has no tint material set", base.Owner.gameObject.GetPath()));
			}
			if (!WaitForFinish)
			{
				Finish();
			}
		}

		private void onSetMaterialPropertyComplete()
		{
			setMaterialProperty.OnComplete -= onSetMaterialPropertyComplete;
			CoroutineRunner.Start(End(), this, "onSetMaterialPropertyComplete");
		}

		private IEnumerator End()
		{
			yield return new WaitForEndOfFrame();
			Finish();
		}
	}
}
