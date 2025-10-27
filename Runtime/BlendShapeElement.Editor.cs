#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Knit.FacialExpression
{
	[System.Serializable]
	public abstract partial class BlendShapeElement
	{
		public abstract bool OnInspectorGUI( ref Rect position, SkinnedMeshRenderer renderer, IFaceSetting setting, float weight);
		public abstract void SetBlendShapeWeight( SkinnedMeshRenderer renderer, IFaceSetting setting, float weight);
		public abstract void Reset( SkinnedMeshRenderer renderer, IFaceSetting setting, float weight);
		public abstract bool IsValid();
		
		public int Index
		{
			get{ return m_Index; }
		}
		public string DisplayName
		{
			get{ return m_DisplayName; }
		}
		[SerializeField]
		int m_Index;
		[SerializeField]
		string m_DisplayName;
	}
	[System.Serializable]
	internal sealed partial class BlendShapeToggle : BlendShapeElement
	{
		public override bool OnInspectorGUI( ref Rect position, SkinnedMeshRenderer renderer, IFaceSetting setting, float weight)
		{
			Rect toggleRect = position;
			toggleRect.height = EditorGUIUtility.singleLineHeight;
			position.yMin += toggleRect.height + EditorGUIUtility.standardVerticalSpacing;
			
			EditorGUI.BeginChangeCheck();
			m_Enabled = EditorGUI.Toggle( toggleRect, DisplayName, m_Enabled);
			if( EditorGUI.EndChangeCheck() != false)
			{
				SetBlendShapeWeight( renderer, setting, weight);
				return true;
			}
			return false;
		}
		public override void SetBlendShapeWeight( SkinnedMeshRenderer renderer, IFaceSetting setting, float weight)
		{
			if( Application.isPlaying == false)
			{
				renderer?.SetBlendShapeWeight( Index, (m_Enabled == false)? 0 : 100);
			}
			setting?.SetValue( this);
		}
		public override void Reset( SkinnedMeshRenderer renderer, IFaceSetting setting, float weight)
		{
			m_Enabled = false;
			
			if( Application.isPlaying == false)
			{
				renderer?.SetBlendShapeWeight( Index, (m_Enabled == false)? 0 : 100);
			}
			setting?.SetValue( this);
		}
		public override bool IsValid()
		{
			return m_Enabled;
		}
	}
	[System.Serializable]
	internal sealed partial class BlendShapeValue : BlendShapeElement
	{
		public override bool OnInspectorGUI( ref Rect position, SkinnedMeshRenderer renderer, IFaceSetting setting, float weight)
		{
			Rect sliderRect = position;
			sliderRect.height = EditorGUIUtility.singleLineHeight;
			position.yMin += sliderRect.height + EditorGUIUtility.standardVerticalSpacing;
			
			EditorGUI.BeginChangeCheck();
			m_Value = EditorGUI.Slider( sliderRect, DisplayName, m_Value, 0.0f, 1.0f);
			if( EditorGUI.EndChangeCheck() != false)
			{
				SetBlendShapeWeight( renderer, setting, weight);
				return true;
			}
			return false;
		}
		public override void SetBlendShapeWeight( SkinnedMeshRenderer renderer, IFaceSetting setting, float weight)
		{
			if( Application.isPlaying == false)
			{
				renderer?.SetBlendShapeWeight( Index, m_Value * 100.0f);
			}
			setting?.SetValue( this);
		}
		public override void Reset( SkinnedMeshRenderer renderer, IFaceSetting setting, float weight)
		{
			m_Value = 0.0f;
			
			if( Application.isPlaying == false)
			{
				renderer?.SetBlendShapeWeight( Index, m_Value * 100.0f);
			}
			setting?.SetValue( this);
		}
		public override bool IsValid()
		{
			return m_Value > 0.0f;
		}
	}
	[System.Serializable]
	internal sealed partial class BlendShapeRange : BlendShapeElement
	{
		public override bool OnInspectorGUI( ref Rect position, SkinnedMeshRenderer renderer, IFaceSetting setting, float weight)
		{
			bool bChangeValue = false;
			var rect = position;
			rect.height = EditorGUIUtility.singleLineHeight;
			position.yMin += rect.height + EditorGUIUtility.standardVerticalSpacing;
			
			Rect labelRect = rect;
			labelRect.width = EditorGUIUtility.labelWidth - 13;
			rect.xMin += labelRect.width;
			
			EditorGUI.BeginChangeCheck();
			Reverse = EditorGUI.ToggleLeft( labelRect, DisplayName, Reverse);
			if( EditorGUI.EndChangeCheck() != false)
			{
				bChangeValue = true;
			}
			Rect sliderRect = rect;
			Rect minRect = rect;
			Rect maxRect = rect;
			minRect.width = 64;
			maxRect.width = 64;
			sliderRect.xMin += minRect.width;
			sliderRect.xMax -= maxRect.width;
			maxRect.x = sliderRect.xMax;
			
			EditorGUI.BeginChangeCheck();
			m_MinValue = EditorGUI.FloatField( minRect, m_MinValue);
			if( EditorGUI.EndChangeCheck() != false)
			{
				bChangeValue = true;
			}
			EditorGUI.BeginChangeCheck();
			EditorGUI.MinMaxSlider( sliderRect, ref m_MinValue, ref m_MaxValue, 0.0f, 1.0f);
			if( EditorGUI.EndChangeCheck() != false)
			{
				bChangeValue = true;
			}
			EditorGUI.BeginChangeCheck();
			m_MaxValue = EditorGUI.FloatField( maxRect, m_MaxValue);
			if( EditorGUI.EndChangeCheck() != false)
			{
				bChangeValue = true;
			}
			if( bChangeValue != false)
			{
				SetBlendShapeWeight( renderer, setting, weight);
			}
			return bChangeValue;
		}
		public override void SetBlendShapeWeight( SkinnedMeshRenderer renderer, IFaceSetting setting, float weight)
		{
			if( Application.isPlaying == false && renderer != null)
			{
				float value = (Reverse == false)? 
					Mathf.Lerp( m_MinValue, m_MaxValue, weight):
					Mathf.Lerp( m_MaxValue, m_MinValue, weight);
				renderer?.SetBlendShapeWeight( Index, value * 100.0f);
			}
			setting?.SetValue( this);
		}
		public override void Reset( SkinnedMeshRenderer renderer, IFaceSetting setting, float weight)
		{
			m_MinValue = m_MaxValue = 0.0f;
			Reverse = false;
			renderer?.SetBlendShapeWeight( Index, Mathf.Lerp( m_MinValue, m_MaxValue, weight) * 100.0f);
			setting?.SetValue( this);
		}
		public override bool IsValid()
		{
			return m_MinValue > 0.0f || m_MaxValue > 0.0f;
		}
	}
}
#endif
