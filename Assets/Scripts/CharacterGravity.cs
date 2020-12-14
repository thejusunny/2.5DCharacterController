using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : MonoBehaviour
{
    [SerializeField]private float gravityScale=1f;
    private Vector3 Gravity => Physics2D.gravity;
    public Vector3 GetGravity()
    {
        return Gravity * gravityScale;
    }
}
