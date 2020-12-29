using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Command
{
    public int currentFrameNo;
    public bool isPressed;
    public string buttonName;
    public void ClearFrameBuffer()
    {
        isPressed = false;
        currentFrameNo = 0;
    }
}

public class InputController : MonoBehaviour
{
    [SerializeField] private int maxFramesToBuffer = 10;
    [SerializeField]Command jumpCommand;
    List<Command> buttonCommands;

    public Command JumpCommand { get => jumpCommand; }

    private void Start()
    {
        jumpCommand.buttonName = "Jump";
    }

    public void ReadInputs()
    {
        if (Input.GetButtonDown(jumpCommand.buttonName))
        {
            jumpCommand.isPressed = true;
            jumpCommand.currentFrameNo = 0;
        }
        if (jumpCommand.isPressed && jumpCommand.currentFrameNo < maxFramesToBuffer)
        {
            jumpCommand.currentFrameNo++;
        }
        else
        {
            jumpCommand.isPressed = false;
            jumpCommand.currentFrameNo = 0;
        }

    }
}
