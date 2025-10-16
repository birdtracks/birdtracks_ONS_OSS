using System;
using System.Collections.Generic;

namespace BirdTracks.Game.Core
{
    
    public struct StoryGameCompleteEvent
    {
        public string Answers;
    }

    public class FlutterVADGameResult
    {
        public string SessionId;
        public long Time;
        public string Endpoint;
        public string Payload;
    }

    [Serializable]
    public class SubmitStoryGameResultRequest
    {
        public StoryGameCompleteEvent Event { get; set; }
    }

    public static class VADAnalytics
    {
        

        public static void Initialize()
        {

        }

        public static void CreateNewGameCompleteEvent(string gameDataResults)
        {
            var gameCompleteEvent = new StoryGameCompleteEvent();
            gameCompleteEvent.Answers = gameDataResults;
            
            var newEventSubmission = new SubmitStoryGameResultRequest();
            newEventSubmission.Event = gameCompleteEvent;

            TrackosaurusAPI.SubmitStoryGameResult(newEventSubmission);
        }


    }
}
