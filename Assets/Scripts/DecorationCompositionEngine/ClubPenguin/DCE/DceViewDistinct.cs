using System;
using UnityEngine;

namespace ClubPenguin.DCE
{
	[RequireComponent(typeof(Rig))]
	public class DceViewDistinct : DceView
	{
		private DceViewDistinctChild[,] children;

		private Rig rig;

		private bool boundsIsDirty = false;

		private Bounds cachedBounds;

		public event Action<DceViewDistinctChild> OnChildAdded;

		protected override void onAwake()
		{
			rig = GetComponent<Rig>();
		}

		public override Bounds GetBounds()
		{
			if (boundsIsDirty)
			{
				Debug.Log("CARL: UPDATING");
				cachedBounds = new Bounds(base.transform.position, Vector3.zero);
				for (int i = 0; i < children.GetLength(0); i++)
				{
					for (int j = 0; j < children.GetLength(1); j++)
					{
						DceViewDistinctChild dceViewDistinctChild = children[i, j];
						if (dceViewDistinctChild != null)
						{
							cachedBounds.Encapsulate(dceViewDistinctChild.Renderer.bounds);
						}
					}
				}
				boundsIsDirty = false;
			}
			return cachedBounds;
		}

		public void OnEnable()
		{
			Model.PartChanged += model_PartChanged;
		}

		public void OnDisable()
		{
			Model.PartChanged -= model_PartChanged;
		}

		protected override void cleanup()
		{
		}

		private void applyPart(int slotIndex, int partIndex, DceModel.Part newPart)
		{
			DceViewDistinctChild dceViewDistinctChild = children[slotIndex, partIndex];
			if (dceViewDistinctChild == null && newPart != null)
			{
				string name = "DceViewDistinct";
				GameObject gameObject = new GameObject(name);
				gameObject.transform.SetParent(base.transform, false);
				gameObject.layer = base.gameObject.layer;
				gameObject.SetActive(false);
				dceViewDistinctChild = gameObject.AddComponent<DceViewDistinctChild>();
				dceViewDistinctChild.SlotIndex = slotIndex;
				dceViewDistinctChild.PartIndex = partIndex;
				dceViewDistinctChild.Model = Model;
				dceViewDistinctChild.Rig = rig;
				gameObject.SetActive(true);
				children[slotIndex, partIndex] = dceViewDistinctChild;
				if (this.OnChildAdded != null)
				{
					this.OnChildAdded(dceViewDistinctChild);
				}
			}
			boundsIsDirty = true;
			if (dceViewDistinctChild != null)
			{
				dceViewDistinctChild.Apply(newPart);
				if (dceViewDistinctChild.IsBusy && !base.IsBusy)
				{
					startWork();
				}
			}
		}

		public void Update()
		{
			bool flag = false;
			bool flag2 = true;
			for (int i = 0; i < children.GetLength(0); i++)
			{
				for (int j = 0; j < children.GetLength(1); j++)
				{
					DceViewDistinctChild dceViewDistinctChild = children[i, j];
					if (dceViewDistinctChild != null)
					{
						flag |= dceViewDistinctChild.IsBusy;
						flag2 &= dceViewDistinctChild.IsReady;
					}
				}
			}
			if (flag && !base.IsBusy)
			{
				startWork();
			}
			else if (!flag && base.IsBusy)
			{
				stopWork();
			}
			else if (flag2 && !base.IsReady)
			{
				startWork();
				stopWork();
			}
		}

		private void model_PartChanged(int slotIndex, int partIndex, DceModel.Part oldPart, DceModel.Part newPart)
		{
			applyPart(slotIndex, partIndex, newPart);
		}
	}
}
