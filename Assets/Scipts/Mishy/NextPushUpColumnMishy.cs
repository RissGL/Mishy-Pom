using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPushUpColumnMishy : MonoBehaviour
{
    private int columnCount;
    private MishyType[] mishies;
    public event EventHandler<MishyType[]> OnMishyPushUp;
    
    private void Start()
    {
        columnCount=GridManager.Instance.GetWidth();
        mishies = new MishyType[columnCount];
    }

    public void NextPushUpColumnMishyInit() 
    {
        RandomAllMishy();
    }

    public void RandomAllMishy() 
    {
        for (int i = 0; i < mishies.Length; i++) 
        {
            mishies[i] = MishyManager.Instance.RandomMishyType();
        }
    }

    public void PushColumnMishyUp()
    {
        OnMishyPushUp?.Invoke(this, mishies);

        RandomAllMishy();
    }
}
