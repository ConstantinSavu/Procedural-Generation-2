using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{   
    [SerializeField] float spawnDelay = 1f;

    [SerializeField] int maxSpawnedEnemies = 100;

    [SerializeField] int enemiesToDefeat = 7;

    [SerializeField] int enemiesDefeated = 0;

    [SerializeField] SpawnMethod enemySpawnMethod = SpawnMethod.RoundRobin;

    [SerializeField] List<Enemy> enemySpawnList = new List<Enemy>();

    Transform target;
    [SerializeField] Enemy boss_prefab;
    ObjectPool Boss_pool;

    [SerializeField] float maxSpawnDistanceFromTarget = 50f;

    [SerializeField] UnityEvent onPause;

    private Dictionary<int, ObjectPool> enemyObjectPools = new Dictionary<int, ObjectPool>();
    NavMeshTriangulation triangulation;
    public void SetupEnemySpawner(Transform target, Transform parent){

        

        triangulation = NavMesh.CalculateTriangulation();
        this.target = target;

        for(int i = 0; i < enemySpawnList.Count; i++){
            ObjectPool enemy = ObjectPool.CreateInstance(
                enemySpawnList[i],
                maxSpawnedEnemies,
                parent, this);
            
            enemyObjectPools.Add(i, enemy);
            
        }

        Boss_pool = ObjectPool.CreateInstance(boss_prefab, 1, parent, this);
        
        enemiesDefeated = 0;

        StartCoroutine(SpawnEnemies(target));
    }

    private IEnumerator SpawnEnemies(Transform target){

        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        int spawnedEnemies = 0;
        int spawnIndex = 0;
        int maxIter = 10000;
        int iter = 0;

        while(spawnedEnemies < maxSpawnedEnemies && iter < maxIter){
            
            
            if(enemySpawnMethod == SpawnMethod.Random){
                spawnIndex = GetRandomEnemyIndex();
            }
            else if(enemySpawnMethod == SpawnMethod.RoundRobin){
                spawnIndex = GetRoundRobinEnemyIndex(spawnedEnemies);
            }
            
            DoSpawnEnemy(spawnIndex, target);
            spawnedEnemies++;
            iter++;

            yield return wait;
        }

        if(!bossSpawned){
            bossSpawned = true;
            SpawnBoss();
        }

        Debug.Log("Finished Spawning");
    }

    private int GetRandomEnemyIndex()
    {
        return UnityEngine.Random.Range(0, enemySpawnList.Count);
    }

    private int GetRoundRobinEnemyIndex(int spawnedEnemiesIndex)
    {
        return spawnedEnemiesIndex % enemySpawnList.Count;
    }

    
    private bool DoSpawnEnemy(int index, Transform target){

        PoolableObject poolableObject = enemyObjectPools[index].GetObject();

        if(poolableObject == null){
            Debug.Log("Pool ran out of capacity");
            return false;
        }

        Enemy enemy = poolableObject.GetComponent<Enemy>();

        if(enemy == null){
            Debug.Log("Transform " + poolableObject.name +  " had no enemy component");
            return false;
        }

        int vertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);

        NavMeshHit hit;
        
        if(NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out hit, 2f, 1)){
            enemy.StartupEnemy(target, hit);
            return true;
        }

        Debug.Log("No hit found, returning to FATHER");
        poolableObject.gameObject.SetActive(false);
        return false;

    }

    public enum SpawnMethod{
        RoundRobin,
        Random
    }

    
    bool bossSpawned = false;
    public void OnEnemyDieEvent(EnemyType enemyType){
        enemiesDefeated++;

        if(enemiesDefeated >= enemiesToDefeat && !bossSpawned){
            bossSpawned = true;
            SpawnBoss();
        }

        if(enemyType == EnemyType.Boss){
            
            KilledBoss();
        }
    }

    private void KilledBoss(){

        Debug.Log("Boss killed");
        onPause?.Invoke();
        

    }
    private void SpawnBoss()
    {
        PoolableObject poolableObject = Boss_pool.GetObject();

        if(poolableObject == null){
            Debug.Log("No Boss today");
        }

        Enemy enemy = poolableObject.GetComponent<Enemy>();

        if(enemy == null){
            Debug.Log("No Boss today");
        }

        bool bossFound = false;
        for(int i = 0; i < triangulation.vertices.Length; i++){

            NavMeshHit hit;
            if(NavMesh.SamplePosition(triangulation.vertices[i], out hit, 2f, 1)){
                float distance = Vector3.Distance(target.position, hit.position);
                distance = Mathf.Abs(distance);

                if(distance < maxSpawnDistanceFromTarget){
                    enemy.StartupEnemy(target, hit);
                    bossFound = true;
                    break;
                }
            
            }

        }

        if(bossFound){
            return;
        }

        onPause.Invoke();
        
        Debug.Log("No hit found, returning to FATHER");
        poolableObject.gameObject.SetActive(false);
        

    }

    protected IEnumerator Timer(float seconds, Action callback){
        yield return new WaitForSeconds(seconds);

        callback();
    }

}
