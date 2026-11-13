using CpRemixShaders;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class DCustomizerLayer
	{
		private bool isSelected = false;

		public DecalType Type
		{
			get;
			set;
		}

		public Texture2D Decal
		{
			get;
			set;
		}

		public bool IsTiled
		{
			get;
			set;
		}

		public Color Tint
		{
			get;
			set;
		}

		public float Scale
		{
			get;
			set;
		}

		public float Rotation
		{
			get;
			set;
		}

		public Vector2 UVOffset
		{
			get;
			set;
		}

		public DecalColorChannel ShaderChannel
		{
			get;
			set;
		}

		public Renderer OriginalRenderer
		{
			get;
			set;
		}

		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
		}

		public void Select()
		{
			isSelected = true;
		}

		public void Deselect()
		{
			isSelected = false;
		}
	}
}
