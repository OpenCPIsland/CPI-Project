// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Close a group started with {{GUILayoutBeginScrollView}}.")]
	public class GUILayoutEndScrollView : FsmStateAction
	{
		public override void OnGUI()
		{
			GUILayout.EndScrollView();
		}
	}
}