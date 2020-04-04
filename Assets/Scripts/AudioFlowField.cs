using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlowField))]
public class AudioFlowField : MonoBehaviour
{
    FlowField _flowField;
    AudioManager _audioManager;

    // Start is called before the first frame update
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
        
    }
}
