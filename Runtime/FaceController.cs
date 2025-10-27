
using UnityEngine;
using System.Collections.Generic;

namespace Knit.FacialExpression
{
	[RequireComponent( typeof( SkinnedMeshRenderer))]
	public sealed class FaceController : VocalizeSource
	{
		public void SetAction( List<BlendShapeAction> actions, float seconds)
		{
			m_CurrentBlends.Clear();
			m_CurrentEyeBlinks.Clear();
			m_CurrentLipSyncs.Clear();
			string blendShapeName;
			BlendShapeBlender blender;
			
			if( seconds < 0.0f)
			{
				foreach( BlendShapeIndex index in m_BlendShapeIndices.Values)
				{
					m_Renderer.SetBlendShapeWeight( index.value, 0);
				}
			}
			if( (m_CurrentActions?.Count ?? 0) > 0)
			{
				foreach( var action in m_CurrentActions)
				{
					blendShapeName = action.BlendShapeName;
					
					if( m_BlendShapeIndices.TryGetValue( blendShapeName, out BlendShapeIndex index) != false)
					{
						if( action.Blendable == false)
						{
							m_Renderer.SetBlendShapeWeight( index.value, 0);
						}
						else
						{
							if( m_CurrentBlends.TryGetValue( index.value, out blender) == false)
							{
								blender = new BlendShapeBlender();
								m_CurrentBlends.Add( index.value, blender);
							}
							blender.StartAction = action;
						}
					}
				}
			}
			if( (actions?.Count ?? 0) > 0)
			{
				foreach( var action in actions)
				{
					blendShapeName = action.BlendShapeName;
					
					if( m_BlendShapeIndices.TryGetValue( blendShapeName, out BlendShapeIndex index) != false)
					{
						if( action.Blendable == false)
						{
							m_Renderer.SetBlendShapeWeight( index.value, action.MaxValue * 100.0f);
						}
						else
						{
							if( m_CurrentBlends.TryGetValue( index.value, out blender) == false)
							{
								blender = new BlendShapeBlender();
								m_CurrentBlends.Add( index.value, blender);
							}
							blender.EndAction = action;
						}
					}
				}
			}
			m_BlendTotalSeconds = Mathf.Max( 0.001f, seconds);
			m_BlendElapsedSeconds = 0;
			m_CurrentActions = actions;
		}
		public void SetAction( IFaceSetting setting, float seconds)
		{
			if( m_CurrentSetting != setting || seconds < 0.0f)
			{
				SetAction( setting.GetActions(), seconds);
				m_CurrentSetting = setting;
			}
		}
		public bool IsLipSyncPlaying()
		{
			return m_LipSyncSeconds < m_LipSyncDuration;
		}
		public override void SetLipSyncCurve( AnimationCurve curve, float manualSeconds = -1.0f)
		{
			if( curve != null)
			{
				float duration = (curve.length > 0)? curve[ curve.length - 1].time : 0;
				
				if( manualSeconds < 0)
				{
					m_LipSyncSeconds = 0;
					m_LipSyncDuration = duration;
				}
				else
				{
					m_LipSyncSeconds = Mathf.Clamp( manualSeconds, 0, duration);
					m_LipSyncDuration = 0;
				}
			}
			else
			{
				m_LipSyncSeconds = 0;
				m_LipSyncDuration = 0;
			}
			m_LipSyncCurve = curve;
		}
		internal void SetTimeline( Dictionary<int, BlendShapeClip> timelineClips)
		{
			m_CurrentTimelineClips = timelineClips;
		}
		void OnEnable()
		{
			m_Renderer = GetComponent<SkinnedMeshRenderer>();
			Mesh mesh = m_Renderer?.sharedMesh;
			
			if( mesh != null && mesh != m_CachedMesh)
			{
				m_BlendShapeIndices = new();
				
				for( int i0 = 0; i0 < mesh.blendShapeCount; ++i0)
				{
					string blendShapeName = mesh.GetBlendShapeName( i0);
					int i1;
					
					for( i1 = 0; i1 < kPrefixes.Length; ++i1)
					{
						if( blendShapeName.StartsWith( kPrefixes[ i1].name) != false)
						{
							break;
						}
					}
					if( i1 < kPrefixes.Length)
					{
						m_BlendShapeIndices.Add( blendShapeName, new BlendShapeIndex
						{
							value = i0,
							parts = kPrefixes[ i1].parts
						});
					}
				}
				m_CachedMesh = mesh;
			}
		}
	#if UNITY_EDITOR
		void Start()
		{
			SetAction( m_Setting, 0);
		}
		internal override void EvaluateTimeline()
		{
			if( m_CurrentTimelineClips != null)
			{
				float lipSyncWeight = 0;
				
				if( m_LipSyncCurve != null)
				{
					lipSyncWeight = 1.0f - m_LipSyncCurve.Evaluate( m_LipSyncSeconds);
				}
				foreach( var timeline in m_CurrentTimelineClips)
				{
					float weight = timeline.Value.Parts switch
					{
						BlendShapeParts.LipForm => lipSyncWeight,
						_ => 0
					};
					m_Renderer.SetBlendShapeWeight( timeline.Key, timeline.Value.Lerp( weight) * 100.0f);
				}
			}
		}
	#endif
		internal void Evaluate( float deltaTime, float eyeBlinkWeight, float lipSyncWeight)
		{
			if( m_CurrentTimelineClips != null)
			{
				foreach( var timeline in m_CurrentTimelineClips)
				{
					float weight = timeline.Value.Parts switch
					{
						BlendShapeParts.EyeForm => eyeBlinkWeight,
						BlendShapeParts.LipForm => lipSyncWeight,
						_ => 0
					};
					m_Renderer.SetBlendShapeWeight( timeline.Key, timeline.Value.Lerp( weight) * 100.0f);
				}
			}
			else if( m_CurrentBlends.Count > 0)
			{
				if( m_BlendElapsedSeconds < m_BlendTotalSeconds)
				{
					m_BlendElapsedSeconds = Mathf.Min( m_BlendElapsedSeconds + deltaTime, m_BlendTotalSeconds);
					
					float t = (m_BlendTotalSeconds <= 0.0f)? 1.0f : m_BlendElapsedSeconds / m_BlendTotalSeconds;
					
					foreach( var blendShape in m_CurrentBlends)
					{
						float weight = 1.0f;
						
						if( blendShape.Value.EyeBlink != false)
						{
							weight = eyeBlinkWeight;
						}
						if( blendShape.Value.LipSync != false)
						{
							weight = lipSyncWeight;
						}
						m_Renderer.SetBlendShapeWeight( blendShape.Key, blendShape.Value.Lerp( weight, t) * 100.0f);
					}
					if( m_BlendElapsedSeconds >= m_BlendTotalSeconds)
					{
						m_CurrentEyeBlinks.Clear();
						m_CurrentLipSyncs.Clear();
						
						foreach( var blend in m_CurrentBlends)
						{
							BlendShapeAction action = blend.Value.EndAction;
							
							if( action != null)
							{
								if( action.EyeBlink != false)
								{
									m_CurrentEyeBlinks.Add( blend.Key, action);
								}
								if( action.LipSync != false)
								{
									m_CurrentLipSyncs.Add( blend.Key, action);
								}
							}
						}
						m_CurrentBlends.Clear();
					}
				}
			}
			else
			{
				if( m_CurrentEyeBlinks.Count > 0)
				{
					foreach( var blendShape in m_CurrentEyeBlinks)
					{
						m_Renderer.SetBlendShapeWeight( blendShape.Key, blendShape.Value.Lerp( eyeBlinkWeight) * 100.0f);
					}
				}
				if( m_CurrentLipSyncs.Count > 0)
				{
					foreach( var blendShape in m_CurrentLipSyncs)
					{
						m_Renderer.SetBlendShapeWeight( blendShape.Key, blendShape.Value.Lerp( lipSyncWeight) * 100.0f);
					}
				}
			}
		}
		void LateUpdate()
		{
			if( m_Renderer != null)
			{
				float deltaTime = Time.deltaTime;
				float eyeBlinkWeight = 1.0f;
				float lipSyncWeight = 1.0f;
				
				m_EyeBlinkSeconds -= deltaTime;
				
				if( m_EyeBlinkSeconds < 0.0f)
				{
					m_EyeBlinkSeconds = Random.Range( 3.0f, 5.0f);
				}
				if( m_EyeBlinkSeconds <= 0.2f)
				{
					eyeBlinkWeight = Mathf.Abs( (m_EyeBlinkSeconds * 10.0f) - 1.0f);
				}
				if( m_LipSyncSeconds < m_LipSyncDuration)
				{
					m_LipSyncSeconds = Mathf.Min( m_LipSyncSeconds + deltaTime, m_LipSyncDuration);
				}
				if( m_LipSyncCurve != null)
				{
					lipSyncWeight = 1.0f - m_LipSyncCurve.Evaluate( m_LipSyncSeconds);
				}
				Evaluate( deltaTime, eyeBlinkWeight, lipSyncWeight);
			}
		}
		internal const string kPrefixEyebrow = "Eyebrow_";
		internal const string kPrefixEyeForm = "EyeForm_";
		internal const string kPrefixIrisForm = "IrisForm_";
		internal const string kPrefixIrisType = "IrisType_";
		internal const string kPrefixLipForm = "LipForm_";
		internal const string kPrefixTongueForm = "TongueForm_";
		internal const string kPrefixTearsForm = "TearsForm_";
		
