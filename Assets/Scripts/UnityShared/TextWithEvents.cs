using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
public class TextWithEvents : Text
{
	[TextArea(3, 10)]
	public string nonParsedStr;

	public bool linksDefined;

	private StringBuilder sb = new StringBuilder();

	public Dictionary<string, List<int[]>> charsIdForClass = new Dictionary<string, List<int[]>>();

	public Dictionary<string, string[]> links = new Dictionary<string, string[]>();

	private List<HyperlinkButtonInfo> hyperlinkButtonInfoList = new List<HyperlinkButtonInfo>();

	private string[] splittedStr;

	private static Regex _regex = new Regex("<a href=([^>\\n\\s]+)\\s?>(.*?)(</a>)", RegexOptions.Singleline);

	public new string text
	{
		get
		{
			return base.text;
		}
		set
		{
			linksDefined = false;
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(text))
				{
					base.text = string.Empty;
					charsIdForClass.Clear();
					SetVerticesDirty();
				}
			}
			else if (base.text != value)
			{
				base.text = OnBeforeValueChange(value);
				SetAllDirty();
			}
		}
	}

	public void AddButtons()
	{
		foreach (Transform item in base.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (HyperlinkButtonInfo hyperlinkButtonInfo in hyperlinkButtonInfoList)
		{
			AddButton(hyperlinkButtonInfo);
		}
	}

	protected override void Awake()
	{
		base.text = OnBeforeValueChange(nonParsedStr);
	}

	private string OnBeforeValueChange(string strToParse)
	{
		strToParse = strToParse.Replace("<a", "<color=#0052AF><b><a");
		strToParse = strToParse.Replace("</a>", "</a></b></color>");
		strToParse = strToParse.Replace("\"", null);
		strToParse = strToParse.Replace("target=_blank ", null);
		splittedStr = _regex.Split(strToParse);
		int num = 0;
		sb.Length = 0;
		sb.EnsureCapacity(strToParse.Length);
		charsIdForClass.Clear();
		links.Clear();
		int num2 = 1;
		string[] array = splittedStr;
		foreach (string text in array)
		{
			if (num + 2 < splittedStr.Length && splittedStr[num + 2] == "</a>")
			{
				string key = "link" + num2++;
				links.Add(key, new string[2]
				{
					text,
					splittedStr[num + 1]
				});
				int[] item = new int[2]
				{
					sb.Length,
					sb.Length + splittedStr[num + 1].Length - 1
				};
				if (charsIdForClass.ContainsKey(key))
				{
					charsIdForClass[key].Add(item);
				}
				else
				{
					charsIdForClass.Add(key, new List<int[]>
					{
						item
					});
				}
			}
			else if (text != "</a>" && text != string.Empty)
			{
				sb.Append(text);
			}
			num++;
		}
		return HtmlStrip(sb.ToString());
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		hyperlinkButtonInfoList.Clear();
		base.OnPopulateMesh(vh);
		if (charsIdForClass.Count != 0)
		{
			TextGenerator textGenerator = base.cachedTextGenerator;
			if (textGenerator == null)
			{
				textGenerator = base.cachedTextGeneratorForLayout;
			}
			foreach (KeyValuePair<string, List<int[]>> item in charsIdForClass)
			{
				foreach (int[] item2 in item.Value)
				{
					if (textGenerator.lineCount > 1 && textGenerator.verts[item2[0] * 4 + 2].position.y > textGenerator.verts[(item2[1] * 4 + 3 > textGenerator.vertexCount - 1) ? (textGenerator.vertexCount - 4) : (item2[1] * 4)].position.y)
					{
						int num = 1;
						for (int i = 0; i < textGenerator.lineCount - 1; i++)
						{
							if (item2[0] > textGenerator.lines[i].startCharIdx && item2[0] < textGenerator.lines[i + 1].startCharIdx)
							{
								if (item2[0] * 4 + 3 < textGenerator.vertexCount && (textGenerator.lines[i + 1].startCharIdx - 1) * 4 + 1 < textGenerator.vertexCount)
								{
									hyperlinkButtonInfoList.Add(new HyperlinkButtonInfo(item.Key + num++, textGenerator.verts[item2[0] * 4].position.x, textGenerator.verts[item2[0] * 4 + 3].position.y, textGenerator.verts[(textGenerator.lines[i + 1].startCharIdx - 1) * 4 + 1].position.x - textGenerator.verts[item2[0] * 4].position.x, textGenerator.lines[i].height));
								}
							}
							else
							{
								if (item2[1] > textGenerator.lines[i].startCharIdx && item2[1] < textGenerator.lines[i + 1].startCharIdx)
								{
									if (item2[1] * 4 + 3 < textGenerator.vertexCount && textGenerator.lines[i].startCharIdx * 4 < textGenerator.vertexCount)
									{
										hyperlinkButtonInfoList.Add(new HyperlinkButtonInfo(item.Key + num++, textGenerator.verts[item2[1] * 4].position.x, textGenerator.verts[item2[1] * 4 + 3].position.y, textGenerator.verts[item2[1] * 4 + 1].position.x - textGenerator.verts[textGenerator.lines[i].startCharIdx * 4].position.x, textGenerator.lines[i].height));
									}
									break;
								}
								if (item2[0] < textGenerator.lines[i + 1].startCharIdx && textGenerator.lines[i].startCharIdx * 4 + 3 < textGenerator.vertexCount && textGenerator.lines[i].startCharIdx * 4 < textGenerator.vertexCount && (textGenerator.lines[i + 1].startCharIdx - 1) * 4 + 1 < textGenerator.vertexCount)
								{
									hyperlinkButtonInfoList.Add(new HyperlinkButtonInfo(item.Key + num++, textGenerator.verts[textGenerator.lines[i].startCharIdx * 4].position.x, textGenerator.verts[textGenerator.lines[i].startCharIdx * 4 + 3].position.y, textGenerator.verts[(textGenerator.lines[i + 1].startCharIdx - 1) * 4 + 1].position.x - textGenerator.verts[textGenerator.lines[i].startCharIdx * 4].position.x, textGenerator.lines[i].height));
								}
							}
							if (i == textGenerator.lineCount - 2)
							{
								if (item2[1] * 4 + 3 > textGenerator.vertexCount - 1)
								{
									if (textGenerator.lines[i + 1].startCharIdx * 4 < textGenerator.vertexCount && textGenerator.lines[i + 1].startCharIdx * 4 < textGenerator.vertexCount && textGenerator.vertexCount >= 5)
									{
										hyperlinkButtonInfoList.Add(new HyperlinkButtonInfo(item.Key + num++, textGenerator.verts[textGenerator.lines[i + 1].startCharIdx * 4].position.x, textGenerator.verts[textGenerator.vertexCount - 5].position.y, textGenerator.verts[textGenerator.vertexCount - 3].position.x - textGenerator.verts[textGenerator.lines[i + 1].startCharIdx * 4].position.x, textGenerator.lines[i + 1].height));
									}
								}
								else if (textGenerator.lines[i + 1].startCharIdx * 4 < textGenerator.vertexCount && item2[1] * 4 + 3 < textGenerator.vertexCount)
								{
									hyperlinkButtonInfoList.Add(new HyperlinkButtonInfo(item.Key + num++, textGenerator.verts[textGenerator.lines[i + 1].startCharIdx * 4].position.x, textGenerator.verts[item2[1] * 4 + 3].position.y, textGenerator.verts[item2[1] * 4 + 1].position.x - textGenerator.verts[textGenerator.lines[i + 1].startCharIdx * 4].position.x, textGenerator.lines[i + 1].height));
								}
							}
						}
					}
					else if (item2[1] * 4 + 3 > textGenerator.vertexCount - 1)
					{
						if (item2[0] * 4 + 3 < textGenerator.vertexCount && textGenerator.vertexCount >= 3)
						{
							hyperlinkButtonInfoList.Add(new HyperlinkButtonInfo(item.Key + 1, textGenerator.verts[item2[0] * 4].position.x, textGenerator.verts[item2[0] * 4 + 3].position.y, textGenerator.verts[textGenerator.vertexCount - 3].position.x - textGenerator.verts[item2[0] * 4].position.x, textGenerator.verts[item2[0] * 4].position.y - textGenerator.verts[item2[0] * 4 + 3].position.y));
						}
					}
					else if (item2[0] * 4 + 3 < textGenerator.vertexCount && item2[1] * 4 + 1 < textGenerator.vertexCount)
					{
						hyperlinkButtonInfoList.Add(new HyperlinkButtonInfo(item.Key + 1, textGenerator.verts[item2[0] * 4].position.x, textGenerator.verts[item2[0] * 4 + 3].position.y, textGenerator.verts[item2[1] * 4 + 1].position.x - textGenerator.verts[item2[0] * 4].position.x, textGenerator.verts[item2[0] * 4].position.y - textGenerator.verts[item2[0] * 4 + 3].position.y));
					}
				}
			}
			linksDefined = true;
		}
	}

	private void AddButton(HyperlinkButtonInfo buttonInfo)
	{
		float num = 1f / base.canvas.scaleFactor;
		float num2 = buttonInfo.W * num;
		float num3 = buttonInfo.H * num;
		float x = buttonInfo.X * num + num2 / 2f;
		float y = buttonInfo.Y * num + num3 / 2f;
		GameObject gameObject = new GameObject(buttonInfo.Name);
		gameObject.AddComponent<RectTransform>();
		gameObject.AddComponent<Image>();
		gameObject.AddComponent<Button>();
		gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
		gameObject.GetComponent<RectTransform>().SetParent(base.transform, false);
		gameObject.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0f);
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(num2, num3);
	}

	private string HtmlStrip(string input)
	{
		char[] trimChars = new char[3]
		{
			'"',
			' ',
			'\''
		};
		input = input.Trim(trimChars);
		input = Regex.Replace(input, "<style>(.|\n)*?</style>", string.Empty);
		input = Regex.Replace(input, "<xml>(.|\\n)*?</xml>", string.Empty);
		input = Regex.Replace(input, "<(ul|/li)>", string.Empty);
		input = Regex.Replace(input, "<(br|br/|br /|/p|/ul|/ol)>", "\n");
		input = Regex.Replace(input, "<(li)>", "\n  ● ");
		input = Regex.Replace(input, "<em>", "<i>");
		input = Regex.Replace(input, "</em>", "</i>");
		return input;
	}
}
