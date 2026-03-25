using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyPreviewQueue : MonoBehaviour
{
    private PlayerBoard board;
    [SerializeField] private int reviewCount=10;
    private Queue<MishyType> nextMishies = new Queue<MishyType>();

    public event EventHandler<MishyPairEventArgs> OnNextMishyNeedSpawn;

    public event EventHandler<Queue<MishyType>> OnNextMishyEequeue;
    public event EventHandler OnPreviewQueueInit;

    private void Awake()
    {
        board = GetComponentInParent<PlayerBoard>();
    }


    public class MishyPairEventArgs : EventArgs
    {
        public MishyPairEventArgs(MishyType type_one, MishyType type_two)
        {
            this.type_one = type_one;
            this.type_two = type_two;
        }

        public MishyType type_one;
        public MishyType type_two;
    }

   /* private void Start()
    {
        MishyPreviewQueueInit();
    }*/

    public void MishyPreviewQueueInit() 
    {
        nextMishies.Clear();

        for (int i = 0; i < reviewCount; i++) 
        {
            nextMishies.Enqueue(board.mishyManager.RandomMishyType());
        }
        OnPreviewQueueInit?.Invoke(this,EventArgs.Empty);
    }

    public void DequeueNextMishy() 
    {
        if (nextMishies == null||nextMishies.Count<=0)
        {
            MishyPreviewQueueInit();
        }


        MishyPairEventArgs twoMishy = new MishyPairEventArgs(nextMishies.Dequeue(),nextMishies.Dequeue());

        Queue<MishyType> twoEnqueueMishy = new Queue<MishyType>();

        for (int i = 0; i < 2; i++) 
        {
            MishyType mishyType = board.mishyManager.RandomMishyType();
            nextMishies.Enqueue(mishyType);
            twoEnqueueMishy.Enqueue(mishyType);
        }
        OnNextMishyNeedSpawn?.Invoke(this, twoMishy);
        OnNextMishyEequeue?.Invoke(this, twoEnqueueMishy);
    }

    /// <summary>
    /// 提供给UI的方法
    /// </summary>
    /// <returns></returns>
    public Queue<MishyType> GetAllNextMishy() 
    {
        return nextMishies;
    }
}
