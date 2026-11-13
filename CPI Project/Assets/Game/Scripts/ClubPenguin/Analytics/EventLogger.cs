using ClubPenguin.Core;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.Task;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections.Generic;
using System.Linq;

namespace ClubPenguin.Analytics
{
	public class EventLogger
	{
		public EventLogger(EventDispatcher eventDispatcher)
		{
			eventDispatcher.AddListener<FriendsServiceEvents.UnfriendSent>(onUnfriendSent);
			eventDispatcher.AddListener<FriendsServiceEvents.RejectFriendInvitationSent>(onRejectFriendInvitationSent);
			eventDispatcher.AddListener<FriendsServiceEvents.AcceptFriendInvitationSent>(onAcceptFriendInvitationSent);
			eventDispatcher.AddListener<FriendsServiceEvents.SendFriendInvitationSent>(onSendFriendInvitationSent);
			eventDispatcher.AddListener<SessionEvents.SessionStartedEvent>(onSessionStarted);
			eventDispatcher.AddListener<PlayerStateServiceEvents.LocalPlayerDataReceived>(onLocalPlayerDataReceived);
			eventDispatcher.AddListener<PlayerStateServiceErrors.PlayerProfileError>(onGetProfileError);
			eventDispatcher.AddListener<TaskEvents.TaskCompleted>(onCompleteTask);
			eventDispatcher.AddListener<TaskEvents.TaskUpdated>(onUpdateTask);
			MixLoginCreateService mixLoginCreateService = Service.Get<MixLoginCreateService>();
			mixLoginCreateService.OnCreateFailed += onMixCreateFailed;
			mixLoginCreateService.OnCreateSuccess += onMixCreateSuccess;
			mixLoginCreateService.OnLoginFailed += onMixLoginFailed;
			mixLoginCreateService.OnGetRegistrationConfigStart += onGetRegistrationConfigStart;
			mixLoginCreateService.OnGetRegistrationConfigComplete += onGetRegistrationConfigComplete;
			mixLoginCreateService.OnGetAgeBandStart += onGetAgeBandStart;
			mixLoginCreateService.OnGetAgeBandComplete += onGetAgeBandComplete;
			mixLoginCreateService.OnGetUpdateAgeBandStart += onGetUpdateAgeBandStart;
			mixLoginCreateService.OnGetUpdateAgeBandComplete += onGetUpdateAgeBandComplete;
			mixLoginCreateService.OnCreateChildAccountStart += onCreateChildAccountStart;
			mixLoginCreateService.OnCreateChildAccountComplete += onCreateChildAccountComplete;
			mixLoginCreateService.OnSoftLoginStart += onSoftLoginStart;
			mixLoginCreateService.OnSoftLoginFailed += onSoftLoginFailed;
			mixLoginCreateService.OnSoftLoginCompleteSuccess += onSoftLoginCompleteSuccess;
			mixLoginCreateService.OnLoginStart += onLoginStart;
			mixLoginCreateService.OnLoginCompleteSuccess += onLoginCompleteSuccess;
		}

		private bool onUpdateTask(TaskEvents.TaskUpdated evt)
		{
			ClubPenguin.Task.Task task = evt.Task;
			string callID = string.Format("{0}_{1}", task.Definition.name, task.Counter);
			if (task.Definition.Group != 0)
			{
				string currentWorldName = getCurrentWorldName();
				Service.Get<ICPSwrveService>().ActionSingular(callID, "daily_task.community." + task.Definition.name, string.Format("{0}/{1}", task.Counter, task.Definition.Threshold), currentWorldName);
			}
			else if (task.Definition.Group == TaskDefinition.TaskGroup.Individual)
			{
				Service.Get<ICPSwrveService>().ActionSingular(callID, "daily_task.individual." + task.Definition.name, string.Format("{0}/{1}", task.Counter, task.Definition.Threshold));
			}
			return false;
		}

		private bool onCompleteTask(TaskEvents.TaskCompleted evt)
		{
			ClubPenguin.Task.Task task = evt.Task;
			if (task.Definition.Group == TaskDefinition.TaskGroup.Individual)
			{
				Service.Get<ICPSwrveService>().ActionSingular(task.Definition.name, "daily_task.individual." + task.Definition.name, "completion");
			}
			else
			{
				string currentWorldName = getCurrentWorldName();
				Service.Get<ICPSwrveService>().Action(task.Definition.name, "daily_task.community." + task.Definition.name, "completion", currentWorldName);
			}
			return false;
		}

