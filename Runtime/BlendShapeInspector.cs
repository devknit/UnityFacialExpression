#if UNITY_EDITOR

using UnityEngine;

namespace Knit.FacialExpression
{
	[System.Serializable]
	internal sealed class BlendShapeInspector
	{
		internal void Configure( SkinnedMeshRenderer renderer, IFaceSetting setting, BlendShapeWeight eyeFormWeight, BlendShapeWeight lipFormWeight)
		{
			m_IrisType = new BlensShapeToggleGroup( FaceController.kPrefixIrisType, BlendShapeAttribute.None);
			m_IrisForm = new BlensShapeValueGroup( FaceController.kPrefixIrisForm, BlendShapeAttribute.None);
			m_TearsForm = new BlensShapeValueGroup( FaceController.kPrefixTearsForm, BlendShapeAttribute.None);
			m_Eyebrow = new BlensShapeValueGroup( FaceController.kPrefixEyebrow, BlendShapeAttribute.None);
			m_EyeForm = new BlensShapeEyeFormGroup( FaceController.kPrefixEyeForm, eyeFormWeight);
			m_LipForm = new BlensShapeLipFormGroup( FaceController.kPrefixLipForm, lipFormWeight);
			m_TongueForm = new BlensShapeValueGroup( FaceController.kPrefixTongueForm, BlendShapeAttribute.None);
			m_IrisType.Configure( renderer, setting);
			m_IrisForm.Configure( renderer, setting);
			m_Eyebrow.Configure( renderer, setting);
			m_EyeForm.Configure( renderer, setting);
			m_LipForm.Configure( renderer, setting);
			m_TongueForm.Configure( renderer, setting);
		}
		internal void Reset( SkinnedMeshRenderer renderer, IFaceSetting setting)
		{
			m_IrisType.Reset( renderer, setting);
			m_IrisForm.Reset( renderer, setting);
			m_TearsForm.Reset( renderer, setting);
			m_Eyebrow.Reset( renderer, setting);
			m_EyeForm.Reset( renderer, setting);
			m_LipForm.Reset( renderer, setting);
			m_TongueForm.Reset( renderer, setting);
		}
		internal void Flush( IFaceSetting setting)
		{
			m_IrisType.Flush( setting);
			m_IrisForm.Flush( setting);
			m_TearsForm.Flush( setting);
			m_Eyebrow.Flush( setting);
			m_EyeForm.Flush( setting);
			m_LipForm.Flush( setting);
			m_TongueForm.Flush( setting);
		}
		internal float GetPropertyHeight()
		{
			float height = 0;
			height += m_IrisType.GetPropertyHeight();
			height += m_IrisForm.GetPropertyHeight();
			height += m_TearsForm.GetPropertyHeight();
			height += m_Eyebrow.GetPropertyHeight();
			height += m_EyeForm.GetPropertyHeight();
			height += m_LipForm.GetPropertyHeight();
			height += m_TongueForm.GetPropertyHeight();
			return height;
		}
		internal bool OnInspectorGUI( Rect position, FaceController faceController, IFaceSetting setting)
		{
			bool bChangeValue = false;
			
			if( m_IrisType.OnInspectorGUI( ref position, faceController, setting) != false)
			{
				bChangeValue = true;
			}
			if( m_IrisForm.OnInspectorGUI( ref position, faceController, setting) != false)
			{
				bChangeValue = true;
			}
			if( m_TearsForm.OnInspectorGUI( ref position, faceController, setting) != false)
			{
				bChangeValue = true;
			}
			if( m_Eyebrow.OnInspectorGUI( ref position, faceController, setting) != false)
			{
				bChangeValue = true;
			}
			if( m_EyeForm.OnInspectorGUI( ref position, faceController, setting) != false)
			{
				bChangeValue = true;
			}
			if( m_LipForm.OnInspectorGUI( ref position, faceController, setting) != false)
			{
				bChangeValue = true;
			}
			if( m_TongueForm.OnInspectorGUI( ref position, faceController, setting) != false)
			{
				bChangeValue = true;
			}
			return bChangeValue;
		}
		[SerializeField]
		BlensShapeToggleGroup m_IrisType;
		[SerializeField]
		BlensShapeValueGroup m_IrisForm;
		[SerializeField]
		BlensShapeValueGroup m_TearsForm;
		[SerializeField]
		BlensShapeValueGroup m_Eyebrow;
		[SerializeField]
		BlensShapeEyeFormGroup m_EyeForm;
		[SerializeField]
		BlensShapeLipFormGroup m_LipForm;
		[SerializeField]
		BlensShapeValueGroup m_TongueForm;
	}
}
#endif
