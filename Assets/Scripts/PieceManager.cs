using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class PieceManager : MonoBehaviour
{
    public AudioClip winSound;

    public TextMeshProUGUI turnText;
    public TextMeshProUGUI winText;

    [HideInInspector]
    public static bool mIsAnyAlive = true;
    public static bool mIsWhiteWin;

    public GameObject mPiecePrefab;

    private List<BasePiece> mWhitePiece = null;
    private List<BasePiece> mBlackPiece = null;

    private int mPieceOrder = 12;

    public void Setup(Board board)
    {
        mWhitePiece = CreatePieces(Color.white, new Color32(80,124,159,255), board);
        mBlackPiece = CreatePieces(Color.black, new Color32(210,95,64,255), board);

        PlacePieces(0,mWhitePiece,board);
        PlacePieces(5,mBlackPiece,board);

        SwitchSides(Color.black);
    }
    private List<BasePiece> CreatePieces(Color teamColor,Color32 spriteColor,Board board)
    {
        List<BasePiece> newPieces = new List<BasePiece>();
        for (int i = 0; i < mPieceOrder; i++)
        {
            GameObject newPieceObject = Instantiate(mPiecePrefab);
            newPieceObject.transform.SetParent(transform);

            newPieceObject.transform.localScale = new Vector3(1,1,1);
            newPieceObject.transform.localRotation = Quaternion.identity;

            Type pieceType = typeof(Pawn);

            BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);
            newPieces.Add(newPiece);

            newPiece.Setup(teamColor,spriteColor,this);
        }
        return newPieces;
    }
    private void PlacePieces(int startRow,List<BasePiece> pieces, Board board)
    {

        int index = 0;
        for (int i = startRow; i < startRow+3; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (i % 2 == 0 && j % 2 == 0)
                {
                    pieces[index].Place(board.mAllCells[j, i]);
                    index++;
                }
                else if (i % 2 != 0 && j % 2 != 0)
                {
                    pieces[index].Place(board.mAllCells[j, i]);
                    index++;
                }
            }
        }
    }

    private void SetInteractive(List<BasePiece> allPieces,bool value)
    {
        foreach (BasePiece piece in allPieces)
        {
            piece.enabled = value;
        }
    }
    public void SwitchSides(Color teamColor)
    {
        bool IsBlackTurn;


        if (!mIsAnyAlive)
        {
            if (mIsWhiteWin)
            {
                winText.text = "White Win";
            }
            if (!mIsWhiteWin)
            {
                winText.text = "Black Win";
            }
            AudioSource.PlayClipAtPoint(winSound, new Vector3(0,0,0), 1.0f);
            ResetPieces();
            mIsAnyAlive = true;
            teamColor = Color.black;
            Pawn.RevertKingsToPawns(mWhitePiece);
            Pawn.RevertKingsToPawns(mBlackPiece);
        }

        if (teamColor == Color.white)
        {
            IsBlackTurn = true;
            turnText.text = "Black Turn";
        }
        else
        {
            IsBlackTurn = false;
            turnText.text = "White Turn";
        }


        SetInteractive(mWhitePiece, !IsBlackTurn);
        SetInteractive(mBlackPiece, IsBlackTurn);

    }
    public void ResetPieces()
    {
        foreach (BasePiece piece in mWhitePiece)
        {
            piece.Reset();
        }
        foreach (BasePiece piece in mBlackPiece)
        {
            piece.Reset();
        }
    }
}
