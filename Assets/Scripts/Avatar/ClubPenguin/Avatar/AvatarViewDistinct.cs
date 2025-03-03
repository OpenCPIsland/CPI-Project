using System;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[RequireComponent(typeof(Rig))]
	public class AvatarViewDistinct : AvatarView
	{
		private AvatarViewDistinctChild[,] children;

		private Rig rig;

		private bool boundsIsDirty = false;

		private Bounds cachedBounds;

		public event Action<AvatarViewDistinctChild> OnChildAdded;

		protected override void onAwake()
		{
			children = new AvatarViewDistinctChild[Model.RowMax, Model.ColumnMax];
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
						AvatarViewDistinctChild avatarViewDistinctChild = children[i, j];
						if (avatarViewDistinctChild != null)
						{
							cachedBounds.Encapsulate(avatarViewDistinctChild.Renderer.bounds);
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

		private void applyPart(int slotIndex, int partIndex, AvatarModel.Part newPart)
		{
			AvatarViewDistinctChild avatarViewDistinctChild = children[slotIndex, partIndex];
			if (avatarViewDistinctChild == null && newPart != null)
			{
				string text = Model.Definition.Slots[slotIndex].Name;
				if (partIndex > 0)
				{
					text = text + "_" + AvatarDefinition.PartTypeStrings[partIndex];
				}
				GameObject gameObject = new GameObject(text);
				gameObject.transform.SetParent(base.transform, false);
				gameObject.layer = base.gameObject.layer;
				gameObject.SetActive(false);
				avatarViewDistinctChild = gameObject.AddComponent<AvatarViewDistinctChild>();
				avatarViewDistinctChild.SlotIndex = slotIndex;
				avatarViewDistinctChild.PartIndex = partIndex;
				avatarViewDistinctChild.Model = Model;
				avatarViewDistinctChild.Rig = rig;
				gameObject.SetActive(true);
				children[slotIndex, partIndex] = avatarViewDistinctChild;
				if (this.OnChildAdded != null)
				{
					this.OnChildAdded(avatarViewDistinctChild);
				}
			}
			boundsIsDirty = true;
			if (avatarViewDistinctChild != null)
			{
				avatarViewDistinctChild.Apply(newPart);
				if (avatarViewDistinctChild.IsBusy && !base.IsBusy)
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
					AvatarViewDistinctChild avatarViewDistinctChild = children[i, j];
					if (avatarViewDistinctChild != null)
					{
						flag |= avatarViewDistinctChild.IsBusy;
						flag2 &= avatarViewDistinctChild.IsReady;
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

		private void model_PartChanged(int slotIndex, int partIndex, AvatarModel.Part oldPart, AvatarModel.Part newPart)
		{
			applyPart(slotIndex, partIndex, newPart);
		}
	}
}
