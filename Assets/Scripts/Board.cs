using System.Collections.Generic;
using UnityEngine;

public class Board : Singleton<Board>
{
    [SerializeField] private GameObject boardHolder;
    [SerializeField] private GameObject physicalBoard;

    private Square[,] squareMatrix;
    private SquareBehaviour[,] squareBehaviourMatrix;
    private SquareBehaviour activeSquare = null;

    private List<Move> possibleMovesActive = null;

    protected override void Awake()
    {
        base.Awake();

        squareMatrix = new Square[8, 8];
        squareBehaviourMatrix = new SquareBehaviour[8, 8];
        int counter = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                squareBehaviourMatrix[i, j] = transform.GetChild(counter).GetComponent<SquareBehaviour>();
                squareMatrix[i, j] = squareBehaviourMatrix[i, j].Square;
                squareMatrix[i, j].InitPosition(j, i);
                counter++;
            }
        }
    }

    public void ActivateBoardHolder(bool activate)
    {
        boardHolder.SetActive(activate);
    }

    public void ActivatePhysicalBoard(bool activate)
    {
        physicalBoard.SetActive(activate);
    }

    public void MovePiece(int originalX, int originalY, int destinationX, int destinationY)
    {
        SquareBehaviour originalSquare = squareBehaviourMatrix[originalY, originalX];
        SquareBehaviour destinationSquare = squareBehaviourMatrix[destinationY, destinationX];

        possibleMovesActive = originalSquare.Square.PieceContainer.PossibleMoves;
        originalSquare.Square.PieceContainer.Move(destinationSquare);
        GetMoveFromPossibles(destinationSquare.Square).RunCallback();
    }

    public void SquareClicked(SquareBehaviour square)
    {
        bool canMove = (GameController.Instance.ActualPlayer == GameController.Instance.PlayerOne && NetworkController.Instance.CurrentTeam == 0) ||
            (GameController.Instance.ActualPlayer != GameController.Instance.PlayerOne && NetworkController.Instance.CurrentTeam == 1) || !GameController.Instance.IsNetworkGame;
        if (square.Square.PieceContainer != null && square.Square.PieceContainer.Propietary.Equals(GameController.Instance.ActualPlayer) &&
            (activeSquare == null || activeSquare.Square.PieceContainer.Propietary.Equals(square.Square.PieceContainer.Propietary))
            && square.Square.PieceContainer.Propietary.Type == PlayerType.HUMAN && canMove)
        {
            ResetMoves();
            activeSquare = square;
            possibleMovesActive = activeSquare.Square.PieceContainer.PossibleMoves;
            for (int i = 0; i < possibleMovesActive.Count; i++)
            {
                possibleMovesActive[i].Square.RelatedBehaviour.MarkPossible();
            }
        }
        else if (activeSquare != null && square.BlockColor)
        {
            activeSquare.Square.PieceContainer.Move(square);
            GetMoveFromPossibles(square.Square).RunCallback();

            if (GameController.Instance.IsNetworkGame)
            {
                NetMakeMove mm = new NetMakeMove();
                mm.OriginalX = activeSquare.Square.X;
                mm.OriginalY = activeSquare.Square.Y;
                mm.DestinationX = square.Square.X;
                mm.DestinationY = square.Square.Y;
                mm.TeamID = NetworkController.Instance.CurrentTeam;
                ClientBehaviour.Instance.SendToServer(mm);
            }

            activeSquare = null;
            ResetMoves();
            GameController.Instance.ChangeTurns();
            GameController.Instance.EvaluatePlayers();
        }
    }

    private Move GetMoveFromPossibles(Square square)
    {
        for (int i = 0; i < possibleMovesActive.Count; i++)
        {
            if (possibleMovesActive[i].Square.Equals(square))
            {
                return possibleMovesActive[i];
            }
        }
        return null;
    }

    public void ResetMoves()
    {
        if (possibleMovesActive != null)
        {
            for (int i = 0; i < possibleMovesActive.Count; i++)
            {
                possibleMovesActive[i].Square.RelatedBehaviour.ResetColor();
            }
        }
    }

    public Square[,] GenerateBoardCopy()
    {
        Square[,] copy = new Square[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                copy[i, j] = new Square(squareMatrix[i, j]);
            }
        }
        return copy;
    }

    public Square[,] GenerateBoardCopy(Square[,] board)
    {
        Square[,] copy = new Square[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                copy[i, j] = new Square(board[i, j]);
            }
        }
        return copy;
    }

    public Square[,] SquareMatrix => squareMatrix;

    public SquareBehaviour[,] SquareBehaviourMatrix => squareBehaviourMatrix;
}
