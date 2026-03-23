using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform skillPointVisual;
    

    private void Start()
    {
        skillPointVisual.gameObject.SetActive(false);
    }


    public void SetSkillPointVisual(bool x) 
    {
        skillPointVisual.gameObject.SetActive(x);
    }


}
