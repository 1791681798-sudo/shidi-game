using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthLogic : MonoBehaviour
{
    public string IsGrowth;
    public bool IsLuWeiGrowth;
    public Animator LuWeianimtor;
    public bool IsFlowerGrowth;
    public Animator Floweranimtor;
    public GameObject PS;
    public Animator Grass1Anim,Grass2Anim,Grass3Anim;
    public Animator Ground1Anim,Ground2Anim,Ground3Anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(IsGrowth=="LuWei"){
            IsLuWeiGrowth=true;
        }
         if(IsGrowth=="Flower"){
            IsFlowerGrowth=true;
        }
    if(IsLuWeiGrowth==true){
       LuWeianimtor.SetBool("IsGrowth",true);
       PS.SetActive(true);
       Invoke("GrassGrowth",2f);
    }
    if(IsFlowerGrowth==true){
        Floweranimtor.SetBool("IsGrowth",true);
         PS.SetActive(true);
    }
    }
   void GrassGrowth(){

    Grass1Anim.SetBool("IsGrowth",true);
    Grass2Anim.SetBool("IsGrowth",true);
     Grass3Anim.SetBool("IsGrowth",true);
     Ground1Anim.SetBool("IsChange",true);
    Ground2Anim.SetBool("IsChange",true);
    Ground3Anim.SetBool("IsChange",true);
   }
}
