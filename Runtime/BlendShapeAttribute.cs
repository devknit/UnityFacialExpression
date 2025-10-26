
namespace Knit.FacialExpression
{
	[System.Flags]
	public enum BlendShapeAttribute
	{
		None = 0,
		Reverse = 1 << 0,
		Blendable = 1 << 1,
		EyeBlink = 1 << 2,
		LipSync = 1 << 3,
		Keep = EyeBlink | LipSync
	}
}
