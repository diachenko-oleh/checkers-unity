using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

public abstract class BasePiece : EventTrigger
{

    private static int blackCount = 12, whiteCount = 12;
    [HideInInspector]
    public Color mColor = Color.clear;

    protected Cell mOriginalCell = null;
    protected Cell mCurrentCell = null;

    protected RectTransform mRectTransform = null;
    protected PieceManager mPieceManager;

    protected Vector3Int mMovement = Vector3Int.one;
    protected List<Cell> mHilightedCells = new List<Cell>();

    protected Cell mTargetCell = null;
    public virtual void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        mPieceManager = newPieceManager;
        mColor = newTeamColor;
        GetComponent<Image>().color = newSpriteColor;
        mRectTransform = GetComponent<RectTransform>();
    }

    public void Place(Cell newCell)
    {
        mCurrentCell = newCell;
        mOriginalCell = newCell;
        mCurrentCell.mCurrentPiece = this;

        transform.position = newCell.transform.position;
        gameObject.SetActive(true);
    }

    public void Reset()
    {
        Kill();
        Place(mOriginalCell);
    }
    public virtual void Kill()
    {
        mCurrentCell.mCurrentPiece = null;
        gameObject.SetActive(false);
        if (mColor==Color.black)
        {
            blackCount--;
        }
        if (mColor == Color.white)
        {
            whiteCount--;
        }
        if (blackCount == 0)
        {
            PieceManager.mIsAnyAlive = false;
            PieceManager.mIsWhiteWin = true;
            blackCount = 12;
            whiteCount = 12;
        }
        else if (whiteCount == 0)
        {
            PieceManager.mIsAnyAlive = false;
            PieceManager.mIsWhiteWin = false;
            blackCount = 12;
            whiteCount = 12;
        }
    }



    protected void CreateCellPath(int xDirection, int yDirection, int movement)
    {
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        for (int i = 1; i <= movement; i++)
        {
            currentX += xDirection;
            currentY += yDirection;

            CellState cellState = CellState.None;
            cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY, this);

            if (cellState == CellState.Enemy)
            {
                int jumpX = currentX + xDirection;
                int jumpY = currentY + yDirection;

                if (mCurrentCell.mBoard.ValidateCell(jumpX, jumpY, this) == CellState.Free)
                {
                    Cell enemyCell = mCurrentCell.mBoard.mAllCells[currentX, currentY];
                    Cell jumpCell = mCurrentCell.mBoard.mAllCells[jumpX, jumpY];

                    jumpCell.mCapturedPiece = enemyCell.mCurrentPiece;
                    mHilightedCells.Add(jumpCell);
                }
                break;
            }

            if (cellState!=CellState.Free)
            {
                break;
            }

            mHilightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
        }
    }

    protected virtual void CheckPathing(Color teamColor)
    {
        CreateCellPath(-1, -1, mMovement.z);        //діагональ вниз
        CreateCellPath(1, -1, mMovement.z);   

        CreateCellPath(1, 1, mMovement.z);          //діагональ вгору
        CreateCellPath(-1, 1, mMovement.z);
    }
    protected void ShowCells()
    {
        foreach (Cell cell in mHilightedCells)
        {
            cell.mOutlineImage.enabled = true;
        }
    }
    protected void ClearCells()
    {
        foreach (Cell cell in mHilightedCells)
        {
            cell.mOutlineImage.enabled = false;
        }
        mHilightedCells.Clear();
    }

    protected virtual void Move()
    {
        if (mTargetCell.mCapturedPiece != null)
        {
            mTargetCell.mCapturedPiece.Kill();
            mTargetCell.mCapturedPiece = null;
        }

        mCurrentCell.mCurrentPiece = null;
        mCurrentCell = mTargetCell;
        mCurrentCell.mCurrentPiece = this;

        transform.position = mCurrentCell.transform.position;
        mTargetCell = null;
    }



    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        CheckPathing(mColor);
        ShowCells();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        transform.position += (Vector3)eventData.delta;

        foreach (Cell cell in mHilightedCells)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.mRectTransform,Input.mousePosition))
            {
                mTargetCell = cell;
                break;
            }

            mTargetCell = null;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        ClearCells();

        if (!mTargetCell)
        {
            transform.position = mCurrentCell.gameObject.transform.position;
            return;
        }
        Move();

        mPieceManager.SwitchSides(mColor);
    }
}
