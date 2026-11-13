using System.Diagnostics;
using System.Text;
using UnityEngine;

public static class NGUIText
{
	public enum Alignment
	{
		Automatic,
		Left,
		Center,
		Right,
		Justified
	}

	public enum SymbolStyle
	{
		None,
		Normal,
		Colored
	}

	public class GlyphInfo
	{
		public Vector2 v0;

		public Vector2 v1;

		public Vector2 u0;

		public Vector2 u1;

		public float advance = 0f;

		public int channel = 0;

		public bool rotatedUVs = false;
	}

	public static UIFont bitmapFont;

	public static Font dynamicFont;

	public static GlyphInfo glyph = new GlyphInfo();

	public static int fontSize = 16;

	public static float fontScale = 1f;

	public static float pixelDensity = 1f;

	public static FontStyle fontStyle = FontStyle.Normal;

	public static Alignment alignment = Alignment.Left;

	public static Color tint = Color.white;

	public static int rectWidth = 1000000;

	public static int rectHeight = 1000000;

	public static int maxLines = 0;

	public static bool gradient = false;

	public static Color gradientBottom = Color.white;

	public static Color gradientTop = Color.white;

	public static bool encoding = false;

	public static float spacingX = 0f;

	public static float spacingY = 0f;

	public static bool premultiply = false;

	public static SymbolStyle symbolStyle;

	public static int finalSize = 0;

	public static float finalSpacingX = 0f;

	public static float finalLineHeight = 0f;

	public static float baseline = 0f;

	public static bool useSymbols = false;

	private static Color mInvisible = new Color(0f, 0f, 0f, 0f);

	private static BetterList<Color> mColors = new BetterList<Color>();

	private static CharacterInfo mTempChar;

	private static BetterList<float> mSizes = new BetterList<float>();

	private static Color32 s_c0;

	private static Color32 s_c1;

	private static float[] mBoldOffset = new float[8]
	{
		-0.5f,
		0f,
		0.5f,
		0f,
		0f,
		-0.5f,
		0f,
		0.5f
	};

	public static void Update()
	{
		Update(true);
	}

	public static void Update(bool request)
	{
		finalSize = Mathf.RoundToInt((float)fontSize / pixelDensity);
		finalSpacingX = spacingX * fontScale;
		finalLineHeight = ((float)fontSize + spacingY) * fontScale;
		useSymbols = (bitmapFont != null && bitmapFont.hasSymbols && encoding && symbolStyle != SymbolStyle.None);
		if (!(dynamicFont != null) || !request)
		{
			return;
		}
		dynamicFont.RequestCharactersInTexture(")_-", finalSize, fontStyle);
		if (!dynamicFont.GetCharacterInfo(')', out mTempChar, finalSize, fontStyle))
		{
			dynamicFont.RequestCharactersInTexture("A", finalSize, fontStyle);
			if (!dynamicFont.GetCharacterInfo('A', out mTempChar, finalSize, fontStyle))
			{
				baseline = 0f;
				return;
			}
		}
		float num = mTempChar.maxY;
		float num2 = mTempChar.minY;
		baseline = Mathf.Round(num + ((float)finalSize - num + num2) * 0.5f);
	}

	public static void Prepare(string text)
	{
		if (dynamicFont != null)
		{
			dynamicFont.RequestCharactersInTexture(text, finalSize, fontStyle);
		}
	}

	public static BMSymbol GetSymbol(string text, int index, int textLength)
	{
		return (bitmapFont != null) ? bitmapFont.MatchSymbol(text, index, textLength) : null;
	}

	public static float GetGlyphWidth(int ch, int prev)
	{
		if (bitmapFont != null)
		{
			BMGlyph bMGlyph = bitmapFont.bmFont.GetGlyph(ch);
			if (bMGlyph != null)
			{
				return fontScale * (float)((prev != 0) ? (bMGlyph.advance + bMGlyph.GetKerning(prev)) : bMGlyph.advance);
			}
		}
		else if (dynamicFont != null && dynamicFont.GetCharacterInfo((char)ch, out mTempChar, finalSize, fontStyle))
		{
			return (float)mTempChar.advance * fontScale * pixelDensity;
		}
		return 0f;
	}

