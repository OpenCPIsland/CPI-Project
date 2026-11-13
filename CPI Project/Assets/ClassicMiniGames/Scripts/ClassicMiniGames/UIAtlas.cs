using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Atlas")]
public class UIAtlas : MonoBehaviour
{
	[Serializable]
	private class Sprite
	{
		public string name = "Unity Bug";

		public Rect outer = new Rect(0f, 0f, 1f, 1f);

		public Rect inner = new Rect(0f, 0f, 1f, 1f);

		public bool rotated = false;

		public float paddingLeft = 0f;

		public float paddingRight = 0f;

		public float paddingTop = 0f;

		public float paddingBottom = 0f;

		public bool hasPadding
		{
			get
			{
				return paddingLeft != 0f || paddingRight != 0f || paddingTop != 0f || paddingBottom != 0f;
			}
		}
	}

	private enum Coordinates
	{
		Pixels,
		TexCoords
	}

	[SerializeField]
	[HideInInspector]
	private Material material;

	[SerializeField]
	[HideInInspector]
	private List<UISpriteData> mSprites = new List<UISpriteData>();

	[HideInInspector]
	[SerializeField]
	private float mPixelSize = 1f;

	[SerializeField]
	[HideInInspector]
	private UIAtlas mReplacement;

	[HideInInspector]
	[SerializeField]
	private Coordinates mCoordinates = Coordinates.Pixels;

	[HideInInspector]
	[SerializeField]
	private List<Sprite> sprites = new List<Sprite>();

	private int mPMA = -1;

