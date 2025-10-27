
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel; 

namespace Knit.FacialExpression
{
	[TrackClipType( typeof( FaceControllerClip))]
	[TrackBindingType( typeof( FaceController))]
	[TrackColor( 127.0f / 255.0f, 252.0f / 255.0f, 228.0f / 255.0f)]
	[DisplayName( "Knit.Timeline/Face Controller Track")]
	sealed class FaceControllerTrack : TrackAsset, ILayerable
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			if( go.GetComponent<PlayableDirector>().GetGenericBinding( this) is FaceController controller)
			{
				InitializeClips( this, controller);
			}
			return ScriptPlayable<FaceControllerMixerBehaviour>.Create( graph, inputCount);
		}
		void InitializeClips( TrackAsset track, FaceController controller)
		{
			foreach( var clip in track.GetClips())
			{
				if( clip.asset is FaceControllerClip asset)
				{
					asset.Initialize( controller, m_EyeFormWeight, m_LipFormWeight);
				}
			}
			foreach( var childTrack in track.GetChildTracks())
			{
				InitializeClips( childTrack, controller);
			}
		}
		Playable ILayerable.CreateLayerMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return Playable.Null;
		}
		[SerializeField]
		BlendShapeWeight m_EyeFormWeight;
		[SerializeField]
		BlendShapeWeight m_LipFormWeight;
	}
}
