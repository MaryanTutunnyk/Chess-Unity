using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera playerOneCamera, playerTwoCamera;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera orthographicCamera;

    [SerializeField] private TextMeshProUGUI switchOrthographicModeButtonText;

    private bool is3D = true;

    protected override void Awake()
    {
        base.Awake();

        GameController.Instance.OnChangeTurns += OnChangeTurns;
    }

    private void OnDestroy()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnChangeTurns -= OnChangeTurns;
        }
    }

    private void OnChangeTurns(bool playerOneActive)
    {
        if (GameController.Instance.ShouldUpdateCamera)
        {
            SwitchCamera(playerOneActive);
        }
    }

    public void SwitchCamera(bool activeCamera)
    {
        playerOneCamera.gameObject.SetActive(activeCamera);
        playerTwoCamera.gameObject.SetActive(!activeCamera);
    }

    public void SwitchGraphicMode()
    {
        is3D = !is3D;
        SwitchGraphicMode(!is3D);
        GameController.Instance.SwitchGraphicMode(is3D);
    }

    private void SwitchGraphicMode(bool orthographic)
    {
        switchOrthographicModeButtonText.text = orthographic ? "3D" : "2D";
        orthographicCamera.gameObject.SetActive(orthographic);
        Camera.main.orthographic = orthographic;
    }
}