		static readonly (string name, BlendShapeParts parts)[] kPrefixes = new (string, BlendShapeParts)[]
		{
			(kPrefixEyebrow, BlendShapeParts.Eyebrow),
			(kPrefixEyeForm, BlendShapeParts.EyeForm),
			(kPrefixIrisForm, BlendShapeParts.IrisForm),
			(kPrefixIrisType, BlendShapeParts.IrisType),
			(kPrefixLipForm, BlendShapeParts.LipForm),
			(kPrefixTongueForm, BlendShapeParts.TongueForm),
			(kPrefixTearsForm, BlendShapeParts.TearsForm),
		};
	#if UNITY_EDITOR
		void Reset()
		{
			m_Renderer = GetComponent<SkinnedMeshRenderer>();
		}
		internal BlendShapeInspector Inspector
		{
			get{ return m_Inspector; }
			set{ m_Inspector = value; }
		}
	#endif
		internal SkinnedMeshRenderer Renderer
		{
			get{ return m_Renderer; }
		}
		internal Dictionary<string, BlendShapeIndex> BlendShapeIndices
		{
			get
			{
				if( m_BlendShapeIndices == null)
				{
					OnEnable();
				}
				return m_BlendShapeIndices;
			}
		}
		[SerializeField]
		SkinnedMeshRenderer m_Renderer;
	#if UNITY_EDITOR
		[SerializeField]
		FaceSettingObject m_Setting;
		[SerializeField]
		BlendShapeInspector m_Inspector;
	#endif
		[System.NonSerialized]
		Mesh m_CachedMesh;
		[System.NonSerialized]
		Dictionary<string, BlendShapeIndex> m_BlendShapeIndices;
		[System.NonSerialized]
		readonly Dictionary<int, BlendShapeBlender> m_CurrentBlends = new();
		[System.NonSerialized]
		readonly Dictionary<int, BlendShapeAction> m_CurrentEyeBlinks = new();
		[System.NonSerialized]
		readonly Dictionary<int, BlendShapeAction> m_CurrentLipSyncs = new();
		[System.NonSerialized]
		Dictionary<int, BlendShapeClip> m_CurrentTimelineClips;
		[System.NonSerialized]
		List<BlendShapeAction> m_CurrentActions;
		[System.NonSerialized]
		IFaceSetting m_CurrentSetting;
		[System.NonSerialized]
		float m_BlendTotalSeconds;
		[System.NonSerialized]
		float m_BlendElapsedSeconds;
		[System.NonSerialized]
		float m_EyeBlinkSeconds;
		[System.NonSerialized]
		float m_LipSyncSeconds;
		[System.NonSerialized]
		float m_LipSyncDuration;
		[System.NonSerialized]
		AnimationCurve m_LipSyncCurve;
	#if UNITY_EDITOR
		[System.NonSerialized]
		internal float m_EyeBlinkWeight;
		[System.NonSerialized]
		internal float m_LipSyncWeight;
	#endif
	}
}