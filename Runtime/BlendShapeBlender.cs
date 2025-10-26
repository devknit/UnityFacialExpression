
using UnityEngine;

namespace Knit.FacialExpression
{
	internal sealed class BlendShapeBlender
	{
		internal float Lerp( float weight, float t)
		{
			float p = m_StartAction?.Lerp( weight) ?? 0;
			float q = m_EndAction?.Lerp( weight) ?? 0;
			return Mathf.Lerp( p, q, t);
		}
		internal BlendShapeAction StartAction
		{
			get{ return m_StartAction; }
			set{ m_StartAction = value; }
		}
		internal BlendShapeAction EndAction
		{
			get{ return m_EndAction; }
			set{ m_EndAction = value; }
		}
		internal bool EyeBlink
		{
			get{ return (m_EndAction?.EyeBlink ?? false) || (m_StartAction?.EyeBlink ?? false); }
		}
		internal bool LipSync
		{
			get{ return (m_EndAction?.LipSync ?? false) || (m_StartAction?.LipSync ?? false); }
		}
		BlendShapeAction m_StartAction;
		BlendShapeAction m_EndAction;
	}
}