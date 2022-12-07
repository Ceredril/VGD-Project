using System;
using UnityEngine;

/*
 * Manages the spawn in the game.
 */

public class spawnManager : MonoBehaviour
{
    private Transform _spawnPoint;
    public static Transform LastCheckpoint;

    private void Start()
    {
        _spawnPoint = GameObject.Find("checkPoint_0").transform;
        LastCheckpoint = _spawnPoint;
        GameManager.OnGameStart += Spawn;
        GameManager.OnPlayerSpawn += Spawn; //Temporary. This is just to use T to respawn
        GameManager.OnCheckpointReached += SetSpawnPoint;
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerSpawn -= Spawn;
        GameManager.OnCheckpointReached -= SetSpawnPoint;
    }

    private void SetSpawnPoint(Transform checkpoint)
    {
        LastCheckpoint = checkpoint;
        Debug.Log("Spawn point set");
    }
    private void Spawn()
    {
        BodyMovement.instance.transform.position = LastCheckpoint.position;
        Physics.SyncTransforms();
        Debug.Log("Player spawned");
    }
}
