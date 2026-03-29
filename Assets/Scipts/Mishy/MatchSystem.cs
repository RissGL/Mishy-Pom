using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSystem : MonoBehaviour
{
    private PlayerBoard board;

    private int currentCombo =0;
    public bool IsMatching { get; private set; }

    public event EventHandler<MatchInfo> OnMatchCleared;
    public event EventHandler<int> OnSkillMatch;
    public event EventHandler<Vector3> OnBadMishyClear;

    public event EventHandler<PlayerBoard> OnGameOver;

    private bool[,] visitedCache;
    List<Mishy> connected = new List<Mishy>();
    Stack<GridPosition> stack = new Stack<GridPosition>();

    private bool isGameOverTriggered = false;

    private void Awake()
    {
        board = GetComponentInParent<PlayerBoard>();

    }

    private void Start()
    {
        visitedCache = new bool[board.gridManager.GetWidth(),
board.gridManager.GetHeight()];
    }

    public class MatchInfo :EventArgs
    {
        public MatchInfo(int matchCount, int matchCombo,Vector3 matchCenter) 
        {
            this.matchCount = matchCount;
            this.matchCombo = matchCombo;
            this.matchCenter = matchCenter;
        }

        public int matchCount;
        public int matchCombo;
        public Vector3 matchCenter;
    }


    // 返回所有符合消除条件的咪西集合
    public List<List<Mishy>> FindAllMatches()
    {
        List<List<Mishy>> allMatches=new List<List<Mishy>>();

        //重置数组
        Array.Clear(visitedCache, 0, visitedCache.Length);

        for (int x = 0; x < board.gridManager.GetWidth(); x++)
        {
            for (int y = 0; y < board.gridManager.GetHeight(); y++) 
            {
                GridPosition gridPosition =new GridPosition(x,y);

                if (!visitedCache[x, y] && board.gridManager.HasMishy(gridPosition))
                {
                    List<Mishy> connectedMishyies = GetConnectedMishies(gridPosition, visitedCache);

                    if (connectedMishyies.Count >= 3)
                    {
                        allMatches.Add(new List<Mishy>(connectedMishyies));
                    }
                }
            }
        }

        return allMatches;
    }

    // DFS 寻找相同颜色的连通块
    private List<Mishy> GetConnectedMishies(GridPosition startPos, bool[,] visited)
    {
        //清空缓存
        connected.Clear();
        stack.Clear();

        MishyType mishyType = board.gridManager.GetGridMishy(startPos).GetMishyType();
        stack.Push(startPos);

        /*
        //判断是否结束游戏
        for (int i = board.mishyManager.GetSpawnY(); i < board.gridManager.GetHeight(); i++)
        {
            GridPosition gridPosition=new GridPosition(board.mishyManager.GetSpawnX(), i);
            if (board.gridManager.HasMishy(gridPosition))
            {
                if (!isGameOverTriggered)
                {
                    OnGameOver?.Invoke(this, board);
                    isGameOverTriggered = true;
                }
            }
        }
        */


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

            connected.Add(board.gridManager.GetGridMishy(current));


            foreach (GridPosition dir in dirs) 
            {
                GridPosition gridPosition= dir+current;

                if (board.gridManager.IsValidGridPosition(gridPosition))
                {
                    if (board.gridManager.HasMishy(gridPosition))
                    {
                        if (board.gridManager.GetGridMishy(gridPosition).GetMishyType() == mishyType)
                        {
                            stack.Push(gridPosition);
                        }
                    }
                }
            }
        }

        if (connected.Count >= 3)
        {
            HashSet<Mishy> badMishiesToDestroy = new HashSet<Mishy>();

            foreach (var mishy in connected)
            {
                GridPosition mishyPosition = mishy.GetGridPosition();

                foreach (GridPosition dir in dirs)
                {
                    GridPosition testPosition = mishyPosition + dir;

                    // 1. 先判断是否越界
                    if (board.gridManager.IsValidGridPosition(testPosition))
                    {
                        if (board.gridManager.TryGetGridMishy(testPosition, out Mishy mishy_test) &&
                            mishy_test.GetMishyType() == MishyType.BadMishy)
                        {
                            badMishiesToDestroy.Add(mishy_test);
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
            List<List<Mishy>> mishyGroups = FindAllMatches();

            if (mishyGroups.Count == 0)
            {
                //判断是否结束游戏
                for (int i = board.mishyManager.GetSpawnY(); i < board.gridManager.GetHeight(); i++)
                {
                    GridPosition gridPosition = new GridPosition(board.mishyManager.GetSpawnX(), i);
                    if (board.gridManager.HasMishy(gridPosition))
                    {
                        if (!isGameOverTriggered)
                        {
                            OnGameOver?.Invoke(this, board);
                            isGameOverTriggered = true;

                            IsMatching = false;
                            yield break;
                        }
                    }
                }

                // 没有任何消除，结算结束，重置 Combo
                currentCombo = 0;

                //board.mishyManager.SpawnNextPair();
                board.mishyManager.OnTurnSettlementFinished();

                IsMatching = false;
                yield break; // 结束协程
            }

            foreach (var matches in mishyGroups)
            {
                OnMatchCleared?.Invoke(this, new MatchInfo(matches.Count,currentCombo,GetMatchCenter(matches)));

                //CameraShakeManager.instance.ShakeMedium();

                // 销毁并清理网格
                foreach (Mishy m in matches)
                {
                    if (m.GetMishyType() == MishyType.BadMishy)
                    {
                        OnBadMishyClear?.Invoke(this,board.gridManager.GetWorldPosition(m.GetGridPosition()));
                    }

                    board.gridManager.ClearGridMishy(m.GetGridPosition());

                    // TODO: 咪西消灭动画
                    m.PlayVanishAni();

                    Destroy(m.gameObject, 0.4f);
                }
            }

            // 等待半秒钟
            yield return new WaitForSeconds(0.4f);

            // 变成协程等待它掉完
            yield return StartCoroutine(board.mishyManager.ApplyGravityRoutine());

            // 连击数增加，进入下一次循环
            currentCombo++;
        }

    }

    public Vector3 GetMatchCenter(List<Mishy> mishies) 
    {
        Vector3 points=Vector3.zero;

        for (int i = 0; i <mishies.Count; i++) 
        {
            points += mishies[i].transform.position;
        }

        return points/mishies.Count;
    }

    public void StartMatchBottomColumnMishy(int count)
    {
        if (IsMatching)
            return;
        StartCoroutine(MatchBottomColumnMishy(count));
    }

    public IEnumerator MatchBottomColumnMishy(int count) 
    {
        int matchScore = 0;
        IsMatching=true;

        for (int x = 0; x < board.gridManager.GetWidth(); x++)
        {
            for (int y = 0; y < count; y++) 
            {
                GridPosition gridPosition=new GridPosition(x,y);

                if (board.gridManager.TryGetGridMishy(gridPosition, out Mishy mishy))
                {
                    board.gridManager.ClearGridMishy(mishy.GetGridPosition());
                    mishy.PlayVanishAni();
                    matchScore++;
                    Destroy(mishy.gameObject, 0.4f);
                }


            }
        }
        OnSkillMatch?.Invoke(this, matchScore);

        yield return new WaitForSeconds(0.4f);

        yield return StartCoroutine(board.mishyManager.ApplyGravityRoutine());
        //board.mishyManager.SpawnNextPair();
        //board.mishyManager.OnTurnSettlementFinished();
        IsMatching = false;
        StartMatchSequence();
    }
}
