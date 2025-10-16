using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BirdTracks.Game.Core;
using UnityEngine;

namespace Birdtracks.Game.ONS
{
    public class SaveAudioUtil
    {
        
        private static int _fileIncrement = 0;

        public static void SaveToWAV(AudioClip clip, string filename, out string filePath)
        {
            filePath = "";
            if (clip == null)
            {
                Debug.LogError("Attempted to save null AudioClip");
                return;
            }

            if (clip.samples == 0)
            {
                Debug.LogError($"Attempted to save AudioClip with 0 samples: {filename}");
                return;
            }

            _fileIncrement += 1;
            var dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var increment = _fileIncrement.ToString();
            var versionNumber = Application.version;
            var newFileName = string.Concat(filename, "_V", versionNumber, "_", dateTime, "_", increment, ".wav");

            filePath = Path.Combine(Application.persistentDataPath, newFileName);
            
            try
            {
                File.WriteAllBytes(filePath, ConvertAudioClipToWAVBytes(clip));
                Debug.Log(
                    $"Saved WAV to {filePath} - Samples: {clip.samples}, Channels: {clip.channels}, Frequency: {clip.frequency}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save WAV file: {e.Message}");
            }
        }

        private static byte[] ConvertAudioClipToWAVBytes(AudioClip clip)
        {
            var samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            using (var memoryStream = new MemoryStream())
            {
                WriteWavHeader(memoryStream, clip);
                WriteSampleData(memoryStream, samples);
                return memoryStream.ToArray();
            }
        }

        private static void WriteWavHeader(MemoryStream stream, AudioClip clip)
        {
            var headerSize = 44;
            var dataSize = clip.samples * clip.channels * 2; // 16-bit samples

            // RIFF header
            WriteString(stream, "RIFF"); // ChunkID
            WriteInt(stream, headerSize + dataSize - 8); // ChunkSize
            WriteString(stream, "WAVE"); // Format

            // Format chunk
            WriteString(stream, "fmt "); // Subchunk1ID
            WriteInt(stream, 16); // Subchunk1Size
            WriteShort(stream, 1); // AudioFormat (PCM)
            WriteShort(stream, (short)clip.channels); // NumChannels
            WriteInt(stream, clip.frequency); // SampleRate
            WriteInt(stream, clip.frequency * clip.channels * 2); // ByteRate
            WriteShort(stream, (short)(clip.channels * 2)); // BlockAlign
            WriteShort(stream, 16); // BitsPerSample

            // Data chunk
            WriteString(stream, "data"); // Subchunk2ID
            WriteInt(stream, dataSize); // Subchunk2Size
        }

        private static void WriteSampleData(MemoryStream stream, float[] samples)
        {
            const float rescaleFactor = 32767;

            for (int i = 0; i < samples.Length; i++)
            {
                var intValue = (short)(samples[i] * rescaleFactor);
                var byteArr = BitConverter.GetBytes(intValue);
                stream.Write(byteArr, 0, 2);
            }
        }

        private static void WriteString(MemoryStream stream, string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static void WriteInt(MemoryStream stream, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            stream.Write(bytes, 0, 4);
        }

        private static void WriteShort(MemoryStream stream, short value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            stream.Write(bytes, 0, 2);
        }
        
        private static List<VoiceGameData> inMemoryCollection = new List<VoiceGameData>();
        
        public static void AddGameData(VoiceGameData newData)
        {
            inMemoryCollection.Add(newData);
            Debug.Log($"Added new entry to memory: Game: {newData.gameName}, Filepath: {newData.filePath}");
        }
        
        public static void SaveAllData(string filename)
        {
            string savePath = Path.Combine(Application.persistentDataPath, filename);

            if (inMemoryCollection.Count == 0)
            {
                Debug.LogWarning("no data collected for audio games");
            }

            var newPayload = new VoiceGameDataPayload()
            {
                Payload = inMemoryCollection,
            };

            try
            {
                string jsonData = JsonUtility.ToJson(newPayload);
                File.WriteAllText(savePath, jsonData);
                Debug.Log($"All game data saved successfully to: {savePath}");
                Debug.Log($"StoryResults: {jsonData}");
                
                VADAnalytics.CreateNewGameCompleteEvent(jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving game data: {e.Message}");
            }
        }
    }

    [Serializable]
    public class VoiceGameData
    {
        public string question;
        public string gameName;
        public string filePath;
    }

    [Serializable]
    public class VoiceGameDataPayload
    {
        public List<VoiceGameData> Payload = new List<VoiceGameData>();
    }
}