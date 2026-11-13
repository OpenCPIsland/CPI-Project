using ClubPenguin.ClothingDesigner;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Tubes;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
    public class ChatActivityService : AbstractDataModelService
    {
        private ChatActivityData chatActivityData;

        private EventDispatcher dispatcher;

        private const string ERROR_TEXT = "Error! That ID does not exist.";
        private const string GRANTED_TUBE = "Granted Tube.";
        private const string GRANTED_EQUIPMENT = "Granted Equipment.";

        private void Start()
        {
            dispatcher = Service.Get<EventDispatcher>();
            DataEntityHandle handle = dataEntityCollection.AddEntity("ChatActivity");
            chatActivityData = dataEntityCollection.AddComponent<ChatActivityData>(handle);
            ChatActivityData obj = chatActivityData;
            obj.OnTimeOutComplete = (System.Action)Delegate.Combine(obj.OnTimeOutComplete, new System.Action(onTimeOutComplete));
            ChatActivityData obj2 = chatActivityData;
            obj2.SendChatActivity = (System.Action)Delegate.Combine(obj2.SendChatActivity, new System.Action(onSendChatActivity));
            dispatcher.AddListener<ChatActivityServiceEvents.SendChatActivity>(onSendChatActivity);
            dispatcher.AddListener<ChatActivityServiceEvents.SendChatActivityCancel>(onSendChatActivityCancel);
            dispatcher.AddListener<ChatMessageSender.SendChatMessage>(onSendChatMessage, EventDispatcher.Priority.FIRST);
        }

        private bool onSendChatActivity(ChatActivityServiceEvents.SendChatActivity evt)
        {
            if (!chatActivityData.IsChatActive)
            {
                dispatcher.DispatchEvent(default(ChatServiceEvents.SendChatActivity));
            }
            chatActivityData.OnSendChatActivity();
            return true;
        }

        private bool onSendChatActivityCancel(ChatActivityServiceEvents.SendChatActivityCancel evt)
        {
            if (chatActivityData.IsChatActive)
            {
                dispatcher.DispatchEvent(default(ChatServiceEvents.SendChatActivityCancel));
            }
            chatActivityData.OnSetChatActiveCancel();
            return true;
        }

        private bool onSendChatMessage(ChatMessageSender.SendChatMessage evt)
        {
            string message = evt.Message != null ? evt.Message.Trim() : "";
            if (message.StartsWith("!"))
            {
                return handleCommand(message);
            }

            if (chatActivityData.IsChatActive)
            {
                dispatcher.DispatchEvent(default(ChatServiceEvents.SendChatActivityCancel));
            }
            chatActivityData.OnSendChatMessage();
            return false;
        }

        private bool handleCommand(string message)
        {
            string[] parts = message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string command = parts[0].ToLower();

            switch (command)
            {
                case "!ac":
                    handleAddCoins(parts);
                    break;
                case "!ae":
                    handleAddItem(parts);
                    break;
                case "!at":
                    handleAddTube(parts);
                    break;
                default:
                    postSystemChatError();
                    break;
            }
            return true;
        }

        private void postSystemChat(string text, bool isError)
        {
            if (dispatcher == null || dataEntityCollection == null)
            {
                return;
            }

            StartCoroutine(postSystemChatRoutine(text, isError));
        }

        private IEnumerator postSystemChatRoutine(string text, bool isError)
        {
            if (dispatcher == null || dataEntityCollection == null)
            {
                yield break;
            }

            string localOnly;
            if (isError)
            {
                localOnly = new string(new char[] { WorldSpeechBubble.SystemMessagePrefix, WorldSpeechBubble.SystemErrorPrefix }) + text;
            }
            else
            {
                localOnly = new string(new char[] { WorldSpeechBubble.SystemMessagePrefix }) + text;
            }

            dispatcher.DispatchEvent(new ChatMessageSender.SendChatMessage(localOnly, null, false));
            yield return null;

            long sessionId = dataEntityCollection.LocalPlayerSessionId;
            dispatcher.DispatchEvent(new ChatServiceEvents.ChatMessageReceived(sessionId, localOnly, 0));
        }

        private void postSystemChatError()
        {
            postSystemChat(ERROR_TEXT, true);
        }

        private void postSystemChatGrantedTube()
        {
            postSystemChat(GRANTED_TUBE, false);
        }

        private void postSystemChatGrantedEquipment()
        {
            postSystemChat(GRANTED_EQUIPMENT, false);
        }

        private void handleAddCoins(string[] parts)
        {
            if (parts.Length < 2)
            {
                postSystemChatError();
                return;
            }

            int amount;
            if (!int.TryParse(parts[1], out amount) || amount <= 0)
            {
                postSystemChatError();
                return;
            }

            CoinsData coinsData;
            if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out coinsData))
            {
                QARewards.AddCoinsToAccount(amount);
            }
            else
            {
                postSystemChatError();
            }
        }

        private void handleAddItem(string[] parts)
        {
            if (parts.Length < 2)
            {
                postSystemChatError();
                return;
            }

            int definitionId;
            if (!int.TryParse(parts[1], out definitionId))
            {
                postSystemChatError();
                return;
            }

            AddEquipmentToInventory(definitionId);
        }

        public void AddEquipmentToInventory(int definitionId)
        {
            addEquipmentToInventory(definitionId, true);
        }

        public void AddEquipmentFromTweaker(int definitionId)
        {
            addEquipmentToInventory(definitionId, false);
        }

        private void addEquipmentToInventory(int definitionId, bool notifyChat)
        {
            Dictionary<int, TemplateDefinition> templates = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
            TemplateDefinition templateDef;
            if (templates == null || !templates.TryGetValue(definitionId, out templateDef) || templateDef == null)
            {
                reportGrantError(notifyChat, "The requested template is unavailable.");
                return;
            }

            Reward reward = new Reward();
            reward.Add(new EquipmentInstanceReward(templateDef.Id));
            Service.Get<INetworkServicesManager>().RewardService.QA_SetReward(reward);
            if (notifyChat)
            {
                postSystemChatGrantedEquipment();
            }
        }

        public void AddAllEquipmentToInventory()
        {
            startAddingAllEquipment(true);
        }

        public void AddAllEquipmentFromTweaker()
        {
            startAddingAllEquipment(false);
        }

        private void startAddingAllEquipment(bool notifyChat)
        {
            Dictionary<int, TemplateDefinition> templates = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
            if (templates == null || templates.Count == 0)
            {
                reportGrantError(notifyChat, "No templates are available.");
                return;
            }

            Reward reward = new Reward();
            foreach (TemplateDefinition template in templates.Values)
            {
                if (template != null)
                {
                    reward.Add(new EquipmentInstanceReward(template.Id));
                }
            }

            if (reward.isEmpty())
            {
                reportGrantError(notifyChat, "No templates are available.");
                return;
            }

            Service.Get<INetworkServicesManager>().RewardService.QA_SetReward(reward);
            if (notifyChat)
            {
                postSystemChatGrantedEquipment();
            }
        }

        private void handleAddTube(string[] parts)
        {
            if (parts.Length < 2)
            {
                postSystemChatError();
                return;
            }

            int definitionId;
            if (!int.TryParse(parts[1], out definitionId))
            {
                postSystemChatError();
                return;
            }

            AddTubeToInventory(definitionId);
        }

        public void AddTubeToInventory(int definitionId)
        {
            addTubeToInventory(definitionId, true);
        }

        public void AddTubeFromTweaker(int definitionId)
        {
            addTubeToInventory(definitionId, false);
        }

        private void addTubeToInventory(int definitionId, bool notifyChat)
        {
            Dictionary<int, TubeDefinition> tubeDefinitions = Service.Get<GameData>().Get<Dictionary<int, TubeDefinition>>();
            if (tubeDefinitions == null)
            {
                reportGrantError(notifyChat, "Tube definitions are unavailable.");
                return;
            }

            TubeDefinition tubeDef;
            if (!tubeDefinitions.TryGetValue(definitionId, out tubeDef) || tubeDef == null)
            {
                reportGrantError(notifyChat, "The requested tube is unavailable.");
                return;
            }

            Reward reward = new Reward();
            reward.Add(new TubeReward(definitionId));
            Service.Get<INetworkServicesManager>().RewardService.QA_SetReward(reward);

            if (notifyChat)
            {
                postSystemChatGrantedTube();
            }
        }

        public void AddAllTubesToInventory()
        {
            addAllTubesToInventory(true);
        }

        public void AddAllTubesFromTweaker()
        {
            addAllTubesToInventory(false);
        }

        private void addAllTubesToInventory(bool notifyChat)
        {
            Dictionary<int, TubeDefinition> tubeDefinitions = Service.Get<GameData>().Get<Dictionary<int, TubeDefinition>>();
            if (tubeDefinitions == null)
            {
                reportGrantError(notifyChat, "Tube definitions are unavailable.");
                return;
            }

            Reward reward = new Reward();
            bool hasTubeReward = false;
            foreach (TubeDefinition tube in tubeDefinitions.Values)
            {
                if (tube != null)
                {
                    reward.Add(new TubeReward(tube.Id));
                    hasTubeReward = true;
                }
            }
            if (!hasTubeReward)
            {
                reportGrantError(notifyChat, "No tubes are available.");
                return;
            }
            Service.Get<INetworkServicesManager>().RewardService.QA_SetReward(reward);
            if (notifyChat)
            {
                postSystemChatGrantedTube();
            }
        }

        private void reportGrantError(bool notifyChat, string message)
        {
            if (notifyChat)
            {
                postSystemChatError();
            }
            else
            {
                Debug.LogError(message);
            }
        }

        private void onTimeOutComplete()
        {
            dispatcher.DispatchEvent(default(ChatServiceEvents.SendChatActivityCancel));
        }

        private void onSendChatActivity()
        {
            Service.Get<EventDispatcher>().DispatchEvent(default(ChatServiceEvents.SendChatActivity));
        }
    }
}