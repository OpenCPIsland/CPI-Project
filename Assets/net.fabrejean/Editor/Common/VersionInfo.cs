// Totally inspired by the great work of Patrick Hogan with InControl
// https://github.com/pbhogan/InControl
// Only that I don't rely on the number of time you play the project, but on the number of compilation, which I think reflects more true internal changes of the code


using System;
using System.Collections;
using System.Collections.Generic;


using System.Text.RegularExpressions;
using UnityEngine;


namespace Net.FabreJean.UnityEditor
{
	
	public struct VersionInfo : IComparable<VersionInfo>
	{
		public enum VersionType {Alpha,Beta,ReleaseCandidate,Final};
		
		public int Major;
		public int Minor;
		public int Patch;
		public int Maintenance;
		public VersionType Type;
		public int Build;
		public string Appendix;
		
		
		public VersionInfo( int major, int minor = 0, int patch = 0 )
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			Type  = VersionType.Final;
			Build = 0;
			Appendix = "";
			Maintenance = 0;
		}
		
		public VersionInfo( int major, int minor = 0, int patch = 0, int build = 0 )
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			Type  = VersionType.Final;
			Build = build;
			Appendix = "";
			Maintenance = 0;
		}
		
		public VersionInfo( int major, int minor = 0, int patch = 0, VersionType type = VersionType.Final , int build = 0 )
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			Type  = type;
			Build = build;
			Appendix = "";
			Maintenance = 0;
		}
		public VersionInfo( int major, int minor = 0, int patch = 0,int maintenance = 0, VersionType type = VersionType.Final , int build = 0 )
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			Maintenance = maintenance;
			Type  = type;
			Build = build;
			Appendix = "";
		}
		
		public VersionInfo( string version )
		{
			
			Major = 0;
			Minor = 0;
			Patch=0;
			Maintenance = 0;
			Type = VersionType.Final;
			Build=0;
			Appendix = "";
			if (string.IsNullOrEmpty(version))
			{
				
			}else{
				// (\d+)(?:\.)?(\d+)?(?:\.)?(\d+)?(?:\.)?(\d+)?(?:\.)?(\d+)?(\h*?(a|b|rc|f)(\d+)?)?(\h+?\w+)?
				var match = Regex.Match(version, @"^(\d+)\.(\d+)(\.(\d+))?\s*\.*(\w+)?",RegexOptions.IgnoreCase);	
				
				if (match.Groups.Count>1)
				{
					Major = Convert.ToInt32( match.Groups[1].Value );
				}
				if (match.Groups.Count>2)
				{
					Minor = Convert.ToInt32( match.Groups[2].Value );
				}
				if (match.Groups.Count>3)
				{
					string _v = match.Groups[3].Value;
					if (_v.StartsWith("."))
					{
						Patch = Convert.ToInt32( match.Groups[4].Value );
					}
				}
				if (match.Groups.Count>5)
				{
					Appendix = (string)match.Groups[5].Value;
					Appendix = Appendix.Trim();
				}
				Type = VersionType.Final;
				Build = 0;
			}
		}
		
		public static VersionInfo VersionInfoFromJson(String jsonString)
		{
			// {"Major":0, "Patch":4, "Build":73, "Type":"f", "Minor":4}
			if (string.IsNullOrEmpty(jsonString))
			{
				return new VersionInfo();
			}
			
			Hashtable _details = (Hashtable)JSON.JsonDecode(jsonString);
			if (_details ==null)
			{
				return new VersionInfo();
			}
			
			return new VersionInfo(
				(int)_details["Major"],
				(int)_details["Minor"],
				(int)_details["Patch"],
				GetVersionTypeFromString((string)_details["Type"]),
				(int)_details["Build"]
				);
			
		}
		
		public static VersionInfo UnityVersion()
		{
			var match = Regex.Match( Application.unityVersion, @"^(\d+)\.(\d+)\.(\d+)" );
			var build = 0;
			return new VersionInfo() {
				Major = Convert.ToInt32( match.Groups[1].Value ),
				Minor = Convert.ToInt32( match.Groups[2].Value ),
				Patch = Convert.ToInt32( match.Groups[3].Value ),
				Build = build
			};
		}
		
		/// <summary>
		/// I haven't digged into this but if I assign a struct from a variable to another does that create a shallow copy or it remains the same in memory?
		/// like myversion == someother version ? what happens there? 
		/// </summary>
		public VersionInfo Clone() { 
			return new VersionInfo(Major, Minor, Patch,Type,Build); 
		}
		
		public int CompareTo( VersionInfo other )
		{
			if (Major < other.Major) return -1;
			if (Major > other.Major) return +1;
			if (Minor < other.Minor) return -1;
			if (Minor > other.Minor) return +1;
			if (Patch < other.Patch) return -1;
			if (Patch > other.Patch) return +1;
			if (Build < other.Build) return -1;
			if (Build > other.Build) return +1;
			return 0;
		}

		public bool isDefined()
		{
			if (Major == 0 && Minor == 0 && Patch == 0 && Maintenance == 0)
			{
				return false;
			}

			return true;
		}

		
		public static bool operator ==( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) == 0;
		}
		
		
		public static bool operator !=( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) != 0;
		}
		
		
		public static bool operator <=( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) <= 0;
		}
		
		
		public static bool operator >=( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) >= 0;
		}
		
		
		public static bool operator <( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) < 0;
		}
		
		
		public static bool operator >( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) > 0;
		}
		
		
		public static VersionType GetVersionTypeFromString(string type)
		{
			if (string.IsNullOrEmpty(type))
			{
				return VersionType.Final;
			}
			
			switch (type.ToLower())
			{
			case "a": case "alpha":
				return VersionType.Alpha;
			case "b": case"beta":
				return VersionType.Beta;
			case "rc": case"releasecandidate":
				return VersionType.ReleaseCandidate;
			case "f": case"final":
				return VersionType.Final;
			}
			
			return VersionType.Final;
		}
		
		public static string GetVersionTypeAsString(VersionType type)
		{
			if (type== VersionType.Alpha)
			{
				return "a";
			}
			if (type== VersionType.Beta)
			{
				return "b";
			}
			if (type== VersionType.ReleaseCandidate)
			{
				return "rc";
			}
			
			return "f";
		}
		
		public static string GetVersionTypeAsLongString(VersionType type)
		{
			if (type== VersionType.Alpha)
			{
				return "Alpha";
			}
			if (type== VersionType.Beta)
			{
				return "Beta";
			}
			if (type== VersionType.ReleaseCandidate)
			{
				return "Release Candidate";
			}
			
			return "Final";
		}
		
		/// <summary>
		/// VersionInfo string : x.x.x t x x
		/// </summary>
		/// <returns>The VersionInfo as string</returns>
		public override string ToString()
		{
			if (Build == 0)
			{
				return string.Format( "{0}.{1}.{2} {3}", Major, Minor, Patch, Appendix ).Trim();
			}
			return string.Format( "{0}.{1}.{2} {3} {4} {5}", Major, Minor, Patch, GetVersionTypeAsLongString(Type), Build,Appendix ).Trim();
		}
		
		/// <summary>
		///  Short string: x.x.xtx x
		/// </summary>
		/// <returns>The short string.</returns>
		public string ToShortString()
		{
			if (Build == 0)
			{
				return string.Format( "{0}.{1}.{2} {3}", Major, Minor, Patch, Appendix ).Trim();
			}
			return string.Format( "{0}.{1}.{2}{3}{4}{5}", Major, Minor, Patch, GetVersionTypeAsString(Type), Build, Appendix ).Trim();
		}
		
		/// <summary>
		/// Custom format if wanted. {0} is Major, {1} is Minor, {2} is Patch, {3} is short Type, {4} is long type, 5 is Build, 6 is appendix
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="format">Format. default to "{0}.{1}.{2}{3}{5}"</param>
		public string ToString(string format = "{0}.{1}.{2}{3}{5} {6}")
		{
			string _result = string.Format("{0}.{1}.{2}{3}{5} {6}", 
			                               /* 0 */ Major, 
			                               /* 1 */ Minor, 
			                               /* 2 */ Patch,
			                               /* 3 */ GetVersionTypeAsString(Type),
			                               /* 4 */ GetVersionTypeAsLongString(Type),
			                               /* 5 */  Build,
			                               /* 6 */ Appendix);
			
			return _result.Trim();
		}
		
		
		public override bool Equals( object other )
		{
			if (other is VersionInfo)
			{
				return this == ((VersionInfo) other);
			}
			return false;
		}
		
		
		public override int GetHashCode()
		{
			return Major.GetHashCode() ^ Minor.GetHashCode() ^ Patch.GetHashCode() ^ Build.GetHashCode();
		}
		
		
		
	}
}