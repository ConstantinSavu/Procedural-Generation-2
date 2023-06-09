using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartManager : MonoBehaviour
{
    Queue<GameObject> heartsPool = new Queue<GameObject>();
    Queue<GameObject> activeHearts = new Queue<GameObject>();

    [SerializeField] GameObject heartObject;

    RectTransform rectTransform;

    public void InstantiateHearts(int maxHearts){

        for(int i = 0; i < maxHearts; i++){
            AddHeart();
        }

    }

    public void AddHeart(){

        GameObject heart;

        if(heartsPool.Count > 0){
            heart = heartsPool.Dequeue();
            heart.SetActive(true);
        }
        else{
            heart = Instantiate(heartObject, transform.position, transform.rotation, transform);
        }

        activeHearts.Enqueue(heart);

    }

    public void RemoveHeart(){

        GameObject heart;

        if(activeHearts.Count <= 0){
            Debug.Log("No more hearts");
            return;
        }
        
        heart = activeHearts.Dequeue();
        heart.SetActive(false);
        heartsPool.Enqueue(heart);
    }



}
