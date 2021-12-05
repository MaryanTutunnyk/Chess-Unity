using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBehaviour : MonoBehaviour
{
    private Square square;

    private Renderer rend;
    private Color originalColor;
    [SerializeField] private Color possibleColor;

    private bool blockColor = false;

    private void Awake()
    {
        this.square = new Square();

        rend = GetComponent<Renderer>();
        originalColor = rend.sharedMaterial.GetColor("_BaseColor");
    }

    public void ResetColor()
    {
        rend.material.SetColor("_BaseColor", originalColor);
        blockColor = false;
    }

    public void MarkPossible()
    {
        blockColor = true;
        rend.material.SetColor("_BaseColor", possibleColor);
    }

    private void OnMouseDown()
    {
        Board.Instance.SquareClicked(this);
    }

    public Square Square => square;

    public bool BlockColor => blockColor;
}
