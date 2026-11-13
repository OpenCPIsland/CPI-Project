using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_GameLogic : MonoBehaviour
	{
		private const int MAX_LIVES = 4;

		private const float MOVEMENT_MULTIPLIER = 2f;

		protected mg_BeanCounter m_minigame;

		private GameObject m_background;

		private int m_draggingFinger = -1;

		private bool m_didDrag;

		protected mg_bc_EGameState m_state;

		private Vector3 lastWorldPosition;

		public mg_bc_Penguin Penguin
		{
			get;
			private set;
		}

		public mg_bc_DropZone DropZone
		{
			get;
			private set;
		}

		public mg_bc_Truck Truck
		{
			get;
			private set;
		}

		public mg_bc_WarningUIScript HintDisplayer
		{
			get;
			set;
		}

		public mg_bc_INoticeDisplayer NoticeDisplayer
		{
			get;
			set;
		}

		public mg_bc_ScoreController ScoreController
		{
			get;
			protected set;
		}

		public bool DidWin
		{
			get;
			private set;
		}

		public virtual void Awake()
		{
			m_state = mg_bc_EGameState.STATE_RUNNING;
			m_minigame = MinigameManager.GetActive<mg_BeanCounter>();
			m_minigame.SetLogic(this);
		}

		public void Initialize()
		{
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			if (!active.Resources.HasLoadedGameSounds)
			{
				active.Resources.LoadGameSounds();
			}
			m_background = active.Resources.GetInstancedResource(mg_bc_EResourceList.GAME_ASSET_BACKGROUND);
			MinigameSpriteHelper.SetSpriteLayer(m_background, -100);
			Vector3 localScale = m_background.transform.localScale;
			MinigameSpriteHelper.FitSpriteToScreen(active.MainCamera, m_background, false);
			MinigameSpriteHelper.AssignParent(m_background, base.gameObject);
			base.gameObject.transform.localScale = m_background.transform.localScale;
			m_background.transform.localScale = localScale;
			GameObject gameObject = base.gameObject.transform.Find("mg_bc_penguin").gameObject;
			Penguin = gameObject.GetComponent<mg_bc_Penguin>();
			Penguin.GameLogic = this;
			GameObject gameObject2 = base.gameObject.transform.Find("mg_bc_dropzone").gameObject;
			DropZone = gameObject2.GetComponent<mg_bc_DropZone>();
			GameObject gameObject3 = base.gameObject.transform.Find("mg_bc_truck").gameObject;
			Truck = gameObject3.GetComponent<mg_bc_Truck>();
			Truck.CanSpawnLives = (Penguin.Lives.Value < 4);
			GameObject gameObject4 = base.gameObject.transform.Find("mg_bc_left_edge").gameObject;
			GameObject gameObject5 = base.gameObject.transform.Find("mg_bc_right_edge").gameObject;
			Penguin.SetMovementLimits(gameObject4.transform.localPosition.x, gameObject5.transform.localPosition.x);
			ScoreController.Initialize(Truck);
		}

		public virtual void MinigameUpdate(float _deltaTime)
		{
			if (m_state != mg_bc_EGameState.STATE_GAME_OVER)
			{
				if (NoticeDisplayer != null)
				{
					NoticeDisplayer.NoticeUpdate(_deltaTime);
				}
				if (HintDisplayer != null)
				{
					HintDisplayer.HintUpdate(_deltaTime);
				}
				Penguin.PenguinUpdate(_deltaTime);
				Truck.TruckUpdate(_deltaTime);
				DropZone.DropUpdate(_deltaTime);
			}
		}

		public void OnTouchDrag(Gesture gesture)
		{
			if (m_state == mg_bc_EGameState.STATE_RUNNING && m_draggingFinger == gesture.fingerIndex)
			{
				m_didDrag = true;
				Vector3 vector = Camera.main.ScreenToWorldPoint(gesture.position);
				float x = vector.x - lastWorldPosition.x;
				Penguin.GetComponent<mg_bc_Penguin>().Move(x);
				lastWorldPosition = Camera.main.ScreenToWorldPoint(gesture.position);
			}
		}

		internal void OnMouseMove(Vector3 mousePosition)
		{
			if (m_state == mg_bc_EGameState.STATE_RUNNING)
			{
				m_didDrag = true;
				Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
				worldPoint.z = 0f;
				Penguin.GetComponent<mg_bc_Penguin>().MoveTo(worldPoint);
			}
		}

		public void OnTouchPress(bool _isDown, Gesture gesture)
		{
			bool flag = true;
			if (_isDown && m_draggingFinger == -1)
			{
				m_draggingFinger = gesture.fingerIndex;
				m_didDrag = false;
				lastWorldPosition = Camera.main.ScreenToWorldPoint(gesture.position);
			}
			else if (gesture.fingerIndex == m_draggingFinger && !_isDown)
			{
				m_draggingFinger = -1;
				flag = !m_didDrag;
			}
			if (!_isDown && m_state == mg_bc_EGameState.STATE_RUNNING && flag)
			{
				ThrowBagsAndCheckForComplete();
			}
		}

		private void ThrowBagsAndCheckForComplete()
		{
			Truck.TakeBag();
			DropZone.TakeBag();
			if (DropZone.StoredBags >= Truck.Goal())
			{
				if (Truck.NextLevel())
				{
					OnRoundEnd();
				}
				else
				{
					GameOver(true);
				}
			}
		}

		internal void OnMouseDown()
		{
			if (m_state == mg_bc_EGameState.STATE_RUNNING)
			{
				ThrowBagsAndCheckForComplete();
			}
		}

		private void OnRoundEnd()
		{
			NoticeDisplayer.ShowMessage("Truck Unloaded!", 1.5f);
			Penguin.DisableCollisions();
			Penguin.ClearBags();
			DropZone.ClearBags();
			m_state = mg_bc_EGameState.STATE_ROUND_TRANSITION;
			Truck.StartRoundTransition(OnRoundTransitionHalf, OnRoundTransitionEnded);
		}

		private void OnRoundTransitionHalf()
		{
			NoticeDisplayer.ShowMessage("Next Truck!", 1.5f);
		}

		private void OnRoundTransitionEnded()
		{
			Penguin.EnableCollisions();
			m_state = mg_bc_EGameState.STATE_RUNNING;
		}

		private void GameOver(bool _didWin)
		{
			DidWin = _didWin;
			ScoreController.OnGameOver(DidWin);
			m_state = mg_bc_EGameState.STATE_GAME_OVER;
			Truck.DestroyAll();
			MinigameManager.GetActive().PlaySFX("mg_bc_sfx_UIGameOver");
			UIManager.Instance.OpenScreen("mg_bc_ResultScreen", false, null, null);
		}

		internal void OnPenguinDeath()
		{
			Truck.IsSpawning = false;
			Truck.CanSpawnLives = (Penguin.Lives.Value < 4);
			if (Penguin.Lives.Value <= 0)
			{
				GameOver(false);
			}
		}

		internal void OnPenguinGainLife()
		{
			Truck.CanSpawnLives = (Penguin.Lives.Value < 4);
		}

		internal void OnPenguinRevive()
		{
			Truck.IsSpawning = true;
			Truck.CanSpawnLives = (Penguin.Lives.Value < 4);
		}

		internal void OnDropAllowanceExceeded()
		{
			Penguin.OnDropAllowanceExceeded();
		}

		internal virtual void OnObjectDropped(mg_bc_FlyingObject _groundedObject)
		{
			if (_groundedObject is mg_bc_Bag && !Penguin.IsDead && Truck.DeductDropAllowance() > 0)
			{
				OnDropAllowanceExceeded();
			}
		}

		internal void ShowHint(string _hint, float _duration)
		{
			if (HintDisplayer != null && !Penguin.IsDead && !Truck.IsExiting)
			{
				MinigameManager.GetActive().PlaySFX("mg_bc_sfx_UIHintPopUp");
				HintDisplayer.ShowMessage(_hint, _duration);
			}
		}

		public int GetGoalProgress()
		{
			return DropZone.StoredBags;
		}

		public int GetGoal()
		{
			return Truck.Goal();
		}
	}
}
