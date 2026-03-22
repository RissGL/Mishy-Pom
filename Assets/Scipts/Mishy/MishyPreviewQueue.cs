using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyPreviewQueue : MonoBehaviour
{
    [SerializeField] private int reviewCount=10;
    private Queue<MishyType> nextMishies = new Queue<MishyType>();

    public event EventHandler<MishyPairEventArgs> OnNextMishyNeedSpawn;

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

    private void Start()
    {
        MishyPreviewQueueInit();
    }

    public void MishyPreviewQueueInit() 
    {
        nextMishies.Clear();

        for (int i = 0; i < reviewCount; i++) 
        {
            nextMishies.Enqueue(MishyManager.Instance.RandomMishyType());
        }
    }

    public void DequeueNextMishy() 
    {
        MishyPairEventArgs twoMishy = new MishyPairEventArgs(nextMishies.Dequeue(),nextMishies.Dequeue());

        for (int i = 0; i < 2; i++) 
        {
            nextMishies.Enqueue(MishyManager.Instance.RandomMishyType());
        }
        OnNextMishyNeedSpawn?.Invoke(this, twoMishy);
    }

}
