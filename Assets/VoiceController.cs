using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceController : MonoBehaviour
{
    
    public float RmsValue;
    public float DbValue;
    public float PitchValue;
 
    private const int audioBufferSize = 2048;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;
 
    float[] _samples;
    private float[] _spectrum;
    private float _fSample;
    
    private AudioSource _audioSource;
    private AudioClip _audio;
    private string _headset;

    private int minFreq;
    private int maxFreq;
    // Start is called before the first frame update
    void Start()
    {
        _samples = new float[audioBufferSize];
        _spectrum = new float[audioBufferSize];
        _fSample = AudioSettings.outputSampleRate;
        
        _headset = Microphone.devices[0];
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
        
        Microphone.GetDeviceCaps(_headset, out minFreq, out maxFreq);
        if(minFreq == 0 && maxFreq == 0)  
        {  
            //...meaning 44100 Hz can be used as the recording sampling rate  
            maxFreq = 44100;  
        }

        _audio = Microphone.Start(_headset, true, 3600, maxFreq);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _audio;
        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Microphone.IsRecording(_headset))
        {
            AnalyzeSound();
        }
    }
    
    void AnalyzeSound()
    {
        _audioSource.GetOutputData(_samples, 0); // fill array with samples
        int i;
        float sum = 0;
        foreach (float soundByte in _samples)
        {
            sum += Mathf.Pow(soundByte,2);
        }

        RmsValue = Mathf.Sqrt(sum / audioBufferSize); // rms = square root of average
        DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
        if (DbValue < -160) DbValue = -160; // clamp it to -160dB min
       // get sound spectrum
       /* _audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < QSamples; i++)
        { // find max 
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;
 
            maxV = _spectrum[i];
            maxN = i; // maxN is the index of max
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < QSamples - 1)
        { // interpolate index using neighbours
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency*/
    }
}
