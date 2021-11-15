using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera playerOneCamera, playerTwoCamera;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera orthographicCamera;

    [SerializeField] private GameObject SwitchGraphicModeButton;
    [SerializeField] private TextMeshProUGUI switchOrthographicModeButtonText;

    public void SwitchCamera(bool activeCamera)
    {
        playerOneCamera.gameObject.SetActive(activeCamera);
        playerTwoCamera.gameObject.SetActive(!activeCamera);
    }

    public void SwitchGraphicMode(bool orthographic)
    {
        switchOrthographicModeButtonText.text = orthographic ? "3D" : "2D";
        orthographicCamera.gameObject.SetActive(orthographic);
        Camera.main.orthographic = orthographic;
    }
}
