using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin
{
	public static class AnimationHashes
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct Params
		{
			public static readonly int Jump = Animator.StringToHash("Jump");

			public static readonly int WalkSpeedMult = Animator.StringToHash("WalkSpeedMult");

			public static readonly int JogSpeedMult = Animator.StringToHash("JogSpeedMult");

			public static readonly int SprintSpeedMult = Animator.StringToHash("SprintSpeedMult");

			public static readonly int LandingDistance = Animator.StringToHash("LandingDistance");

			public static readonly int LandingTime = Animator.StringToHash("LandingTime");

			public static readonly int LocoMode = Animator.StringToHash("LocoMode");

			public static readonly int Emote = Animator.StringToHash("Emote");

			public static readonly int LoopEmote = Animator.StringToHash("LoopEmote");

			public static readonly int PlayEmote = Animator.StringToHash("PlayEmote");

			public static readonly int Dancing = Animator.StringToHash("Dancing");

			public static readonly int DanceMove = Animator.StringToHash("DanceMove");

			public static readonly int PivotAngle = Animator.StringToHash("PivotAngle");

			public static readonly int NormTime = Animator.StringToHash("NormTime");

			public static readonly int LoopCount = Animator.StringToHash("LoopCount");

			public static readonly int Freefall = Animator.StringToHash("Freefall");

			public static readonly int GroundFriction = Animator.StringToHash("GroundFriction");

			public static readonly int SprintLean = Animator.StringToHash("SprintLean");

			public static readonly int Scripted = Animator.StringToHash("Scripted");

			public static readonly int Slide = Animator.StringToHash("Slide");

			public static readonly int SlideAirborne = Animator.StringToHash("SlideAirborne");

			public static readonly int SlideTrick1 = Animator.StringToHash("SlideTrick1");

			public static readonly int StrafeX = Animator.StringToHash("StrafeX");

			public static readonly int StrafeY = Animator.StringToHash("StrafeY");

			public static readonly int Woohoo = Animator.StringToHash("Woohoo");

			public static readonly int Bump = Animator.StringToHash("Bump");

			public static readonly int Angle = Animator.StringToHash("Angle");

			public static readonly int NormSpeed = Animator.StringToHash("NormSpeed");

			public static readonly int ReactToHit = Animator.StringToHash("ReactToHit");

			public static readonly int Swim = Animator.StringToHash("Swim");

			public static readonly int SwimSpeed = Animator.StringToHash("SwimSpeed");

			public static readonly int SwimSpeedMult = Animator.StringToHash("SwimSpeedMult");

			public static readonly int SwimToWalk = Animator.StringToHash("SwimToWalk");

			public static readonly int Resurface = Animator.StringToHash("Resurface");

			public static readonly int QuickResurface = Animator.StringToHash("QuickResurface");

			public static readonly int AbortTorpedo = Animator.StringToHash("AbortTorpedo");

			public static readonly int LowAirAnimChooser = Animator.StringToHash("LowAirAnimChooser");

			public static readonly int InviteOffer = Animator.StringToHash("InviteOffer");

			public static readonly int InviteConsume = Animator.StringToHash("InviteConsume");

			public static readonly int Sit = Animator.StringToHash("Sit");

			public static readonly int EnterSitAnimIndex = Animator.StringToHash("EnterSitAnimIndex");

			public static readonly int SitAnimIndex = Animator.StringToHash("SitAnimIndex");

			public static readonly int PropRetrieveType = Animator.StringToHash("PropRetrieveType");

			public static readonly int SnowballThrowEnabled = Animator.StringToHash("ThrowEnabled");

			public static readonly int AwayFromKeyboardEnterTrigger = Animator.StringToHash("AwayFromKeyboardEnterTrigger");

			public static readonly int AwayFromKeyboardExitTrigger = Animator.StringToHash("AwayFromKeyboardExitTrigger");

			public static readonly int ExitPropAction = Animator.StringToHash("ExitPropAction");

			public static readonly int Shuffle = Animator.StringToHash("Shuffle");
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct States
		{
			[StructLayout(LayoutKind.Sequential, Size = 1)]
			public struct Run
			{
				public static readonly int Jog = Animator.StringToHash("Base Layer.Run.Jog");

				public static readonly int Sprint = Animator.StringToHash("Base Layer.Run.Sprint");

				public static readonly int PivotLeft = Animator.StringToHash("Base Layer.Run.PivotLeft");

				public static readonly int PivotRight = Animator.StringToHash("Base Layer.Run.PivotRight");

				public static readonly int JogStopLeft = Animator.StringToHash("Base Layer.Run.JogStopLeft");

				public static readonly int JogStopRight = Animator.StringToHash("Base Layer.Run.JogStopRight");

				public static readonly int SprintStopLeft = Animator.StringToHash("Base Layer.Run.SprintStopLeft");

				public static readonly int SprintStopRight = Animator.StringToHash("Base Layer.Run.SprintStopRight");

				public static readonly int SprintPivot = Animator.StringToHash("Base Layer.Run.SprintPivot");
			}

			[StructLayout(LayoutKind.Sequential, Size = 1)]
			public struct Jump
			{
				public static readonly int StandingJumpEnter = Animator.StringToHash("Base Layer.Jump.StandingJumpEnter");

				public static readonly int WalkingJumpEnter = Animator.StringToHash("Base Layer.Jump.WalkingJumpEnter");

				public static readonly int JumpLoop = Animator.StringToHash("Base Layer.Jump.JumpLoop");

				public static readonly int StandingJumpExit = Animator.StringToHash("Base Layer.Jump.StandingJumpExit");

				public static readonly int WalkingJumpExit = Animator.StringToHash("Base Layer.Jump.WalkingJumpExit");

				public static readonly int RunningLeapEnter = Animator.StringToHash("Base Layer.Jump.RunningLeapEnter");

				public static readonly int LeapLoop = Animator.StringToHash("Base Layer.Jump.LeapLoop");

				public static readonly int RunningLeapExit = Animator.StringToHash("Base Layer.Jump.RunningLeapExit");

				public static readonly int StandingLeapExit = Animator.StringToHash("Base Layer.Jump.StandingLeapExit");

				public static readonly int Freefall = Animator.StringToHash("Base Layer.Jump.Freefall");

				public static readonly int StandingFreefallExit = Animator.StringToHash("Base Layer.Jump.StandingFreefallExit");

				public static readonly int RunningFreefallExit = Animator.StringToHash("Base Layer.Jump.RunningFreefallExit");
			}

			[StructLayout(LayoutKind.Sequential, Size = 1)]
			public struct Dance
			{
				public static readonly int Idle = Animator.StringToHash("Base Layer.Dance.Idle");

				public static readonly int Up = Animator.StringToHash("Base Layer.Dance.Up");

				public static readonly int Right = Animator.StringToHash("Base Layer.Dance.Right");

				public static readonly int Down = Animator.StringToHash("Base Layer.Dance.Down");

				public static readonly int Left = Animator.StringToHash("Base Layer.Dance.Left");
			}

			[StructLayout(LayoutKind.Sequential, Size = 1)]
			public struct Slide
			{
				public static readonly int Enter = Animator.StringToHash("Base Layer.Slide.SlideEnter");
			}

			[StructLayout(LayoutKind.Sequential, Size = 1)]
			public struct Interactions
			{
				public static readonly int Sit = Animator.StringToHash("Base Layer.Interactions.Sit");

				public static readonly int Celebration = Animator.StringToHash("Base Layer.Interactions.Celebration");

				public static readonly int PropAction1 = Animator.StringToHash("Base Layer.Interactions.PropAction1");

				public static readonly int PropAction2 = Animator.StringToHash("Base Layer.Interactions.PropAction2");

				public static readonly int PropAction3 = Animator.StringToHash("Base Layer.Interactions.PropAction3");

				public static readonly int TorsoAction1 = Animator.StringToHash("Torso Layer.TorsoAction1");

				public static readonly int TorsoAction2 = Animator.StringToHash("Torso Layer.TorsoAction2");

				public static readonly int TorsoAction3 = Animator.StringToHash("Torso Layer.TorsoAction3");
			}

			[StructLayout(LayoutKind.Sequential, Size = 1)]
			public struct Swim
			{
				public static readonly int SwimIdle = Animator.StringToHash("Base Layer.SwimSSM.SwimIdle");

				public static readonly int SwimState = Animator.StringToHash("Base Layer.SwimSSM.Swim");

				public static readonly int SwimTorpedo = Animator.StringToHash("Base Layer.SwimSSM.SwimTorpedo");

				public static readonly int WaterTunnel = Animator.StringToHash("Base Layer.SwimSSM.WaterTunnel");

				public static readonly int SwimTakeDamage = Animator.StringToHash("Base Layer.SwimSSM.SwimTakeDamage");

				public static readonly int QuickResurface = Animator.StringToHash("Base Layer.SwimSSM.QuickResurface");

				public static readonly int Resurface = Animator.StringToHash("Base Layer.SwimSSM.Resurface");
			}

			public static readonly int Idle = Animator.StringToHash("Base Layer.Idle");

			public static readonly int Walk = Animator.StringToHash("Base Layer.Walk");

			public static readonly int TorsoIdle = Animator.StringToHash("Torso Layer.Idle");

			public static readonly int TorsoRetrieve = Animator.StringToHash("Torso Layer.Retrieve");

			public static readonly int TorsoHold = Animator.StringToHash("Torso Layer.Hold");

			public static readonly int TorsoUse = Animator.StringToHash("Torso Layer.Use");

			public static readonly int TorsoStore = Animator.StringToHash("Torso Layer.Store");

			public static readonly int TorsoCelebration = Animator.StringToHash("Torso Layer.Celebration");

			public static readonly int TorsoOffer = Animator.StringToHash("Torso Layer.OfferLoop");
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct Tags
		{
			public static readonly int InAir = Animator.StringToHash("InAir");

			public static readonly int Landing = Animator.StringToHash("Landing");

			public static readonly int Idling = Animator.StringToHash("Idling");

			public static readonly int Walking = Animator.StringToHash("Walking");

			public static readonly int Jogging = Animator.StringToHash("Jogging");

			public static readonly int Sprinting = Animator.StringToHash("Sprinting");

			public static readonly int Stopping = Animator.StringToHash("Stopping");

			public static readonly int Pivoting = Animator.StringToHash("Pivoting");

			public static readonly int ReactingToHit = Animator.StringToHash("ReactingToHit");

			public static readonly int Turboing = Animator.StringToHash("Turboing");

			public static readonly int Resurfacing = Animator.StringToHash("Resurfacing");

			public static readonly int QuickResurfacing = Animator.StringToHash("QuickResurfacing");

			public static readonly int ThrowingSnowball = Animator.StringToHash("ThrowingSnowball");

			public static readonly int ChargingSnowball = Animator.StringToHash("ChargingSnowball");

			public static readonly int Sitting = Animator.StringToHash("Sitting");
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct Layers
		{
			public static readonly int Base = 0;

			public static readonly int Torso = 1;

			public static readonly int Snowball = 2;

			public static readonly int Invite = 3;
		}

		private static Dictionary<int, string> hashMap;

		private static List<string> states;

		private static List<string> parameters;

		private static List<string> tags;

		private static List<string> defaultStates;

		private static void addHash(string name)
		{
			addHash(name, Animator.StringToHash(name));
		}

		private static void addHash(string name, int value)
		{
			if (hashMap.ContainsKey(value))
			{
				throw new UnityException(string.Format("Animation Hash Collision: {0} and {1} both resolve to {2}", name, hashMap[value], value));
			}
			hashMap[value] = name;
		}

		private static void enumerate(Type type, List<string> names, List<string> defaults = null, string prefix = "")
		{
			List<string> defaults2 = defaults;
			FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				string text = prefix + fieldInfo.Name;
				addHash(text, (int)fieldInfo.GetValue(null));
				names.Add(text);
				if (defaults != null)
				{
					defaults.Add(text);
					defaults = null;
				}
			}
			Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Public);
			foreach (Type type2 in nestedTypes)
			{
				enumerate(type2, names, defaults2, prefix + type2.Name + ".");
			}
		}

		[RuntimeInitializeOnLoadMethod]
		private static void initialize()
		{
			hashMap = new Dictionary<int, string>();
			states = new List<string>();
			defaultStates = new List<string>();
			tags = new List<string>();
			parameters = new List<string>();
			enumerate(typeof(Params), parameters);
			enumerate(typeof(States), states, defaultStates, "Base Layer.");
			enumerate(typeof(Tags), tags);
			foreach (string state in states)
			{
				addHash("Entry -> " + state);
				addHash(state + " -> Exit");
				foreach (string state2 in states)
				{
					addHash(state + " -> " + state2);
				}
			}
			foreach (string defaultState in defaultStates)
			{
				addHash("Entry -> Default State ( " + defaultState + " )");
			}
		}

		public static string ToString(int hash)
		{
			string value;
			return hashMap.TryGetValue(hash, out value) ? value : ("<unknown hash: " + hash + ">");
		}
	}
}
