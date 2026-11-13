using System;

public class SCButtonClickedEventArgs : EventArgs
{
	public int Id
	{
		get;
		set;
	}

	public SCButtonClickedEventArgs(int aId)
	{
		Id = aId;
	}
}
