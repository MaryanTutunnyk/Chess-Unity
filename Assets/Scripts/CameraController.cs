using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera playerOneCamera, playerTwoCamera;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera orthographicCamera;

    [SerializeField] private TextMeshProUGUI switchOrthographicModeButtonText;

    [SerializeField] private GameController gameController;

    private bool is3D = true;

    private void Awake()
    {
        gameController.OnChangeTurns += OnChangeTurns;
    }

    private void OnDestroy()
    {
        if (gameController != null)
        {
            gameController.OnChangeTurns -= OnChangeTurns;
        }
    }

    private void OnChangeTurns(bool playerOneActive)
    {
        SwitchCamera(playerOneActive);
    }

    private void SwitchCamera(bool activeCamera)
    {
        playerOneCamera.gameObject.SetActive(activeCamera);
        playerTwoCamera.gameObject.SetActive(!activeCamera);
    }

    public void SwitchGraphicMode()
    {
        is3D = !is3D;
        SwitchGraphicMode(!is3D);
        gameController.SwitchGraphicMode(is3D);
    }

    private void SwitchGraphicMode(bool orthographic)
    {
        switchOrthographicModeButtonText.text = orthographic ? "3D" : "2D";
        orthographicCamera.gameObject.SetActive(orthographic);
        Camera.main.orthographic = orthographic;
    }
}
