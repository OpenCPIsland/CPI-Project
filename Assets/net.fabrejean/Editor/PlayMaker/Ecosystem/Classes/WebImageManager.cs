using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using  Net.FabreJean.UnityEditor;

#pragma warning disable 618

public class WebImageManager {

	/*
	/// <summary>
	/// The cache limit. This constraint how much images are kept in memory, first in first out rule applied when limit is reached.
	/// </summary>
	public static int cacheLimit = 100;
	*/

	static Dictionary<string,WebImageItem> imageLUT = new Dictionary<string, WebImageItem>();

	public static Texture2D GetWebImage(string Url,Vector2 ExpectedSize)
	{
		if (!imageLUT.ContainsKey(Url))
		{
			imageLUT[Url] = new WebImageItem(Url,ExpectedSize);
		}

		return imageLUT[Url].Texture2d;
	}

}

public class WebImageItem {

	public string Url;
	public Texture2D Texture2d;
	public HttpWrapper Loader;

	public WebImageItem (string Url,Vector2 ExpectedSize)
	{
		this.Url = Url;
		this.Texture2d = new Texture2D((int)ExpectedSize.x,(int)ExpectedSize.y);
		this.Loader = new HttpWrapper();
		this.Loader.GET
			(
				this.Url,
			    (WWW www) => 
			    	{
						if (string.IsNullOrEmpty(www.error))
						{
							this.Texture2d = www.texture;
						}
					}
			);
	}
}

	