	public static GlyphInfo GetGlyph(int ch, int prev)
	{
		if (bitmapFont != null)
		{
			BMGlyph bMGlyph = bitmapFont.bmFont.GetGlyph(ch);
			if (bMGlyph != null)
			{
				int num = (prev != 0) ? bMGlyph.GetKerning(prev) : 0;
				glyph.v0.x = ((prev != 0) ? (bMGlyph.offsetX + num) : bMGlyph.offsetX);
				glyph.v1.y = -bMGlyph.offsetY;
				glyph.v1.x = glyph.v0.x + (float)bMGlyph.width;
				glyph.v0.y = glyph.v1.y - (float)bMGlyph.height;
				glyph.u0.x = bMGlyph.x;
				glyph.u0.y = bMGlyph.y + bMGlyph.height;
				glyph.u1.x = bMGlyph.x + bMGlyph.width;
				glyph.u1.y = bMGlyph.y;
				glyph.advance = bMGlyph.advance + num;
				glyph.channel = bMGlyph.channel;
				glyph.rotatedUVs = false;
				if (fontScale != 1f)
				{
					glyph.v0 *= fontScale;
					glyph.v1 *= fontScale;
					glyph.advance *= fontScale;
				}
				return glyph;
			}
		}
		else if (dynamicFont != null && dynamicFont.GetCharacterInfo((char)ch, out mTempChar, finalSize, fontStyle))
		{
			glyph.v0.x = mTempChar.minX;
			glyph.v1.x = glyph.v0.x + (float)mTempChar.maxX - (float)mTempChar.minX;
			glyph.v0.y = (float)mTempChar.maxY - baseline;
			glyph.v1.y = glyph.v0.y - (float)mTempChar.maxY - (float)mTempChar.minY;
			glyph.u0.x = mTempChar.minX;
			glyph.u0.y = mTempChar.minY;
			glyph.u1.x = mTempChar.maxX;
			glyph.u1.y = mTempChar.maxY;
			glyph.advance = mTempChar.maxX - mTempChar.minX;
			glyph.channel = 0;
			glyph.v0.x = Mathf.Round(glyph.v0.x);
			glyph.v0.y = Mathf.Round(glyph.v0.y);
			glyph.v1.x = Mathf.Round(glyph.v1.x);
			glyph.v1.y = Mathf.Round(glyph.v1.y);
			float num2 = fontScale * pixelDensity;
			if (num2 != 1f)
			{
				glyph.v0 *= num2;
				glyph.v1 *= num2;
				glyph.advance *= num2;
			}
			return glyph;
		}
		return null;
	}

	public static Color ParseColor(string text, int offset)
	{
		return ParseColor24(text, offset);
	}

