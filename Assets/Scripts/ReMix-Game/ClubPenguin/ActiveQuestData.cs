using Disney.Kelowna.Common.DataModel;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	[Serializable]
	public class ActiveQuestData : ScopedData
	{
		public struct Variables
		{
			public string FSMID;

			public FsmFloat[] Floats;

			public FsmInt[] Ints;

			public FsmString[] Strings;

			public FsmBool[] Bools;

			public FsmVector2[] Vec2s;

			public FsmVector3[] Vec3s;

			public void Reset()
			{
				FSMID = "";
				Floats = null;
				Ints = null;
				Strings = null;
				Bools = null;
				Vec2s = null;
				Vec3s = null;
			}
		}

		private readonly string localVariablePrefix = "local_";

		private Variables fsmVars;

		private Dictionary<string, Variables> subFsmVars;

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Quest.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(ActiveQuestDataMonoBehaviour);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}

		public void LoadFSMVariables(Fsm fsm, string id)
		{
			if (subFsmVars == null)
			{
				subFsmVars = new Dictionary<string, Variables>();
			}
			bool flag = false;
			Variables value = fsmVars;
			if (value.FSMID == id)
			{
				flag = true;
			}
			else if (subFsmVars.ContainsKey(id))
			{
				value = subFsmVars[id];
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			FsmVariables variables = fsm.Variables;
			if (value.Floats != null)
			{
				for (int i = 0; i < value.Floats.Length; i++)
				{
					variables.GetFsmFloat(value.Floats[i].Name).Value = value.Floats[i].Value;
				}
			}
			if (value.Ints != null)
			{
				for (int i = 0; i < value.Ints.Length; i++)
				{
					variables.GetFsmInt(value.Ints[i].Name).Value = value.Ints[i].Value;
				}
			}
			if (value.Strings != null)
			{
				for (int i = 0; i < value.Strings.Length; i++)
				{
					variables.GetFsmString(value.Strings[i].Name).Value = value.Strings[i].Value;
				}
			}
			if (value.Bools != null)
			{
				for (int i = 0; i < value.Bools.Length; i++)
				{
					variables.GetFsmBool(value.Bools[i].Name).Value = value.Bools[i].Value;
				}
			}
			if (value.Vec2s != null)
			{
				for (int i = 0; i < value.Vec2s.Length; i++)
				{
					variables.GetFsmVector2(value.Vec2s[i].Name).Value = value.Vec2s[i].Value;
				}
			}
			if (value.Vec3s != null)
			{
				for (int i = 0; i < value.Vec3s.Length; i++)
				{
					variables.GetFsmVector3(value.Vec3s[i].Name).Value = value.Vec3s[i].Value;
				}
			}
			if (fsmVars.FSMID == id)
			{
				fsmVars = value;
			}
			else if (subFsmVars.ContainsKey(id))
			{
				subFsmVars[id] = value;
			}
		}

		public void SaveFSMVariables(Fsm fsm, string id)
		{
			if (subFsmVars == null)
			{
				subFsmVars = new Dictionary<string, Variables>();
			}
			Variables value = default(Variables);
			value.FSMID = id;
			FsmVariables variables = fsm.Variables;
			FsmFloat[] floatVariables = variables.FloatVariables;
			if (floatVariables.Length > 0)
			{
				value.Floats = new FsmFloat[floatVariables.Length];
				for (int i = 0; i < floatVariables.Length; i++)
				{
					if (!floatVariables[i].Name.StartsWith(localVariablePrefix))
					{
						value.Floats[i] = new FsmFloat(floatVariables[i]);
					}
				}
			}
			FsmInt[] intVariables = variables.IntVariables;
			if (intVariables.Length > 0)
			{
				value.Ints = new FsmInt[intVariables.Length];
				for (int i = 0; i < intVariables.Length; i++)
				{
					if (!intVariables[i].Name.StartsWith(localVariablePrefix))
					{
						value.Ints[i] = new FsmInt(intVariables[i]);
					}
				}
			}
			FsmString[] stringVariables = variables.StringVariables;
			if (stringVariables.Length > 0)
			{
				value.Strings = new FsmString[stringVariables.Length];
				for (int i = 0; i < stringVariables.Length; i++)
				{
					if (!stringVariables[i].Name.StartsWith(localVariablePrefix))
					{
						value.Strings[i] = new FsmString(stringVariables[i]);
					}
				}
			}
			FsmBool[] boolVariables = variables.BoolVariables;
			if (boolVariables.Length > 0)
			{
				value.Bools = new FsmBool[boolVariables.Length];
				for (int i = 0; i < boolVariables.Length; i++)
				{
					if (!boolVariables[i].Name.StartsWith(localVariablePrefix))
					{
						value.Bools[i] = new FsmBool(boolVariables[i]);
					}
				}
			}
			FsmVector2[] vector2Variables = variables.Vector2Variables;
			if (vector2Variables.Length > 0)
			{
				value.Vec2s = new FsmVector2[vector2Variables.Length];
				for (int i = 0; i < vector2Variables.Length; i++)
				{
					if (!vector2Variables[i].Name.StartsWith(localVariablePrefix))
					{
						value.Vec2s[i] = new FsmVector2(vector2Variables[i]);
					}
				}
			}
			FsmVector3[] vector3Variables = variables.Vector3Variables;
			if (vector3Variables.Length > 0)
			{
				value.Vec3s = new FsmVector3[vector3Variables.Length];
				for (int i = 0; i < vector3Variables.Length; i++)
				{
					if (!vector3Variables[i].Name.StartsWith(localVariablePrefix))
					{
						value.Vec3s[i] = new FsmVector3(vector3Variables[i]);
					}
				}
			}
			if (fsmVars.FSMID == id)
			{
				fsmVars = value;
			}
			else if (!subFsmVars.ContainsKey(id))
			{
				subFsmVars.Add(id, value);
			}
			else
			{
				subFsmVars[id] = value;
			}
		}
	}
}
