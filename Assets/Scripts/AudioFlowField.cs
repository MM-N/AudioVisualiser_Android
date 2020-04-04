using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlowField))]
public class AudioFlowField : MonoBehaviour
{
    FlowField _flowField;
    AudioManager _audioManager;

    [Header("Speed")]
    [SerializeField, GetSet("UseSpeed")]
    bool _useSpeed;
    public bool UseSpeed 
    { 
        get { return _useSpeed; } 
        set { _useSpeed = value; }
    }

    [SerializeField, GetSet("MoveSpeedMinMax")]
    Vector2 _moveSpeedMinMax;
    public Vector2 MoveSpeedMinMax 
    {
        get { return _moveSpeedMinMax; }
        set { _moveSpeedMinMax = value; } 
    }
    
    [SerializeField, GetSet("RotateSpeedMinMax")]
    Vector2 _rotateSpeedMinMax;
    public Vector2 RotateSpeedMinMax 
    { 
        get { return _rotateSpeedMinMax; }
        set { _rotateSpeedMinMax = value; }
    }

    void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        DebugHelper.debugHelper.IsValidObject(_audioManager);

        _flowField = GetComponent<FlowField>();

        int bandCount = 0;
        for (int i = 0; i < _flowField.NumberOfParticles; i++)
        {
            //Create a more even distribution across the frequency bands
            int band = bandCount % 8;
            _flowField.Particles[i].AudioBand = band;
            bandCount++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_useSpeed)
        {
            _flowField.ParticleSpeed = Mathf.Lerp(_moveSpeedMinMax.x, _moveSpeedMinMax.y, _audioManager.AmplitudeBuffer);
            _flowField.ParticleRotSpeed = Mathf.Lerp(_rotateSpeedMinMax.x, _rotateSpeedMinMax.y, _audioManager.AmplitudeBuffer);
        }
    }
}
