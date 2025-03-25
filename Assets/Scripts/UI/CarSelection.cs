using UnityEngine;
using UnityEngine.InputSystem;

public class CarSelection : MonoBehaviour
{
    [Header("Input")]

    private PlayerInput playerInput;


    [Header("Car Selection")]

    [SerializeField] private Canvas[] carSelectCanvas;

    public int canvasIndex;


    private void Awake()
    {
        playerInput = new PlayerInput();
    }


    public void MoveToNextCanvas(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            canvasIndex++;

            ManageCanvasSwap();

        }
    }


    public void MoveToPreviusCanvas(InputAction.CallbackContext context)
    {
        if (context.performed && canvasIndex > 0)
        {
            canvasIndex --;


            ManageCanvasSwap();
        }
    }


    public void ManageCanvasSwap()
    {
        if (canvasIndex >= carSelectCanvas.Length)
        {
            canvasIndex = 0;
        }


        carSelectCanvas[canvasIndex].enabled = true;
    }
}
