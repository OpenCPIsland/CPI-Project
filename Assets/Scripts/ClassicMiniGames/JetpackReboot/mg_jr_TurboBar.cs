using MinigameFramework;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace JetpackReboot
{
	public class mg_jr_TurboBar : MonoBehaviour
	{
		private GameObject m_disabledIconGO;

		private GameObject m_readyIconGO;

		private Button m_activationButton;

		private Sprite m_progrssMask;

		private Image m_progrssBar;

		private mg_jr_TurboDevice m_displaying;

		private mg_JetpackReboot m_miniGame;

		private bool m_lastTurboBlockedState = false;

		private void Awake()
		{
			m_miniGame = MinigameManager.GetActive<mg_JetpackReboot>();
		}

		public void LoadUIElement()
		{
			Image[] componentsInChildren = GetComponentsInChildren<Image>(true);
			Image[] array = componentsInChildren;
			foreach (Image image in array)
			{
				if (image.gameObject.name == "mg_jr_progressMask")
				{
					m_progrssBar = image;
					m_progrssMask = m_progrssBar.sprite;
				}
				else if (image.gameObject.name == "mg_jr_TurboReadyIcon")
				{
					m_readyIconGO = image.gameObject;
					m_activationButton = image.GetComponent<Button>();
				}
				else if (image.gameObject.name == "mg_jr_TurboDisabled")
				{
					m_disabledIconGO = image.gameObject;
				}
			}
			m_activationButton.onClick.AddListener(ActivateTurbo);
		}

		public void UnloadUIElement()
		{
			m_activationButton.onClick.RemoveListener(ActivateTurbo);
		}

		public void SetDisplayedDevice(mg_jr_TurboDevice _device)
		{
			if (m_displaying != null && m_displaying != _device)
			{
				UnRegisterEvents();
			}
			m_displaying = _device;
			if (m_displaying != null)
			{
				RegisterEvents();
			}
		}

		private void RegisterEvents()
		{
			m_displaying.PointsChanged += OnTurboFuelLevelChanged;
			m_displaying.StateChanged += OnTurboStateChanged;
			OnTurboStateChanged(m_displaying.DeviceState);
			OnTurboFuelLevelChanged(m_displaying.FuelPercentage);
			m_disabledIconGO.SetActive(!m_miniGame.GameLogic.IsTurboAllowed);
			m_lastTurboBlockedState = !m_miniGame.GameLogic.IsTurboAllowed;
		}

		private void UnRegisterEvents()
		{
			m_displaying.PointsChanged -= OnTurboFuelLevelChanged;
			m_displaying.StateChanged -= OnTurboStateChanged;
		}

		private void Update()
		{
			if (!MinigameManager.IsPaused)
			{
				UpdateTurboBlockedIcon(!m_miniGame.GameLogic.IsTurboAllowed);
				if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
				{
					ActivateTurbo();
				}
			}
		}

		private void UpdateTurboBlockedIcon(bool _isBlocked)
		{
			if (m_lastTurboBlockedState != _isBlocked)
			{
				m_lastTurboBlockedState = _isBlocked;
				m_disabledIconGO.SetActive(_isBlocked);
			}
		}

		private void ActivateTurbo()
		{
			if (m_miniGame.GameLogic != null && m_miniGame.GameLogic.IsTurboAllowed)
			{
				m_miniGame.GameLogic.ActivateTurboMode();
			}
		}

		public void OnTurboFuelLevelChanged(float _fuelLevel)
		{
			if (m_progrssMask != null)
			{
				Rect rect = new Rect(m_progrssMask.rect.x, m_progrssMask.rect.y, m_progrssMask.rect.width * _fuelLevel, m_progrssMask.rect.height);
				m_progrssBar.sprite = Sprite.Create(m_progrssMask.texture, rect, new Vector2(0f, 0.5f));
			}
		}

		public void OnTurboStateChanged(mg_jr_TurboDevice.TurboState _newState)
		{
			switch (_newState)
			{
			case mg_jr_TurboDevice.TurboState.INACTIVE:
				m_readyIconGO.SetActive(false);
				break;
			case mg_jr_TurboDevice.TurboState.FILLING:
				m_readyIconGO.SetActive(false);
				break;
			case mg_jr_TurboDevice.TurboState.FULL:
				m_readyIconGO.SetActive(true);
				break;
			case mg_jr_TurboDevice.TurboState.ENGAGED:
				m_readyIconGO.SetActive(false);
				break;
			default:
				Assert.IsTrue(false, "Unhandled tubo state in turbo ui");
				break;
			}
		}
	}
}
