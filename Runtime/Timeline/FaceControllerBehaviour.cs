
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.FacialExpression
{
	[Serializable]
	sealed class FaceControllerBehaviour : PlayableBehaviour
	{
		internal IFaceSetting Setting
		{
			get
			{
				if( m_Setting != null)
				{
					return m_Setting;
				}
				return m_Property;
			}
		}
		[SerializeField]
		internal FaceSettingObject m_Setting;
		[SerializeField]
		internal FaceSettingProperty m_Property;
	}
}
