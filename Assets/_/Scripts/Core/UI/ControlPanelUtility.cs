using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using SweetEngine.Routine;
using UnityEngine.Networking;

namespace BirdTracks.Game.Core
{
    public class ControlPanelUtility
    {
        private static Dictionary<string, Action<string[][]>> _callbacks = new Dictionary<string, Action<string[][]>>();


        public static Coroutine RefreshAllSheets()
        {
            var refreshRoutines = new List<WhenCoroutine>();

            foreach (var callback in _callbacks)
            {
                refreshRoutines.Add(CoroutineHost.HostCoroutine(GetRoutine(callback.Key)));
            }

            return CoroutineHost.HostCoroutine(new WhenAll<WhenCoroutine>(refreshRoutines.ToArray()));
        }

        
        //TODO: Why are we using Google Sheets to hold data? Change to JSON? Anticipated remote data changes?
        public static void SubscribeData(string sheet, Action<string[][]> onDataReady)
        {
            var cachePath = GetCachePath(sheet);

            if (File.Exists(cachePath))
            {
                var cachedSheet = File.ReadAllText(cachePath);

                try
                {
                    var splitSheet = cachedSheet
                        .Split('\n')
                        .Select(l => l.Split(','))
                        .ToArray();

                    onDataReady.Invoke(splitSheet);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            _callbacks[sheet] = onDataReady;
            CoroutineHost.HostCoroutine(GetRoutine(sheet));
        }
        
        private static IEnumerator GetRoutine(string sheet)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(sheet))
            {
                // Send the request and wait for it to complete
                yield return request.SendWebRequest();

                // Check for errors
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    // Get the text content of the response
                    var text = request.downloadHandler.text;
                    // Debug.Log("ReceivedText: \n" + text);

                    var cachePath = GetCachePath(sheet);
                    var directory = Path.GetDirectoryName(cachePath);

                    try
                    {
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        File.WriteAllText(cachePath, text);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Failed to cache data sheet to '{cachePath}': {e.GetType().Name} - {e.Message}");
                    }

                    if (_callbacks.TryGetValue(sheet, out var onDataReady))
                    {
                        try
                        {
                            // Debug.Log("GotSheet: \n" + text);
                            var splitSheet = text
                                .Split('\n')
                                .Select(l => l.Split(','))
                                .ToArray();

                            onDataReady.Invoke(splitSheet);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }



        public static void Unsubscribe(string sheet)
        {
            if (_callbacks != null)
            {
                _callbacks.Remove(sheet);
            }
        }

        private static string GetCachePath(string sheet)
        {
            
            //TODO: Relook this path creation - Hash might not always be the same
            return $"{Application.persistentDataPath}/cpcache/{(uint)sheet.GetHashCode()}.cache"; 
        }
    }
}
