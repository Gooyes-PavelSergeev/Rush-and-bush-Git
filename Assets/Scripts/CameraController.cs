using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : PlayerComponent
{
    [SerializeField] Transform _lookAtPoint;
    [SerializeField] Transform _followPoint;

    private CinemachineFreeLook _cmFreeLook;
    private void Start()
    {
        if (_updatable)
        {
            _cmFreeLook = CinemachineFreeLook.FindObjectOfType<CinemachineFreeLook>();
            if (_cmFreeLook != null)
            {
                _cmFreeLook.LookAt = _lookAtPoint;
                _cmFreeLook.Follow = _followPoint;
            }
        }
    }
}
