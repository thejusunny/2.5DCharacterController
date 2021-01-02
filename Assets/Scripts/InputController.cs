using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandType
{
    Move,Jump,Dash
};
[System.Serializable]
public class Command
{
    [SerializeField]private int currentFrameNo=0;
    [SerializeField]private bool isPressed = false;
    [SerializeField]public string buttonName;
    [SerializeField]private int maxFramesToBuffer=1;
    public void ClearFrameBuffer()
    {
        isPressed = false;
        currentFrameNo = 0;
    }
    public void UpdateInput()
    {
        if (Input.GetButtonDown(buttonName))
        {
           isPressed = true;
           currentFrameNo = 0;
        }
        if (isPressed && currentFrameNo < maxFramesToBuffer)
        {
            currentFrameNo++;
        }
        else
        {
            isPressed = false;
            currentFrameNo = 0;
        }


    }
    public bool IsPressed() { return isPressed; }
}

public class InputController : MonoBehaviour
{
    [SerializeField] private int maxFramesToBuffer = 10;
    [SerializeField]Command jumpCommand;
    [SerializeField]Command dashCommand;
    [SerializeField]Command moveCommand;
    Dictionary<CommandType,Command> inputCommands;

    public Command JumpCommand { get => jumpCommand; }


    private void Start()
    {
        inputCommands = new Dictionary<CommandType, Command>();
        inputCommands.Add(CommandType.Jump, jumpCommand);
        inputCommands.Add(CommandType.Dash, dashCommand);
    }

    public void ReadInputs()
    {
        foreach (Command command in inputCommands.Values)
        {
            command.UpdateInput();
        }
    }
    public Command GetCommad(CommandType type)
    {
        return inputCommands[type];
    }
}
