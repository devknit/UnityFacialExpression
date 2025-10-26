
using UnityEngine;

namespace Knit.FacialExpression
{
	internal class BlendShapeClip
	{
		internal BlendShapeClip( BlendShapeParts parts)
		{
			m_Parts = parts;
		}
		internal BlendShapeClip( BlendShapeClip source)
		{
			m_Parts = source.m_Parts;
			m_MinValue = source.m_MinValue;
			m_MaxValue = source.m_MaxValue;
		}
		internal float Lerp( float t)
		{
			return Mathf.Lerp( Mathf.Clamp01( m_MinValue), Mathf.Clamp01( m_MaxValue), t);
		}
		internal bool IsRange()
		{
			return m_MinValue != m_MaxValue;
		}
		internal BlendShapeParts Parts
		{
			get{ return m_Parts; }
		}
		internal float MinValue
		{
			get{ return m_MinValue; }
			set{ m_MinValue = value; }
		}
		internal float MaxValue
		{
			get{ return m_MaxValue; }
			set{ m_MaxValue = value; }
		}
		[SerializeField]
		BlendShapeParts m_Parts;
		[SerializeField]
		float m_MinValue;
		[SerializeField]
		float m_MaxValue;
	}
}