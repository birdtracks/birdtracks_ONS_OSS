using UnityEngine;
using System.IO;

public class AudioRecorder : MonoBehaviour
{
    private AudioClip recording;
    private bool isRecording = false;
    private string microphoneName;
    private int sampleRate = 16000;
    private int maxRecordingDuration = 1800; // Maximum duration in seconds (e.g., 1/2 hour)

    void Start()
    {
        // Check if a microphone is available
        if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0];

            // Start recording
            recording = Microphone.Start(microphoneName, false, maxRecordingDuration, sampleRate);
            isRecording = true;
            Debug.Log("Recording started...");
        }
        else
        {
            Debug.LogError("No microphone devices found!");
        }
    }

    void OnApplicationQuit()
    {
        if (isRecording)
        {
            // Stop the recording
            Microphone.End(microphoneName);
            isRecording = false;

            // Save the recording
            SaveRecording();
        }
    }

    void SaveRecording()
    {
        if (recording == null)
        {
            Debug.LogError("No recording found to save.");
            return;
        }

        // Get the data from the AudioClip
        float[] samples = new float[recording.samples * recording.channels];
        recording.GetData(samples, 0);

        // Convert and save the AudioClip to WAV
        SaveWavFile("GameSessionRecording.wav", samples, recording.channels, recording.frequency);
    }

    void SaveWavFile(string filename, float[] samples, int channels, int frequency)
    {
        string filepath = Path.Combine(Application.persistentDataPath, filename);
        using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
        {
            // Write WAV header
            int byteRate = frequency * channels * 2;
            int fileSize = 44 + samples.Length * 2;

            // RIFF header
            WriteString(fileStream, "RIFF");
            WriteInt(fileStream, fileSize - 8);
            WriteString(fileStream, "WAVE");

            // fmt subchunk
            WriteString(fileStream, "fmt ");
            WriteInt(fileStream, 16);
            WriteShort(fileStream, 1);
            WriteShort(fileStream, (short)channels);
            WriteInt(fileStream, frequency);
            WriteInt(fileStream, byteRate);
            WriteShort(fileStream, (short)(channels * 2));
            WriteShort(fileStream, 16);

            // data subchunk
            WriteString(fileStream, "data");
            WriteInt(fileStream, samples.Length * 2);

            // Write sample data
            float rescaleFactor = 32767;
            for (int i = 0; i < samples.Length; i++)
            {
                short sample = (short)(samples[i] * rescaleFactor);
                WriteShort(fileStream, sample);
            }
        }
        Debug.Log("Recording saved to: " + filepath);
    }

    void WriteString(FileStream fs, string value)
    {
        byte[] info = System.Text.Encoding.ASCII.GetBytes(value);
        fs.Write(info, 0, info.Length);
    }

    void WriteInt(FileStream fs, int value)
    {
        byte[] info = System.BitConverter.GetBytes(value);
        fs.Write(info, 0, info.Length);
    }

    void WriteShort(FileStream fs, short value)
    {
        byte[] info = System.BitConverter.GetBytes(value);
        fs.Write(info, 0, info.Length);
    }
}