	public Material spriteMaterial
	{
		get
		{
			return (mReplacement != null) ? mReplacement.spriteMaterial : material;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteMaterial = value;
				return;
			}
			if (material == null)
			{
				mPMA = 0;
				material = value;
				return;
			}
			MarkAsChanged();
			mPMA = -1;
			material = value;
			MarkAsChanged();
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.premultipliedAlpha;
			}
			if (mPMA == -1)
			{
				Material spriteMaterial = this.spriteMaterial;
				mPMA = ((spriteMaterial != null && spriteMaterial.shader != null && spriteMaterial.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public List<UISpriteData> spriteList
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.spriteList;
			}
			if (mSprites.Count == 0)
			{
				Upgrade();
			}
			return mSprites;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteList = value;
			}
			else
			{
				mSprites = value;
			}
		}
	}

	public Texture texture
	{
		get
		{
			return (mReplacement != null) ? mReplacement.texture : ((material != null) ? material.mainTexture : null);
		}
	}

	public float pixelSize
	{
		get
		{
			return (mReplacement != null) ? mReplacement.pixelSize : mPixelSize;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.pixelSize = value;
				return;
			}
			float num = Mathf.Clamp(value, 0.25f, 4f);
			if (mPixelSize != num)
			{
				mPixelSize = num;
				MarkAsChanged();
			}
		}
	}

	public UIAtlas replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UIAtlas uIAtlas = value;
			if (uIAtlas == this)
			{
				uIAtlas = null;
			}
			if (mReplacement != uIAtlas)
			{
				if (uIAtlas != null && uIAtlas.replacement == this)
				{
					uIAtlas.replacement = null;
				}
				if (mReplacement != null)
				{
					MarkAsChanged();
				}
				mReplacement = uIAtlas;
				if (uIAtlas != null)
				{
					material = null;
				}
				MarkAsChanged();
			}
		}
	}

	public UISpriteData GetSprite(string name)
	{
		if (mReplacement != null)
		{
			return mReplacement.GetSprite(name);
		}
		if (!string.IsNullOrEmpty(name))
		{
			if (mSprites.Count == 0)
			{
				Upgrade();
			}
			int i = 0;
			for (int count = mSprites.Count; i < count; i++)
			{
				UISpriteData uISpriteData = mSprites[i];
				if (!string.IsNullOrEmpty(uISpriteData.name) && name == uISpriteData.name)
				{
					return uISpriteData;
				}
			}
		}
		return null;
	}

	public void SortAlphabetically()
	{
		mSprites.Sort((UISpriteData s1, UISpriteData s2) => s1.name.CompareTo(s2.name));
	}

	public BetterList<string> GetListOfSprites()
	{
		if (mReplacement != null)
		{
			return mReplacement.GetListOfSprites();
		}
		if (mSprites.Count == 0)
		{
			Upgrade();
		}
		BetterList<string> betterList = new BetterList<string>();
		int i = 0;
		for (int count = mSprites.Count; i < count; i++)
		{
			UISpriteData uISpriteData = mSprites[i];
			if (uISpriteData != null && !string.IsNullOrEmpty(uISpriteData.name))
			{
				betterList.Add(uISpriteData.name);
			}
		}
		return betterList;
	}

	public BetterList<string> GetListOfSprites(string match)
	{
		if ((bool)mReplacement)
		{
			return mReplacement.GetListOfSprites(match);
		}
		if (string.IsNullOrEmpty(match))
		{
			return GetListOfSprites();
		}
		if (mSprites.Count == 0)
		{
			Upgrade();
		}
		BetterList<string> betterList = new BetterList<string>();
		int i = 0;
		for (int count = mSprites.Count; i < count; i++)
		{
			UISpriteData uISpriteData = mSprites[i];
			if (uISpriteData != null && !string.IsNullOrEmpty(uISpriteData.name) && string.Equals(match, uISpriteData.name, StringComparison.OrdinalIgnoreCase))
			{
				betterList.Add(uISpriteData.name);
				return betterList;
			}
		}
		string[] array = match.Split(new char[1]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		for (i = 0; i < array.Length; i++)
		{
			array[i] = array[i].ToLower();
		}
		i = 0;
		for (int count = mSprites.Count; i < count; i++)
		{
			UISpriteData uISpriteData = mSprites[i];
			if (uISpriteData == null || string.IsNullOrEmpty(uISpriteData.name))
			{
				continue;
			}
			string text = uISpriteData.name.ToLower();
			int num = 0;
			for (int j = 0; j < array.Length; j++)
			{
				if (text.Contains(array[j]))
				{
					num++;
				}
			}
			if (num == array.Length)
			{
				betterList.Add(uISpriteData.name);
			}
		}
		return betterList;
	}

	private bool References(UIAtlas atlas)
	{
		if (atlas == null)
		{
			return false;
		}
		if (atlas == this)
		{
			return true;
		}
		return mReplacement != null && mReplacement.References(atlas);
	}

	public static bool CheckIfRelated(UIAtlas a, UIAtlas b)
	{
		if (a == null || b == null)
		{
			return false;
		}
		return a == b || a.References(b) || b.References(a);
	}

	public void MarkAsChanged()
	{
		if (mReplacement != null)
		{
			mReplacement.MarkAsChanged();
		}
		UISprite[] array = NGUITools.FindActive<UISprite>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			UISprite uISprite = array[i];
			if (CheckIfRelated(this, uISprite.atlas))
			{
				UIAtlas atlas = uISprite.atlas;
				uISprite.atlas = null;
				uISprite.atlas = atlas;
			}
		}
		UIFont[] array2 = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];
		i = 0;
		for (int num = array2.Length; i < num; i++)
		{
			UIFont uIFont = array2[i];
			if (CheckIfRelated(this, uIFont.atlas))
			{
				UIAtlas atlas = uIFont.atlas;
				uIFont.atlas = null;
				uIFont.atlas = atlas;
			}
		}
		UILabel[] array3 = NGUITools.FindActive<UILabel>();
		i = 0;
		for (int num = array3.Length; i < num; i++)
		{
			UILabel uILabel = array3[i];
			if (uILabel.bitmapFont != null && CheckIfRelated(this, uILabel.bitmapFont.atlas))
			{
				UIFont uIFont = uILabel.bitmapFont;
				uILabel.bitmapFont = null;
				uILabel.bitmapFont = uIFont;
			}
		}
	}

	private bool Upgrade()
	{
		if ((bool)mReplacement)
		{
			return mReplacement.Upgrade();
		}
		if (mSprites.Count == 0 && sprites.Count > 0 && (bool)material)
		{
			Texture mainTexture = material.mainTexture;
			int width = (mainTexture != null) ? mainTexture.width : 512;
			int height = (mainTexture != null) ? mainTexture.height : 512;
			for (int i = 0; i < sprites.Count; i++)
			{
				Sprite sprite = sprites[i];
				Rect outer = sprite.outer;
				Rect inner = sprite.inner;
				if (mCoordinates == Coordinates.TexCoords)
				{
					NGUIMath.ConvertToPixels(outer, width, height, true);
					NGUIMath.ConvertToPixels(inner, width, height, true);
				}
				UISpriteData uISpriteData = new UISpriteData();
				uISpriteData.name = sprite.name;
				uISpriteData.x = Mathf.RoundToInt(outer.xMin);
				uISpriteData.y = Mathf.RoundToInt(outer.yMin);
				uISpriteData.width = Mathf.RoundToInt(outer.width);
				uISpriteData.height = Mathf.RoundToInt(outer.height);
				uISpriteData.paddingLeft = Mathf.RoundToInt(sprite.paddingLeft * outer.width);
				uISpriteData.paddingRight = Mathf.RoundToInt(sprite.paddingRight * outer.width);
				uISpriteData.paddingBottom = Mathf.RoundToInt(sprite.paddingBottom * outer.height);
				uISpriteData.paddingTop = Mathf.RoundToInt(sprite.paddingTop * outer.height);
				uISpriteData.borderLeft = Mathf.RoundToInt(inner.xMin - outer.xMin);
				uISpriteData.borderRight = Mathf.RoundToInt(outer.xMax - inner.xMax);
				uISpriteData.borderBottom = Mathf.RoundToInt(outer.yMax - inner.yMax);
				uISpriteData.borderTop = Mathf.RoundToInt(inner.yMin - outer.yMin);
				mSprites.Add(uISpriteData);
			}
			sprites.Clear();
			return true;
		}
		return false;
	}
}
