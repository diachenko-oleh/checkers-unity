using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }        //singleton

    public Board mBoard;
    public PieceManager mPieceManager;

    public InfoOutput infoOutput;

    public static bool mIsAnyAlive = true;
    public static bool mIsWhiteWin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        mBoard.Create();
        mPieceManager.Setup(mBoard);
    }

    public void SwitchSides(Color teamColor)
    {
        bool IsBlackTurn;
        if (!mIsAnyAlive)
        {
            if (mIsWhiteWin)
            {
                infoOutput.ShowWinText("White Win");
            }
            if (!mIsWhiteWin)
            {
                infoOutput.ShowWinText("Black Win");
            }
            infoOutput.PlaySound();
            mPieceManager.ResetPieces();
            mIsAnyAlive = true;
            teamColor = Color.black;
            Pawn.RevertKingsToPawns(mPieceManager.mWhitePiece);
            Pawn.RevertKingsToPawns(mPieceManager.mBlackPiece);
        }

        if (teamColor == Color.white)
        {
            IsBlackTurn = true;
            infoOutput.ShowTurnText("Black Turn");
        }
        else
        {
            IsBlackTurn = false;
            infoOutput.ShowTurnText("White Turn");
        }

        mPieceManager.SetInteractive(mPieceManager.mWhitePiece, !IsBlackTurn);
        mPieceManager.SetInteractive(mPieceManager.mBlackPiece, IsBlackTurn);
    }

    public void TeamWin(bool isAnyAlive,bool isWhiteWin)
    {
        mIsAnyAlive = isAnyAlive;
        mIsWhiteWin = isWhiteWin;
    }
}
