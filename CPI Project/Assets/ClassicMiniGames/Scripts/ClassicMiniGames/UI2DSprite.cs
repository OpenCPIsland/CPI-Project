using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Unity2D Sprite")]
[ExecuteInEditMode]
public class UI2DSprite : UIBasicSprite
{
	[HideInInspector]
	[SerializeField]
	private Sprite mSprite;

	[SerializeField]
	[HideInInspector]
	private Material mMat;

	[SerializeField]
	[HideInInspector]
	private Shader mShader;

	[HideInInspector]
	[SerializeField]
	private Vector4 mBorder = Vector4.zero;

	public Sprite nextSprite;

	[NonSerialized]
	private int mPMA = -1;

	public Sprite sprite2D
	{
		get
		{
			return mSprite;
		}
		set
		{
			if (mSprite != value)
			{
				RemoveFromPanel();
				mSprite = value;
				nextSprite = null;
				MarkAsChanged();
			}
		}
	}

	public override Material material
	{
		get
		{
			return mMat;
		}
		set
		{
			if (mMat != value)
			{
				RemoveFromPanel();
				mMat = value;
				mPMA = -1;
				MarkAsChanged();
			}
		}
	}

	public override Shader shader
	{
		get
		{
			if (mMat != null)
			{
				return mMat.shader;
			}
			if (mShader == null)
			{
				mShader = Shader.Find("Unlit/Transparent Colored");
			}
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				RemoveFromPanel();
				mShader = value;
				if (mMat == null)
				{
					mPMA = -1;
					MarkAsChanged();
				}
			}
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (mSprite != null)
			{
				return mSprite.texture;
			}
			if (mMat != null)
			{
				return mMat.mainTexture;
			}
			return null;
		}
	}

	public override bool premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Shader shader = this.shader;
				mPMA = ((shader != null && shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public override Vector4 drawingDimensions
	{
		get
		{
			Vector2 pivotOffset = base.pivotOffset;
			float num = (0f - pivotOffset.x) * (float)mWidth;
			float num2 = (0f - pivotOffset.y) * (float)mHeight;
			float num3 = num + (float)mWidth;
			float num4 = num2 + (float)mHeight;
			if (mSprite != null && mType != Type.Tiled)
			{
				int num5 = Mathf.RoundToInt(mSprite.rect.width);
				int num6 = Mathf.RoundToInt(mSprite.rect.height);
				int num7 = Mathf.RoundToInt(mSprite.textureRectOffset.x);
				int num8 = Mathf.RoundToInt(mSprite.textureRectOffset.y);
				int num9 = Mathf.RoundToInt(mSprite.rect.width - mSprite.textureRect.width - mSprite.textureRectOffset.x);
				int num10 = Mathf.RoundToInt(mSprite.rect.height - mSprite.textureRect.height - mSprite.textureRectOffset.y);
				float num11 = 1f;
				float num12 = 1f;
				if (num5 > 0 && num6 > 0 && (mType == Type.Simple || mType == Type.Filled))
				{
					if ((num5 & 1) != 0)
					{
						num9++;
					}
					if ((num6 & 1) != 0)
					{
						num10++;
					}
					num11 = 1f / (float)num5 * (float)mWidth;
					num12 = 1f / (float)num6 * (float)mHeight;
				}
				if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
				{
					num += (float)num9 * num11;
					num3 -= (float)num7 * num11;
				}
				else
				{
					num += (float)num7 * num11;
					num3 -= (float)num9 * num11;
				}
				if (mFlip == Flip.Vertically || mFlip == Flip.Both)
				{
					num2 += (float)num10 * num12;
					num4 -= (float)num8 * num12;
				}
				else
				{
					num2 += (float)num8 * num12;
					num4 -= (float)num10 * num12;
				}
			}
			Vector4 border = this.border;
			float num13 = border.x + border.z;
			float num14 = border.y + border.w;
			float x = Mathf.Lerp(num, num3 - num13, mDrawRegion.x);
			float y = Mathf.Lerp(num2, num4 - num14, mDrawRegion.y);
			float z = Mathf.Lerp(num + num13, num3, mDrawRegion.z);
			float w = Mathf.Lerp(num2 + num14, num4, mDrawRegion.w);
			return new Vector4(x, y, z, w);
		}
	}

	public override Vector4 border
	{
		get
		{
			return mBorder;
		}
		set
		{
			if (mBorder != value)
			{
				mBorder = value;
				MarkAsChanged();
			}
		}
	}

	protected override void OnUpdate()
	{
		if (nextSprite != null)
		{
			if (nextSprite != mSprite)
			{
				sprite2D = nextSprite;
			}
			nextSprite = null;
		}
		base.OnUpdate();
	}

	public override void MakePixelPerfect()
	{
		base.MakePixelPerfect();
		if (mType == Type.Tiled)
		{
			return;
		}
		Texture mainTexture = this.mainTexture;
		if (!(mainTexture == null) && (mType == Type.Simple || mType == Type.Filled || !base.hasBorder) && mainTexture != null)
		{
			Rect rect = mSprite.rect;
			int num = Mathf.RoundToInt(rect.width);
			int num2 = Mathf.RoundToInt(rect.height);
			if ((num & 1) == 1)
			{
				num++;
			}
			if ((num2 & 1) == 1)
			{
				num2++;
			}
			base.width = num;
			base.height = num2;
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture mainTexture = this.mainTexture;
		if (!(mainTexture == null))
		{
			Rect textureRect = mSprite.textureRect;
			Rect inner = textureRect;
			Vector4 border = this.border;
			inner.xMin += border.x;
			inner.yMin += border.y;
			inner.xMax -= border.z;
			inner.yMax -= border.w;
			float num = 1f / (float)mainTexture.width;
			float num2 = 1f / (float)mainTexture.height;
			textureRect.xMin *= num;
			textureRect.xMax *= num;
			textureRect.yMin *= num2;
			textureRect.yMax *= num2;
			inner.xMin *= num;
			inner.xMax *= num;
			inner.yMin *= num2;
			inner.yMax *= num2;
			Fill(verts, uvs, cols, textureRect, inner);
		}
	}
}
