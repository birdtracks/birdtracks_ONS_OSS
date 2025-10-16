using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public static class TrackosaurusAPI
    {
        private static bool s_LoggingEnabled = true;
        private static int s_LogId = 0;

        private static FlutterInitArgs s_FlutterArgs;

        public static FlutterInitArgs Flutter { get { return s_FlutterArgs; } }

        public static void Initialize(FlutterInitArgs args)
        {
            s_FlutterArgs = args;
        }
        
        [Serializable]
        public struct SubmitGameDataResultResponse
        {
        }

        public static Task<SubmitGameDataResultResponse> SubmitStoryGameResult(SubmitStoryGameResultRequest request)
        {
            Debug.Log("SubmitStoryGameResult");
            
            if (Flutter == null)
            {
                return Task.FromException<SubmitGameDataResultResponse>(new NullReferenceException());
            }
            
            var flutter = new FlutterVADGameResult();
            flutter.SessionId = s_FlutterArgs.Session.SessionId;
            flutter.Time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            flutter.Endpoint = AssessmentCategory.Storytelling;
            flutter.Payload = JsonConvert.SerializeObject(request.Event);
            
            string jsonString = JsonConvert.SerializeObject(flutter);
            Debug.Log("Serialized FlutterVADGameResult: " + jsonString);

            if (!Application.isEditor)
            {
                PlaySessionService.Instance.SendSessionDataToFlutter(jsonString);
            }
            
            return Task.FromResult(new SubmitGameDataResultResponse());
            
        }
        
    }
}