using HutongGames.PlayMaker;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof (PlayMaker.Actions.RunFSM))]
	public class RunFsmEditor : CustomActionEditor
    {
        private PlayMaker.Actions.RunFSM runFSM;
        private FsmTemplateControl fsmTemplateControl;

        public override void OnEnable()
        {
	        runFSM = target as PlayMaker.Actions.RunFSM;
	        fsmTemplateControl = runFSM.fsmTemplateControl;

            fsmTemplateControl.Reinitialize();
	    }

	    public override bool OnGUI()
	    {
	        return DrawDefaultInspector();
	    }
	}
}