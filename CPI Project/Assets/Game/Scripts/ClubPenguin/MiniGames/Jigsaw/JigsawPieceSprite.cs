using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.MiniGames.Jigsaw
{
	[RequireComponent(typeof(Button))]
	public class JigsawPieceSprite : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IEventSystemHandler
	{
		private static int poolID;

		private int id;

		private EventDispatcher dispatcher;

		private Button button;

		private Shadow shadow;

		private Vector2 shadowOffset = new Vector2(8f, -8f);

		private Vector2 pickupOffset;

		private Vector2 lockPosition;

		private float snapThreshold = 20f;

		private Vector3 originalScale;

		private float lowerScale = 0.6f;

		private float xMin;

		private float xMax;

		private float yMin;

		private float yMax;

		private float screenSafetyZone = 30f;

		private float dividerY;

		private Vector2 lowerOffset;

		public int Id
		{
			get
			{
				return id;
			}
		}

		private void Awake()
		{
			id = poolID++;
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<JigsawEventsSprite.Register>(onRegister);
			button = base.gameObject.GetComponent<Button>();
			shadow = base.gameObject.GetComponent<Shadow>();
			if (shadow == null)
			{
				shadow = base.gameObject.AddComponent<Shadow>();
				shadow.effectDistance = shadowOffset;
				Color effectColor = shadow.effectColor;
				effectColor.a = 0.5f;
				shadow.effectColor = effectColor;
			}
			shadow.enabled = false;
			if (base.gameObject.GetComponent<RaycastMask>() == null)
			{
				base.gameObject.AddComponent<RaycastMask>();
			}
		}

		private void Start()
		{
			RectTransform component = base.transform.parent.GetComponent<RectTransform>();
			if (component != null)
			{
				Vector3[] array = new Vector3[4];
				component.GetWorldCorners(array);
				xMin = array[0].x + screenSafetyZone;
				yMin = array[0].y + screenSafetyZone;
				xMax = array[2].x - screenSafetyZone;
				yMax = array[2].y - screenSafetyZone;
			}
			lockPosition = base.gameObject.transform.position;
			GameObject gameObject = base.gameObject.transform.parent.transform.Find("Divider").gameObject;
			dividerY = gameObject.transform.position.y;
			gameObject.SetActive(false);
			originalScale = base.gameObject.transform.localScale;
		}

		private void OnDestroy()
		{
			dispatcher.RemoveListener<JigsawEventsSprite.Register>(onRegister);
		}

		private bool onRegister(JigsawEventsSprite.Register e)
		{
			dispatcher.DispatchEvent(new JigsawEventsSprite.RegisterRespond(Id));
			return false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			shadow.enabled = true;
			base.gameObject.transform.position -= (Vector3)shadowOffset;
			pickupOffset = (Vector2)base.gameObject.transform.position - eventData.position;
			lowerOffset.x = pickupOffset.x * lowerScale;
			lowerOffset.y = pickupOffset.y * lowerScale;
		}

		public void OnDrag(PointerEventData eventData)
		{
			Vector2 zero = Vector2.zero;
			if (eventData.position.y > dividerY)
			{
				base.gameObject.transform.localScale = originalScale;
				zero = pickupOffset;
			}
			else
			{
				base.gameObject.transform.localScale = new Vector3(lowerScale, lowerScale, lowerScale);
				zero = lowerOffset;
			}
			base.gameObject.transform.position = eventData.position + zero;
			Vector2 v = base.gameObject.transform.position;
			if (v.x < xMin)
			{
				v.x = xMin;
			}
			else if (v.x > xMax)
			{
				v.x = xMax;
			}
			if (v.y < yMin)
			{
				v.y = yMin;
			}
			else if (v.y > yMax)
			{
				v.y = yMax;
			}
			base.gameObject.transform.position = v;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			shadow.enabled = false;
			pickupOffset = Vector2.zero;
			base.gameObject.transform.position += (Vector3)shadowOffset;
			float num = Vector2.Distance(lockPosition, base.gameObject.transform.position);
			if (num < snapThreshold)
			{
				base.gameObject.transform.position = lockPosition;
			}
		}

		private void setTransparency(float transparency)
		{
			transparency = Mathf.Clamp(transparency, 0f, 1f);
			Color color = button.image.color;
			color.a = transparency;
			button.image.color = color;
		}
	}
}
