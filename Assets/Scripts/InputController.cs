using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandType
{
    DashDirection,Jump,Dash, Hook
};
[System.Serializable]
public class Command
{
    [SerializeField]private int currentFrameNo=0;
    [SerializeField]private bool isPressed = false;
    [SerializeField]public string buttonName;
    [SerializeField]public string horizontalAxis;
    [SerializeField]public string verticalAxis;
    [SerializeField]bool axis;
    [SerializeField]private int maxFramesToBuffer=1;
    Vector2 axisInput;
    public void ClearFrameBuffer()
    {
        isPressed = false;
        currentFrameNo = 0;
    }
    public void UpdateInput()
    {
        if (axis)
        {
            if (Mathf.Abs(Input.GetAxis(horizontalAxis)) > 0 || Mathf.Abs(Input.GetAxis(verticalAxis))>0)
            {
                axisInput.x = Input.GetAxis(horizontalAxis);
                axisInput.y = Input.GetAxis(verticalAxis);
                isPressed = true;
                currentFrameNo = 0;
            }
            if (isPressed && currentFrameNo < maxFramesToBuffer)
            {
                currentFrameNo++;
                //Debug.Log(currentFrameNo+":" + axisInput);
            }
            else
            {
                isPressed = false;
                currentFrameNo = 0;
            }
        }
        else
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
    }
    public bool IsPressed() { return isPressed; }
    public Vector2 GetAxis() { return axisInput; }
}

public class InputController : MonoBehaviour
{
    [SerializeField] private int maxFramesToBuffer = 10;
    [SerializeField]Command jumpCommand;
    [SerializeField]Command dashCommand;
    [SerializeField]Command dashDirectionCommand;
    [SerializeField]Command hookCommand;
    Dictionary<CommandType,Command> inputCommands;

    private void Start()
    {
        inputCommands = new Dictionary<CommandType, Command>();
        inputCommands.Add(CommandType.Jump, jumpCommand);
        inputCommands.Add(CommandType.Dash, dashCommand);
        inputCommands.Add(CommandType.Hook, hookCommand);
        inputCommands.Add(CommandType.DashDirection, dashDirectionCommand);
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
