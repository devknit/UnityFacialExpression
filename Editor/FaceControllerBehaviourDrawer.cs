
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using System.IO;
using System.Reflection;

namespace Knit.FacialExpression
{
	[CustomPropertyDrawer( typeof( FaceControllerBehaviour), true)]
	sealed class FaceControllerBehaviourDrawer : PropertyDrawer 
	{
		public override float GetPropertyHeight( SerializedProperty property, GUIContent label)
		{
			float height = 0;
			
			if( property.serializedObject.targetObject is FaceControllerClip clip)
			{
				height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				
				if( clip.Controller != null)
				{
					height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
					height += clip.Inspector.GetPropertyHeight();
				}
			}
			return height;
		}
		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label)
		{
			const BindingFlags kBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			
			if( property.serializedObject.targetObject is FaceControllerClip clip
			&&	clip.GetType().GetField( property.propertyPath, kBindingFlags)?.GetValue( clip) is FaceControllerBehaviour behaviour)
			{
				position.height = EditorGUIUtility.singleLineHeight;
				var prevSetting = behaviour.m_Setting;
				
				var settingLabel = new GUIContent( "Setting");
				EditorGUI.BeginProperty( position, settingLabel, property);
				EditorGUI.BeginChangeCheck();
				behaviour.m_Setting = EditorGUI.ObjectField( position, settingLabel, behaviour.m_Setting, typeof( FaceSettingObject), true) as FaceSettingObject;
				if( EditorGUI.EndChangeCheck() != false)
				{
					if( behaviour.m_Setting != null)
					{
						behaviour.m_Property.Clear();
					}
					else if( prevSetting != null)
					{
						behaviour.m_Property.Copy( prevSetting);
					}
				}
				EditorGUI.EndProperty();
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				
				if( property.hasMultipleDifferentValues != false)
				{
					EditorGUI.HelpBox( position, "Multiple clips selected. Only common properties are shown.", MessageType.None);
				}
				else
				{
					
					
					if( clip.Controller != null)
					{
						Rect generateRect = position;
						generateRect.xMin = generateRect.xMax - 68;
						Rect resetRect = generateRect;
						resetRect.x -= generateRect.width + 2;
						resetRect.xMin = resetRect.xMax - 44;
						
						Mesh mesh = clip.Controller.Renderer?.sharedMesh;
						
						if( behaviour.m_Setting != null)
						{
							Rect toggleRect = resetRect;
							toggleRect.x -= resetRect.width + 2;
							toggleRect.xMin = toggleRect.xMax - 38;
							
							s_Edittable = GUI.Toggle( toggleRect, s_Edittable, "Edit", EditorStyles.miniButton);
						}
						if( GUI.Button( resetRect, "Reset") != false)
						{
							clip.Inspector.Reset( clip.Controller.Renderer, behaviour.Setting);
						}
						if( GUI.Button( generateRect, "Generate") != false)
						{
							var settingObject = ScriptableObject.CreateInstance<FaceSettingObject>();
							settingObject.Copy( behaviour.Setting);
							behaviour.m_Setting = settingObject;
							behaviour.m_Property.Clear();
							
							string playablePath = AssetDatabase.GetAssetPath( clip);
							string meshPath = AssetDatabase.GetAssetPath( mesh);
							string directory = Path.GetDirectoryName( playablePath);
							string pFileName = Path.GetFileNameWithoutExtension( playablePath);
							string qFileName = Path.GetFileNameWithoutExtension( meshPath);
							string assetPath = Path.Combine( directory, $"{pFileName}@{qFileName}.asset");
							
							AssetDatabase.CreateAsset( settingObject, assetPath);
							AssetDatabase.ImportAsset( assetPath);
							EditorGUIUtility.PingObject( settingObject);
						}
						position.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
						
						using( new EditorGUI.DisabledGroupScope( behaviour.m_Setting != null && s_Edittable == false))
						{
							if( clip.Inspector.OnInspectorGUI( position, null, behaviour.Setting) != false)
							{
								TimelineEditor.Refresh( RefreshReason.ContentsModified);
							}
						}
					}
				}
			}
		}
		static bool s_Edittable;
	}
}