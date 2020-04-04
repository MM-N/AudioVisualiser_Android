using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldParticle : MonoBehaviour
{
    [SerializeField, GetSet("MoveSpeed")]
    private float _moveSpeed;
    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    [SerializeField, GetSet("AudioBand")]
    private int _audioBand;
    public int AudioBand
    {
        get { return _audioBand; }
        set { _audioBand = value; }
    }

    void Update()
    {
        this.transform.position += transform.forward * MoveSpeed * Time.deltaTime;
    }

    // Called when particles instantiated from FlowField
    public void ApplyRotation(Vector3 rotation, float rotateSpeed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(rotation.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
}
