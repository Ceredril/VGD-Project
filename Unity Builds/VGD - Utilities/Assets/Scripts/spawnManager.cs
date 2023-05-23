using System;
using UnityEngine;

/*
 * Manages the spawn in the game.
 */

public class spawnManager : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform lastCheckpoint;

    public static spawnManager Instance;

    private void Awake() => Instance = this;
    private void Start()
    {
        GameManager.OnGameStart += Spawn;
        GameManager.OnPlayerSpawn += Respawn; // Temporary. This is just to use T to respawn
        GameManager.OnGameRestart += Respawn;
        GameManager.OnCheckpointReached += SetSpawnPoint;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= Spawn;
        GameManager.OnPlayerSpawn -= Respawn;
        GameManager.OnGameRestart -= Respawn;
        GameManager.OnCheckpointReached -= SetSpawnPoint;
    }

    private void SetSpawnPoint(Transform checkpoint)
    {
        lastCheckpoint = checkpoint;
        Debug.Log("Spawn point set");
    }

    private void Spawn()
    {
        BodyMovement.instance.transform.position = Instance.spawnPoint.position;
        BodyMovement.instance.transform.rotation = Instance.spawnPoint.rotation;
        Physics.SyncTransforms();
    }
    private void Respawn()
    {
        if (GameManager.GameIsOver) return;
        BodyMovement.instance.transform.position = lastCheckpoint.position;
        BodyMovement.instance.transform.rotation = lastCheckpoint.rotation;
        Physics.SyncTransforms();
    }
}
