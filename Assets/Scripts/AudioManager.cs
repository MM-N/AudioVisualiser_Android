using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource _audioSource;

    public float[] Samples = new float[512];

    public float[] FreqBand = new float[8];

    public float[] _bandBuffer = new float[8];
    float[] _bufferDecrease = new float[8];

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudio();
        MakeFrequencyBands();
        BandBuffer();
    }

    void GetSpectrumAudio()
    {
        _audioSource.GetSpectrumData(Samples, 0, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if (FreqBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = FreqBand[g];
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
            FreqBand[i] = average * 10; //As average is a little below 0
        }
    }
}
