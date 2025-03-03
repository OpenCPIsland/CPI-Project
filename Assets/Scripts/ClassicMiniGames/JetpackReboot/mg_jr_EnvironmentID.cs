using NUnit.Framework;

namespace JetpackReboot
{
	public class mg_jr_EnvironmentID
	{
		public EnvironmentType Type
		{
			get;
			private set;
		}

		public EnvironmentVariant Variant
		{
			get;
			private set;
		}

		public mg_jr_EnvironmentID(EnvironmentType _environment, EnvironmentVariant _variant)
		{
			Assert.AreNotEqual(_environment, EnvironmentType.MAX);
			Assert.AreNotEqual(_variant, EnvironmentVariant.MAX);
			Type = _environment;
			Variant = _variant;
		}

		public static bool operator ==(mg_jr_EnvironmentID _left, mg_jr_EnvironmentID _right)
		{
			return _left.Type == _right.Type && _left.Variant == _right.Variant;
		}

		public static bool operator !=(mg_jr_EnvironmentID _left, mg_jr_EnvironmentID _right)
		{
			return !(_left == _right);
		}

		public override bool Equals(object obj)
		{
			return obj is mg_jr_EnvironmentID && this == (mg_jr_EnvironmentID)obj;
		}

		public override int GetHashCode()
		{
			return (int)Type + (int)(5 + Variant);
		}

		public override string ToString()
		{
			return Type.ToString() + "_" + Variant;
		}
	}
}
