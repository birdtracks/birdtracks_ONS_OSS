using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Birdtracks.Game.ONS
{

    [CreateAssetMenu(fileName = "VoiceOverAudioLibrary", menuName = "Audio/Audio Library")]
    public class VoiceOverAudioLibrary : ScriptableObject
    {
        public Language ActiveLanguage;
        [SerializeField] private List<VoiceOverPack> VoiceOverPacks = new List<VoiceOverPack>();
        private Dictionary<string, VoiceOverPack> clipLookup;

        public void InitializeLookup()
        {
            clipLookup = VoiceOverPacks.ToDictionary(clip => clip.ID, clip => clip);
        }
    
        public AudioClip GetClip(string id)
        {
            if (clipLookup == null) InitializeLookup();
            return clipLookup.TryGetValue(id, out var data) ? data.GetClip(ActiveLanguage) : null;
        }

        
        public void SetActiveLanguage(Language activeLanguage)
        {
            ActiveLanguage = activeLanguage;
        }

        public bool TryGetActiveLanguageAudioClips(out List<AudioClip> activeClips)
        {
            activeClips = new List<AudioClip>();
            switch (ActiveLanguage)
            {
                case Language.English:
                {
                    activeClips.AddRange(VoiceOverPacks.Select(vop => vop.EnglishClip));
                    break;
                }
                case Language.Seswati:
                {
                    activeClips.AddRange(VoiceOverPacks.Select(vop => vop.SeswatiClip));
                    break;
                }
                default:
                    return false;
            }

            return true;
        }
    }

    public enum Language
    {
        English = 0,
        Seswati = 1
    }

    [Serializable]
    public class VoiceOverPack
    {
        public string ID;
        public AudioClip EnglishClip;
        public AudioClip SeswatiClip;

        public AudioClip GetClip(Language language)
        {
            return language == Language.English ? EnglishClip : SeswatiClip ;
        }
       
    }
}


