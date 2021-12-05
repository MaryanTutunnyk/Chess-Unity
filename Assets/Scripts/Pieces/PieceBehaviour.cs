using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBehaviour : MonoBehaviour
{
    [SerializeField] private PieceType type;
    [SerializeField] private int pieceValue;
    [SerializeField] private SquareTableValues values;

    private GameObject child3D, child2D;

    [SerializeField]
    private Sprite spriteWhite, spriteBlack;
    private SpriteRenderer spriteRend;

    private Piece piece;

    public Piece Piece => piece;

    private void Awake()
    {
        values.Init();
        switch (type)
        {
            case PieceType.BISHOP:
                piece = new Bishop(this, pieceValue, values);
                break;
            case PieceType.KING:
                piece = new King(this, pieceValue, values);
                break;

            case PieceType.KNIGHT:
                piece = new Knight(this, pieceValue, values);
                break;

            case PieceType.PAWN:
                piece = new Pawn(this, pieceValue, values);
                break;

            case PieceType.QUEEN:
                piece = new Queen(this, pieceValue, values);
                break;

            case PieceType.ROOK:
                piece = new Rook(this, pieceValue, values);
                break;
        }
    }

    public void InitGraphics(int id)
    {
        child3D = transform.GetChild(0).gameObject;
        child2D = transform.GetChild(1).gameObject;
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        spriteRend.sprite = id == 0 ? spriteWhite : spriteBlack;
        child2D.SetActive(false);
    }

    public void SwitchGraphicMode(bool is3D)
    {
        child2D.SetActive(!is3D);
        child3D.SetActive(is3D);
    }

    public void ChangeMaterial(Material material)
    {
        var rends = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material = material;
        }
    }
}
