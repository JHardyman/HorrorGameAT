using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.PS5;

public class ProximityTrigger : MonoBehaviour
{
    //public UnityEvent onPlayerEnter;
    //public UnityEvent onPlayerExit;

    DualSenseGamepad dualSenseGamepad;
    bool controllerDetected = false;
    bool inRange = false;


    private void Start()
    {

    }

    private void Update()
    {
        if (controllerDetected == false)
        {
            dualSenseGamepad = DualSenseGamepad.GetBySlotIndex(0);
            if (dualSenseGamepad != null)
            {
                controllerDetected = true;
                Debug.Log("DualSense controller detected");
                dualSenseGamepad.SetVibrationMode(VibrationMode.Compatible);
            }
            else
            {
                Debug.Log("No DualSense controller detected");
            }
        }

        if (inRange)
        {
            dualSenseGamepad.SetMotorSpeeds(0.5f, 1f);
        }
        else
        {
            dualSenseGamepad.SetMotorSpeeds(0, 0);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player entered the range");
            dualSenseGamepad.SetVibrationMode(VibrationMode.Compatible);
            inRange = true;
            //onPlayerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player exited the range");
            inRange = false;
            //onPlayerExit.Invoke();
        }
    }


}
