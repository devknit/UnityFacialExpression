#if UNITY_EDITOR

namespace Knit.FacialExpression
{
	public sealed partial class BlendShapeAction
	{
		internal bool SetValue( BlendShapeElement source)
		{
			if( m_BlendShapeName == source.BlendShapeName)
			{
				switch( source)
				{
					case BlendShapeToggle toggle:
					{
						m_MinValue = m_MaxValue = (toggle.IsValid() == false)? 0.0f : 1.0f;
						m_Attribute = toggle.Attribute;
						return true;
					}
					case BlendShapeValue value:
					{
						m_MinValue = m_MaxValue = value.Value;
						m_Attribute = value.Attribute;
						return true;
					}
					case BlendShapeRange range:
					{
						m_MinValue = range.MinValue;
						m_MaxValue = range.MaxValue;
						m_Attribute = range.Attribute;
						return true;
					}
				}
			}
			return false;
		}
	}
}
#endif
