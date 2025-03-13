using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

public abstract class BasePiece : EventTrigger
{
    public GameManager gameManager;
    private static int blackCount = 12, whiteCount = 12;
    [HideInInspector]
    public Color mColor = Color.clear;

    public Cell mOriginalCell = null;
    public Cell mCurrentCell = null;

    protected RectTransform mRectTransform = null;
    protected PieceManager mPieceManager;

    protected Vector3Int mMovement = Vector3Int.one;
    protected List<Cell> mHilightedCells = new List<Cell>();

    protected Cell mTargetCell = null;
    protected PieceMove movement;

    protected virtual void Awake()
    {
        movement = GetComponent<PieceMove>();
        if (movement == null)
        {
            movement = gameObject.AddComponent<PieceMove>();
        }
        movement.Initialize(this);
    }

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
        if (mColor == Color.black)
        {
            blackCount--;
        }
        if (mColor == Color.white)
        {
            whiteCount--;
        }

        if (blackCount == 0)
        {
            gameManager.TeamWin(false,true);
            blackCount = 12;
            whiteCount = 12;
        }
        else if (whiteCount == 0)
        {
            gameManager.TeamWin(false, false);
            blackCount = 12;
            whiteCount = 12;
        }
    }

    #region Movement
    protected void CreateCellPath(int xDirection, int yDirection, int moveDirecton)
    {
        movement.CreateCellPath(xDirection, yDirection, moveDirecton,mCurrentCell,mHilightedCells);
    }
    protected virtual void CheckPathing(Color teamColor)
    {
        movement.CheckPathing(teamColor, mCurrentCell, mHilightedCells);
    }
    protected void ShowCells()
    {
        movement.ShowCells(mHilightedCells);
    }
    protected void ClearCells()
    {
        movement.ClearCells(mHilightedCells);
    }
    protected virtual void Move()
    {
        movement.Move(mCurrentCell,mTargetCell);
    }
    #endregion

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
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.mRectTransform, Input.mousePosition))
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
        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.None)[0];
        if (!mTargetCell)
        {
            transform.position = mCurrentCell.gameObject.transform.position;
            return;
        }
        Move();

        GameManager.Instance.SwitchSides(mColor);
    }
}