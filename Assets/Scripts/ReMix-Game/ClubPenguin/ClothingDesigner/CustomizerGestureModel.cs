using ClubPenguin.ClothingDesigner.ItemCustomizer;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner
{
	public class CustomizerGestureModel
	{
		public static Vector2 NOT_ON_SCREEN = new Vector2(-1f, -1f);

		private AreaTouchedEnum touchDownStartArea;

		private Vector2 touchDownStartPos;

		private float touchStartTime;

		private GameObject startGameObject;

		private Texture2D dragIconTexture;

		public int ItemDefinitionId;

		private bool isEnabled = true;

		public TemplateDefinition TemplateData
		{
			get;
			set;
		}

		public long ItemId
		{
			get;
			set;
		}

		public bool IsEquippable
		{
			get;
			set;
		}

		public bool IsEnabled
		{
			get
			{
				return isEnabled;
			}
			set
			{
				isEnabled = value;
			}
		}

		public AreaTouchedEnum TouchDownStartArea
		{
			get
			{
				return touchDownStartArea;
			}
			set
			{
				touchDownStartArea = value;
			}
		}

		public Vector2 TouchDownStartPos
		{
			get
			{
				return touchDownStartPos;
			}
			set
			{
				touchDownStartPos = value;
			}
		}

		public float TouchStartTime
		{
			get
			{
				return touchStartTime;
			}
			set
			{
				touchStartTime = value;
			}
		}

		public GameObject StartGameObject
		{
			get
			{
				return startGameObject;
			}
			set
			{
				startGameObject = value;
			}
		}

		public CustomizationChannel Channel
		{
			get
			{
				switch (touchDownStartArea)
				{
				case AreaTouchedEnum.BLUE_CHANNEL:
					return CustomizationChannel.BLUE;
				case AreaTouchedEnum.GREEN_CHANNEL:
					return CustomizationChannel.GREEN;
				case AreaTouchedEnum.RED_CHANNEL:
					return CustomizationChannel.RED;
				default:
					return CustomizationChannel.NONE;
				}
			}
		}

		public Texture2D DragIconTexture
		{
			get
			{
				return dragIconTexture;
			}
			set
			{
				dragIconTexture = value;
			}
		}

		public CustomizerGestureModel()
		{
			touchDownStartArea = AreaTouchedEnum.NOTHING;
			touchDownStartPos = NOT_ON_SCREEN;
		}

		public override string ToString()
		{
			return string.Format("[CustomizerGestureModel: TemplateData={0}, ItemId={1}, IsEquippable={2}, TouchDownStartArea={3}, TouchDownStartPos={4}, TouchStartTime={5}, StartGameObject={6}, Channel={7}, DragIconTexture={8}]", TemplateData, ItemId, IsEquippable, TouchDownStartArea, TouchDownStartPos, TouchStartTime, StartGameObject, Channel, DragIconTexture);
		}
	}
}
