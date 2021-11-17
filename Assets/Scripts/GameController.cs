using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
    private const string gameName = "Game";
    private const string gameWithAIName = "GameWithAI";
    private const string gameWithRealPlayerName = "GameWithRealPlayer";
    private const string gameWithVirtualPlayerName = "GameWithVirtualPlayer";

    [SerializeField] private GameObject kingPrefab, queenPrefab, rookPrefab, bishopPrefab, knightPrefab, pawnPrefab;

    [SerializeField] private Material materialWhite, materialBlack;

    [SerializeField]
    private CanvasGroup cg;
    [SerializeField]
    private TextMeshProUGUI textEnd, textButtonChange;

    [SerializeField]
    private int queenValue;
    [SerializeField]
    private SquareTableValues queenSquareTableValues;

    private Player playerOne, playerTwo;
    private Player actualPlayer, otherPlayer;

    private MiniMax miniMax;
    private List<PieceBehaviour> allPiecesBehaviour;

    private bool is3D = true;

    [SerializeField] private CameraController cameraController;

    void Start()
    {
        playerOne = new Player(materialWhite, Player.PlayerType.HUMAN, 7);

        string game = PlayerPrefs.GetString(gameName);
        if (game == gameWithAIName)
        {
            playerTwo = new Player(materialBlack, Player.PlayerType.AI);
        }
        if (game == gameWithRealPlayerName)
        {
            playerTwo = new Player(materialBlack, Player.PlayerType.HUMAN, 7);
        }
        if (game == gameWithVirtualPlayerName)
        {

        }

        miniMax = new MiniMax();

        InstantiatePlayerOnePieces();
        InstantiatePlayerTwoPieces();

        actualPlayer = playerOne;
        otherPlayer = playerTwo;

        cameraController.SwitchCamera(true);

        allPiecesBehaviour = new List<PieceBehaviour>(GameObject.FindObjectsOfType<PieceBehaviour>());

        EvaluatePlayers();
    }

    private void InstantiatePlayerOnePieces()
    {
        var board = Board.Instance.SquareBehaviourMatrix;

        for (int i = 0; i < 8; i++)
        {
            InstantiatePiece(PieceType.PAWN, board[1, i], playerOne);
        }
        InstantiatePiece(PieceType.ROOK, board[0, 0], playerOne);
        InstantiatePiece(PieceType.ROOK, board[0, 7], playerOne);

        InstantiatePiece(PieceType.KNIGHT, board[0, 1], playerOne);
        InstantiatePiece(PieceType.KNIGHT, board[0, 6], playerOne);

        InstantiatePiece(PieceType.BISHOP, board[0, 2], playerOne);
        InstantiatePiece(PieceType.BISHOP, board[0, 5], playerOne);

        InstantiatePiece(PieceType.QUEEN, board[0, 3], playerOne);
        InstantiatePiece(PieceType.KING, board[0, 4], playerOne);
    }

    private void InstantiatePlayerTwoPieces()
    {
        var board = Board.Instance.SquareBehaviourMatrix;
        for (int i = 0; i < 8; i++)
        {
            InstantiatePiece(PieceType.PAWN, board[6, i], playerTwo);
        }
        InstantiatePiece(PieceType.ROOK, board[7, 0], playerTwo);
        InstantiatePiece(PieceType.ROOK, board[7, 7], playerTwo);

        InstantiatePiece(PieceType.KNIGHT, board[7, 1], playerTwo);
        InstantiatePiece(PieceType.KNIGHT, board[7, 6], playerTwo);

        InstantiatePiece(PieceType.BISHOP, board[7, 2], playerTwo);
        InstantiatePiece(PieceType.BISHOP, board[7, 5], playerTwo);

        InstantiatePiece(PieceType.QUEEN, board[7, 3], playerTwo);
        InstantiatePiece(PieceType.KING, board[7, 4], playerTwo);
    }

    public void InstantiatePiece(PieceType type, SquareBehaviour squareBehaviour, Player player)
    {
        GameObject prefab = null;
        switch (type)
        {
            case PieceType.PAWN:
                prefab = pawnPrefab;
                break;
            case PieceType.QUEEN:
                prefab = queenPrefab;
                break;
            case PieceType.ROOK:
                prefab = rookPrefab;
                break;
            case PieceType.KNIGHT:
                prefab = knightPrefab;
                break;
            case PieceType.KING:
                prefab = kingPrefab;
                break;
            case PieceType.BISHOP:
                prefab = bishopPrefab;
                break;
        }

        var piece = ((GameObject)Instantiate(prefab)).GetComponent<PieceBehaviour>();
        piece.InitGraphics(player.ID);
        piece.ChangeMaterial(player.AsignedMaterial);
        piece.transform.position = new Vector3(squareBehaviour.transform.position.x, piece.transform.position.y, squareBehaviour.transform.position.z);
        squareBehaviour.Square.SetNewPiece(piece.Piece);
        piece.Piece.SetPropietary(player);

    }

    public void ChangeGraphicMode()
    {
        is3D = !is3D;

        cameraController.SwitchGraphicMode(!is3D);

        for (int i = 0; i < allPiecesBehaviour.Count; i++)
        {
            if (allPiecesBehaviour[i] != null)
            {
                allPiecesBehaviour[i].SwitchGraphicMode(!is3D);
            }
        }
    }

    public void Reset()
    {
        SceneManager.LoadScene(0);
    }

    public void InstantiateQueen(Square square, Player player)
    {
        var piece = new Queen(queenValue, queenSquareTableValues, square, player);
    }
    public void ChangeTurns()
    {
        if (actualPlayer == playerOne)
        {
            actualPlayer = playerTwo;
            otherPlayer = playerOne;

            cameraController.SwitchCamera(false);
        }
        else
        {
            actualPlayer = playerOne;
            otherPlayer = playerTwo;

            cameraController.SwitchCamera(true);
        }
    }

    public void EvaluatePlayers()
    {
        actualPlayer.Evaluate(null);
        otherPlayer.Evaluate(null);
        actualPlayer.EvaluateCastlings(null, otherPlayer);
        otherPlayer.EvaluateCastlings(null, actualPlayer);

        actualPlayer.EvaluateCheckOffMoves(otherPlayer);
        otherPlayer.ResetPawnsState();

        if (!actualPlayer.CheckIfCheckMate())
        {
            if (actualPlayer.Type == Player.PlayerType.AI)
            {
                miniMax.RunMiniMax(actualPlayer, otherPlayer, Board.Instance.SquareMatrix);
            }
        }
        else
        {
            cg.DOFade(1, 0.4f);
            cg.interactable = true;
            cg.blocksRaycasts = true;
            textEnd.text = "Гравець: " + otherPlayer.ID + " переміг";
        }
    }
    public Player ActualPlayer
    {
        get
        {
            return actualPlayer;
        }
    }
}
public enum PieceType
{
    PAWN,
    KING,
    QUEEN,
    KNIGHT,
    BISHOP,
    ROOK
}
