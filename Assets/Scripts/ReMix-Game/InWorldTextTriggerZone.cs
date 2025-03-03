using ClubPenguin;
using ClubPenguin.Props;
using ClubPenguin.World;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

public class InWorldTextTriggerZone : MonoBehaviour
{
	private const float FULL_OPACITY_THRESHOLD = 0.5f;

	[LocalizationToken]
	public string displayString = "";

	public ScriptableObject attachedDefinition = null;

	public Sprite displaySprite = null;

	public Vector3 positionOffset = Vector3.zero;

	public Vector3 rotationOffset = Vector3.zero;

	private Transform epicenter = null;

	private bool hasString = false;

	private bool hasSprite = false;

	private Text textUI = null;

	private Image imageUI = null;

	private RectTransform rectTrans = null;

	private bool isInTrigger = false;

	private Transform penguinTransform = null;

	private float enterDist = 0f;

	private bool isDisabled = false;

	private void Awake()
	{
		InitEpicenter();
		InitDisplays();
		Service.Get<EventDispatcher>().AddListener<InWorldUIEvents.DisableInWorldText>(onInWorldTextDisabled);
		Service.Get<EventDispatcher>().AddListener<InWorldUIEvents.EnableInWorldText>(onInWorldTextEnabled);
	}

	private void OnDestroy()
	{
		Service.Get<EventDispatcher>().RemoveListener<InWorldUIEvents.DisableInWorldText>(onInWorldTextDisabled);
		Service.Get<EventDispatcher>().RemoveListener<InWorldUIEvents.EnableInWorldText>(onInWorldTextEnabled);
	}

	private void Update()
	{
		if (isInTrigger && rectTrans != null)
		{
			UpdatePosition();
			UpdateRotation();
			FadeDisplaysBasedOnDistance();
		}
	}

	private void InitEpicenter()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == "Epicenter")
			{
				epicenter = transform;
			}
			if (epicenter != null)
			{
				break;
			}
		}
	}

	private void InitDisplays()
	{
		rectTrans = GetComponentInChildren<RectTransform>();
		textUI = GetComponentInChildren<Text>();
		imageUI = GetComponentInChildren<Image>();
		if (displayString != "" || attachedDefinition != null)
		{
			hasString = true;
		}
		if (displaySprite != null)
		{
			hasSprite = true;
		}
		if (textUI != null && hasString)
		{
			if (!string.IsNullOrEmpty(displayString))
			{
				textUI.text = Service.Get<Localizer>().GetTokenTranslation(displayString);
			}
			else
			{
				bool flag = false;
				ZoneDefinition zoneDefinition = attachedDefinition as ZoneDefinition;
				if (zoneDefinition != null)
				{
					textUI.text = Service.Get<Localizer>().GetTokenTranslation(zoneDefinition.ZoneToken);
					flag = true;
				}
				if (!flag)
				{
					PropDefinition propDefinition = attachedDefinition as PropDefinition;
					if (propDefinition != null)
					{
						textUI.text = Service.Get<Localizer>().GetTokenTranslation(propDefinition.Name);
						flag = true;
					}
				}
				if (flag)
				{
				}
			}
		}
		if (imageUI != null && hasSprite)
		{
			imageUI.sprite = displaySprite;
		}
		EnableDisplays(false);
	}

	private void UpdatePosition()
	{
		rectTrans.localPosition = positionOffset;
	}

	private void UpdateRotation()
	{
		rectTrans.localEulerAngles = rotationOffset;
	}

	private void EnableDisplays(bool status)
	{
		if (!isDisabled)
		{
			if (textUI != null)
			{
				textUI.gameObject.SetActive(status);
			}
			if (imageUI != null)
			{
				imageUI.gameObject.SetActive(status);
			}
		}
	}

	private void FadeDisplaysBasedOnDistance()
	{
		if (penguinTransform != null && epicenter != null)
		{
			float num = Vector3.Distance(penguinTransform.position, epicenter.position);
			float num2 = 1f - num / enterDist;
			num2 *= 2f;
			Color color = textUI.color;
			Color color2 = imageUI.color;
			color.a = (hasString ? num2 : 0f);
			color2.a = (hasSprite ? num2 : 0f);
			textUI.color = color;
			imageUI.color = color2;
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag("Player") && epicenter != null)
		{
			isInTrigger = true;
			penguinTransform = col.GetComponent<Transform>();
			enterDist = Vector3.Distance(penguinTransform.position, epicenter.position);
			EnableDisplays(true);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.CompareTag("Player"))
		{
			isInTrigger = false;
			EnableDisplays(false);
		}
	}

	public bool onInWorldTextDisabled(InWorldUIEvents.DisableInWorldText evt)
	{
		EnableDisplays(false);
		isDisabled = true;
		return false;
	}

	public bool onInWorldTextEnabled(InWorldUIEvents.EnableInWorldText evt)
	{
		isDisabled = false;
		if (isInTrigger)
		{
			EnableDisplays(true);
		}
		return false;
	}
}
