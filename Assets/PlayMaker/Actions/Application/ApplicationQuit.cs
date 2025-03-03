// (c) Copyright HutongGames, LLC. All rights reserved.

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Quits the player application.")]
	public class ApplicationQuit : FsmStateAction
    {

#if UNITY_2018_3_OR_NEWER
        [Tooltip("An optional exit code to return when the player application terminates on Windows, Mac and Linux. Defaults to 0.")]
        public FsmInt exitCode;

		public override void Reset()
        {
            exitCode = 0;
        }
#endif

		public override void OnEnter()
		{
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else

#if UNITY_2018_3_OR_NEWER
			Application.Quit(exitCode.Value);
#else
            Application.Quit();
#endif

#endif
			Finish();
		}
	}
}