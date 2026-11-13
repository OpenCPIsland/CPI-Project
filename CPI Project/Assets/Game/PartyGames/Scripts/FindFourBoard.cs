using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class FindFourBoard : MonoBehaviour
	{
		private const float WIN_EFFECT_START_DELAY = 1f;

		private const float WIN_EFFECT_DELAY = 0.2f;

		public PrefabContentKey RedToken;

		public PrefabContentKey BlueToken;

		public PrefabContentKey PlaceTokenEffect;

		public PrefabContentKey WinHighlightEffect;

		public GameObject CameraPosition;

		public GameObject CameraTarget;

		public GameObject StandaloneCameraPosition;

		public GameObject StandaloneCameraTarget;

		public GameObject TokenContainer;

		public GameObject Arrow;

		public GameObject HighlightBar;

		public Material RedMaterial;

		public Material BlueMaterial;

		public Vector2 GridSpacing;

		public float PlacementPositionHeight;

		public float TokenPlaceAnimTime = 0.4f;

		public iTween.EaseType TokenPlaceAnimEaseType = iTween.EaseType.easeOutBounce;

		public MouseEventsComponent[] ColumnColliders;

		[Space(10f)]
		public string MoveTokenSFXTrigger;

		public string Player1TokenDropSFXTrigger;

		public string Player2TokenDropSFXTrigger;

		public string VictorySequenceSFXTrigger;

		public string Player1VictorySequenceEndSFXTrigger;

		public string Player2VictorySequenceEndSFXTrigger;

		private GameObject redTokenPrefab;

		private GameObject blueTokenPrefab;

		private GameObject placeTokenEffectPrefab;

		private GameObject winHighlightEffectPrefab;

		private FindFourDefinition definition;

		private GameObject currentToken;

		private FindFour.FindFourTokenColor currentTokenColor;

		private int[,] grid;

		private int currentTokenColumn;

		private int currentMouseColumn;

		private int numRows;

		private int numColumns;

		private bool hasSetLocalPlayerColors;

		private List<Vector2> winningPositions;

		public int CurrentTokenColumn
		{
			get
			{
				return currentTokenColumn;
			}
		}

		public bool IsMouseActive
		{
			get;
			set;
		}

		public event Action<int> OnColumnClicked;

		private void Start()
		{
			winningPositions = new List<Vector2>();
			Content.LoadAsync(onRedTokenLoaded, RedToken);
			Content.LoadAsync(onBlueTokenLoaded, BlueToken);
			Content.LoadAsync(onPlaceTokenEffectLoaded, PlaceTokenEffect);
			Content.LoadAsync(onWinHighlightEffectLoaded, WinHighlightEffect);
			currentMouseColumn = -1;
			for (int i = 0; i < ColumnColliders.Length; i++)
			{
				int callbackIndex = i;
				ColumnColliders[i].OnMouseEnterEvent += delegate
				{
					onColumnColliderMouseEnter(callbackIndex);
				};
				ColumnColliders[i].OnMouseDownEvent += delegate
				{
					onColumnColliderMouseDown(callbackIndex);
				};
				ColumnColliders[i].OnMouseExitEvent += delegate
				{
					onColumnColliderMouseExit(callbackIndex);
				};
			}
			Arrow.SetActive(false);
			HighlightBar.SetActive(false);
		}

		public void Init(FindFourDefinition definition)
		{
			this.definition = definition;
			numColumns = definition.GameBoardWidth;
			numRows = definition.GameBoardHeight;
			grid = new int[numColumns, numRows];
		}

		private void onRedTokenLoaded(string path, GameObject prefab)
		{
			redTokenPrefab = prefab;
		}

		private void onBlueTokenLoaded(string path, GameObject prefab)
		{
			blueTokenPrefab = prefab;
		}

		private void onPlaceTokenEffectLoaded(string path, GameObject prefab)
		{
			placeTokenEffectPrefab = prefab;
		}

		private void onWinHighlightEffectLoaded(string path, GameObject prefab)
		{
			winHighlightEffectPrefab = prefab;
		}

		public void CreateNewToken(FindFour.FindFourTokenColor color, bool isLocalPlayer = false, int column = -1)
		{
			if (column == -1)
			{
				column = definition.DefaultColumn;
			}
			if (grid[column, numRows - 1] != 0)
			{
				column = findNextAvailableColumn(column, FindFour.FindFourMoveDirection.RIGHT, true);
			}
			GameObject original = redTokenPrefab;
			if (color == FindFour.FindFourTokenColor.BLUE)
			{
				original = blueTokenPrefab;
			}
			currentTokenColor = color;
			currentTokenColumn = column;
			if (isLocalPlayer && currentMouseColumn > -1 && isColumnAvailable(currentMouseColumn))
			{
				currentTokenColumn = currentMouseColumn;
			}
			currentToken = UnityEngine.Object.Instantiate(original, TokenContainer.transform, false);
			currentToken.transform.localPosition = new Vector3((float)currentTokenColumn * GridSpacing.x, PlacementPositionHeight, 0f);
			if (!hasSetLocalPlayerColors)
			{
				FindFour.FindFourTokenColor findFourTokenColor = (!isLocalPlayer) ? ((color == FindFour.FindFourTokenColor.RED) ? FindFour.FindFourTokenColor.BLUE : FindFour.FindFourTokenColor.RED) : color;
				Service.Get<EventDispatcher>().DispatchEvent(new FindFourEvents.ColorChanged(findFourTokenColor));
				Material material = (findFourTokenColor == FindFour.FindFourTokenColor.RED) ? RedMaterial : BlueMaterial;
				HighlightBar.GetComponentInChildren<MeshRenderer>().material = material;
				hasSetLocalPlayerColors = true;
			}
			if (isLocalPlayer)
			{
				HighlightBar.SetActive(true);
				updateHighlightBarAndArrow();
			}
		}

		public void MoveToken(FindFour.FindFourMoveDirection direction)
		{
			int num = findNextAvailableColumn(currentTokenColumn, direction);
			if (num >= 0 && num < numColumns)
			{
				setTokenColumn(num);
			}
			EventManager.Instance.PostEvent(MoveTokenSFXTrigger, EventAction.PlaySound);
		}

		private void setTokenColumn(int column)
		{
			currentToken.transform.localPosition = new Vector3((float)column * GridSpacing.x, PlacementPositionHeight, 0f);
			currentTokenColumn = column;
			updateHighlightBarAndArrow();
		}

		private int findNextAvailableColumn(int startColumn, FindFour.FindFourMoveDirection direction, bool wrap = false)
		{
			int num = (direction != FindFour.FindFourMoveDirection.LEFT) ? 1 : (-1);
			int num2 = startColumn + num;
			bool flag = false;
			if (num2 < 0 || num2 > numColumns - 1)
			{
				return startColumn;
			}
			while (!isColumnAvailable(num2))
			{
				num2 += num;
				if (num2 >= numColumns || num2 < 0)
				{
					if (!wrap || flag)
					{
						break;
					}
					num2 = startColumn;
					num = -num;
					flag = true;
				}
			}
			return num2;
		}

		private bool isColumnAvailable(int column)
		{
			return grid[column, numRows - 1] == 0;
		}

		private void updateHighlightBarAndArrow()
		{
			HighlightBar.transform.localPosition = new Vector3((float)currentTokenColumn * GridSpacing.x, 0f, 0f);
		}

		public void PlaceToken(int column)
		{
			Arrow.SetActive(false);
			HighlightBar.SetActive(false);
			currentToken.transform.localPosition = new Vector3((float)column * GridSpacing.x, PlacementPositionHeight, 0f);
			int num = 0;
			for (int i = 0; i < numRows; i++)
			{
				if (grid[column, i] == 0)
				{
					num = i;
					break;
				}
			}
			grid[column, num] = (int)(currentTokenColor + 1);
			Vector3 vector = new Vector3((float)column * GridSpacing.x, (float)num * GridSpacing.y, 0f);
			if (currentToken != null)
			{
				Hashtable args = iTween.Hash("name", "PlaceToken", "position", vector, "time", TokenPlaceAnimTime, "easetype", TokenPlaceAnimEaseType, "islocal", true, "oncomplete", "placeAnimComplete", "oncompletetarget", base.gameObject);
				iTween.MoveTo(currentToken, args);
			}
			if (checkForWin(column, num))
			{
				CoroutineRunner.Start(showWinEffects(), this, "ShowFindFourWinEffects");
			}
			currentTokenColumn = 0;
			if (currentTokenColor == FindFour.FindFourTokenColor.RED)
			{
				EventManager.Instance.PostEvent(Player1TokenDropSFXTrigger, EventAction.PlaySound);
			}
			else
			{
				EventManager.Instance.PostEvent(Player2TokenDropSFXTrigger, EventAction.PlaySound);
			}
		}

		private void placeAnimComplete()
		{
			UnityEngine.Object.Instantiate(placeTokenEffectPrefab, currentToken.transform.position, Quaternion.identity, TokenContainer.transform);
			currentToken = null;
		}

		private IEnumerator showWinEffects()
		{
			yield return new WaitForSeconds(1f);
			if (winningPositions[0].x != winningPositions[1].x)
			{
				winningPositions.Sort((Vector2 a, Vector2 b) => a.x.CompareTo(b.x));
			}
			else
			{
				winningPositions.Sort((Vector2 a, Vector2 b) => a.y.CompareTo(b.y));
			}
			for (int i = 0; i < winningPositions.Count; i++)
			{
				GameObject effect = UnityEngine.Object.Instantiate(winHighlightEffectPrefab, TokenContainer.transform, false);
				effect.transform.localPosition = new Vector3(winningPositions[i].x * GridSpacing.x, winningPositions[i].y * GridSpacing.y, 0f);
				string sfxTrigger = string.Format(VictorySequenceSFXTrigger, i + 1);
				if (i == winningPositions.Count - 1)
				{
					if (currentTokenColor == FindFour.FindFourTokenColor.RED)
					{
						EventManager.Instance.PostEvent(Player1VictorySequenceEndSFXTrigger, EventAction.PlaySound);
					}
					else
					{
						EventManager.Instance.PostEvent(Player2VictorySequenceEndSFXTrigger, EventAction.PlaySound);
					}
				}
				else
				{
					EventManager.Instance.PostEvent(sfxTrigger, EventAction.PlaySound);
				}
				yield return new WaitForSeconds(0.2f);
			}
		}

		private bool checkForWin(int column, int row)
		{
			bool result = false;
			if (getSequenceDiagonal1(column, row) >= definition.SequenceCountToWin)
			{
				result = true;
			}
			else if (getSequenceDiagonal2(column, row) >= definition.SequenceCountToWin)
			{
				result = true;
			}
			else if (getSequenceVertical(column, row) >= definition.SequenceCountToWin)
			{
				result = true;
			}
			else if (getSequenceHorizontal(column, row) >= definition.SequenceCountToWin)
			{
				result = true;
			}
			return result;
		}

		private int getSequenceDiagonal1(int column, int row)
		{
			winningPositions.Clear();
			winningPositions.Add(new Vector2(column, row));
			int num = 1;
			num += getSequenceCountRecursive(column + 1, row - 1, 1, -1, 0);
			return num + getSequenceCountRecursive(column - 1, row + 1, -1, 1, 0);
		}

		private int getSequenceDiagonal2(int column, int row)
		{
			winningPositions.Clear();
			winningPositions.Add(new Vector2(column, row));
			int num = 1;
			num += getSequenceCountRecursive(column - 1, row - 1, -1, -1, 0);
			return num + getSequenceCountRecursive(column + 1, row + 1, 1, 1, 0);
		}

		private int getSequenceVertical(int column, int row)
		{
			winningPositions.Clear();
			winningPositions.Add(new Vector2(column, row));
			int num = 1;
			num += getSequenceCountRecursive(column, row - 1, 0, -1, 0);
			return num + getSequenceCountRecursive(column, row + 1, 0, 1, 0);
		}

		private int getSequenceHorizontal(int column, int row)
		{
			winningPositions.Clear();
			winningPositions.Add(new Vector2(column, row));
			int num = 1;
			num += getSequenceCountRecursive(column - 1, row, -1, 0, 0);
			return num + getSequenceCountRecursive(column + 1, row, 1, 0, 0);
		}

		private int getSequenceCountRecursive(int column, int row, int xDelta, int yDelta, int sequenceCount)
		{
			if (row < 0 || column < 0 || row >= numRows || column >= numColumns)
			{
				return sequenceCount;
			}
			if (grid[column, row] == (int)(currentTokenColor + 1))
			{
				sequenceCount++;
				winningPositions.Add(new Vector2(column, row));
				return getSequenceCountRecursive(column + xDelta, row + yDelta, xDelta, yDelta, sequenceCount);
			}
			return sequenceCount;
		}

		private void onColumnColliderMouseEnter(int columnIndex)
		{
			currentMouseColumn = columnIndex;
			if (IsMouseActive && isColumnAvailable(columnIndex))
			{
				setTokenColumn(columnIndex);
				EventManager.Instance.PostEvent(MoveTokenSFXTrigger, EventAction.PlaySound);
			}
		}

		private void onColumnColliderMouseExit(int columnIndex)
		{
			if (currentMouseColumn == columnIndex)
			{
				currentMouseColumn = -1;
			}
		}

		private void onColumnColliderMouseDown(int columnIndex)
		{
			if (IsMouseActive && isColumnAvailable(columnIndex))
			{
				this.OnColumnClicked.InvokeSafe(columnIndex);
			}
		}

		private void OnDestroy()
		{
			for (int i = 0; i < ColumnColliders.Length; i++)
			{
				ColumnColliders[i].ClearListeners();
			}
		}
	}
}
