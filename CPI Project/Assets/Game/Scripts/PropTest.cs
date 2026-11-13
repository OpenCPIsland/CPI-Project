using ClubPenguin.Props;
using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PropTest : MonoBehaviour
{
	public Prop Prop;

	public PropUser PropUser;

	public Transform RootBone;

	public Button UseButton;

	public Button RetrieveButton;

	public Button StoreButton;

	private Vector3 onUseDestination = new Vector3(0f, 0f, 3f);

	private void Awake()
	{
		UseButton.onClick.AddListener(UseProp);
		RetrieveButton.onClick.AddListener(RetrieveProp);
		StoreButton.onClick.AddListener(StoreProp);
		Prop.gameObject.SetActive(false);
		PropUser.EPropRetrieved += onPropRetrieved;
		PropUser.EPropUseCompleted += onUseCompleted;
		PropUser.EPropRemoved += onPropRemoved;
	}

	public void RetrieveProp()
	{
		CoroutineRunner.Start(retrieveProp(), this, "retrieveProp");
	}

	private IEnumerator retrieveProp()
	{
		if (Prop.UseOnceImmediately)
		{
			onUseDestination = onUseDestination.normalized * Prop.MaxDistanceFromUser;
			yield return CoroutineRunner.Start(PropUser.RetrievePropWithImmediateUseDest(Prop, onUseDestination), this, "PropUser.RetrievePropWithImmediateUseDest");
		}
		else
		{
			yield return CoroutineRunner.Start(PropUser.RetrieveProp(Prop), this, "PropUser.RetrieveProp");
		}
		Prop.gameObject.SetActive(true);
	}

	private void onPropRetrieved(Prop prop)
	{
	}

	public void UseProp()
	{
		onUseDestination = PropUser.transform.position + PropUser.transform.forward * Prop.MaxDistanceFromUser;
		Vector3 midPoint = PropUser.transform.position + (onUseDestination - PropUser.transform.position) * 0.5f;
		midPoint.y = 1f;
		Vector3 groundFromArc = GroundFinder.GetGroundFromArc(PropUser.transform.position, midPoint, onUseDestination);
		if (!PropUser.UsePropAtDestination(groundFromArc))
		{
		}
	}

	private void onUseCompleted(Prop prop)
	{
		if (!(PropUser.Prop != null))
		{
		}
	}

	public void StoreProp()
	{
		if (!PropUser.StoreProp())
		{
		}
	}

	private void onPropRemoved(Prop prop)
	{
		prop.gameObject.SetActive(false);
	}
}
