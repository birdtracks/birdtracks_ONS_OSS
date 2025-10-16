using System;
using System.Collections.Generic;

namespace BirdTracks.Game.Core
{
    public static class Analytics
    {
        private static string _Session;

        public static void StartNewSession()
        {
            _Session = Guid.NewGuid().ToString();
        }

        public static void PrepareItem(string eventType)
        {
        }

        public static void WriteString(string name, string value)
        {
        }

        public static void WriteDate(string name, DateTime value)
        {
        }

        public static void WriteBool(string name, bool value)
        {
        }

        public static void WriteStringList(string name, List<string> list)
        {
        }

        public static void WriteStringList(string name, HashSet<string> list)
        {
        }

        public static void WriteNumberList(string name, List<int> list)
        {
        }

        public static void WriteNumberList(string name, int[] array)
        {
        }

        public static void WriteNumberList(string name, HashSet<int> list)
        {
        }

        public static void WriteNumberList(string name, List<float> list)
        {
        }

        public static void WriteDecimal(string name, float number)
        {
        }

        public static void WriteNumber(string name, long number)
        {
        }

        public static void WriteDecimal(string name, double number)
        {
        }

        public static async void FlushItem()
        {
            // await TrackosaurusAPI.RecordEvent(new TrackosaurusAPI.RecordEventRequest
            // {

            // });
        }
    }
}