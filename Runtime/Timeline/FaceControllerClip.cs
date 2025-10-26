
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.FacialExpression
{
	[System.Serializable]
	sealed class FaceControllerClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			return ScriptPlayable<FaceControllerBehaviour>.Create( graph, m_Source);
		}
		internal void Initialize( FaceController controller, BlendShapeWeight eyeFormWeight, BlendShapeWeight lipFormWeight)
		{
			m_Controller = controller;
		#if UNITY_EDITOR
			m_Inspector.Configure( controller?.Renderer, m_Source.Setting, eyeFormWeight, lipFormWeight);
		#endif
		}
		internal FaceController Controller
		{
			get{ return m_Controller; }
		}
	#if UNITY_EDITOR
		internal BlendShapeInspector Inspector
		{
			get{ return m_Inspector; }
		}
		[SerializeField, HideInInspector]
        BlendShapeInspector m_Inspector;
		[SerializeField, HideInInspector]
	#endif
		FaceController m_Controller;
		[SerializeField]
		FaceControllerBehaviour m_Source;
	}
}
