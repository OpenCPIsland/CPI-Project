using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Texture")]
public class UITexture : UIBasicSprite
{
	[SerializeField]
	[HideInInspector]
	private Rect mRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private Texture mTexture;

	[HideInInspector]
	[SerializeField]
	private Material mMat;

	[SerializeField]
	[HideInInspector]
	private Shader mShader;

	[SerializeField]
	[HideInInspector]
	private Vector4 mBorder = Vector4.zero;

	[NonSerialized]
	private int mPMA = -1;

	public override Texture mainTexture
	{
		get
		{
			if (mTexture != null)
			{
				return mTexture;
			}
			if (mMat != null)
			{
				return mMat.mainTexture;
			}
			return null;
		}
		set
		{
			if (mTexture != value)
			{
				RemoveFromPanel();
				mTexture = value;
				if (value != null)
				{
					mMat = null;
				}
				mPMA = -1;
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
				mShader = null;
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
				mPMA = -1;
				mMat = null;
				MarkAsChanged();
			}
		}
	}

	public override bool premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Material material = this.material;
				mPMA = ((material != null && material.shader != null && material.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
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

	public Rect uvRect
	{
		get
		{
			return mRect;
		}
		set
		{
			if (mRect != value)
			{
				mRect = value;
				MarkAsChanged();
			}
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
			if (mTexture != null && mType != Type.Tiled)
			{
				int width = mTexture.width;
				int height = mTexture.height;
				int num5 = 0;
				int num6 = 0;
				float num7 = 1f;
				float num8 = 1f;
				if (width > 0 && height > 0 && (mType == Type.Simple || mType == Type.Filled))
				{
					if ((width & 1) != 0)
					{
						num5++;
					}
					if ((height & 1) != 0)
					{
						num6++;
					}
					num7 = 1f / (float)width * (float)mWidth;
					num8 = 1f / (float)height * (float)mHeight;
				}
				if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
				{
					num += (float)num5 * num7;
				}
				else
				{
					num3 -= (float)num5 * num7;
				}
				if (mFlip == Flip.Vertically || mFlip == Flip.Both)
				{
					num2 += (float)num6 * num8;
				}
				else
				{
					num4 -= (float)num6 * num8;
				}
			}
			Vector4 border = this.border;
			float num9 = border.x + border.z;
			float num10 = border.y + border.w;
			float x = Mathf.Lerp(num, num3 - num9, mDrawRegion.x);
			float y = Mathf.Lerp(num2, num4 - num10, mDrawRegion.y);
			float z = Mathf.Lerp(num + num9, num3, mDrawRegion.z);
			float w = Mathf.Lerp(num2 + num10, num4, mDrawRegion.w);
			return new Vector4(x, y, z, w);
		}
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
			int num = mainTexture.width;
			int num2 = mainTexture.height;
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
			Rect rect = new Rect(mRect.x * (float)mainTexture.width, mRect.y * (float)mainTexture.height, (float)mainTexture.width * mRect.width, (float)mainTexture.height * mRect.height);
			Rect inner = rect;
			Vector4 border = this.border;
			inner.xMin += border.x;
			inner.yMin += border.y;
			inner.xMax -= border.z;
			inner.yMax -= border.w;
			float num = 1f / (float)mainTexture.width;
			float num2 = 1f / (float)mainTexture.height;
			rect.xMin *= num;
			rect.xMax *= num;
			rect.yMin *= num2;
			rect.yMax *= num2;
			inner.xMin *= num;
			inner.xMax *= num;
			inner.yMin *= num2;
			inner.yMax *= num2;
			Fill(verts, uvs, cols, rect, inner);
		}
	}
}
