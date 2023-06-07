using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool{
        
    private PoolableObject Prefab;
    private Queue<PoolableObject> availableObjects;

    private ObjectPool(PoolableObject Prefab){
        this.Prefab = Prefab;
        availableObjects = new Queue<PoolableObject>();
    }

    public static ObjectPool CreateInstance(PoolableObject Prefab, int Size, Transform parent, EnemySpawner enemySpawner)
    {
        ObjectPool pool = new ObjectPool(Prefab);
        GameObject poolObject = new GameObject(Prefab.name + " Pool");
        poolObject.transform.parent = parent;
        pool.CreateObjects(poolObject.transform, Size, enemySpawner);

        return pool;
    }

    private void CreateObjects(Transform parent, int Size, EnemySpawner enemySpawner)
    {
        for(int i = 0; i < Size; i++){

            PoolableObject poolableObject = GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity, parent.transform);
            

            poolableObject.Parent = this;
            EnemyHealthSystem health;

            if(poolableObject.TryGetComponent<EnemyHealthSystem>(out health)){
                health.onDie.AddListener(enemySpawner.OnEnemyDieEvent);
            }

            poolableObject.gameObject.SetActive(false);
            

        }
    }

    public void ReturnObjectToPool(PoolableObject poolableObject){
        availableObjects.Enqueue(poolableObject);
    }

    public PoolableObject GetObject(){

        if(availableObjects.Count > 0){
            PoolableObject instance = availableObjects.Dequeue();

            instance.gameObject.SetActive(true);

            return instance;
        }

        return null; 

    }


}
