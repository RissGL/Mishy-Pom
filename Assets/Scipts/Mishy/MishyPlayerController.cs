using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyPlayerController : MonoBehaviour
{
    private Mishy mishy_One;
    private Mishy mishy_Two;
    private bool isActive;

    public void SetActivePair(Mishy mishy_One, Mishy mishy_Two)
    {
        this.mishy_One = mishy_One;
        this.mishy_Two = mishy_Two;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) 
            return;

        if (Input.GetKeyDown(KeyCode.A))
        { 

        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            
        }
    }

    private bool TryMove(GridPosition moveDir,Mishy mishy) 
    {
        //TODO: 检测是否能动，不能未来要触发错误音效
        return true;
    }

    private void SwapMishies() 
    {
        Vector3 temp = mishy_One.transform.localPosition;
        mishy_One.transform.localPosition = mishy_Two.transform.localPosition;
        mishy_Two.transform.localPosition=temp;
    }
}
