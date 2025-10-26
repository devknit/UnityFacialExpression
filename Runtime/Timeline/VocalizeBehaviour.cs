
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace Knit.FacialExpression
{
	[System.Serializable]
	internal sealed class VocalizeBehaviour : PlayableBehaviour
	{
		internal void Initialize( AudioSource audioSource)
		{
			m_AudioSource = audioSource;
		}
		internal double GetDuration()
		{
			AnimationCurve curve = m_LipSync?.Curve;
			AudioClip clip = Clip;
			
			if( curve != null || clip != null)
			{
				double duration = 0;
				
				if( curve != null)
				{
					duration = System.Math.Max( duration, curve[ curve.length - 1].time);
				}
				if( clip != null)
				{
					duration = System.Math.Max( duration, clip.length);
				}
				return duration;
			}
			return PlayableBinding.DefaultDuration;
		}
		public override void OnBehaviourPlay( Playable playable, FrameData info)
		{
		#if UNITY_EDITOR
			if( Application.isPlaying == false && TimelineEditor.masterDirector != null)
			{
				if( TimelineEditor.masterDirector.state != PlayState.Playing)
				{
					return;
				}
			}
		#endif
			PlayAudioClip( playable);
		}
		public override void OnBehaviourPause( Playable playable, FrameData info)
		{
			m_IsPlaying = false;
			m_AudioSource?.Stop();
			m_Component?.SetLipSyncCurve( null);
		}
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
		#if UNITY_EDITOR
			if( Application.isPlaying == false && TimelineEditor.masterDirector != null)
			{
				if( m_IsPlaying == false && TimelineEditor.masterDirector.state == PlayState.Playing)
				{
					PlayAudioClip( playable);
				}
			}
		#endif
			if( playerData is VocalizeSource component)
			{
				if( m_LipSync != null)
				{
					component.SetLipSyncCurve( m_LipSync.Curve, (float)playable.GetTime());
				#if UNITY_EDITOR
					if( Application.isPlaying == false)
					{
						component.EvaluateTimeline();
					}
				#endif
				}
				m_Component = component;
			}
		}
		void PlayAudioClip( Playable playable)
		{
			if( m_AudioSource != null && Clip != null)
			{
				m_AudioSource.clip = Clip;
				m_AudioSource.time = (float)playable.GetTime();
				m_AudioSource.Play();
			}
			m_IsPlaying = true;
		}
		internal AudioClip Clip
		{
			get
			{
				// if( m_LipSync?.Clip != null)
				// {
				// 	return m_LipSync.Clip;
				// }
				return m_AudioClip;
			}
		}
		internal AnimationCurve Curve
		{
			get{ return m_LipSync?.Curve; }
		}
		[SerializeField]
		LipSync.SingleCurve m_LipSync;
		[SerializeField]
		AudioClip m_AudioClip;
		[System.NonSerialized]
		AudioSource m_AudioSource;
		[System.NonSerialized]
		VocalizeSource m_Component;
		[System.NonSerialized]
		bool m_IsPlaying;
	}
}
