
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Audio;
using System.Net.Http.Headers;

namespace Knit.FacialExpression
{
	[TrackClipType( typeof( VocalizeClip))]
	[TrackBindingType( typeof( VocalizeSource))]
	[TrackColor( 255.0f / 255.0f, 162.0f / 255.0f, 0.0f / 255.0f)]
	sealed class VocalizeTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			if( go.GetComponent<PlayableDirector>().GetGenericBinding( this) is VocalizeSource vocalize)
			{
				foreach( var clip in GetClips())
				{
					if( clip.asset is VocalizeClip vocalizeClip)
					{
						vocalizeClip.Initialize( vocalize.AudioSource);
					}
				}
			}
			return ScriptPlayable<VocalizeMixerBehaviour>.Create( graph, inputCount);
		}
	}
}
