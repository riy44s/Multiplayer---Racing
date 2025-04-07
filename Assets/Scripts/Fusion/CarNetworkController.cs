using Fusion;
using UnityEngine;
using UnityEngine.Windows;

public class CarNetworkController : NetworkBehaviour
{
    private PrometeoCarController _carController;
    private Rigidbody _rb;

    private float _throttleInput;
    private float _steeringInput;
    private bool _handbrakeInput;

    void Start()
    {
        _carController = GetComponent<PrometeoCarController>();
        _rb = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        if (GetInput(out NetworkInputData inputData))
        {
            // Store input values
            _throttleInput = inputData.Throttle;
            _steeringInput = inputData.Steering;
            _handbrakeInput = inputData.Handbrake;
            Debug.Log($"{name} (Player {Object.InputAuthority}): " +
                $"Throttle={inputData.Throttle}, Steering={inputData.Steering}");
        }

        // Apply inputs to the car controller
      //  ApplyCarInputs();
    }

    //private void ApplyCarInputs()
    //{
    //    // Handle throttle/reverse
    //    if (_throttleInput > 0)
    //    {
    //        _carController.GoForward();
    //    }
    //    else if (_throttleInput < 0)
    //    {
    //        _carController.GoReverse();
    //    }
    //    else
    //    {
    //        _carController.ThrottleOff();
    //    }

    //    // Handle steering
    //    if (_steeringInput > 0)
    //    {
    //        _carController.TurnRight();
    //    }
    //    else if (_steeringInput < 0)
    //    {
    //        _carController.TurnLeft();
    //    }
    //    else
    //    {
    //        _carController.ResetSteeringAngle();
    //    }

    //    // Handle handbrake
    //    if (_handbrakeInput)
    //    {
    //        _carController.Handbrake();
    //    }
    //    else
    //    {
    //        _carController.RecoverTraction();
    //    }
    //}
}