using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MatchSystem : MonoBehaviour
{
    private int currentCombo =0;
    public const int MAX_COMBOO_COUNT = 6;

    // 返回所有符合消除条件的咪西集合
    public List<Mishy> FindAllMatches()
    {
        List<Mishy> allMatches=new List<Mishy>();
        bool[,] visited = new bool[MishyManager.Instance.GetGridSystemWidth(), 
            MishyManager.Instance.GetGridSystemHeight()];

        for (int x = 0; x < MishyManager.Instance.GetGridSystemWidth(); x++)
        {
            for (int y = 0; y < MishyManager.Instance.GetGridSystemHeight(); y++) 
            {
                GridPosition gridPosition =new GridPosition(x,y);

                if (!visited[x, y] && MishyManager.Instance.GetGridSystem().HasMishy(gridPosition))
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
        MishyType mishyType=MishyManager.Instance.GetGridSystem().GetGridMishy(startPos).GetMishyType();

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

            connected.Add(MishyManager.Instance.GetGridSystem().GetGridMishy(current));


            foreach (GridPosition dir in dirs) 
            {
                GridPosition gridPosition= dir+current;

                if (MishyManager.Instance.GetGridSystem().IsValidGridPosition(gridPosition))
                {
                    if (MishyManager.Instance.GetGridSystem().GetGridMishy(gridPosition).GetMishyType() == mishyType)
                    {
                        stack.Push(gridPosition);
                    }
                }
            }
        }

        if (connected.Count >= 3)
        {
            foreach (var mishy in connected)
            {
                GridPosition mishyPosition = mishy.GetGridPosition();

                foreach (GridPosition dir in dirs)
                {
                    GridPosition testPosition = mishyPosition + dir;
                    if (MishyManager.Instance.GetGridSystem().GetGridMishy(testPosition).GetMishyType()
                        ==MishyType.BadMishy)
                    {
                        connected.Add(MishyManager.Instance.GetGridSystem().GetGridMishy(testPosition));
                    }
                }
            }
        }
        return connected;
    }

    private IEnumerator ProcessMatchesRoutine()
    {
        currentCombo = 1;
        while (true)
        {
            List<Mishy> matches = FindAllMatches();

            if (matches.Count == 0)
            {
                // 没有任何消除，结算结束，重置 Combo，生成下一对咪西
                currentCombo = 0;
                // 通知 Controller 生成新的
                yield break;
            }

            // 1. 计算分数
            int multiplier = Mathf.Min(currentCombo, MAX_COMBOO_COUNT);
            int score = matches.Count * 10 * multiplier;
            Debug.Log($"消除了 {matches.Count} 个，连击数 {currentCombo}，获得分数：{score}");

            // 2. 销毁并清理网格
            foreach (Mishy m in matches)
            {
                MishyManager.Instance.GetGridSystem().ClearGridMishy(m.GetGridPosition());
                Destroy(m.gameObject);
            }

            // 3. 等待半秒钟，让玩家看清楚消除了
            yield return new WaitForSeconds(0.5f);

            // 4. 重力下落算法 (把上面的方块往下挪)
            MishyManager.Instance.ApplyGravity();

            // 5. 等待方块掉下去
            yield return new WaitForSeconds(0.3f);

            // 连击数增加，进入下一次 while 循环检测
            currentCombo++;
        }
    }
}
