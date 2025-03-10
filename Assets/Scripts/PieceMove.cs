using System.Collections.Generic;
using UnityEngine;

public class PieceMove : MonoBehaviour
{
    private BasePiece mPiece;

    protected Cell mCurrentCell;
    protected Cell mTargetCell;

    public Vector3Int mMovement = Vector3Int.one;
    public List<Cell> mHilightedCells = new List<Cell>();
    public void SetCell(Cell currentCell,Cell targetCell)
    {
        mCurrentCell = currentCell;
        mTargetCell = targetCell;
    }
    public void SetPiece(BasePiece piece)
    {
        mPiece = piece;
    }
    public virtual void CheckPathing(Color teamColor)
    {
        CreateCellPath(-1, -1, mMovement.z);        //діагональ вниз
        CreateCellPath(1, -1, mMovement.z);

        CreateCellPath(1, 1, mMovement.z);          //діагональ вгору
        CreateCellPath(-1, 1, mMovement.z);
    }
    public void CreateCellPath(int xDirection, int yDirection, int movement)
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

    public void ShowCells()
    {
        foreach (Cell cell in mHilightedCells)
        {
            cell.mOutlineImage.enabled = true;
        }
    }
    public void ClearCells()
    {
        foreach (Cell cell in mHilightedCells)
        {
            cell.mOutlineImage.enabled = false;
        }
        mHilightedCells.Clear();
    }
    public virtual void Move()
    {
        if (mTargetCell.mCapturedPiece != null)
        {
            mTargetCell.mCapturedPiece.Kill();
            mTargetCell.mCapturedPiece = null;
        }

        mCurrentCell.mCurrentPiece = null;
        mCurrentCell = mTargetCell;
        mCurrentCell.mCurrentPiece = mPiece;

        transform.position = mCurrentCell.transform.position;
        mTargetCell = null;
    }
}
