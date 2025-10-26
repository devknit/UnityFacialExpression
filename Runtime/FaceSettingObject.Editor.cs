#if UNITY_EDITOR

using UnityEngine;
using System.Linq;
using UnityEditor;

namespace Knit.FacialExpression
{
	public sealed partial class FaceSettingObject : ScriptableObject, IFaceSetting
	{
		public void Clear()
		{
			m_Elements.Clear();
		}
		public void Copy( IFaceSetting source)
		{
			m_Elements.Clear();
			
			foreach( var action in source.GetActions())
			{
				m_Elements.Add( new BlendShapeAction( action));
			}
		}
		public void SetValue( BlendShapeElement source)
		{
			BlendShapeAction action = m_Elements.FirstOrDefault( x => x.BlendShapeName == source.BlendShapeName);
			
			if( (source?.IsValid() ?? false) != false)
			{
				if( action == null)
				{
					action = new BlendShapeAction( source);
					m_Elements.Add( action);
					m_Elements = m_Elements.OrderBy( x => x.BlendShapeName).ToList();
				}
				action.SetValue( source);
			}
			else if( action != null)
			{
				m_Elements.Remove( action);
				m_Elements = m_Elements.OrderBy( x => x.BlendShapeName).ToList();
			}
			EditorUtility.SetDirty( this);
		}
		public bool TryGetAction( string blendShapeName, out BlendShapeAction action)
		{
			action = m_Elements.FirstOrDefault( x => x.BlendShapeName == blendShapeName);
			return action != null;
		}
	}
	public sealed partial class FaceSettingProperty : IFaceSetting
	{
		public void Clear()
		{
			m_Elements.Clear();
		}
		public void Copy( IFaceSetting source)
		{
			m_Elements.Clear();
			
			foreach( var action in source.GetActions())
			{
				m_Elements.Add( new BlendShapeAction( action));
			}
		}
		public void SetValue( BlendShapeElement source)
		{
			BlendShapeAction action = m_Elements.FirstOrDefault( x => x.BlendShapeName == source.BlendShapeName);
			
			if( (source?.IsValid() ?? false) != false)
			{
				if( action == null)
				{
					action = new BlendShapeAction( source);
					m_Elements.Add( action);
					m_Elements = m_Elements.OrderBy( x => x.BlendShapeName).ToList();
				}
				action.SetValue( source);
			}
			else if( action != null)
			{
				m_Elements.Remove( action);
				m_Elements = m_Elements.OrderBy( x => x.BlendShapeName).ToList();
			}
		}
		public bool TryGetAction( string blendShapeName, out BlendShapeAction action)
		{
			action = m_Elements.FirstOrDefault( x => x.BlendShapeName == blendShapeName);
			return action != null;
		}
	}
}
#endif
