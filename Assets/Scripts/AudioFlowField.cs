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

    [Header("Scale")]
    [SerializeField, GetSet("UseScale")]
    bool _useScale;
    public bool UseScale
    {
        get { return _useScale; }
        set { _useScale = value; }
    }

    [SerializeField, GetSet("ScaleMinMax")]
    Vector2 _scaleMinMax;
    public Vector2 ScaleMinMax
    {
        get { return _scaleMinMax; }
        set { _scaleMinMax = value; }
    }

    [Header("Material")]
    public Material _material;
    public Gradient _gradientOne;
    public Gradient _gradientTwo;
    public string _colourNameOne;
    public string _colourNameTwo;
    Color[] _colourOne;
    Color[] _colourTwo;
    

    [SerializeField, GetSet("UseColourOne")]
    bool _useColourOne;
    public bool UseColourOne
    {
        get { return _useColourOne; }
        set { _useColourOne = value; }
    }

    [SerializeField, GetSet("UseColourTwo")]
    bool _useColourTwo;
    public bool UseColourTwo
    {
        get { return _useColourTwo; }
        set { _useColourTwo = value; }
    }

    [SerializeField, GetSet("ColourThresholdOne")]
    float _colourThresholdOne;
    public float ColourThresholdOne
    {
        get { return _colourThresholdOne; }
        set { _colourThresholdOne = value; }
    }

    [SerializeField, GetSet("ColourThresholdTwo")]
    float _colourThresholdTwo;
    public float ColourThresholdTwo
    {
        get { return _colourThresholdTwo; }
        set { _colourThresholdTwo = value; }
    }

    [SerializeField, GetSet("ColourMultiplierOne")]
    float _colourMultiplierOne;
    public float ColourMultiplierOne
    {
        get { return _colourMultiplierOne; }
        set { _colourMultiplierOne = value; }
    }

    [SerializeField, GetSet("ColourMultiplierTwo")]
    float _colourMultiplierTwo;
    public float ColourMultiplierTwo
    {
        get { return _colourMultiplierTwo; }
        set { _colourMultiplierTwo = value; }
    }

    Material[] _audioMaterial;
    public Material[] AudioMaterial
    {
        get { return _audioMaterial; }
        set { _audioMaterial = value; }
    }


    void Start()
    {
        _audioMaterial = new Material[8];
        _colourOne = new Color[8];
        _colourTwo = new Color[8];

        for (int i = 0; i < 8; i++)
        {
            _colourOne[i] = _gradientOne.Evaluate((1.0f / 8.0f) * i);
            _colourTwo[i] = _gradientTwo.Evaluate((1.0f / 8.0f) * i);
            _audioMaterial[i] = new Material(_material);
        }

        _audioManager = FindObjectOfType<AudioManager>();
        DebugHelper.debugHelper.IsValidObject(_audioManager);

        _flowField = GetComponent<FlowField>();

        int bandCount = 0;
        for (int i = 0; i < _flowField.NumberOfParticles; i++)
        {
            //Create a more even distribution across the frequency bands
            int band = bandCount % 8;
            _flowField.ParticleMeshRenderers[i].material = _audioMaterial[band];
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

        for (int i = 0; i < _flowField.NumberOfParticles; i++)
        {
            if (_useScale)
            {
                float scale = Mathf.Lerp(_scaleMinMax.x, _scaleMinMax.y, _audioManager.AudioBandBuffer[_flowField.Particles[i].AudioBand]);
                _flowField.Particles[i].transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        for (int i = 0; i < 8; i++)
        {
            //Using audio band buffer
            if (_useColourOne)
            {
                if (_audioManager.AudioBandBuffer[i] > _colourThresholdOne)
                {
                    _audioMaterial[i].SetColor(_colourNameOne, _colourOne[i] * _audioManager.AudioBandBuffer[i] * _colourMultiplierOne);
                }
                else 
                {
                    //Set to black
                    _audioMaterial[i].SetColor(_colourNameOne, _colourOne[i] * 0.0f);
                }
            }

            //using audio band no buffer
            if (_useColourTwo)
            {
                if (_audioManager.AudioBand[i] > _colourThresholdTwo)
                {
                    _audioMaterial[i].SetColor(_colourNameTwo, _colourTwo[i] * _audioManager.AudioBand[i] * _colourMultiplierTwo);
                }
                else
                {
                    //Set to black
                    _audioMaterial[i].SetColor(_colourNameTwo, _colourTwo[i] * 0.0f);
                }
            }
        }

    }
}
