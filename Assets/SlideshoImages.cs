using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SceneType{
    Plains,
    Desert,
    Islands,
    Caves
}

[Serializable]
public class SceneImage{
    public SceneType sceneType;
    public Sprite sceneImage;
}

public class SlideshoImages : MonoBehaviour
{
    public Image panel;
    [SerializeField] public List<SceneImage> sceneImageList;
    private Dictionary<SceneType, Sprite> sceneImageDictionary;
    public float timeBetweenSpriteChange = 5f;

    public int spriteIndex = 0;

    public bool startSlideShowOnEnable = true;

    void Awake()
    {
        if(panel == null){
            panel = GetComponent<Image>();
        }

        panel.sprite = sceneImageList[spriteIndex].sceneImage;

        sceneImageDictionary = new Dictionary<SceneType, Sprite>();
        foreach(var item in sceneImageList){
            sceneImageDictionary.Add(item.sceneType, item.sceneImage);
        }

    }

    void Update(){
        if(panel.sprite == null){
            panel.enabled = false;
        }
        else{
            panel.enabled = true;
        }
    }


    IEnumerator ChangeSprite(float time){
        while(true){
            yield return new WaitForSeconds(time);

            spriteIndex = (++spriteIndex) % sceneImageList.Count;
            panel.sprite = sceneImageList[spriteIndex].sceneImage;

        }
        
    }
    
    public void ChangeSceneImage(string sceneImageString){

        SceneType sceneType;

        if(!Enum.TryParse<SceneType>(sceneImageString, true, out sceneType)){
            Debug.Log("Scene string not found");
            return;
        }

        Sprite sceneImage;

        if(!sceneImageDictionary.TryGetValue(sceneType, out sceneImage)){
            Debug.Log("Scene image not found");
            return;
        }

        panel.sprite = sceneImage;
        spriteIndex = (int)sceneType;


    }

    public void StartSceneChangeCoroutine(){
        StopCoroutine(ChangeSprite(timeBetweenSpriteChange));
        StartCoroutine(ChangeSprite(timeBetweenSpriteChange));
    }

    void OnEnable()
    {   
        if(startSlideShowOnEnable){
            StartSceneChangeCoroutine();
        }
    }

    void OnDisable()
    {
        panel.sprite = null;
        StopCoroutine(ChangeSprite(timeBetweenSpriteChange));
    }


    void OnValidate()
    {
        sceneImageDictionary = new Dictionary<SceneType, Sprite>();
        foreach(var item in sceneImageList){
            sceneImageDictionary.Add(item.sceneType, item.sceneImage);
        }
    }
}
