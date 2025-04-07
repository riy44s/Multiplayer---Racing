using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _carPrefab;
    private NetworkRunner _runner;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCars = new Dictionary<PlayerRef, NetworkObject>();

    async void Start()
    {
        // Create the Fusion runner and let it know we'll be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Start or join (depends on GameMode) a session with specific settings
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient, 
            SessionName = "RaceSession",
           // Scene = 1, // Make sure this matches your gameplay scene index
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 4 // Maximum players
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % 10) * 3, 0, 0);

            // Spawn the car for this player
            NetworkObject networkPlayerObject = runner.Spawn(_carPrefab, spawnPosition, Quaternion.identity, player);
            networkPlayerObject.AssignInputAuthority(player);
            // Keep track of the player avatars for easy access
            _spawnedCars.Add(player, networkPlayerObject);

            Debug.Log($"Player {player.PlayerId} joined. Spawned car.");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCars.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCars.Remove(player);
            Debug.Log($"Player {player.PlayerId} left. Removed car.");
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (runner.IsClient)
        {
            var inputData = new NetworkInputData
            {
                Throttle = Input.GetAxis("Vertical"),
                Steering = Input.GetAxis("Horizontal"),
                Handbrake = Input.GetKey(KeyCode.Space)
            };

            //  Send input only for the local player
            input.Set(inputData);
        }
    }


    // Other required callbacks with empty implementations
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)   { }
}