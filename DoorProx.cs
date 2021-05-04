using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorProx : MonoBehaviour
{
    
    [SerializeField] private Animator door = null;
    
    [SerializeField] private bool isOpen = false;
    
    void OnTriggerEnter(Collider col){
        if(!isOpen && col.tag == "Player" && door.GetCurrentAnimatorStateInfo(0).IsName("doorClosed")){
            // yield return new WaitUntil(() => door.GetCurrentAnimatorStateInfo(0).IsName("doorClosed"));
           
            door.Play("doorOpen");
            
            isOpen=true;
        }    
    }

    IEnumerator OnTriggerStay(Collider col){
        if(col.tag == "Player" && isOpen == false){
            yield return new WaitUntil(() => door.GetCurrentAnimatorStateInfo(0).IsName("doorClosed"));
            
            door.Play("doorOpen");
            
            isOpen=true;
        }
    }

    IEnumerator OnTriggerExit(Collider col){
        if(isOpen && col.tag == "Player" ){
            yield return new WaitUntil(() => door.GetCurrentAnimatorStateInfo(0).IsName("doorOpened"));
            door.Play("doorClose");
           
            isOpen=false;
        }    
    }
    

}
