using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation
{
	[RequireComponent(typeof(ManipulatableObject))]
	public class ManipulatableObjectEffects : MonoBehaviour
	{
		private const float HIGHLIGHT_OFF = 0f;

		[Tooltip("The colour the child renderers will be set to when this object is selected")]
		public Color ColorSelected = Color.green;

		[Tooltip("The colour the child renderers will be set to when this object is not allowed")]
		public Color ColorNotAllowed = Color.red;

		[Tooltip("The colour the child renderers will be set to when this object is in a position to be deleted if the currently selected object is placed")]
		public Color ColorSquashed = Color.red;

		[Tooltip("The colour the child renderers will be set to when this object is not selectable")]
		public Color ColorNotSelectable = Color.gray;

		[Tooltip("An additional brightness added to selected object")]
		[Range(0f, 1f)]
		public float Highlight = 0.3f;

		private int colorId;

		private int hightlightId;

		private List<GameObject> myChildren = null;

		private Dictionary<Renderer, Color> myChildrenRenderersAndOriginalColours = null;

		private bool isSelectable;

		private ObjectManipulator objectManipulator;

		private List<GameObject> MyChildren
		{
			get
			{
				if (myChildren == null)
				{
					myChildren = new List<GameObject>();
				}
				if (myChildren.Count == 0 && base.transform.childCount != 0)
				{
					myChildren = getChildrenRecursive(base.transform);
				}
				return myChildren;
			}
			set
			{
			}
		}

		private Dictionary<Renderer, Color> MyChildrenRenderersAndOriginalColours
		{
			get
			{
				if (myChildrenRenderersAndOriginalColours == null)
				{
					myChildrenRenderersAndOriginalColours = new Dictionary<Renderer, Color>(MyChildren.Count);
					for (int i = 0; i < MyChildren.Count; i++)
					{
						Renderer component = MyChildren[i].GetComponent<Renderer>();
						if (component != null && component.material.HasProperty(colorId))
						{
							myChildrenRenderersAndOriginalColours[component] = component.material.GetColor(colorId);
						}
					}
				}
				return myChildrenRenderersAndOriginalColours;
			}
		}

		private static List<GameObject> getChildrenRecursive(Transform t)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < t.childCount; i++)
			{
				GameObject gameObject = t.GetChild(i).gameObject;
				if (gameObject.GetComponent<ManipulatableObject>() == null)
				{
					list.Add(gameObject);
					list.AddRange(getChildrenRecursive(gameObject.transform));
				}
			}
			return list;
		}

		public void Awake()
		{
			colorId = Shader.PropertyToID("_Color");
			hightlightId = Shader.PropertyToID("_Highlight");
			isSelectable = true;
		}

		public void SetObjectManipulator(ObjectManipulator objectManipulator)
		{
			this.objectManipulator = objectManipulator;
			Select();
			objectManipulator.IsAllowedChanged += onObjectManipulatorIsAllowedChanged;
			objectManipulator.GetComponent<ManipulatableObject>().IsSquashedChanged += onObjectManipulatorIsSquashedChanged;
		}

		public void ClearObjectManipulator()
		{
			if (objectManipulator != null)
			{
				Deselect();
				objectManipulator.IsAllowedChanged -= onObjectManipulatorIsAllowedChanged;
				objectManipulator.GetComponent<ManipulatableObject>().IsSquashedChanged -= onObjectManipulatorIsSquashedChanged;
				objectManipulator = null;
			}
		}

		public void OnDestroy()
		{
			ClearObjectManipulator();
		}

		private void onObjectManipulatorIsAllowedChanged()
		{
			if (objectManipulator.IsAllowed)
			{
				setColorAndHighlightInChildren(ColorSelected);
			}
			else
			{
				setColorAndHighlightInChildren(ColorNotAllowed);
			}
		}

		private void Select()
		{
			setColorAndHighlightInChildren(ColorSelected);
		}

		private void Deselect()
		{
			restoreOriginalColorAndHighlight();
		}

		private void onObjectManipulatorIsSquashedChanged(bool isSquashed)
		{
			if (isSquashed)
			{
				setColorAndHighlightInChildren(ColorSquashed);
			}
			else
			{
				SetSelectable(isSelectable);
			}
		}

		public void SetSelectable(bool selectedAble)
		{
			isSelectable = selectedAble;
			if (!selectedAble)
			{
				setColorAndHighlightInChildren(ColorNotSelectable);
			}
			else
			{
				restoreOriginalColorAndHighlight();
			}
		}

		private void setColorAndHighlightInChildren(Color value)
		{
			if (MyChildrenRenderersAndOriginalColours.Count == 0)
			{
			}
			foreach (Renderer key in MyChildrenRenderersAndOriginalColours.Keys)
			{
				key.material.SetColor(colorId, value);
				key.material.SetFloat(hightlightId, Highlight);
			}
		}

		private void restoreOriginalColorAndHighlight()
		{
			foreach (Renderer key in MyChildrenRenderersAndOriginalColours.Keys)
			{
				key.material.SetColor(colorId, MyChildrenRenderersAndOriginalColours[key]);
				key.material.SetFloat(hightlightId, 0f);
			}
		}
	}
}
