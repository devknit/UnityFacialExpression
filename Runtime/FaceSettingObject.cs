
using UnityEngine;
using System.Collections.Generic;

namespace Knit.FacialExpression
{
	public interface IFaceSetting
	{
		List<BlendShapeAction> GetActions();
	#if UNITY_EDITOR
		void Clear();
		void Copy( IFaceSetting source);
		void SetValue( BlendShapeElement source);
		bool TryGetAction( string blendShapeName, out BlendShapeAction action);
	#endif
	}
	public sealed partial class FaceSettingObject : ScriptableObject, IFaceSetting
	{
		public List<BlendShapeAction> GetActions()
		{
			return m_Elements;
		}
		[SerializeField]
		List<BlendShapeAction> m_Elements = new();
	}
	[System.Serializable]
	public sealed partial class FaceSettingProperty : IFaceSetting
	{
		public List<BlendShapeAction> GetActions()
		{
			return m_Elements;
		}
		[SerializeField]
		List<BlendShapeAction> m_Elements = new();
	}
}