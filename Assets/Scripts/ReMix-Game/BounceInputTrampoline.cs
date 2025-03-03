using ClubPenguin;
using ClubPenguin.Actions;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using UnityEngine;

public class BounceInputTrampoline : MonoBehaviour
{
	private const float MIN_VELOCITY_THRESHOLD = 0.4f;

	private const float MIN_RECORD_THRESHOLD = 4f;

	private const float RECORD_MARKER_LERP_RATE = 7f;

	private const float FOLLOW_DIST = 3.2f;

	private const float OUT_OF_BOUNDS_DIST = 7.5f;

	private EventDispatcher eventDispatcher;

	private ImpulseAction impulseAction;

	private bool inputWindow;

	private float originalMagnitude;

	public float JumpMagnitudeIncrease = 0.5f;

	public ParticleSystem SuccessParticles;

	public ParticleSystem NewRecordParticles;

	public ParticleSystem FailParticles;

	public GameObject highJumpAltitudeMarker;

	private float highJumpAltitude;

	private Vector3 markerPosition;

	private bool recordSet;

	private Vector3 recordMarkerOffScreenPosition = new Vector3(0f, -100f, 0f);

	private float prevVelocity;

	private float totalHeights;

	private int totalJumps;

	public Transform contactTransform;

	private CharacterController charController;

	public TextMesh NameText;

	public TextMesh RecordText;

	public TextMesh RecordMarkerText;

	public TextMesh RecordMarkerShadowText;

	private GameObject playerObject;

	private void OnEnable()
	{
		Init();
	}

	private void OnDisable()
	{
		eventDispatcher.RemoveListener<InputEvents.ActionEvent>(onActionEvent);
		logRecordBI();
		resetRecord();
	}

	private void Init()
	{
		getReferences();
		setScreenDisplay();
		eventDispatcher.AddListener<InputEvents.ActionEvent>(onActionEvent);
	}

	private void LateUpdate()
	{
		if (!(charController != null))
		{
			return;
		}
		if (prevVelocity > 0f && charController.velocity.y < 0f)
		{
			trackHeight(charController.transform.position.y);
		}
		if (charController.velocity.y > 0f && charController.velocity.y < 0.4f)
		{
			float y = charController.transform.position.y;
			if (y > highJumpAltitude && y > 4f)
			{
				setNewRecord(y);
				setScreenDisplay();
			}
		}
		if (!isOutOfBounds(3.2f))
		{
			setMarkerPosition();
		}
		else if (isOutOfBounds(7.5f))
		{
			resetRecord();
		}
		prevVelocity = charController.velocity.y;
	}

	private void getReferences()
	{
		impulseAction = GetComponentInParent<ImpulseAction>();
		originalMagnitude = 8f;
		if (impulseAction != null)
		{
			originalMagnitude = impulseAction.Magnitude;
		}
		eventDispatcher = Service.Get<EventDispatcher>();
	}

	private void resetRecord()
	{
		recordSet = false;
		highJumpAltitude = 0f;
		impulseAction.Magnitude = originalMagnitude;
		if (highJumpAltitudeMarker != null)
		{
			highJumpAltitudeMarker.transform.position = recordMarkerOffScreenPosition;
		}
		RecordText.text = "";
		RecordMarkerText.text = "";
		RecordMarkerShadowText.text = "";
		NameText.text = "";
		setScreenDisplay();
	}

	private bool isOutOfBounds(float dist)
	{
		bool result = false;
		float num = Vector3.Distance(base.transform.position, new Vector3(charController.transform.position.x, base.transform.position.y, charController.transform.position.z));
		if (num > dist)
		{
			result = true;
		}
		return result;
	}