		private string getCurrentWorldName()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			PresenceData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				return component.World;
			}
			return "";
		}

		private bool onUnfriendSent(FriendsServiceEvents.UnfriendSent evt)
		{
			Service.Get<ICPSwrveService>().Action("game.friends", "remove", evt.FriendName);
			return false;
		}

		private bool onRejectFriendInvitationSent(FriendsServiceEvents.RejectFriendInvitationSent evt)
		{
			Service.Get<ICPSwrveService>().Action("game.friends", "rejected", evt.FriendName);
			return false;
		}

		private bool onSendFriendInvitationSent(FriendsServiceEvents.SendFriendInvitationSent evt)
		{
			Service.Get<ICPSwrveService>().Action("game.friends", "request_sent", evt.FriendName);
			return false;
		}

		private bool onAcceptFriendInvitationSent(FriendsServiceEvents.AcceptFriendInvitationSent evt)
		{
			Service.Get<ICPSwrveService>().Action("game.friends", "accepted", evt.FriendName);
			return false;
		}

		private bool onSessionStarted(SessionEvents.SessionStartedEvent evt)
		{
			Service.Get<ICPSwrveService>().StartTimer("getprofiletimer", "cp_profile_data");
			return false;
		}

		private bool onGetProfileError(PlayerStateServiceErrors.PlayerProfileError evt)
		{
			Service.Get<ICPSwrveService>().EndTimer("getprofiletimer", null, null, "fail");
			return false;
		}

		private bool onLocalPlayerDataReceived(PlayerStateServiceEvents.LocalPlayerDataReceived evt)
		{
			Service.Get<ICPSwrveService>().EndTimer("getprofiletimer", null, null, "success");
			return false;
		}

		private void onMixCreateFailed(IRegisterResult result)
		{
			string tier = "unknown";
			List<IInvalidProfileItemError> list = new List<IInvalidProfileItemError>();
			if (result.Errors != null)
			{
				list.AddRange(result.Errors);
			}
			if (list.Count > 1)
			{
				tier = string.Join(", ", list.Select((IInvalidProfileItemError e) => e.ToString()).ToArray());
			}
			Service.Get<ICPSwrveService>().Action("game.account_creation.submit", "fail", tier);
		}

		private void onMixCreateSuccess(ISession session)
		{
			Service.Get<ICPSwrveService>().Action("game.account_creation.submit", "success");
		}

		private void onMixLoginFailed(ILoginResult result)
		{
			Service.Get<ICPSwrveService>().Action("game.login.submit", "fail", result.GetType().Name);
			Service.Get<ICPSwrveService>().EndTimer("logintimer", null, null, "fail");
		}

		private void onGetRegistrationConfigStart()
		{
			Service.Get<ICPSwrveService>().StartTimer("regconfigtimer", "reg_config", "config");
		}

		private void onGetRegistrationConfigComplete(bool success)
		{
			string overrideStepName = success ? "success" : "fail";
			Service.Get<ICPSwrveService>().EndTimer("regconfigtimer", null, null, overrideStepName);
		}

		private void onGetAgeBandStart()
		{
			Service.Get<ICPSwrveService>().StartTimer("agebandtimer", "reg_config", "age_band_registration");
		}

		private void onGetAgeBandComplete(bool success)
		{
			string overrideMessage = success ? "success" : "fail";
			Service.Get<ICPSwrveService>().EndTimer("agebandtimer", null, overrideMessage);
		}

		private void onGetUpdateAgeBandStart()
		{
			Service.Get<ICPSwrveService>().StartTimer("agebandtimer", "reg_config", "age_band_update");
		}

		private void onGetUpdateAgeBandComplete(bool success)
		{
			string overrideMessage = success ? "success" : "fail";
			Service.Get<ICPSwrveService>().EndTimer("agebandupdatetimer", null, overrideMessage);
		}

		private void onCreateChildAccountStart()
		{
			Service.Get<ICPSwrveService>().StartTimer("createtimer", "account_creation");
		}

		private void onCreateChildAccountComplete(IRegisterResult result)
		{
			string overrideMessage = result.Success ? "success" : "fail";
			Service.Get<ICPSwrveService>().EndTimer("createtimer", null, overrideMessage);
			if (result.Success)
			{
				Service.Get<ICPSwrveService>().Action("game.account_creation.submit", "success");
				return;
			}
			string tier = "unknown";
			List<IInvalidProfileItemError> list = new List<IInvalidProfileItemError>();
			if (result.Errors != null)
			{
				list.AddRange(result.Errors);
			}
			if (list.Count > 1)
			{
				tier = string.Join(", ", list.Select((IInvalidProfileItemError e) => e.ToString()).ToArray());
			}
			Service.Get<ICPSwrveService>().Action("game.account_creation.submit", "fail", tier);
		}

		private void onSoftLoginStart()
		{
			Service.Get<ICPSwrveService>().StartTimer("softlogintimer", "soft_login");
		}

		private void onSoftLoginCompleteSuccess(int friendCount)
		{
			Service.Get<ICPSwrveService>().EndTimer("softlogintimer", null, friendCount.ToString(), "success");
		}

		private void onSoftLoginFailed(IRestoreLastSessionResult obj)
		{
			Service.Get<ICPSwrveService>().EndTimer("softlogintimer", null, null, "fail");
		}

		private void onLoginStart()
		{
			Service.Get<ICPSwrveService>().StartTimer("logintimer", "hard_login");
		}

		private void onLoginCompleteSuccess(int friendCount)
		{
			Service.Get<ICPSwrveService>().EndTimer("logintimer", null, friendCount.ToString(), "success");
		}
	}
}
