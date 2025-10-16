using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BirdTracks.Game.Core
{
	public class InputConfig : MonoBehaviour 
	{
		[SerializeField] private EventSystem m_EventSystem = default;
		[SerializeField] private StandaloneInputModule m_StandaloneInputModule = default;


		private void Awake()
		{
			Input.simulateMouseWithTouches = false;
			m_EventSystem.enabled = true;
			m_StandaloneInputModule.enabled = true;
		}
	}
}