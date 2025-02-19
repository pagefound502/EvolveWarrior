using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;
    
    void Start()
    {

        VirtualCamera.Follow = GameManagerCustom.GetPlayer().transform;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
