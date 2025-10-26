
using UnityEngine;

namespace Knit.FacialExpression
{
	public interface ILipSync
	{
		void SetLipSyncCurve( AnimationCurve curve, float currentSeconds = -1.0f);
	}
	public class VocalizeSource : MonoBehaviour, ILipSync
	{
		public virtual void SetLipSyncCurve( AnimationCurve curve, float currentSeconds = -1.0f)
		{
		}
		internal virtual void EvaluateTimeline()
		{
		}
	#if ENABLE_AUDIO
		public AudioSource AudioSource
		{
			get{ return m_AudioSource; }
		}
		[SerializeField]
		AudioSource m_AudioSource;
	#endif
	}
}