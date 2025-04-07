using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public float Throttle;
    public float Steering;
    public bool Handbrake;

    //public class NetworkInputHandler : MonoBehaviour
    //{
    //    private NetworkRunner _runner;

    //    void Start()
    //    {
    //        _runner = FindFirstObjectByType<NetworkRunner>();
    //    }

    //    void Update()
    //    {
    //        if (_runner == null || !_runner.IsRunning) return;

    //        NetworkInputData inputData = new NetworkInputData()
    //        {
    //            Throttle = Input.GetAxis("Vertical"),
    //            Steering = Input.GetAxis("Horizontal")
    //        };

    //        _runner.(inputData);
    //    }
    //}
}

