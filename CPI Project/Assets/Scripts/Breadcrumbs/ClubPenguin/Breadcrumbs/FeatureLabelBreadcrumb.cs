using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Breadcrumbs
{
	public class FeatureLabelBreadcrumb : AbstractNotificationBreadcrumb
	{
		private const string FEATURE_LABEL_BREADCRUMB_SEEN_KEY_PREFIX = "seen_feature_label_breadcrumb_";

		public FeatureLabelBreadcrumbDefinition FeatureLabelBreadcrumbDef;

		[Header("Specify the Feature Label's Id and Type if it's definition is unknown")]
		public string TypeId;

		public PersistentBreadcrumbTypeDefinition Type;

		private void OnEnable()
		{
			SetBreadcrumbVisibility();
		}

		public void SetBreadcrumbVisibility()
		{
			if (FeatureLabelBreadcrumbDef == null)
			{
				if (string.IsNullOrEmpty(TypeId) || Type == null)
				{
					hideBreadcrumb();
					return;
				}
				FeatureLabelBreadcrumbDefinition.DictionaryKey key = new FeatureLabelBreadcrumbDefinition.DictionaryKey(TypeId, Type);
				if (!Service.Get<NotificationBreadcrumbController>().AvailableFeatureLabelBreadcrumbs.TryGetValue(key, out FeatureLabelBreadcrumbDef))
				{
					hideBreadcrumb();
					return;
				}
			}
			else if (!Service.Get<NotificationBreadcrumbController>().AvailableFeatureLabelBreadcrumbs.ContainsValue(FeatureLabelBreadcrumbDef))
			{
				hideBreadcrumb();
				return;
			}
			if (!isFeatureLabelBreadcrumbSeen(FeatureLabelBreadcrumbDef))
			{
				string featureLabelBreadcrumbSeenPlayerPrefsKey = getFeatureLabelBreadcrumbSeenPlayerPrefsKey(FeatureLabelBreadcrumbDef);
				PlayerPrefs.SetInt(featureLabelBreadcrumbSeenPlayerPrefsKey, 1);
				showBreadcrumb();
			}
			else
			{
				hideBreadcrumb();
			}
		}

		protected override void init()
		{
		}

		protected override void onBreadcrumbAdded(string breadcrumbId, int count)
		{
		}

		protected override void onBreadcrumbRemoved(string breadcrumbId, int count)
		{
		}

		protected override void onBreadcrumbReset(string breadcrumbId)
		{
		}

		private bool isFeatureLabelBreadcrumbSeen(FeatureLabelBreadcrumbDefinition featureLabelBreadcrumbDef)
		{
			if (featureLabelBreadcrumbDef.DependentFeatureLabelBreadcrumbs == null || featureLabelBreadcrumbDef.DependentFeatureLabelBreadcrumbs.Length == 0)
			{
				string featureLabelBreadcrumbSeenPlayerPrefsKey = getFeatureLabelBreadcrumbSeenPlayerPrefsKey(featureLabelBreadcrumbDef);
				return PlayerPrefs.GetInt(featureLabelBreadcrumbSeenPlayerPrefsKey) == 1;
			}
			for (int i = 0; i < featureLabelBreadcrumbDef.DependentFeatureLabelBreadcrumbs.Length; i++)
			{
				if (!isFeatureLabelBreadcrumbSeen(featureLabelBreadcrumbDef.DependentFeatureLabelBreadcrumbs[i]))
				{
					return false;
				}
			}
			return true;
		}

		private string getFeatureLabelBreadcrumbSeenPlayerPrefsKey(FeatureLabelBreadcrumbDefinition definition)
		{
			return "seen_feature_label_breadcrumb_" + definition.Type.Type + "_" + definition.TypeId;
		}
	}
}
