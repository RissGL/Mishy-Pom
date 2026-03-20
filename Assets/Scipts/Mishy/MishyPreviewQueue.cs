using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyPreviewQueue : MonoBehaviour
{
    [SerializeField] private int reviewCount=10;
    private Queue<MishyType> nextMishies = new Queue<MishyType>();

    public event EventHandler<twoMishy> OnNextMishyNeedSpawn;

    public class twoMishy : EventArgs
    {
        public twoMishy(MishyType type_one, MishyType type_two)
        {
            this.type_one = type_one;
            this.type_two = type_two;
        }

        public MishyType type_one;
        public MishyType type_two;
    }

    public void MishyPreviewQueueInit() 
    {
        nextMishies.Clear();

        for (int i = 0; i < reviewCount; i++) 
        {
            nextMishies.Enqueue(RandomMishyType());
        }
    }

    public void DequeueNextMishy() 
    {
        twoMishy twoMishy = new twoMishy(nextMishies.Dequeue(),nextMishies.Dequeue());

        for (int i = 0; i < 2; i++) 
        {
            nextMishies.Enqueue(RandomMishyType());
        }
        OnNextMishyNeedSpawn?.Invoke(this, twoMishy);
    }

    public MishyType RandomMishyType() 
    {
        return (MishyType)UnityEngine.Random.Range(1, 5);//不会在这生成恶咪西
    }
}
