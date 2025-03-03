using System;
using UnityEngine;

public static class AnimatorExtensions
{
	[Serializable]
	public struct AnimParamState
	{
		private int hash;

		private AnimatorControllerParameterType type;

		private object data;

		public void Set(Animator anim, AnimatorControllerParameter animParam)
		{
			hash = animParam.nameHash;
			type = animParam.type;
			switch (type)
			{
			case AnimatorControllerParameterType.Bool:
				data = anim.GetBool(hash);
				break;
			case AnimatorControllerParameterType.Float:
				data = anim.GetFloat(hash);
				break;
			case AnimatorControllerParameterType.Int:
				data = anim.GetInteger(hash);
				break;
			}
		}

		public void Restore(Animator anim)
		{
			switch (type)
			{
			case (AnimatorControllerParameterType)2:
				break;
			case AnimatorControllerParameterType.Bool:
				anim.SetBool(hash, (bool)data);
				break;
			case AnimatorControllerParameterType.Float:
				anim.SetFloat(hash, (float)data);
				break;
			case AnimatorControllerParameterType.Int:
				anim.SetInteger(hash, (int)data);
				break;
			}
		}
	}

	[Serializable]
	public struct LayerState
	{
		public AnimatorStateInfo Info;
	}

	[Serializable]
	public struct AnimatorState
	{
		public AnimParamState[] Params;

		public LayerState[] Layers;
	}

	public static AnimatorState SaveState(this Animator anim)
	{
		AnimatorState result = default(AnimatorState);
		if (anim.parameterCount > 0)
		{
			result.Params = new AnimParamState[anim.parameterCount];
			AnimatorControllerParameter[] parameters = anim.parameters;
			for (int i = 0; i < result.Params.Length; i++)
			{
				result.Params[i].Set(anim, parameters[i]);
			}
		}
		if (anim.layerCount > 0)
		{
			result.Layers = new LayerState[anim.layerCount];
			for (int i = 0; i < result.Layers.Length; i++)
			{
				result.Layers[i].Info = anim.GetCurrentAnimatorStateInfo(i);
			}
		}
		return result;
	}

	public static void RestoreState(this Animator anim, ref AnimatorState state, int excludeLayer = -1)
	{
		if (state.Params != null)
		{
			for (int i = 0; i < state.Params.Length; i++)
			{
				state.Params[i].Restore(anim);
			}
		}
		if (state.Layers == null)
		{
			return;
		}
		for (int i = 0; i < state.Layers.Length; i++)
		{
			if (i != excludeLayer)
			{
				anim.Play(state.Layers[i].Info.fullPathHash, i, state.Layers[i].Info.normalizedTime);
			}
		}
	}
}
