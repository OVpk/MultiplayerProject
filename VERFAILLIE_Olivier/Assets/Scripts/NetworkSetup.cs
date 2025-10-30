using System;
using Unity.Multiplayer.Playmode;
using Unity.Multiplayer.Tools.Editor.MultiplayerToolsWindow;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkSetup : MonoBehaviour
{
    [SerializeField] private bool debugMode;
    
    [SerializeField] private GameObject hunterPrefab;
    [SerializeField] private GameObject rabbitPrefab;

    private void Start()
    {
        bool mustBeServer = debugMode 
            ? CurrentPlayer.IsMainEditor
            : SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
        
        if (mustBeServer)
        {
            NetworkManager.Singleton.StartServer();
            NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
        }
        else
        {
            NetworkManager.Singleton.StartClient();
            Destroy(this);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        
        GameObject prefabToSpawn = Random.value < 0.5f ? hunterPrefab : rabbitPrefab;
        
        GameObject playerInstance = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

}
