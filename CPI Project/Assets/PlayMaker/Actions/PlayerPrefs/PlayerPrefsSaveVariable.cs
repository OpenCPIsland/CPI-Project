// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerPrefs")]
    [Tooltip("Save a variable value in PlayerPrefs. " +
             "You can load the value later with {{PlayerPrefs Load Variable}}.\n" +
             "NOTE: You cannot save references to Scene Objects in PlayerPrefs!")]
    public class PlayerPrefsSaveVariable : FsmStateAction
	{
		[Tooltip("Case sensitive key.")]
		public FsmString key;

        [UIHint(UIHint.Variable)]
        [Tooltip("The variable to save.")]
        public FsmVar variable;

		public override void Reset()
        {
            key = null;
            variable = null;
        }

		public override void OnEnter()
		{
            if (!FsmString.IsNullOrEmpty(key))
            {
                // Save variable as json string

                variable.UpdateValue();
                var json = JsonUtility.ToJson(variable);

                PlayerPrefs.SetString(key.Value, json);
                PlayerPrefs.Save();
            }

            Finish();
		}

#if UNITY_EDITOR
        public override string AutoName()
        {
            return "SaveVariable: " + ActionHelpers.GetValueLabel(variable.NamedVar);
        }
#endif
    }
}