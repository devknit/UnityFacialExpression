
using UnityEngine;

namespace Knit.FacialExpression
{
	public abstract partial class BlendShapeElement
	{
		public BlendShapeElement( int index, string displayName, string blendShapeName, BlendShapeAttribute attribute)
		{
			m_BlendShapeName = blendShapeName;
			m_Attribute = attribute;
		#if UNITY_EDITOR
			m_Index = index;
			m_DisplayName = displayName;
		#endif
		}
		public string BlendShapeName
		{
			get{ return m_BlendShapeName; }
		}
		public BlendShapeAttribute Attribute
		{
			get{ return m_Attribute; }
			protected set{ m_Attribute = value; }
		}
		[SerializeField]
		string m_BlendShapeName;
		[SerializeField]
		BlendShapeAttribute m_Attribute;
	}
	internal sealed partial class BlendShapeToggle : BlendShapeElement
	{
		public BlendShapeToggle( int index, string displayName, string blendShapeName, bool enabled, BlendShapeAttribute attribute)
			: base( index, displayName, blendShapeName, attribute)
		{
			m_Enabled = enabled;
		}
		[SerializeField]
		bool m_Enabled;
	}
	internal sealed partial class BlendShapeValue : BlendShapeElement
	{
		public BlendShapeValue( int index, string displayName, string blendShapeName, float value, BlendShapeAttribute attribute)
			: base( index, displayName, blendShapeName, attribute)
		{
			m_Value = value;
		}
		public float Value
		{
			get{ return m_Value; }
		}
		[SerializeField]
		float m_Value;
	}
	internal sealed partial class BlendShapeRange : BlendShapeElement
	{
		public BlendShapeRange( int index, string displayName, string blendShapeName, BlendShapeAction action, BlendShapeAttribute attribute)
			: base( index, displayName, blendShapeName, attribute)
		{
			if( action != null)
			{
				m_MinValue = action.MinValue;
				m_MaxValue = action.MaxValue;
				Reverse = action.Reverse;
			}
			else
			{
				m_MinValue = 0;
				m_MaxValue = 0;
				Reverse = false;
			}
		}
		public float MinValue
		{
			get{ return m_MinValue; }
		}
		public float MaxValue
		{
			get{ return m_MaxValue; }
		}
		public bool Reverse
		{
			get{ return (Attribute & BlendShapeAttribute.Reverse) != 0; }
			private set
			{
				if( value != false)
				{
					Attribute |= BlendShapeAttribute.Reverse;
				}
				else
				{
					Attribute &= ~BlendShapeAttribute.Reverse;
				}
			}
		}
		[SerializeField]
		float m_MinValue;
		[SerializeField]
		float m_MaxValue;
	}
}