	private void setMarkerPosition()
	{
		if (recordSet)
		{
			Vector3 position = charController.transform.position;
			Vector3 b = new Vector3(position.x, markerPosition.y, position.z);
			if (highJumpAltitudeMarker != null)
			{
				highJumpAltitudeMarker.transform.position = Vector3.Lerp(highJumpAltitudeMarker.transform.position, b, Time.deltaTime * 7f);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (charController == null)
			{
				charController = other.GetComponent<CharacterController>();
			}
			playerObject = other.gameObject;
			inputWindow = true;
		}
	}

	private bool onActionEvent(InputEvents.ActionEvent evt)
	{
		if (evt.Action == InputEvents.Actions.Jump && !isTubing(playerObject))
		{
			onJumpInput();
		}
		return false;
	}

	private void onJumpInput()
	{
		if (!(charController != null))
		{
			return;
		}
		if (inputWindow && !isOutOfBounds(3.2f))
		{
			if (charController.velocity.y < 0f)
			{
				addMagnitude();
			}
			else
			{
				resetMagnitude();
			}
		}
		else
		{
			resetMagnitude();
		}
	}

	private void resetMagnitude()
	{
		if (impulseAction != null)
		{
			impulseAction.Magnitude = originalMagnitude;
		}
		if (FailParticles != null && !isOutOfBounds(3.05f))
		{
			EventManager.Instance.PostEvent("SFX/AO/TrampolineGame/BounceFail", EventAction.PlaySound);
			playSuccessParticle(false);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			inputWindow = false;
		}
	}

	private void addMagnitude()
	{
		if (impulseAction != null)
		{
			impulseAction.Magnitude += JumpMagnitudeIncrease;
		}
		if (SuccessParticles != null && !isOutOfBounds(3.2f))
		{
			EventManager.Instance.PostEvent("SFX/AO/TrampolineGame/BounceWin", EventAction.PlaySound);
			playSuccessParticle(true);
		}
	}

	private void setNewRecord(float altitude)
	{
		recordSet = true;
		highJumpAltitude = altitude;
		markerPosition = charController.transform.position;
		if (NewRecordParticles != null)
		{
			NewRecordParticles.transform.position = markerPosition;
			NewRecordParticles.Play();
		}
		EventManager.Instance.PostEvent("SFX/AO/TrampolineGame/NewRecord", EventAction.PlaySound);
	}

	private void setScreenDisplay()
	{
		CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
		DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
		DisplayNameData component;
		string text = (!localPlayerHandle.IsNull && cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component)) ? component.DisplayName : "editor";
		string text2 = "";
		text2 = ((!(highJumpAltitude >= 10f)) ? highJumpAltitude.ToString("0.00") : highJumpAltitude.ToString("00.00"));
		if (RecordText != null)
		{
			RecordText.text = text2;
		}
		if (RecordMarkerText != null && RecordMarkerShadowText != null)
		{
			RecordMarkerText.text = text2;
			RecordMarkerShadowText.text = text2;
		}
		if (NameText != null)
		{
			NameText.text = text;
		}
	}

	private void playSuccessParticle(bool success)
	{
		if (charController != null && contactTransform != null)
		{
			Vector3 position = charController.transform.position;
			position.y = contactTransform.position.y;
			if (success)
			{
				SuccessParticles.transform.position = position;
				SuccessParticles.Play();
			}
			else
			{
				FailParticles.transform.position = position;
				FailParticles.Play();
			}
		}
	}

	private void trackHeight(float height)
	{
		if (height > 0f)
		{
			totalHeights += height;
			totalJumps++;
		}
	}

	private void logRecordBI()
	{
		if (highJumpAltitude > 0f)
		{
			Service.Get<ICPSwrveService>().Action("trampoline", "max_height", ((int)highJumpAltitude).ToString());
		}
		if (totalJumps > 0)
		{
			int num = (int)(totalHeights / (float)totalJumps);
			Debug.Log("Height: " + num);
			Service.Get<ICPSwrveService>().Action("trampoline", "average_height", num.ToString());
		}
	}

	private bool isTubing(GameObject playerObj)
	{
		if (playerObj != null && LocomotionHelper.IsCurrentControllerOfType<SlideController>(playerObj))
		{
			return true;
		}
		return false;
	}
}
