using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource _audioSource;

    /** For spectrum data analysis **/
    float[] _samples = new float[512];
    public float[] Samples
    {
        get { return _samples; }
    }

    float[] _freqBand = new float[8];
    public float[] FreqBand
    {
        get { return _freqBand; }
    }

    /** For smoother visuals **/
    float[] _bandBuffer = new float[8];
    public float[] BandBuffer
    {
        get { return _bandBuffer; }
    }

    float[] _bufferDecrease = new float[8];

    /** For Normalised Range **/
    float[] _bandLargestFreq = new float[8];

    float[] _audioBand = new float[8];
    public float[] AudioBand
    {
        get { return _audioBand; }
    }

    float[] _audioBandBuffer = new float[8];
    public float[] AudioBandBuffer
    {
        get { return _audioBandBuffer; }
    }

    /** For amplitude **/
    float _amplitude = 0.01f;
    public float Amplitude
    {
        get { return _amplitude; }
    }

    float _amplitudeBuffer = 0.01f;
    public float AmplitudeBuffer
    {
        get { return _amplitudeBuffer; }
    }

    float _highestAmplitude = 0.01f;
    float _tempHighestFrequency = 5.0f;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        PreloadHighestFrequencies(_tempHighestFrequency);
    }

    // Update is called once per frame
    void Update()
    {
        if (_audioSource.isPlaying)
        {
            GetSpectrumAudio();
            MakeFrequencyBands();
            CreateBandBuffer();
            CreateAudioBands();
            GetAmplitude();
        }

    }

    void GetSpectrumAudio()
    {
        _audioSource.GetSpectrumData(Samples, 0, FFTWindow.Blackman);
    }

    //** For using amplitude values **//
    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _bandLargestFreq[i])
            {
                _bandLargestFreq[i] = _freqBand[i];
            }

            _audioBand[i] = (_freqBand[i] / _bandLargestFreq[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _bandLargestFreq[i]);
        }
    }

    //** To avoid jittery movment at the start as highest frequency values are empty,
    //** pre load values to all bands **//
    void PreloadHighestFrequencies(float tempHighestFrequency)
    {
        for (int i = 0; i < 8; i++)
        {
            _bandLargestFreq[i] = tempHighestFrequency;
        }
    }

    void CreateBandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if (_freqBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }

            if (FreqBand[g] < _bandBuffer[g])
            {
                _bandBuffer[g] -= _bufferDecrease[g];
                //the greater the freq band over the buffer, the faster we want to fall
                _bufferDecrease[g] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        /*
         * hz / bands = 22050 / 512 = 43hz per sample
         * 
         * 20 - 60
         * 60 - 250
         * 250 - 500
         * 500 - 2000
         * 2000 - 4000
         * 4000 - 6000
         * 6000 - 20000
         * 
         * 0, 2 samples = 86hz
         * 1, 4 samples = 172hz, so range = 87 - 258hz
         * 2, 8 samples = 344, range = 259-602
         * 3, 16
         * 4, 32
         * 5, 64
         * 6, 128
         * 7, 256
         * Total = 510, so add final 2 to last band
         * 
         */

        int currentSample = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0.0f;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if (i == 7)
            {
                sampleCount += 2;
            }

            //Assing samples to the bands
            //Find average amplitude of all frequencie samples
            for (int j = 0; j < sampleCount; j++)
            {
                average += Samples[currentSample] * (currentSample + 1);
                currentSample++;
            }

            average /= currentSample;
            _freqBand[i] = average * 10; //As average is a little below 0

        }
    }

    void GetAmplitude()
    {
        float _currentAmplitude = 0.0f;
        float _currentAmplitudeBuffer = 0.0f;

        for (int i = 0; i < 8; i++)
        {
            _currentAmplitude += _audioBand[i];
            _currentAmplitudeBuffer += _audioBandBuffer[i];
        }

        if (_currentAmplitude > _highestAmplitude)
        {
            _highestAmplitude = _currentAmplitude;
        }

        _amplitude = _currentAmplitude / _highestAmplitude;
        _amplitudeBuffer = _currentAmplitudeBuffer / _highestAmplitude;

    }

}
