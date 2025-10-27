
using UnityEngine;

namespace Knit.FacialExpression
{
	[System.Serializable]
	public sealed partial class BlendShapeAction
	{
		internal BlendShapeAction( BlendShapeElement source)
		{
			m_BlendShapeName = source.BlendShapeName;
		}
		internal BlendShapeAction( BlendShapeAction source)
		{
			m_BlendShapeName = source.BlendShapeName;
			m_MinValue = source.m_MinValue;
			m_MaxValue = source.m_MaxValue;
			m_Attribute = source.m_Attribute;
		}
		internal float Lerp( float t)
		{
			return (Reverse == false)?
				Mathf.Lerp( m_MinValue, m_MaxValue, t):
				Mathf.Lerp( m_MaxValue, m_MinValue, t);
		}
		internal string BlendShapeName
		{
			get{ return m_BlendShapeName; }
		}
		internal float MinValue
		{
			get{ return m_MinValue; }
		}
		internal float MaxValue
		{
			get{ return m_MaxValue; }
		}
		internal bool Reverse
		{
			get{ return (m_Attribute & BlendShapeAttribute.Reverse) != 0; }
		}
		internal bool Blendable
		{
			get{ return (m_Attribute & BlendShapeAttribute.Blendable) != 0; }
		}
		internal bool EyeBlink
		{
			get{ return (m_Attribute & BlendShapeAttribute.EyeBlink) != 0 && m_MinValue != m_MaxValue; }
		}
		internal bool LipSync
		{
			get{ return (m_Attribute & BlendShapeAttribute.LipSync) != 0 && m_MinValue != m_MaxValue; }
		}
		[SerializeField]
		string m_BlendShapeName;
		[SerializeField]
		float m_MinValue;
		[SerializeField]
		float m_MaxValue;
		[SerializeField]
		BlendShapeAttribute m_Attribute;
	}
}