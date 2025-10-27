
using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace Knit.FacialExpression
{
	sealed class FaceControllerMixerBehaviour : PlayableBehaviour
	{
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			if( playerData is FaceController component)
			{
				if( m_Component == null)
				{
					m_Component = component;
					
					foreach( var blendShapeIndex in component.BlendShapeIndices.Values)
					{
						m_BlendShapeClips.Add( blendShapeIndex.value, new BlendShapeClip( blendShapeIndex.parts));
					}
					m_Component.SetTimeline( m_BlendShapeClips);
				}
				if( m_Component != null)
				{
					int inputCount = playable.GetInputCount();
					
					
					foreach( var clip in m_BlendShapeClips.Values)
					{
						clip.MinValue = 0;
						clip.MaxValue = 0;
					}
					for( int i0 = 0; i0 < inputCount; ++i0)
					{
						float inputWeight = playable.GetInputWeight( i0);
						float inputStep = (inputWeight < 0.5f)? 0.0f : 1.0f;
						
						if( inputWeight > 0.0f)
						{
							var inputPlayable = (ScriptPlayable<FaceControllerBehaviour>)playable.GetInput( i0);
							FaceControllerBehaviour behaviour = inputPlayable.GetBehaviour();
							List<BlendShapeAction> actions = behaviour.Setting?.GetActions();
							
							if( actions != null)
							{
								foreach( var action in actions)
								{
									if( component.BlendShapeIndices.TryGetValue( action.BlendShapeName, out var index) != false
									&&	m_BlendShapeClips.TryGetValue( index.value, out BlendShapeClip value) != false)
									{
										if( action.Blendable == false)
										{
											value.MinValue += action.MinValue * inputStep;
											value.MaxValue += action.MaxValue * inputStep;
										}
										else if( action.Reverse == false)
										{
											value.MinValue += action.MinValue * inputWeight;
											value.MaxValue += action.MaxValue * inputWeight;
										}
										else
										{
											value.MinValue += action.MaxValue * inputWeight;
											value.MaxValue += action.MinValue * inputWeight;
										}
									}
								}
							}
						}
					}
				#if UNITY_EDITOR
					if( Application.isPlaying == false)
					{
						m_Component.EvaluateTimeline();
					}
				#endif
				}
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_Component != null)
			{
				m_Component.SetTimeline( null);
				m_Component = null;
			}
		}
		FaceController m_Component;
        readonly Dictionary<int, BlendShapeClip> m_BlendShapeClips = new();
	}
}
