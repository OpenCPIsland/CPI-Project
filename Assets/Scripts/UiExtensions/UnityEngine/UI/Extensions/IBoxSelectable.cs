namespace UnityEngine.UI.Extensions
{
	public interface IBoxSelectable
	{
		bool selected
		{
			get;
			set;
		}

		bool preSelected
		{
			get;
			set;
		}

		Transform transform
		{
			get;
		}
	}
}
