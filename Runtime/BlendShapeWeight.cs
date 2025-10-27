
using UnityEngine;

namespace Knit.FacialExpression
{
	[System.Serializable]
	internal sealed class BlendShapeWeight
	{
	#if UNITY_EDITOR
		internal bool Slider( Rect position, string label)
		{
			UnityEditor.EditorGUI.BeginChangeCheck();
			m_Value = UnityEditor.EditorGUI.Slider( position, label, m_Value, 0.0f, 1.0f);
			return UnityEditor.EditorGUI.EndChangeCheck();
		}
	#endif
		internal float Reset()
		{
			m_Value = 0;
			return Weight;
		}
		internal float Weight
		{
			get{ return 1.0f - m_Value; }
		}
		[SerializeField]
		float m_Value;
	}
}