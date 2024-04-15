using Kickstarter.Inputs;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;


public class MoveCamera : MonoBehaviour, IInputReceiver
{
    [SerializeField] private Vector2Input rotationInput;
    [SerializeField] float mouseSensitivity;

    Vector2 rawInput;

    private void Update()
    {
        RotatePlayer();
        RotateCamera();
    }

    private void RotatePlayer()
    {
        //horizontel rotation
        Transform root = transform.root;

        float rotation = root.rotation.eulerAngles.y;
        rotation += rawInput.x * mouseSensitivity;

        root.localRotation = Quaternion.Euler(new Vector3(0, rotation, 0));

    }

    private void RotateCamera()
    {
        //vertical rotation

        float rotation = transform.rotation.eulerAngles.x;
        rotation -= rawInput.y * mouseSensitivity;

        transform.localRotation = Quaternion.Euler(new Vector3(rotation, 0, 0));
    }

    public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        rotationInput.RegisterInput(OnRotationInputChange, playerIdentifier);
    }

    public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        rotationInput.DeregisterInput(OnRotationInputChange, playerIdentifier);
    }

    private void OnRotationInputChange(Vector2 input)
    {
        rawInput = input;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
        
    

