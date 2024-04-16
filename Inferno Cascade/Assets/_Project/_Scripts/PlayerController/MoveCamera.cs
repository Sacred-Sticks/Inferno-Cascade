using Kickstarter.Inputs;
using UnityEngine;


public class MoveCamera : MonoBehaviour, IInputReceiver
{
    [SerializeField] private Vector2Input rotationInput;
    [SerializeField] float mouseSensitivity;

    Vector2 rawInput;

    private void FixedUpdate()
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
        if (rotation > 180)
            rotation -= 360;
        rotation = Mathf.Clamp(rotation, -89, 89);


        transform.localRotation = Quaternion.Euler(new Vector3(rotation, 0, 0));
    }

    #region InputHandler
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
    #endregion

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
        
    

