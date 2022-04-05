using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ShakingCamera : MonoBehaviour
{
    public static ShakingCamera Instance { get; private set; }
    private CinemachineFreeLook cinemachineFreeLook;
    private void Awake()
    {
        Instance = this;
        cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
    }

    public void ShakeCamera(float intensity)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cinemachineFreeLook.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = intensity;
    }
}
