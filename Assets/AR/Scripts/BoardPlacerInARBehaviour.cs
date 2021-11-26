using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class BoardPlacerInARBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private List<ARRaycastHit> arRaycastHits = new List<ARRaycastHit>();
    [SerializeField] private Camera arCamera;

    [SerializeField] private Transform boardParent;
    [SerializeField] private GameObject boardPrefab;
    private Transform board;

    [SerializeField] private float maxDistance;
    private int interactableLayer;

    private bool isBoardPlaced;
    private bool isBoardSelected;

    private Vector3 currentRotation;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private Button startGameButton;

    [SerializeField] private TextMeshProUGUI isPlacedRepresenter;
    [SerializeField] private TextMeshProUGUI isSelectedRepresenter;
    [SerializeField] private TextMeshProUGUI boardPositionRepresenter;

    private IEnumerator selectionIEnumerator;
    private float selectionProgress;
    [SerializeField] private float selectionProgressSpeed;
    [SerializeField] private float selectionHeight;
    [SerializeField] private Transform selectionArea;

    private void Awake()
    {
        interactableLayer = 1 << LayerMask.NameToLayer("Interactable");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isBoardPlaced)
        {
            CreateBoardAtPosition(eventData.position);
        }
        else
        {
            SelectBoard(eventData.position, eventData.dragging);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isBoardPlaced)
        {
            CreateBoardAtPosition(eventData.position);
        }
        else
        {
            SelectBoard(eventData.position, true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isBoardSelected)
        {
            if (arRaycastManager.Raycast(eventData.position, arRaycastHits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
            {
                UpdateBoardParentPose(arRaycastHits);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    private void CreateBoardAtPosition(Vector2 pointerPosition)
    {
        if (arRaycastManager.Raycast(pointerPosition, arRaycastHits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
        {
            isBoardPlaced = true;
            isBoardSelected = true;

            board = Instantiate(boardPrefab, boardParent).transform;
            UpdateBoardParentPose(arRaycastHits);

            currentRotation = boardParent.localEulerAngles;

            if (selectionIEnumerator != null)
            {
                StopCoroutine(selectionIEnumerator);
            }
            selectionIEnumerator = SelectionIEnumerator(true);
            StartCoroutine(selectionIEnumerator);
        }
    }

    private void SelectBoard(Vector2 pointerPosition, bool dragging)
    {
        Ray ray = arCamera.ScreenPointToRay(pointerPosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, interactableLayer))
        {
            if (hitInfo.transform == board)
            {
                isBoardSelected = true;

                if (selectionIEnumerator != null)
                {
                    StopCoroutine(selectionIEnumerator);
                }
                selectionIEnumerator = SelectionIEnumerator(true);
                StartCoroutine(selectionIEnumerator);
            }
        }
        else
        {
            if (!dragging)
            {
                isBoardSelected = false;

                if (selectionIEnumerator != null)
                {
                    StopCoroutine(selectionIEnumerator);
                }
                selectionIEnumerator = SelectionIEnumerator(false);
                StartCoroutine(selectionIEnumerator);
            }
        }
    }

    private IEnumerator SelectionIEnumerator(bool select)
    {
        float targetSelectionProgress = select ? 1f : 0f;

        while (Mathf.Abs(selectionProgress - targetSelectionProgress) > Mathf.Epsilon)
        {
            selectionProgress = Mathf.Lerp(selectionProgress, targetSelectionProgress, selectionProgressSpeed);

            board.localPosition = board.up * selectionHeight * selectionProgress;
            selectionArea.localScale = Vector3.one * selectionProgress;

            yield return null;
        }
    }

    private void UpdateBoardParentPose(List<ARRaycastHit> arRaycastHits)
    {
        ARRaycastHit highestArRaycastHit = arRaycastHits[0];
        for (int i = 1; i < arRaycastHits.Count; i++)
        {
            if (arRaycastHits[i].pose.position.y > highestArRaycastHit.pose.position.y)
            {
                highestArRaycastHit = arRaycastHits[i];
            }
        }

        boardParent.position = highestArRaycastHit.pose.position;
    }

    private void Update()
    {
        UpdateStartButton();

        UpdateSelectionArea();

        isPlacedRepresenter.text = "IsPlaced: " + isBoardPlaced.ToString();
        isSelectedRepresenter.text = "IsSelected: " + isBoardSelected.ToString();
        boardPositionRepresenter.text = "BoardPosition: " + boardParent.position.ToString();
    }

    private void UpdateStartButton()
    {
        startGameButton.interactable = isBoardPlaced;
    }

    private void UpdateSelectionArea()
    {
        Vector3 startSelectionAreaRotation = selectionArea.eulerAngles;
        selectionArea.LookAt(arCamera.transform);
        selectionArea.eulerAngles = new Vector3(startSelectionAreaRotation.x, selectionArea.eulerAngles.y, startSelectionAreaRotation.z);
    }

    public void OnRotateLeftButtonClick()
    {
        currentRotation -= Vector3.up * rotationSpeed;
        boardParent.localEulerAngles = currentRotation;
    }

    public void OnRotateRightButtonClick()
    {
        currentRotation += Vector3.up * rotationSpeed;
        boardParent.localEulerAngles = currentRotation;
    }
}
