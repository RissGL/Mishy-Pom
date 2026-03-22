using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MatchSystem : MonoBehaviour
{
    private int currentCombo =0;
    public const int MAX_COMBO_COUNT = 6;

    public bool IsMatching { get; private set; }


    // 返回所有符合消除条件的咪西集合
    public List<Mishy> FindAllMatches()
    {
        List<Mishy> allMatches=new List<Mishy>();
        bool[,] visited = new bool[GridManager.Instance.GetWidth(),
            GridManager.Instance.GetHeight()];

        for (int x = 0; x < GridManager.Instance.GetWidth(); x++)
        {
            for (int y = 0; y < GridManager.Instance.GetHeight(); y++) 
            {
                GridPosition gridPosition =new GridPosition(x,y);

                if (!visited[x, y] && GridManager.Instance.HasMishy(gridPosition))
                {
                    List<Mishy> connectedMishyies = GetConnectedMishies(gridPosition, visited);

                    if (connectedMishyies.Count >= 3)
                    {
                        allMatches.AddRange(connectedMishyies);
                    }
                }
            }
        }

        return allMatches;
    }

    // DFS 寻找相同颜色的连通块
    private List<Mishy> GetConnectedMishies(GridPosition startPos, bool[,] visited)
    {
        List<Mishy> connected=new List<Mishy>();
        MishyType mishyType= GridManager.Instance.GetGridMishy(startPos).GetMishyType();

        Stack<GridPosition> stack = new Stack<GridPosition>();
        stack.Push(startPos);


        GridPosition[] dirs = {
                new GridPosition(0,1),
                new GridPosition(1,0),
                new GridPosition(0,-1),
                new GridPosition(-1,0),
            };

        while (stack.Count > 0) 
        {
            GridPosition current= stack.Pop();
            if (visited[current.x,current.y]) continue;
            visited[current.x,current.y] = true;

            connected.Add(GridManager.Instance.GetGridMishy(current));


            foreach (GridPosition dir in dirs) 
            {
                GridPosition gridPosition= dir+current;

                if (GridManager.Instance.IsValidGridPosition(gridPosition))
                {
                    if (GridManager.Instance.HasMishy(gridPosition))
                    {
                        if (GridManager.Instance.GetGridMishy(gridPosition).GetMishyType() == mishyType)
                        {
                            stack.Push(gridPosition);
                        }
                    }
                }
            }
        }

        if (connected.Count >= 3)
        {
            List<Mishy> badMishiesToDestroy = new List<Mishy>();
            foreach (var mishy in connected)
            {
                GridPosition mishyPosition = mishy.GetGridPosition();

                foreach (GridPosition dir in dirs)
                {
                    GridPosition testPosition = mishyPosition + dir;

                    // 1. 先判断是否越界
                    if (GridManager.Instance.IsValidGridPosition(testPosition))
                    {
                        // 2. 再安全地获取咪西，看是不是坏咪西
                        if (GridManager.Instance.TryGetGridMishy(testPosition, out Mishy mishy_test) &&
                            mishy_test.GetMishyType() == MishyType.BadMishy)
                        {
                            // 3. 防止同一个坏咪西被多个相邻的普通咪西重复添加
                            if (!badMishiesToDestroy.Contains(mishy_test))
                            {
                                badMishiesToDestroy.Add(mishy_test);
                            }
                        }
                    }
                }
            }
            connected.AddRange(badMishiesToDestroy);
        }
        return connected;
    }

    public void StartMatchSequence()
    {
        if (IsMatching)
            return;
        StartCoroutine(ProcessMatchesRoutine());
    }

    private IEnumerator ProcessMatchesRoutine()
    {
        IsMatching = true;
        currentCombo = 1;
        while (true)
        {
            List<Mishy> matches = FindAllMatches();

            if (matches.Count == 0)
            {
                // 没有任何消除，结算结束，重置 Combo
                currentCombo = 0;

                MishyManager.Instance.SpawnNextPair();

                IsMatching = false;
                yield break; // 结束协程
            }

            // 计算分数
            int multiplier = Mathf.Min(currentCombo, MAX_COMBO_COUNT);
            int score = matches.Count * 10 * multiplier;
            Debug.Log($"消除了 {matches.Count} 个，连击数 {currentCombo}，获得分数：{score}");

            // 销毁并清理网格
            foreach (Mishy m in matches)
            {
                GridManager.Instance.ClearGridMishy(m.GetGridPosition());

                // TODO: 咪西消灭动画
                m.PlayVanishAni();


                Destroy(m.gameObject,0.4f);
            }

            // 等待半秒钟
            yield return new WaitForSeconds(0.4f);

            // 变成协程等待它掉完
            yield return StartCoroutine(MishyManager.Instance.ApplyGravityRoutine());

            // 连击数增加，进入下一次循环
            currentCombo++;
        }
    }
}
