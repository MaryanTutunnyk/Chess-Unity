using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInWorldSpaceBehaviour : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private float maxDistance;

    private int interactableLayer;

    [SerializeField] private UnityEvent onClick;

    private void Awake()
    {
        interactableLayer = 1 << LayerMask.NameToLayer("Interactable");
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, interactableLayer))
            {
                if (hitInfo.transform == transform)
                {
                    onClick.Invoke();
                }
            }
        }
    }
}
