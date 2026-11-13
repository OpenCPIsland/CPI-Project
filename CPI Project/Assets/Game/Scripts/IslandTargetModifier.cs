using System.Collections;
using UnityEngine;

public class IslandTargetModifier : MonoBehaviour
{
	public enum ModifierType
	{
		None,
		SpinY,
		SpinX,
		HSlide,
		VSlide
	}

	public ModifierType _modifierType = ModifierType.None;

	public float _modifySpeed = 1f;

	public float _modifyDelay = 2f;

	public float _modifyDistance = 1f;

	private bool _beenModified = false;

	private Vector3 _startPosition = Vector3.zero;

	private Vector3 _startEuler = Vector3.zero;

	private IslandTarget _islandTarget = null;

	public void Init(IslandTarget islandTarget)
	{
		_islandTarget = islandTarget;
		_startPosition = base.transform.localPosition;
		_startEuler = base.transform.localEulerAngles;
		PlayModifierEffect();
	}

	public void PlayModifierEffect()
	{
		switch (_modifierType)
		{
		case ModifierType.None:
			break;
		case ModifierType.SpinX:
			SpinX();
			break;
		case ModifierType.SpinY:
			SpinY();
			break;
		case ModifierType.HSlide:
			SlideHoriontal();
			break;
		case ModifierType.VSlide:
			SlideVertical();
			break;
		}
	}

	private void OnModifierComplete()
	{
		StartCoroutine("DelayModifier");
	}

	private void OnDisableModifierComplete(bool enableTarget)
	{
		if (enableTarget)
		{
			_islandTarget.ChangeState(IslandTarget.TargetState.Ready);
		}
		StartCoroutine("DelayModifier");
	}

	private IEnumerator DelayModifier()
	{
		yield return new WaitForSeconds(_modifyDelay);
		PlayModifierEffect();
	}

	public void SpinX()
	{
		_islandTarget.ChangeState(IslandTarget.TargetState.Disabled);
		bool flag = false;
		Vector3 vector;
		if (!_beenModified)
		{
			_beenModified = true;
			vector = new Vector3(180f, 0f, 0f);
		}
		else
		{
			_beenModified = false;
			vector = _startEuler;
			flag = true;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("rotation", vector);
		hashtable.Add("speed", _modifySpeed);
		hashtable.Add("islocal", true);
		hashtable.Add("easetype", "easeOutBounce");
		hashtable.Add("oncompletetarget", base.gameObject);
		hashtable.Add("oncomplete", "OnDisableModifierComplete");
		hashtable.Add("oncompleteparams", flag);
		iTween.RotateTo(base.gameObject, hashtable);
	}

	public void SpinY()
	{
		_islandTarget.ChangeState(IslandTarget.TargetState.Disabled);
		bool flag = false;
		Vector3 vector;
		if (!_beenModified)
		{
			_beenModified = true;
			vector = base.transform.localEulerAngles + new Vector3(0f, 180f, 0f);
		}
		else
		{
			_beenModified = false;
			vector = _startEuler;
			flag = true;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("rotation", vector);
		hashtable.Add("speed", _modifySpeed);
		hashtable.Add("islocal", true);
		hashtable.Add("easetype", "easeOutBounce");
		hashtable.Add("oncompletetarget", base.gameObject);
		hashtable.Add("oncomplete", "OnDisableModifierComplete");
		hashtable.Add("oncompleteparams", flag);
		iTween.RotateTo(base.gameObject, hashtable);
	}

	public void SlideHoriontal()
	{
		Vector3 vector;
		if (!_beenModified)
		{
			_beenModified = true;
			vector = _startPosition + new Vector3(_modifyDistance, 0f, 0f);
		}
		else
		{
			_beenModified = false;
			vector = _startPosition;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("position", vector);
		hashtable.Add("islocal", true);
		hashtable.Add("speed", _modifySpeed);
		hashtable.Add("easetype", "easeInOutSine");
		hashtable.Add("oncompletetarget", base.gameObject);
		hashtable.Add("oncomplete", "OnModifierComplete");
		iTween.MoveTo(base.gameObject, hashtable);
	}

	public void SlideVertical()
	{
		Vector3 vector;
		if (!_beenModified)
		{
			_beenModified = true;
			vector = _startPosition + new Vector3(0f, _modifyDistance, 0f);
		}
		else
		{
			_beenModified = false;
			vector = _startPosition;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("position", vector);
		hashtable.Add("islocal", true);
		hashtable.Add("speed", _modifySpeed);
		hashtable.Add("easetype", "easeInOutSine");
		hashtable.Add("oncompletetarget", base.gameObject);
		hashtable.Add("oncomplete", "OnModifierComplete");
		iTween.MoveTo(base.gameObject, hashtable);
	}
}
