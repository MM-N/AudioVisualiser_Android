using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBar : MonoBehaviour
{
    public AudioManager _audioManager;

    [SerializeField, GetSet("Band")]
    int _band;
    public int Band
    {
        get { return _band; }
        set { _band = value; }
    }

    [SerializeField, GetSet("StartScale")]
    float _startScale = 1.0f;
    public float StartScale
    {
        get { return _startScale; }
        set { _startScale = value; }
    }

    [SerializeField, GetSet("ScaleMultiplier")]
    float _scaleMultiplier = 10.0f;
    public float ScaleMultiplier
    {
        get { return _scaleMultiplier; }
        set { _scaleMultiplier = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("Missing ref to AudioManager");
            Application.Quit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        transform.localScale = new Vector3(transform.localScale.x, 
            (_audioManager.FreqBand[Band] * ScaleMultiplier) + StartScale, 
            transform.localScale.z);
            */
    }
}
