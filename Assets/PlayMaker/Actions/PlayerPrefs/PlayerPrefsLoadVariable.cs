// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerPrefs")]
    [Tooltip("Load variable value saved with {{PlayerPrefs Save Variable}}. " +
             "The Key should be a unique identifier for the variable.\n" +
             "NOTE: You cannot save references to Scene Objects in PlayerPrefs!")]
    public class PlayerPrefsLoadVariable : FsmStateAction
	{
		[Tooltip("Case sensitive key.")]
		public FsmString key;

        [UIHint(UIHint.Variable)]
        [Tooltip("The variable to load.")]
        public FsmVar variable;

		public override void Reset()
        {
            key = null;
            variable = null;
        }

		public override void OnEnter()
		{
            if (!FsmString.IsNullOrEmpty(key) && !variable.IsNone)
            {
                // Get json saved with PlayerPrefsSaveVariable

                var json = PlayerPrefs.GetString(key.Value,"");
                if (json == "") // variable keeps current value
                {
                    Finish();
                    return; 
                }

                // PlayerPrefsSaveVariable saves and FsmVar
                // So we have type info to check

                var fsmVar = JsonUtility.FromJson<FsmVar>(json);

                if (fsmVar.Type == variable.Type &&
                    fsmVar.ObjectType == variable.ObjectType)
                {
                    fsmVar.ApplyValueTo(variable.NamedVar);
                }

                variable.NamedVar.Init();
            }

            Finish();
		}


#if UNITY_EDITOR
        public override string AutoName()
        {
            return "LoadVariable: " + ActionHelpers.GetValueLabel(variable.NamedVar);
        }
#endif
    }
}