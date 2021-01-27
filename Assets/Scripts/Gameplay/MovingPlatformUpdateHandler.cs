using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformUpdateHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]IMovingPlatform[] platforms;
    void Start()
    {
        platforms = FindObjectsOfType<IMovingPlatform>();
    }
    private void Update()
    {
        ProcessUpdate();
    }
    public void ProcessUpdate()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i].UpdateMovement();
        }
    }
}
