using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class Hook : MonoBehaviour
{
    public float hookDistance=5f;
    public float hookRange=3f;
    private SphereCollider rangeCollider;
    public void Start()
    {
        rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.isTrigger = true;
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, hookRange / 2);
    }
    public void OnValidate()
    {
        GetComponent<SphereCollider>().radius = hookRange / 2;
    }
}
