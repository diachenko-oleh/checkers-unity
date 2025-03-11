using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pawn : BasePiece
{
    private bool isKing = false;
    private Sprite originalSprite;
    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);
        isKing = false;
        originalSprite = GetComponent<Image>().sprite;
    }
    protected override void Move()
    {
        base.Move();
        if ((mColor == Color.white && mCurrentCell.mBoardPosition.y == 7) ||
            (mColor == Color.black && mCurrentCell.mBoardPosition.y == 0))
        {
            BecomeKing();
        }
    }
    private void BecomeKing()
    {
        isKing = true;
        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_King");
    }
    protected override void CheckPathing(Color teamColor)
    {
        if (isKing)
        {
            CreateCellPath(-1, -1, 1);
            CreateCellPath(1, -1, 1);
            CreateCellPath(1, 1, 1);
            CreateCellPath(-1, 1, 1);
        }
        else
        {
            if (teamColor == Color.black)
            {
                CreateCellPath(-1, -1, 1);
                CreateCellPath(1, -1, 1);
            }
            if (teamColor == Color.white)
            {
                CreateCellPath(1, 1, 1);
                CreateCellPath(-1, 1, 1);
            }
        }
    }
    public static void RevertKingsToPawns(List<BasePiece> allPawns)
    {

        foreach (Pawn pawn in allPawns)
        {
            if (pawn.isKing)
            {
                pawn.isKing = false;
                pawn.GetComponent<Image>().sprite = pawn.originalSprite;
            }
        }
    }
}

