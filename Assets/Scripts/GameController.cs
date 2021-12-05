using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum PieceType
{
    PAWN,
    KING,
    QUEEN,
    KNIGHT,
    BISHOP,
    ROOK
}

public class GameController : Singleton<GameController>
{
    [SerializeField] private bool isARScene;

    private bool shouldUpdateCamera;
    public bool ShouldUpdateCamera => shouldUpdateCamera;

    private const string gameName = "Game";
    private const string gameWithAIName = "GameWithAI";
    private const string gameWithRealPlayerName = "GameWithRealPlayer";
    private const string gameWithVirtualPlayerName = "GameWithVirtualPlayer";

    [SerializeField] private Transform piecesParent;

    [SerializeField] private GameObject kingPrefab, queenPrefab, rookPrefab, bishopPrefab, knightPrefab, pawnPrefab;

    [SerializeField] private Material materialWhite, materialBlack;

    [SerializeField] private CanvasGroup cg;
    [SerializeField] private TextMeshProUGUI textEnd;

    [SerializeField] private int queenValue;
    [SerializeField] private SquareTableValues queenSquareTableValues;

    private Player playerOne, playerTwo;
    private Player actualPlayer, otherPlayer;

    public Player ActualPlayer => actualPlayer;
    public Player PlayerOne => playerOne;

    public event Action<bool> OnChangeTurns;

    private MiniMax miniMax;
    private List<PieceBehaviour> allPiecesBehaviour;

    private bool isNetworkGame;
    public bool IsNetworkGame => isNetworkGame;

    protected override void Awake()
    {
        base.Awake();

        playerOne = new Player(materialWhite, PlayerType.HUMAN, 7);

        string game = PlayerPrefs.GetString(gameName);
        if (game == gameWithAIName)
        {
            playerTwo = new Player(materialBlack, PlayerType.AI);
        }
        else if (game == gameWithRealPlayerName)
        {
            playerTwo = new Player(materialBlack, PlayerType.HUMAN, 7);
            shouldUpdateCamera = true;
        }
        else if (game == gameWithVirtualPlayerName)
        {
            playerTwo = new Player(materialBlack, PlayerType.HUMAN, 7);
            isNetworkGame = true;
        }

        miniMax = new MiniMax();

        InstantiatePlayerOnePieces();
        InstantiatePlayerTwoPieces();

        actualPlayer = playerOne;
        otherPlayer = playerTwo;

        allPiecesBehaviour = new List<PieceBehaviour>(GameObject.FindObjectsOfType<PieceBehaviour>());

        EvaluatePlayers();

        if (!isARScene)
        {
            ConnectToNetwork();
            UpdateCamera();
        }
        else
        {
            Board.Instance.ActivateBoardHolder(false);
            Board.Instance.ActivatePhysicalBoard(false);
        }
    }

    public void ConnectToNetwork()
    {
        if (isNetworkGame)
        {
            NetworkController.Instance.Connect();
        }
    }

    public void UpdateCamera()
    {
        if (!isNetworkGame)
        {
            CameraController.Instance.SwitchCamera(actualPlayer == playerOne);
        }
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

        var piece = ((GameObject)Instantiate(prefab, piecesParent)).GetComponent<PieceBehaviour>();
        piece.InitGraphics(player.ID);
        piece.ChangeMaterial(player.AsignedMaterial);
        piece.transform.position = new Vector3(squareBehaviour.transform.position.x, piece.transform.position.y, squareBehaviour.transform.position.z);
        squareBehaviour.Square.SetNewPiece(piece.Piece);
        piece.Piece.SetPropietary(player);

    }

    public void GoToMainMenu()
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
        }
        else
        {
            actualPlayer = playerOne;
            otherPlayer = playerTwo;
        }
        if (OnChangeTurns != null)
        {
            OnChangeTurns.Invoke(actualPlayer == playerOne);
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
            if (actualPlayer.Type == PlayerType.AI)
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

    public void SwitchGraphicMode(bool is3D)
    {
        for (int i = 0; i < allPiecesBehaviour.Count; i++)
        {
            if (allPiecesBehaviour[i] != null)
            {
                allPiecesBehaviour[i].SwitchGraphicMode(is3D);
            }
        }
    }

    public void StartGame()
    {
        CameraController.Instance.SwitchCamera(NetworkController.Instance.CurrentTeam == 0);
    }
}
