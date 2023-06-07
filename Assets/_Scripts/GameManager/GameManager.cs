using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Unity.AI;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] World world;
    [SerializeField] CinemachineVirtualCamera camera_VM;
    [SerializeField] GameObject RenderedChunks;
    [SerializeField] Transform spawnedEnemies;

    
    [SerializeField]NavMeshManager navMeshManager;
    [SerializeField]PlayerManager playerManager;
    [SerializeField]EnemySpawner enemySpawner;

    public void Awake(){

        camera_VM.transform.position = new Vector3(0, world.worldSettings.voxelMaxMapDimensions.y, 0);
        camera_VM.transform.rotation *= Quaternion.Euler(Vector3.right * 90);
        camera_VM.m_Lens.ModeOverride = Cinemachine.LensSettings.OverrideModes.Orthographic;

        playerManager = GetComponentInChildren<PlayerManager>();
        navMeshManager = GetComponentInChildren<NavMeshManager>();
        enemySpawner = GetComponentInChildren<EnemySpawner>();
    
    }

    public void SetUpManagers(){
        navMeshManager.SetupNavMeshManager(RenderedChunks);
        playerManager.SetupPlayerManager(world, camera_VM);

        Transform player = playerManager.GetPlayer();
        enemySpawner.SetupEnemySpawner(player, spawnedEnemies);

    }

    public void OnEnemyDieEvent(){
        if(enemySpawner == null){
            enemySpawner = GetComponentInChildren<EnemySpawner>();
        }
        enemySpawner.OnEnemyDieEvent();
    }
    

}
