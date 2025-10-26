
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.FacialExpression
{
	[System.Serializable]
	sealed class VocalizeClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get{ return ClipCaps.None; }
		}
		public AudioClip Clip
		{
			get{ return m_Source.Clip; }
		}
		public AnimationCurve Curve
		{
			get{ return m_Source.Curve; }
		}
		public override double duration
		{
			get{ return m_Source.GetDuration(); }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<VocalizeBehaviour>.Create( graph, m_Source);
			playable.GetBehaviour().Initialize( m_AudioSource);
			return playable;
		}
		internal void Initialize( AudioSource audioSource)
		{
			m_AudioSource = audioSource;
		}
		[SerializeField]
		VocalizeBehaviour m_Source;
		[System.NonSerialized]
		AudioSource m_AudioSource;
	}
}
