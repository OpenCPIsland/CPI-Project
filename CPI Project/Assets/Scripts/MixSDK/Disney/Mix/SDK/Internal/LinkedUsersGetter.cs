using Disney.Mix.SDK.Internal.GuestControllerDomain;
using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class LinkedUsersGetter
	{
		public static void Get(AbstractLogger logger, IMixWebCallFactory mixWebCallFactory, IList<Profile> profiles, Action<LinkedUser[]> callback)
		{
			IWebCall<GetUsersByUserIdRequest, GetUsersResponse> webCall = mixWebCallFactory.UsersByUserIdPost(new GetUsersByUserIdRequest
			{
				UserIds = profiles.Select((Profile u) => u.swid).ToList()
			});
			webCall.OnResponse += delegate(object sender, WebCallEventArgs<GetUsersResponse> e)
			{
				HandleGetUsersByIdSuccess(logger, e.Response, profiles, callback);
			};
			webCall.OnError += delegate
			{
				callback(null);
			};
			webCall.Execute();
		}

		private static void HandleGetUsersByIdSuccess(AbstractLogger logger, GetUsersResponse response, IList<Profile> profiles, Action<LinkedUser[]> callback)
		{
			try
			{
				if (response.Users == null)
				{
					logger.Critical("Returned users array is null");
					callback(null);
				}
				else
				{
					Dictionary<Profile, User> profileToUser = new Dictionary<Profile, User>();
					foreach (Profile profile2 in profiles)
					{
						List<User> users = response.Users;
						Func<User, bool> predicate = (User u) => profile2.swid == u.UserId;
						User user = users.FirstOrDefault(predicate);
						if (user == null)
						{
							logger.Critical("Returned users doesn't have " + profile2.swid + ": " + response.Users);
							callback(null);
							return;
						}
						profileToUser[profile2] = user;
					}
					LinkedUser[] obj = profiles.Select(delegate(Profile profile)
					{
						DateTime? dateOfBirth = GuestControllerUtils.ParseDateTime(logger, profile.dateOfBirth);
						AgeBandType ageBand = AgeBandTypeConverter.Convert(profile.ageBand);
						User user2 = profileToUser[profile];
						DisplayName displayName = new DisplayName(user2.DisplayName);
						return new LinkedUser(profile.username, profile.firstName, profile.lastName, displayName, profile.email, profile.parentEmail, ageBand, dateOfBirth, profile.swid);
					}).ToArray();
					callback(obj);
				}
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(null);
			}
		}
	}
}