	public static Color ParseColor24(string text, int offset)
	{
		int num = (NGUIMath.HexToDecimal(text[offset]) << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
		int num2 = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
		int num3 = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
		float num4 = 0.003921569f;
		return new Color(num4 * (float)num, num4 * (float)num2, num4 * (float)num3);
	}

	public static Color ParseColor32(string text, int offset)
	{
		int num = (NGUIMath.HexToDecimal(text[offset]) << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
		int num2 = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
		int num3 = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
		int num4 = (NGUIMath.HexToDecimal(text[offset + 6]) << 4) | NGUIMath.HexToDecimal(text[offset + 7]);
		float num5 = 0.003921569f;
		return new Color(num5 * (float)num, num5 * (float)num2, num5 * (float)num3, num5 * (float)num4);
	}

	public static string EncodeColor(Color c)
	{
		return EncodeColor24(c);
	}

	public static string EncodeColor24(Color c)
	{
		int num = 0xFFFFFF & (NGUIMath.ColorToInt(c) >> 8);
		return NGUIMath.DecimalToHex24(num);
	}

	public static string EncodeColor32(Color c)
	{
		int num = NGUIMath.ColorToInt(c);
		return NGUIMath.DecimalToHex32(num);
	}

	public static bool ParseSymbol(string text, ref int index)
	{
		int sub = 1;
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strike = false;
		return ParseSymbol(text, ref index, null, false, ref sub, ref bold, ref italic, ref underline, ref strike);
	}

	public static bool ParseSymbol(string text, ref int index, BetterList<Color> colors, bool premultiply, ref int sub, ref bool bold, ref bool italic, ref bool underline, ref bool strike)
	{
		int length = text.Length;
		if (index + 3 > length || text[index] != '[')
		{
			return false;
		}
		if (text[index + 2] == ']')
		{
			if (text[index + 1] == '-')
			{
				if (colors != null && colors.size > 1)
				{
					colors.RemoveAt(colors.size - 1);
				}
				index += 3;
				return true;
			}
			switch (text.Substring(index, 3))
			{
			case "[b]":
				bold = true;
				index += 3;
				return true;
			case "[i]":
				italic = true;
				index += 3;
				return true;
			case "[u]":
				underline = true;
				index += 3;
				return true;
			case "[s]":
				strike = true;
				index += 3;
				return true;
			}
		}
		if (index + 4 > length)
		{
			return false;
		}
		if (text[index + 3] == ']')
		{
			switch (text.Substring(index, 4))
			{
			case "[/b]":
				bold = false;
				index += 4;
				return true;
			case "[/i]":
				italic = false;
				index += 4;
				return true;
			case "[/u]":
				underline = false;
				index += 4;
				return true;
			case "[/s]":
				strike = false;
				index += 4;
				return true;
			}
		}
		if (index + 5 > length)
		{
			return false;
		}
		if (text[index + 4] == ']')
		{
			switch (text.Substring(index, 5))
			{
			case "[sub]":
				sub = 1;
				index += 5;
				return true;
			case "[sup]":
				sub = 2;
				index += 5;
				return true;
			}
		}
		if (index + 6 > length)
		{
			return false;
		}
		if (text[index + 5] == ']')
		{
			switch (text.Substring(index, 6))
			{
			case "[/sub]":
				sub = 0;
				index += 6;
				return true;
			case "[/sup]":
				sub = 0;
				index += 6;
				return true;
			case "[/url]":
				index += 6;
				return true;
			}
		}
		if (text[index + 1] == 'u' && text[index + 2] == 'r' && text[index + 3] == 'l' && text[index + 4] == '=')
		{
			int num = text.IndexOf(']', index + 4);
			if (num != -1)
			{
				index = num + 1;
				return true;
			}
		}
		if (index + 8 > length)
		{
			return false;
		}
		Color color;
		if (text[index + 7] == ']')
		{
			color = ParseColor24(text, index + 1);
			if (EncodeColor24(color) != text.Substring(index + 1, 6).ToUpper())
			{
				return false;
			}
			if (colors != null)
			{
				color.a = colors[colors.size - 1].a;
				if (premultiply && color.a != 1f)
				{
					color = Color.Lerp(mInvisible, color, color.a);
				}
				colors.Add(color);
			}
			index += 8;
			return true;
		}
		if (index + 10 > length)
		{
			return false;
		}
		if (text[index + 9] == ']')
		{
			color = ParseColor32(text, index + 1);
			if (EncodeColor32(color) != text.Substring(index + 1, 8).ToUpper())
			{
				return false;
			}
			if (colors != null)
			{
				if (premultiply && color.a != 1f)
				{
					color = Color.Lerp(mInvisible, color, color.a);
				}
				colors.Add(color);
			}
			index += 10;
			return true;
		}
		return false;
	}

	public static string StripSymbols(string text)
	{
		if (text != null)
		{
			int num = 0;
			int length = text.Length;
			while (num < length)
			{
				char c = text[num];
				if (c == '[')
				{
					int sub = 0;
					bool bold = false;
					bool italic = false;
					bool underline = false;
					bool strike = false;
					int index = num;
					if (ParseSymbol(text, ref index, null, false, ref sub, ref bold, ref italic, ref underline, ref strike))
					{
						text = text.Remove(num, index - num);
						length = text.Length;
						continue;
					}
				}
				num++;
			}
		}
		return text;
	}

	public static void Align(BetterList<Vector3> verts, int indexOffset, float printedWidth)
	{
		switch (alignment)
		{
		case Alignment.Right:
		{
			float num = (float)rectWidth - printedWidth;
			if (!(num < 0f))
			{
				for (int num5 = indexOffset; num5 < verts.size; num5++)
				{
					verts.buffer[num5].x += num;
				}
			}
			break;
		}
		case Alignment.Center:
		{
			float num = ((float)rectWidth - printedWidth) * 0.5f;
			if (!(num < 0f))
			{
				int num10 = Mathf.RoundToInt((float)rectWidth - printedWidth);
				int num11 = Mathf.RoundToInt(rectWidth);
				bool flag = (num10 & 1) == 1;
				bool flag2 = (num11 & 1) == 1;
				if ((flag && !flag2) || (!flag && flag2))
				{
					num += 0.5f * fontScale;
				}
				for (int num5 = indexOffset; num5 < verts.size; num5++)
				{
					verts.buffer[num5].x += num;
				}
			}
			break;
		}
		case Alignment.Justified:
		{
			if (printedWidth < (float)rectWidth * 0.65f)
			{
				break;
			}
			float num = ((float)rectWidth - printedWidth) * 0.5f;
			if (num < 1f)
			{
				break;
			}
			int num2 = (verts.size - indexOffset) / 4;
			if (num2 >= 1)
			{
				float num3 = 1f / (float)(num2 - 1);
				float num4 = (float)rectWidth / printedWidth;
				int num5 = indexOffset + 4;
				int num6 = 1;
				while (num5 < verts.size)
				{
					float x = verts.buffer[num5].x;
					float x2 = verts.buffer[num5 + 2].x;
					float num7 = x2 - x;
					float num8 = x * num4;
					float a = num8 + num7;
					float num9 = x2 * num4;
					float b = num9 - num7;
					float t = (float)num6 * num3;
					x = Mathf.Lerp(num8, b, t);
					x2 = Mathf.Lerp(a, num9, t);
					x = Mathf.Round(x);
					x2 = Mathf.Round(x2);
					verts.buffer[num5++].x = x;
					verts.buffer[num5++].x = x;
					verts.buffer[num5++].x = x2;
					verts.buffer[num5++].x = x2;
					num6++;
				}
			}
			break;
		}
		}
	}

	public static int GetClosestCharacter(BetterList<Vector3> verts, Vector2 pos)
	{
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		int result = 0;
		for (int i = 0; i < verts.size; i++)
		{
			float num3 = Mathf.Abs(pos.y - verts[i].y);
			if (!(num3 > num2))
			{
				float num4 = Mathf.Abs(pos.x - verts[i].x);
				if (num3 < num2)
				{
					num2 = num3;
					num = num4;
					result = i;
				}
				else if (num4 < num)
				{
					num = num4;
					result = i;
				}
			}
		}
		return result;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	private static bool IsSpace(int ch)
	{
		return ch == 32 || ch == 8202 || ch == 8203;
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	public static void EndLine(ref StringBuilder s)
	{
		int num = s.Length - 1;
		if (num > 0 && IsSpace(s[num]))
		{
			s[num] = '\n';
		}
		else
		{
			s.Append('\n');
		}
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	private static void ReplaceSpaceWithNewline(ref StringBuilder s)
	{
		int num = s.Length - 1;
		if (num > 0 && IsSpace(s[num]))
		{
			s[num] = '\n';
		}
	}

	public static Vector2 CalculatePrintedSize(string text)
	{
		Vector2 zero = Vector2.zero;
		if (!string.IsNullOrEmpty(text))
		{
			if (encoding)
			{
				text = StripSymbols(text);
			}
			Prepare(text);
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			int length = text.Length;
			int num4 = 0;
			int prev = 0;
			for (int i = 0; i < length; i++)
			{
				num4 = text[i];
				if (num4 == 10)
				{
					if (num > num3)
					{
						num3 = num;
					}
					num = 0f;
					num2 += finalLineHeight;
				}
				else
				{
					if (num4 < 32)
					{
						continue;
					}
					BMSymbol bMSymbol = useSymbols ? GetSymbol(text, i, length) : null;
					float glyphWidth;
					if (bMSymbol == null)
					{
						glyphWidth = GetGlyphWidth(num4, prev);
						if (glyphWidth == 0f)
						{
							continue;
						}
						glyphWidth += finalSpacingX;
						if (Mathf.RoundToInt(num + glyphWidth) > rectWidth)
						{
							if (num > num3)
							{
								num3 = num - finalSpacingX;
							}
							num = glyphWidth;
							num2 += finalLineHeight;
						}
						else
						{
							num += glyphWidth;
						}
						prev = num4;
						continue;
					}
					glyphWidth = finalSpacingX + (float)bMSymbol.advance * fontScale;
					if (Mathf.RoundToInt(num + glyphWidth) > rectWidth)
					{
						if (num > num3)
						{
							num3 = num - finalSpacingX;
						}
						num = glyphWidth;
						num2 += finalLineHeight;
					}
					else
					{
						num += glyphWidth;
					}
					i += bMSymbol.sequence.Length - 1;
					prev = 0;
				}
			}
			zero.x = ((num > num3) ? (num - finalSpacingX) : num3);
			zero.y = num2 + finalLineHeight;
		}
		return zero;
	}

	public static int CalculateOffsetToFit(string text)
	{
		if (string.IsNullOrEmpty(text) || rectWidth < 1)
		{
			return 0;
		}
		Prepare(text);
		int length = text.Length;
		int num = 0;
		int prev = 0;
		int i = 0;
		for (int length2 = text.Length; i < length2; i++)
		{
			BMSymbol bMSymbol = useSymbols ? GetSymbol(text, i, length) : null;
			if (bMSymbol == null)
			{
				num = text[i];
				float glyphWidth = GetGlyphWidth(num, prev);
				if (glyphWidth != 0f)
				{
					mSizes.Add(finalSpacingX + glyphWidth);
				}
				prev = num;
				continue;
			}
			mSizes.Add(finalSpacingX + (float)bMSymbol.advance * fontScale);
			int j = 0;
			for (int num2 = bMSymbol.sequence.Length - 1; j < num2; j++)
			{
				mSizes.Add(0f);
			}
			i += bMSymbol.sequence.Length - 1;
			prev = 0;
		}
		float num3 = rectWidth;
		int num4 = mSizes.size;
		while (num4 > 0 && num3 > 0f)
		{
			num3 -= mSizes[--num4];
		}
		mSizes.Clear();
		if (num3 < 0f)
		{
			num4++;
		}
		return num4;
	}

	public static string GetEndOfLineThatFits(string text)
	{
		int length = text.Length;
		int num = CalculateOffsetToFit(text);
		return text.Substring(num, length - num);
	}

	public static bool WrapText(string text, out string finalText)
	{
		return WrapText(text, out finalText, false);
	}

	public static bool WrapText(string text, out string finalText, bool keepCharCount)
	{
		if (rectWidth < 1 || rectHeight < 1 || finalLineHeight < 1f)
		{
			finalText = "";
			return false;
		}
		float num = (maxLines > 0) ? Mathf.Min(rectHeight, finalLineHeight * (float)maxLines) : ((float)rectHeight);
		int num2 = (maxLines > 0) ? maxLines : 1000000;
		num2 = Mathf.FloorToInt(Mathf.Min(num2, num / finalLineHeight) + 0.01f);
		if (num2 == 0)
		{
			finalText = "";
			return false;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		Prepare(text);
		StringBuilder s = new StringBuilder();
		int length = text.Length;
		float num3 = rectWidth;
		int num4 = 0;
		int i = 0;
		int num5 = 1;
		int num6 = 0;
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		for (; i < length; i++)
		{
			char c = text[i];
			if (c > '\u2fff')
			{
				flag3 = true;
			}
			if (c == '\n')
			{
				if (num5 == num2)
				{
					break;
				}
				num3 = rectWidth;
				if (num4 < i)
				{
					s.Append(text.Substring(num4, i - num4 + 1));
				}
				else
				{
					s.Append(c);
				}
				flag = true;
				num5++;
				num4 = i + 1;
				num6 = 0;
				continue;
			}
			if (encoding && ParseSymbol(text, ref i))
			{
				i--;
				continue;
			}
			BMSymbol bMSymbol = useSymbols ? GetSymbol(text, i, length) : null;
			float num7;
			if (bMSymbol == null)
			{
				float glyphWidth = GetGlyphWidth(c, num6);
				if (glyphWidth == 0f)
				{
					continue;
				}
				num7 = finalSpacingX + glyphWidth;
			}
			else
			{
				num7 = finalSpacingX + (float)bMSymbol.advance * fontScale;
			}
			num3 -= num7;
			if (IsSpace(c) && !flag3)
			{
				bool flag4 = IsSpace(num6);
				if (flag4)
				{
					s.Append(' ');
					num4 = i;
				}
				else if (!flag4 && num4 < i)
				{
					int num8 = i - num4 + 1;
					if (num5 == num2 && num3 <= 0f && i < length)
					{
						char c2 = text[i];
						if (c2 < ' ' || IsSpace(c2))
						{
							num8--;
						}
					}
					s.Append(text.Substring(num4, num8));
					flag = false;
					num4 = i + 1;
					num6 = c;
				}
			}
			if (Mathf.RoundToInt(num3) < 0)
			{
				if (!flag && num5 != num2)
				{
					flag = true;
					num3 = rectWidth;
					i = num4 - 1;
					num6 = 0;
					if (num5++ == num2)
					{
						break;
					}
					if (keepCharCount)
					{
						ReplaceSpaceWithNewline(ref s);
					}
					else
					{
						EndLine(ref s);
					}
					continue;
				}
				s.Append(text.Substring(num4, Mathf.Max(0, i - num4)));
				bool flag5 = IsSpace(c);
				if (!flag5 && !flag3)
				{
					flag2 = false;
				}
				if (num5++ == num2)
				{
					num4 = i;
					break;
				}
				if (keepCharCount)
				{
					ReplaceSpaceWithNewline(ref s);
				}
				else
				{
					EndLine(ref s);
				}
				flag = true;
				if (flag5)
				{
					num4 = i + 1;
					num3 = rectWidth;
				}
				else
				{
					num4 = i;
					num3 = (float)rectWidth - num7;
				}
				num6 = 0;
			}
			else
			{
				num6 = c;
			}
			if (bMSymbol != null)
			{
				i += bMSymbol.length - 1;
				num6 = 0;
			}
		}
		if (num4 < i)
		{
			s.Append(text.Substring(num4, i - num4));
		}
		finalText = s.ToString();
		return flag2 && (i == length || num5 <= Mathf.Min(maxLines, num2));
	}

	public static void Print(string text, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		int size = verts.size;
		Prepare(text);
		mColors.Add(Color.white);
		int num = 0;
		int prev = 0;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = finalSize;
		Color a = tint * gradientBottom;
		Color b = tint * gradientTop;
		Color32 color = tint;
		int length = text.Length;
		Rect rect = default(Rect);
		float num6 = 0f;
		float num7 = 0f;
		float num8 = num5 * pixelDensity;
		bool flag = false;
		int sub = 0;
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strike = false;
		float num9 = 0f;
		if (bitmapFont != null)
		{
			rect = bitmapFont.uvRect;
			num6 = rect.width / (float)bitmapFont.texWidth;
			num7 = rect.height / (float)bitmapFont.texHeight;
		}
		for (int i = 0; i < length; i++)
		{
			num = text[i];
			num9 = num2;
			if (num == 10)
			{
				if (num2 > num4)
				{
					num4 = num2;
				}
				if (alignment != Alignment.Left)
				{
					Align(verts, size, num2 - finalSpacingX);
					size = verts.size;
				}
				num2 = 0f;
				num3 += finalLineHeight;
				prev = 0;
				continue;
			}
			if (num < 32)
			{
				prev = num;
				continue;
			}
			if (encoding && ParseSymbol(text, ref i, mColors, premultiply, ref sub, ref bold, ref italic, ref underline, ref strike))
			{
				Color color2 = tint * mColors[mColors.size - 1];
				color = color2;
				if (gradient)
				{
					a = gradientBottom * color2;
					b = gradientTop * color2;
				}
				i--;
				continue;
			}
			BMSymbol bMSymbol = useSymbols ? GetSymbol(text, i, length) : null;
			float num10;
			float num11;
			float num13;
			float num12;
			if (bMSymbol != null)
			{
				num10 = num2 + (float)bMSymbol.offsetX * fontScale;
				num11 = num10 + (float)bMSymbol.width * fontScale;
				num12 = 0f - (num3 + (float)bMSymbol.offsetY * fontScale);
				num13 = num12 - (float)bMSymbol.height * fontScale;
				if (Mathf.RoundToInt(num2 + (float)bMSymbol.advance * fontScale) > rectWidth)
				{
					if (num2 == 0f)
					{
						return;
					}
					if (alignment != Alignment.Left && size < verts.size)
					{
						Align(verts, size, num2 - finalSpacingX);
						size = verts.size;
					}
					num10 -= num2;
					num11 -= num2;
					num13 -= finalLineHeight;
					num12 -= finalLineHeight;
					num2 = 0f;
					num3 += finalLineHeight;
					num9 = 0f;
				}
				verts.Add(new Vector3(num10, num13));
				verts.Add(new Vector3(num10, num12));
				verts.Add(new Vector3(num11, num12));
				verts.Add(new Vector3(num11, num13));
				num2 += finalSpacingX + (float)bMSymbol.advance * fontScale;
				i += bMSymbol.length - 1;
				prev = 0;
				if (uvs != null)
				{
					Rect uvRect = bMSymbol.uvRect;
					float xMin = uvRect.xMin;
					float yMin = uvRect.yMin;
					float xMax = uvRect.xMax;
					float yMax = uvRect.yMax;
					uvs.Add(new Vector2(xMin, yMin));
					uvs.Add(new Vector2(xMin, yMax));
					uvs.Add(new Vector2(xMax, yMax));
					uvs.Add(new Vector2(xMax, yMin));
				}
				if (cols == null)
				{
					continue;
				}
				if (symbolStyle == SymbolStyle.Colored)
				{
					for (int j = 0; j < 4; j++)
					{
						cols.Add(color);
					}
					continue;
				}
				Color32 item = Color.white;
				item.a = color.a;
				for (int j = 0; j < 4; j++)
				{
					cols.Add(item);
				}
				continue;
			}
			GlyphInfo glyphInfo = GetGlyph(num, prev);
			if (glyphInfo == null)
			{
				continue;
			}
			prev = num;
			if (sub != 0)
			{
				glyphInfo.v0.x *= 0.75f;
				glyphInfo.v0.y *= 0.75f;
				glyphInfo.v1.x *= 0.75f;
				glyphInfo.v1.y *= 0.75f;
				if (sub == 1)
				{
					glyphInfo.v0.y -= fontScale * (float)fontSize * 0.4f;
					glyphInfo.v1.y -= fontScale * (float)fontSize * 0.4f;
				}
				else
				{
					glyphInfo.v0.y += fontScale * (float)fontSize * 0.05f;
					glyphInfo.v1.y += fontScale * (float)fontSize * 0.05f;
				}
			}
			float y = glyphInfo.v0.y;
			float y2 = glyphInfo.v1.y;
			num10 = glyphInfo.v0.x + num2;
			num13 = glyphInfo.v0.y - num3;
			num11 = glyphInfo.v1.x + num2;
			num12 = glyphInfo.v1.y - num3;
			float num14 = glyphInfo.advance;
			if (finalSpacingX < 0f)
			{
				num14 += finalSpacingX;
			}
			if (Mathf.RoundToInt(num2 + num14) > rectWidth)
			{
				if (num2 == 0f)
				{
					return;
				}
				if (alignment != Alignment.Left && size < verts.size)
				{
					Align(verts, size, num2 - finalSpacingX);
					size = verts.size;
				}
				num10 -= num2;
				num11 -= num2;
				num13 -= finalLineHeight;
				num12 -= finalLineHeight;
				num2 = 0f;
				num3 += finalLineHeight;
				num9 = 0f;
			}
			if (IsSpace(num))
			{
				if (underline)
				{
					num = 95;
				}
				else if (strike)
				{
					num = 45;
				}
			}
			num2 += ((sub == 0) ? (finalSpacingX + glyphInfo.advance) : ((finalSpacingX + glyphInfo.advance) * 0.75f));
			if (IsSpace(num))
			{
				continue;
			}
			if (uvs != null)
			{
				if (bitmapFont != null)
				{
					glyphInfo.u0.x = rect.xMin + num6 * glyphInfo.u0.x;
					glyphInfo.u1.x = rect.xMin + num6 * glyphInfo.u1.x;
					glyphInfo.u0.y = rect.yMax - num7 * glyphInfo.u0.y;
					glyphInfo.u1.y = rect.yMax - num7 * glyphInfo.u1.y;
				}
				int k = 0;
				for (int num15 = (!bold) ? 1 : 4; k < num15; k++)
				{
					if (glyphInfo.rotatedUVs)
					{
						uvs.Add(glyphInfo.u0);
						uvs.Add(new Vector2(glyphInfo.u1.x, glyphInfo.u0.y));
						uvs.Add(glyphInfo.u1);
						uvs.Add(new Vector2(glyphInfo.u0.x, glyphInfo.u1.y));
					}
					else
					{
						uvs.Add(glyphInfo.u0);
						uvs.Add(new Vector2(glyphInfo.u0.x, glyphInfo.u1.y));
						uvs.Add(glyphInfo.u1);
						uvs.Add(new Vector2(glyphInfo.u1.x, glyphInfo.u0.y));
					}
				}
			}
			if (cols != null)
			{
				if (glyphInfo.channel == 0 || glyphInfo.channel == 15)
				{
					if (gradient)
					{
						float num16 = num8 + y / fontScale;
						float num17 = num8 + y2 / fontScale;
						num16 /= num8;
						num17 /= num8;
						s_c0 = Color.Lerp(a, b, num16);
						s_c1 = Color.Lerp(a, b, num17);
						int k = 0;
						for (int num15 = (!bold) ? 1 : 4; k < num15; k++)
						{
							cols.Add(s_c0);
							cols.Add(s_c1);
							cols.Add(s_c1);
							cols.Add(s_c0);
						}
					}
					else
					{
						int k = 0;
						for (int num15 = bold ? 16 : 4; k < num15; k++)
						{
							cols.Add(color);
						}
					}
				}
				else
				{
					Color c = color;
					c *= 0.49f;
					switch (glyphInfo.channel)
					{
					case 1:
						c.b += 0.51f;
						break;
					case 2:
						c.g += 0.51f;
						break;
					case 4:
						c.r += 0.51f;
						break;
					case 8:
						c.a += 0.51f;
						break;
					}
					Color32 item2 = c;
					int k = 0;
					for (int num15 = bold ? 16 : 4; k < num15; k++)
					{
						cols.Add(item2);
					}
				}
			}
			if (!bold)
			{
				if (!italic)
				{
					verts.Add(new Vector3(num10, num13));
					verts.Add(new Vector3(num10, num12));
					verts.Add(new Vector3(num11, num12));
					verts.Add(new Vector3(num11, num13));
				}
				else
				{
					float num18 = (float)fontSize * 0.1f * ((num12 - num13) / (float)fontSize);
					verts.Add(new Vector3(num10 - num18, num13));
					verts.Add(new Vector3(num10 + num18, num12));
					verts.Add(new Vector3(num11 + num18, num12));
					verts.Add(new Vector3(num11 - num18, num13));
				}
			}
			else
			{
				for (int k = 0; k < 4; k++)
				{
					float num19 = mBoldOffset[k * 2];
					float num20 = mBoldOffset[k * 2 + 1];
					float num18 = num19 + (italic ? ((float)fontSize * 0.1f * ((num12 - num13) / (float)fontSize)) : 0f);
					verts.Add(new Vector3(num10 - num18, num13 + num20));
					verts.Add(new Vector3(num10 + num18, num12 + num20));
					verts.Add(new Vector3(num11 + num18, num12 + num20));
					verts.Add(new Vector3(num11 - num18, num13 + num20));
				}
			}
			if (!underline && !strike)
			{
				continue;
			}
			GlyphInfo glyphInfo2 = GetGlyph(strike ? 45 : 95, prev);
			if (glyphInfo2 == null)
			{
				continue;
			}
			if (uvs != null)
			{
				if (bitmapFont != null)
				{
					glyphInfo2.u0.x = rect.xMin + num6 * glyphInfo2.u0.x;
					glyphInfo2.u1.x = rect.xMin + num6 * glyphInfo2.u1.x;
					glyphInfo2.u0.y = rect.yMax - num7 * glyphInfo2.u0.y;
					glyphInfo2.u1.y = rect.yMax - num7 * glyphInfo2.u1.y;
				}
				float x = (glyphInfo2.u0.x + glyphInfo2.u1.x) * 0.5f;
				float y3 = (glyphInfo2.u0.y + glyphInfo2.u1.y) * 0.5f;
				uvs.Add(new Vector2(x, y3));
				uvs.Add(new Vector2(x, y3));
				uvs.Add(new Vector2(x, y3));
				uvs.Add(new Vector2(x, y3));
			}
			if (flag && strike)
			{
				num13 = (0f - num3 + glyphInfo2.v0.y) * 0.75f;
				num12 = (0f - num3 + glyphInfo2.v1.y) * 0.75f;
			}
			else
			{
				num13 = 0f - num3 + glyphInfo2.v0.y;
				num12 = 0f - num3 + glyphInfo2.v1.y;
			}
			verts.Add(new Vector3(num9, num13));
			verts.Add(new Vector3(num9, num12));
			verts.Add(new Vector3(num2, num12));
			verts.Add(new Vector3(num2, num13));
			Color c2 = color;
			if (strike)
			{
				c2.r *= 0.5f;
				c2.g *= 0.5f;
				c2.b *= 0.5f;
			}
			c2.a *= 0.75f;
			Color32 item3 = c2;
			cols.Add(item3);
			cols.Add(color);
			cols.Add(color);
			cols.Add(item3);
		}
		if (alignment != Alignment.Left && size < verts.size)
		{
			Align(verts, size, num2 - finalSpacingX);
			size = verts.size;
		}
		mColors.Clear();
	}

	public static void PrintCharacterPositions(string text, BetterList<Vector3> verts, BetterList<int> indices)
	{
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		Prepare(text);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = (float)fontSize * fontScale * 0.5f;
		int length = text.Length;
		int size = verts.size;
		int num5 = 0;
		int prev = 0;
		for (int i = 0; i < length; i++)
		{
			num5 = text[i];
			verts.Add(new Vector3(num, 0f - num2 - num4));
			indices.Add(i);
			if (num5 == 10)
			{
				if (num > num3)
				{
					num3 = num;
				}
				if (alignment != Alignment.Left)
				{
					Align(verts, size, num - finalSpacingX);
					size = verts.size;
				}
				num = 0f;
				num2 += finalLineHeight;
				prev = 0;
				continue;
			}
			if (num5 < 32)
			{
				prev = 0;
				continue;
			}
			if (encoding && ParseSymbol(text, ref i))
			{
				i--;
				continue;
			}
			BMSymbol bMSymbol = useSymbols ? GetSymbol(text, i, length) : null;
			float glyphWidth;
			if (bMSymbol == null)
			{
				glyphWidth = GetGlyphWidth(num5, prev);
				if (glyphWidth == 0f)
				{
					continue;
				}
				glyphWidth += finalSpacingX;
				if (Mathf.RoundToInt(num + glyphWidth) > rectWidth)
				{
					if (num == 0f)
					{
						return;
					}
					if (alignment != Alignment.Left && size < verts.size)
					{
						Align(verts, size, num - finalSpacingX);
						size = verts.size;
					}
					num = glyphWidth;
					num2 += finalLineHeight;
				}
				else
				{
					num += glyphWidth;
				}
				verts.Add(new Vector3(num, 0f - num2 - num4));
				indices.Add(i + 1);
				prev = num5;
				continue;
			}
			glyphWidth = (float)bMSymbol.advance * fontScale + finalSpacingX;
			if (Mathf.RoundToInt(num + glyphWidth) > rectWidth)
			{
				if (num == 0f)
				{
					return;
				}
				if (alignment != Alignment.Left && size < verts.size)
				{
					Align(verts, size, num - finalSpacingX);
					size = verts.size;
				}
				num = glyphWidth;
				num2 += finalLineHeight;
			}
			else
			{
				num += glyphWidth;
			}
			verts.Add(new Vector3(num, 0f - num2 - num4));
			indices.Add(i + 1);
			i += bMSymbol.sequence.Length - 1;
			prev = 0;
		}
		if (alignment != Alignment.Left && size < verts.size)
		{
			Align(verts, size, num - finalSpacingX);
		}
	}

	public static void PrintCaretAndSelection(string text, int start, int end, BetterList<Vector3> caret, BetterList<Vector3> highlight)
	{
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		Prepare(text);
		int num = end;
		if (start > end)
		{
			end = start;
			start = num;
		}
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = (float)fontSize * fontScale;
		int indexOffset = (caret != null) ? caret.size : 0;
		int num6 = (highlight != null) ? highlight.size : 0;
		int length = text.Length;
		int i = 0;
		int num7 = 0;
		int prev = 0;
		bool flag = false;
		bool flag2 = false;
		Vector2 v = Vector2.zero;
		Vector2 v2 = Vector2.zero;
		for (; i < length; i++)
		{
			if (caret != null && !flag2 && num <= i)
			{
				flag2 = true;
				caret.Add(new Vector3(num2 - 1f, 0f - num3 - num5));
				caret.Add(new Vector3(num2 - 1f, 0f - num3));
				caret.Add(new Vector3(num2 + 1f, 0f - num3));
				caret.Add(new Vector3(num2 + 1f, 0f - num3 - num5));
			}
			num7 = text[i];
			if (num7 == 10)
			{
				if (num2 > num4)
				{
					num4 = num2;
				}
				if (caret != null && flag2)
				{
					if (alignment != Alignment.Left)
					{
						Align(caret, indexOffset, num2 - finalSpacingX);
					}
					caret = null;
				}
				if (highlight != null)
				{
					if (flag)
					{
						flag = false;
						highlight.Add(v2);
						highlight.Add(v);
					}
					else if (start <= i && end > i)
					{
						highlight.Add(new Vector3(num2, 0f - num3 - num5));
						highlight.Add(new Vector3(num2, 0f - num3));
						highlight.Add(new Vector3(num2 + 2f, 0f - num3));
						highlight.Add(new Vector3(num2 + 2f, 0f - num3 - num5));
					}
					if (alignment != Alignment.Left && num6 < highlight.size)
					{
						Align(highlight, num6, num2 - finalSpacingX);
						num6 = highlight.size;
					}
				}
				num2 = 0f;
				num3 += finalLineHeight;
				prev = 0;
				continue;
			}
			if (num7 < 32)
			{
				prev = 0;
				continue;
			}
			if (encoding && ParseSymbol(text, ref i))
			{
				i--;
				continue;
			}
			BMSymbol bMSymbol = useSymbols ? GetSymbol(text, i, length) : null;
			float num8 = (bMSymbol != null) ? ((float)bMSymbol.advance * fontScale) : GetGlyphWidth(num7, prev);
			if (num8 == 0f)
			{
				continue;
			}
			float num9 = num2;
			float num10 = num2 + num8;
			float num11 = 0f - num3 - num5;
			float num12 = 0f - num3;
			if (Mathf.RoundToInt(num10 + finalSpacingX) > rectWidth)
			{
				if (num2 == 0f)
				{
					return;
				}
				if (num2 > num4)
				{
					num4 = num2;
				}
				if (caret != null && flag2)
				{
					if (alignment != Alignment.Left)
					{
						Align(caret, indexOffset, num2 - finalSpacingX);
					}
					caret = null;
				}
				if (highlight != null)
				{
					if (flag)
					{
						flag = false;
						highlight.Add(v2);
						highlight.Add(v);
					}
					else if (start <= i && end > i)
					{
						highlight.Add(new Vector3(num2, 0f - num3 - num5));
						highlight.Add(new Vector3(num2, 0f - num3));
						highlight.Add(new Vector3(num2 + 2f, 0f - num3));
						highlight.Add(new Vector3(num2 + 2f, 0f - num3 - num5));
					}
					if (alignment != Alignment.Left && num6 < highlight.size)
					{
						Align(highlight, num6, num2 - finalSpacingX);
						num6 = highlight.size;
					}
				}
				num9 -= num2;
				num10 -= num2;
				num11 -= finalLineHeight;
				num12 -= finalLineHeight;
				num2 = 0f;
				num3 += finalLineHeight;
			}
			num2 += num8 + finalSpacingX;
			if (highlight != null)
			{
				if (start > i || end <= i)
				{
					if (flag)
					{
						flag = false;
						highlight.Add(v2);
						highlight.Add(v);
					}
				}
				else if (!flag)
				{
					flag = true;
					highlight.Add(new Vector3(num9, num11));
					highlight.Add(new Vector3(num9, num12));
				}
			}
			v = new Vector2(num10, num11);
			v2 = new Vector2(num10, num12);
			prev = num7;
		}
		if (caret != null)
		{
			if (!flag2)
			{
				caret.Add(new Vector3(num2 - 1f, 0f - num3 - num5));
				caret.Add(new Vector3(num2 - 1f, 0f - num3));
				caret.Add(new Vector3(num2 + 1f, 0f - num3));
				caret.Add(new Vector3(num2 + 1f, 0f - num3 - num5));
			}
			if (alignment != Alignment.Left)
			{
				Align(caret, indexOffset, num2 - finalSpacingX);
			}
		}
		if (highlight != null)
		{
			if (flag)
			{
				highlight.Add(v2);
				highlight.Add(v);
			}
			else if (start < i && end == i)
			{
				highlight.Add(new Vector3(num2, 0f - num3 - num5));
				highlight.Add(new Vector3(num2, 0f - num3));
				highlight.Add(new Vector3(num2 + 2f, 0f - num3));
				highlight.Add(new Vector3(num2 + 2f, 0f - num3 - num5));
			}
			if (alignment != Alignment.Left && num6 < highlight.size)
			{
				Align(highlight, num6, num2 - finalSpacingX);
			}
		}
	}
}
