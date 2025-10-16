using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

namespace Birdtracks.Game.ONS
{
    public class ONS_CameraShake : MonoBehaviour, INotificationReceiver
    {
        private CinemachineVirtualCamera _vCam;

        [SerializeField] private List<ShakeProperties> _shakePropertiesList = new List<ShakeProperties>();

        private float shakeTime;

        private void Start()
        {
            _vCam = GetComponent<CinemachineVirtualCamera>();
            
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
        }

        public void ShakeCamera(int index)
        {
            Debug.Log("[Shake] called index " + index);
            CinemachineBasicMultiChannelPerlin m_CinemachineBasicMultiChannelPerlin =
                _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            m_CinemachineBasicMultiChannelPerlin.m_AmplitudeGain = _shakePropertiesList[index].Intensity;
            StartCoroutine(ShakeTimer(_shakePropertiesList[index].ShakeTime));
        }

        IEnumerator ShakeTimer(float time)
        {
            shakeTime = time;
            while (shakeTime > 0)
            {
                shakeTime -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            CinemachineBasicMultiChannelPerlin m_CinemachineBasicMultiChannelPerlin =
                _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            m_CinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        }
    }

    [Serializable]
    public struct ShakeProperties
    {
        public float Intensity;
        public float ShakeTime;
    }
}
