using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPushUpColumnMishy : MonoBehaviour
{
    private PlayerBoard board;

    private int columnCount;
    private MishyType[] previewMishies;


    public event EventHandler<MishyType[][]> OnMultiMishyPushUp;
    public event EventHandler<MishyType[]> OnUpdateSingleRow;

    private void Awake()
    {
        board = GetComponentInParent<PlayerBoard>();
    }

    /*private void Start()
    {
        columnCount = board.gridManager.GetWidth();
        NextPushUpColumnMishyInit();
    }*/

    public void NextPushUpColumnMishyInit()
    {
        columnCount = board.gridManager.GetWidth();
        previewMishies = GenerateSingleRow(null);
    }

    public MishyType[] GenerateSingleRow(MishyType[] upperRowTypes)
    {
        MishyType[] newRow = new MishyType[columnCount];

        for (int x = 0; x < columnCount; x++)
        {
            int tryCount = 0;
            MishyType mishyType = board.mishyManager.RandomMishyTypeWithBadMishy();
            while (tryCount < 30)
            {
                bool isConflict = false;

                if (x > 0 && mishyType == newRow[x - 1]) isConflict = true;

                if (upperRowTypes != null && mishyType == upperRowTypes[x]) isConflict = true;

                if (isConflict)
                {
                    if (UnityEngine.Random.Range(1, 100) > 97)
                    {
                        break;
                    }

                    mishyType = board.mishyManager.RandomMishyTypeWithBadMishy();
                    tryCount++;
                }
                else 
                {
                    break;
                }
            }
            newRow[x] = mishyType;
        }
        return newRow;
    }


    /// <summary>
    /// 触发推挤多行 (如果不传参数，默认推 1 行)
    /// </summary>
    public void PushMultiColumnMishyUp(int rowCount = 1)
    {
        MishyType[][] rowsToPush = new MishyType[rowCount][];

        rowsToPush[rowCount - 1] = (MishyType[])previewMishies.Clone();

        for (int y = rowCount - 2; y >= 0; y--)
        {
            rowsToPush[y] = GenerateSingleRow(rowsToPush[y + 1]);
        }

        OnMultiMishyPushUp?.Invoke(this, rowsToPush);

        MishyType[] gridBottomRow = new MishyType[columnCount];
        for (int x = 0; x < columnCount; x++)
        {
            if (board.gridManager.TryGetGridMishy(new GridPosition(x, 0), out Mishy m))
                gridBottomRow[x] = m.GetMishyType();
            else
                gridBottomRow[x] = (MishyType)999; // 给个不会冲突的假类型
        }
        previewMishies = GenerateSingleRow(gridBottomRow);
        OnUpdateSingleRow?.Invoke(this, previewMishies);
    }
}
