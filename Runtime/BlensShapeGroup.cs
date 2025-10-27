#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Knit.FacialExpression
{
	[System.Serializable]
	internal abstract class BlensShapeGroup<T> where T : BlendShapeElement
	{
		public abstract T CreateElement( int index, string displayName, string blendShapeName, BlendShapeAction action);
		
		public BlensShapeGroup( string prefix, BlendShapeAttribute attribute)
		{
			m_Prefix = prefix.Trim( '_');
			m_Attribute = attribute;
			m_Elements = new();
		}
		internal virtual float GetPropertyHeight()
		{
			float height = 0;
			
			if( m_Elements.Count > 0)
			{
				height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				
				if( m_Foldout != false)
				{
					height += m_Elements.Count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
				}
			}
			return height;
		}
		public bool OnInspectorGUI( ref Rect position, FaceController faceController, IFaceSetting setting)
		{
			bool bChangeValue = false;
			
			if( m_Elements.Count > 0)
			{
				m_Foldout = Foldout( ref position, m_Foldout, m_Prefix, faceController?.Renderer, setting, ref bChangeValue);
				
				if( m_Foldout != false)
				{
					++EditorGUI.indentLevel;
					
					foreach( var element in m_Elements)
					{
						if( element.OnInspectorGUI( ref position, faceController?.Renderer, setting, Weight) != false)
						{
							bChangeValue = true;
						}
					}
					if( OnInspectorNestedGUI( ref position, faceController, setting) != false)
					{
						bChangeValue = true;
					}
					--EditorGUI.indentLevel;
				}
			}
			return bChangeValue;
		}
		protected virtual bool OnInspectorNestedGUI( ref Rect position, FaceController faceController, IFaceSetting setting)
		{
			return false;
		}
		public void Configure( SkinnedMeshRenderer renderer, IFaceSetting setting)
		{
			m_Elements.Clear();
			
			Mesh mesh = renderer?.sharedMesh;
			
			if( mesh != null)
			{
				for( int i0 = 0; i0 < mesh.blendShapeCount; ++i0)
				{
					string blendShapeName = mesh.GetBlendShapeName( i0);
					string[] blendShapeNames = blendShapeName.Split( '_');
					
					if( m_Prefix == blendShapeNames[ 0] && blendShapeNames.Length > 1)
					{
						BlendShapeAction action = null;
						setting?.TryGetAction( blendShapeName, out action);
						
						var element = CreateElement( i0, blendShapeNames[ 1], blendShapeName, action);
						element.SetBlendShapeWeight( renderer, setting, Weight);
						m_Elements.Add( element);
					}
				}
			}
		}
		public void Reset( SkinnedMeshRenderer renderer, IFaceSetting setting)
		{
			m_Elements.ForEach( x => x.Reset( renderer, setting, ResetWeight()));
		}
		public virtual void Flush( IFaceSetting setting)
		{
			if( m_Elements.Count > 0 && setting != null)
			{
				foreach( var element in m_Elements)
				{
					setting.SetValue( element);
				}
			}
		}
		protected bool Foldout( ref Rect position, bool foldout, string content, SkinnedMeshRenderer renderer, IFaceSetting setting, ref bool bChangeValue)
		{
			s_FoldoutStyle ??= new GUIStyle( "ShurikenModuleTitle")
			{
				font = new GUIStyle( EditorStyles.boldLabel).font,
				// border = new RectOffset( 15, 7, 4, 4),
				// fixedHeight = 22,
				// contentOffset = new Vector2( 20f, -2f)
			};
			Rect rect = position;
			rect.y += EditorGUIUtility.standardVerticalSpacing;
			rect.height = EditorGUIUtility.singleLineHeight;
			rect.xMax -= 50;
			GUI.Box( rect, content, s_FoldoutStyle);
			position.yMin += rect.height + EditorGUIUtility.standardVerticalSpacing;			
			var ev = Event.current;
			var toggleRect = new Rect( rect.x + 4f, rect.y + 2f, 13f, 13f);
			
			if( ev.type == EventType.Repaint)
			{
				EditorStyles.foldout.Draw( toggleRect, false, false, foldout, false);
			}
			if( ev.type == EventType.MouseDown && rect.Contains( ev.mousePosition))
			{
				foldout = !foldout;
				ev.Use();
			}
			rect.y -= 1;
			rect.xMax += 50;
			rect.xMin = rect.xMax - 46;
			
			if( GUI.Button( rect, "Reset") != false)
			{
				m_Elements.ForEach( x => x.Reset( renderer, setting, ResetWeight()));
				bChangeValue = true;
			}
			return foldout;
		}
		protected virtual float ResetWeight()
		{
			return 0;
		}
		protected virtual float Weight
		{
			get{ return 0; }
		}
		static GUIStyle s_FoldoutStyle;
		[SerializeField]
		protected bool m_Foldout = true;
		[SerializeField]
		protected string m_Prefix;
		[SerializeField]
		protected BlendShapeAttribute m_Attribute;
		[SerializeField]
		protected List<T> m_Elements;
	}
	[System.Serializable]
	internal sealed class BlensShapeToggleGroup : BlensShapeGroup<BlendShapeToggle>
	{
		public BlensShapeToggleGroup( string prefix, BlendShapeAttribute attribute) : base( prefix, attribute)
		{
		}
		public override BlendShapeToggle CreateElement( int index, string displayName, string blendShapeName, BlendShapeAction action)
		{
			return new BlendShapeToggle( index, displayName, blendShapeName, (action?.MinValue ?? 0) > 0.0f, m_Attribute);
		}
	}
	[System.Serializable]
	internal sealed class BlensShapeValueGroup : BlensShapeGroup<BlendShapeValue>
	{
		public BlensShapeValueGroup( string prefix, BlendShapeAttribute attribute) : base( prefix, BlendShapeAttribute.Blendable | attribute)
		{
		}
		public override BlendShapeValue CreateElement( int index, string displayName, string blendShapeName, BlendShapeAction action)
		{
			return new BlendShapeValue( index, displayName, blendShapeName, action?.MinValue ?? 0, m_Attribute);
		}
	}
	[System.Serializable]
	internal class BlensShapeRangeGroup : BlensShapeGroup<BlendShapeRange>
	{
		public BlensShapeRangeGroup( string prefix, string label, BlendShapeWeight weight, BlendShapeAttribute attribute) : base( prefix, BlendShapeAttribute.Blendable | attribute)
		{
			m_Label = label;
			m_Weight = weight ?? new BlendShapeWeight();
		}
		public override BlendShapeRange CreateElement( int index, string displayName, string blendShapeName, BlendShapeAction action)
		{
			return new BlendShapeRange( index, displayName, blendShapeName, action, m_Attribute);
		}
		internal override float GetPropertyHeight()
		{
			float height = base.GetPropertyHeight();
			height += 38;
			return height;
		}
		protected override bool OnInspectorNestedGUI( ref Rect position, FaceController faceController, IFaceSetting setting)
		{
			if( OnSimulateGUI( ref position, faceController, m_Elements.Any( x => x.MinValue != x.MaxValue)) != false)
			{
				m_Elements.ForEach( x => x.SetBlendShapeWeight( faceController?.Renderer, setting, Weight));
				return true;
			}
			return false;
		}
		protected override float ResetWeight()
		{
			return m_Weight.Reset();
		}
		protected bool OnSimulateGUI( ref Rect position, FaceController faceController, bool simulatable)
		{
			GetWeightRect( ref position, out Rect outerRect, out Rect innerRect);
			
			if( simulatable == false)
			{
				EditorGUI.HelpBox( outerRect, GetSimulateWarning(), MessageType.Warning);
				return false;
			}
			GUI.Box( outerRect, string.Empty, EditorStyles.helpBox);
			return OnWeightGUI( innerRect, faceController);
		}
		protected virtual string GetSimulateWarning()
		{
			return "Simulation not supported.";
		}
		protected virtual bool OnWeightGUI( Rect innerRect, FaceController faceController)
		{
			if( Application.isPlaying == false)
			{
				return m_Weight.Slider( innerRect, m_Label);
			}
			return false;
		}
		void GetWeightRect( ref Rect position, out Rect outerRect, out Rect innerRect)
		{
			const float height = 38;
			const float margin = height / 4.0f;
			outerRect = position;
			outerRect.height = height;
			outerRect.xMin += 15;
			position.yMin += outerRect.height + EditorGUIUtility.standardVerticalSpacing;
			innerRect = outerRect;
			innerRect.xMax -= 16;
			innerRect.yMin += margin;
			innerRect.yMax -= margin;
		}
		protected string Label
		{
			get{ return m_Label; }
		}
		protected override float Weight
		{
			get{ return m_Weight.Weight; }
		}
		[SerializeField]
		string m_Label;
		[SerializeField]
		BlendShapeWeight m_Weight;
	}
	[System.Serializable]
	internal class BlensShapeEyeFormGroup : BlensShapeRangeGroup
	{
		public BlensShapeEyeFormGroup( string prefix, BlendShapeWeight weight) : base( prefix, "瞬きの変化量 (0:基準 1:瞬き中)", weight, BlendShapeAttribute.EyeBlink)
		{
		}
		protected override string GetSimulateWarning()
		{
			return "瞬きが出来ない設定です";
		}
	}
	[System.Serializable]
	internal class BlensShapeLipFormGroup : BlensShapeRangeGroup
	{
		public BlensShapeLipFormGroup( string prefix, BlendShapeWeight weight) : base( prefix, "リップシンクの変化量 (0: 基準 1:リップシンク中)",weight, BlendShapeAttribute.LipSync)
		{
		}
		protected override string GetSimulateWarning()
		{
			return "リップシンクが出来ない設定です";
		}
		protected override bool OnWeightGUI( Rect innerRect, FaceController faceController)
		{
			if( Application.isPlaying != false && faceController != null)
			{
				EditorGUI.LabelField( innerRect, "リップシンクテスト");
				innerRect.xMin = innerRect.xMax - 50;
				
				if( GUI.Button( innerRect, "Stop") != false)
				{
					faceController.SetLipSyncCurve( null);
				}
				innerRect.x -= 50 + 2;
				
				if( GUI.Button( innerRect, "Play") != false)
				{
					faceController.SetLipSyncCurve( GenerateLipSyncCurve());
				}
				return false;
			}
			return base.OnWeightGUI( innerRect, faceController);
		}
		static AnimationCurve GenerateLipSyncCurve()
		{
			var curve = new AnimationCurve();
			const float interval = 0.125f;
			const float duration = 10.0f;
			float elapsedTime = 0;
			float value = 0;
			
			while( elapsedTime < duration)
			{
				curve.AddKey( new Keyframe
				{
					time = elapsedTime,
					value = value
				});
				elapsedTime += interval;
				value = (value > 0.0f)? 0.0f : 1.0f;
			}
			return curve;
		}
	}
}
#endif
