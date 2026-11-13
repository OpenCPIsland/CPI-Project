using JetpackReboot.EnumExtensions;
using NUnit.Framework;

namespace JetpackReboot
{
	public class mg_jr_EnvironmentLayerID : mg_jr_EnvironmentID
	{
		public EnvironmentLayer Layer
		{
			get;
			private set;
		}

		public mg_jr_EnvironmentLayerID(EnvironmentType _environment, EnvironmentVariant _variant, EnvironmentLayer _layer)
			: base(_environment, _variant)
		{
			Assert.AreNotEqual(_layer, EnvironmentLayer.MAX);
			Layer = _layer;
		}

		public static bool operator ==(mg_jr_EnvironmentLayerID _left, mg_jr_EnvironmentLayerID _right)
		{
			return _left.Type == _right.Type && _left.Layer == _right.Layer && _left.Variant == _right.Variant;
		}

		public static bool operator !=(mg_jr_EnvironmentLayerID _left, mg_jr_EnvironmentLayerID _right)
		{
			return !(_left == _right);
		}

		public override bool Equals(object obj)
		{
			return obj is mg_jr_EnvironmentLayerID && this == (mg_jr_EnvironmentLayerID)obj;
		}

		public override int GetHashCode()
		{
			return (int)(base.GetHashCode() + (7 + Layer));
		}

		public override string ToString()
		{
			return base.ToString() + "_" + Layer;
		}

		public string ResourceFileName()
		{
			return "JetpackReboot/Environment/mg_jr_pf_" + base.Type.FileNameFragment(base.Variant) + "_" + Layer.FileNameFragment();
		}
	}
}
