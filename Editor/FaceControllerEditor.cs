
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Knit.FacialExpression
{
	[CustomEditor( typeof( FaceController))]
	sealed class FacialExpressionEditor : UnityEditor.Editor
	{
		void OnEnable()
		{
			m_AlternativeSetting = new();
			m_RendererProperty = serializedObject.FindProperty( "m_Renderer");
			m_SettingProperty = serializedObject.FindProperty( "m_Setting");
			Configure();
		}
		void OnDisable()
		{
			m_AlternativeSetting = null;
		}
		void Configure()
		{
			var renderer = m_RendererProperty.objectReferenceValue as SkinnedMeshRenderer;
			
			if( m_SettingProperty.objectReferenceValue is not IFaceSetting setting)
			{
				setting = m_AlternativeSetting;
			}
			if( target is FaceController faceController)
			{
				faceController.Inspector ??= new();
				faceController.Inspector.Configure( renderer, setting, null, null);
			}
		}
		public override void OnInspectorGUI()
		{
			var prevSetting = m_SettingProperty.objectReferenceValue as IFaceSetting;
			bool bReconfigure = false;
			
			using( new EditorGUI.DisabledGroupScope( true))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( m_RendererProperty);
				if( EditorGUI.EndChangeCheck() != false)
				{
					bReconfigure = true;
				}
			}
			serializedObject.UpdateIfRequiredOrScript();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( m_SettingProperty);
			if( EditorGUI.EndChangeCheck() != false)
			{
				bReconfigure = true;
			}
			serializedObject.ApplyModifiedProperties();
			
			if( target is FaceController faceController)
			{
				var setting = m_SettingProperty.objectReferenceValue as IFaceSetting ?? m_AlternativeSetting;
				Mesh mesh = faceController?.Renderer?.sharedMesh;
				
				if( m_CachedSetting != setting)
				{
					if( setting == m_AlternativeSetting && prevSetting != null)
					{
						setting.Copy( prevSetting);
					}
					bReconfigure = true;
				}
				using( new EditorGUI.DisabledGroupScope( mesh == null))
				{
					using( new EditorGUILayout.HorizontalScope())
					{
						GUILayout.FlexibleSpace();
						
						var settingObject = setting as FaceSettingObject;
						
						if( GUILayout.Button( "Reset") != false)
						{
							faceController.Inspector.Reset( faceController.Renderer, setting);
						}
						if( GUILayout.Button( "Generate") != false)
						{
							setting = settingObject = CreateInstance<FaceSettingObject>();
							string assetPath = AssetDatabase.GenerateUniqueAssetPath(
								Path.ChangeExtension( AssetDatabase.GetAssetPath( mesh), ".asset"));
							faceController.Inspector.Flush( setting);
							AssetDatabase.CreateAsset( settingObject, assetPath);
							AssetDatabase.ImportAsset( assetPath);
							EditorGUIUtility.PingObject( settingObject);
							m_SettingProperty.objectReferenceValue = settingObject;
							serializedObject.ApplyModifiedProperties();
							bReconfigure = true;
						}
					}
				}
				if( bReconfigure != false)
				{
					Configure();
				}
				if( m_CachedSetting != setting)
				{
					m_CachedSetting = setting;
					
					if( Application.isPlaying != false)
					{
						faceController.SetAction( setting, 0.15f);
					}
				}
				Rect position = GUILayoutUtility.GetRect( 16.0f, faceController.Inspector.GetPropertyHeight());
				
				if( faceController.Inspector.OnInspectorGUI( position, faceController, setting) != false)
				{
					if( Application.isPlaying != false)
					{
						faceController.SetAction( setting, -1);
					}
				}
			}
		}
		SerializedProperty m_RendererProperty;
		SerializedProperty m_SettingProperty;
		IFaceSetting m_CachedSetting;
		FaceSettingProperty m_AlternativeSetting;
	}
}