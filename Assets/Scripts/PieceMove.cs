using System.Collections.Generic;
using UnityEngine;

public class PieceMove : MonoBehaviour
{
    protected BasePiece mPiece;
    public Vector3Int mMovement = Vector3Int.one;

    public virtual void Initialize(BasePiece piece)
    {
        this.mPiece = piece;
    }
    public void CheckPathing(Color teamColor, Cell mCurrentCell, List<Cell> mHilightedCells)
    {
        CreateCellPath(-1, -1, mMovement.z,mCurrentCell,mHilightedCells);        
        CreateCellPath(1, -1, mMovement.z, mCurrentCell, mHilightedCells);

        CreateCellPath(1, 1, mMovement.z, mCurrentCell, mHilightedCells);          
        CreateCellPath(-1, 1, mMovement.z, mCurrentCell, mHilightedCells);
    }
    public void CreateCellPath(int xDirection, int yDirection, int movement,Cell mCurrentCell, List<Cell> mHilightedCells)
    {
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        for (int i = 1; i <= movement; i++)
        {
            currentX += xDirection;
            currentY += yDirection;

            CellState cellState = CellState.None;
            cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY, mPiece);

            if (cellState == CellState.Enemy)
            {
                int jumpX = currentX + xDirection;
                int jumpY = currentY + yDirection;

                if (mCurrentCell.mBoard.ValidateCell(jumpX, jumpY, mPiece) == CellState.Free)
                {
                    Cell enemyCell = mCurrentCell.mBoard.mAllCells[currentX, currentY];
                    Cell jumpCell = mCurrentCell.mBoard.mAllCells[jumpX, jumpY];

                    jumpCell.mCapturedPiece = enemyCell.mCurrentPiece;
                    mHilightedCells.Add(jumpCell);
                }
                break;
            }

            if (cellState != CellState.Free)
            {
                break;
            }

            mHilightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
        }
    }
    public void ShowCells(List<Cell> mHilightedCells)
    {
        foreach (Cell cell in mHilightedCells)
        {
            cell.mOutlineImage.enabled = true;
        }
    }
    public void ClearCells(List<Cell> mHilightedCells)
    {
        foreach (Cell cell in mHilightedCells)
        {
            cell.mOutlineImage.enabled = false;
        }
        mHilightedCells.Clear();
    }
    public virtual void Move(Cell mCurrentCell, Cell mTargetCell)
    {
        if (mTargetCell.mCapturedPiece != null)
        {
            mTargetCell.mCapturedPiece.Kill();
            mTargetCell.mCapturedPiece = null;
        }

        mPiece.mCurrentCell.mCurrentPiece = null;
        mPiece.mCurrentCell = mTargetCell;
        mPiece.mCurrentCell.mCurrentPiece = mPiece;

        mPiece.transform.position = mPiece.mCurrentCell.transform.position;
    }
}
