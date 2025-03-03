using ClubPenguin.Core;
using ClubPenguin.Props;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
    [ActionCategory("Props")]
    public class WaitForPlayerHeldItemChange : FsmStateAction
    {
        public FsmString ObjectId;
        public FsmEvent RetrievedEvent;

        private static DataEntityHandle handle;

        public override void OnEnter()
        {
            Prop component = base.Owner.GetComponent<Prop>();
            if (component != null)
            {
                var cpDataEntityCollection = Service.Get<CPDataEntityCollection>();
                if (cpDataEntityCollection != null)
                {
                    handle = cpDataEntityCollection.AddEntity("FishingRod");
                    if (!handle.IsNull)
                    {
                        var heldObjectsData = cpDataEntityCollection.GetComponent<HeldObjectsData>(handle);
                        if (heldObjectsData != null)
                        {
                            heldObjectsData.PlayerHeldObjectChanged += onPlayerHeldItemChanged;
                        }
                        else
                        {
                            Debug.LogError("HeldObjectsData is not available for the given handle.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to add entity or entity is null.");
                    }
                }
                else
                {
                    Debug.LogError("CPDataEntityCollection is not available.");
                }
            }
        }

        public override void OnExit()
        {
            if (!handle.IsNull)
            {
                var cpDataEntityCollection = Service.Get<CPDataEntityCollection>();
                var heldObjectsData = cpDataEntityCollection?.GetComponent<HeldObjectsData>(handle);
                if (heldObjectsData != null)
                {
                    heldObjectsData.PlayerHeldObjectChanged -= onPlayerHeldItemChanged;
                }
            }
        }

        private void onPlayerHeldItemChanged(DHeldObject heldObject)
        {
            ObjectId.Value = (heldObject != null) ? heldObject.ObjectId : "";
            base.Fsm.Event(RetrievedEvent);
        }
    }
}
