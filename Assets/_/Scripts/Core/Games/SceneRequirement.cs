using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BirdTracks.Game.Core
{
    public sealed class SceneRequirement : MonoBehaviour
    {
        [SerializeField] private RequirementType m_Type = default;
        [SerializeField] private string m_Requirement = default;


        private void OnEnable()
        {
            bool requirementMet = false;

            switch (m_Type)
            {
                case RequirementType.StartsWith:
                {
                    for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                    {
                        string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

                        if (string.IsNullOrEmpty(scenePath))
                        {
                            continue;
                        }

                        string[] splitPath = scenePath.Split('/');
                        string sceneName = Path.GetFileNameWithoutExtension(splitPath[splitPath.Length - 1]);

                        if (sceneName.StartsWith(m_Requirement))
                        {
                            requirementMet = true;
                            break;
                        }
                    }
                }
                break;
                default:
                {
                    for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                    {
                        string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                        
                        if (string.IsNullOrEmpty(scenePath))
                        {
                            continue;
                        }

                        string[] splitPath = scenePath.Split('/');
                        string sceneName = Path.GetFileNameWithoutExtension(splitPath[splitPath.Length - 1]);

                        if (sceneName.Equals(m_Requirement))
                        {
                            requirementMet = true;
                            break;
                        }
                    }
                }
                break;
            }

            if (!requirementMet)
            {
                gameObject.SetActive(false);
            }
        }


        private enum RequirementType
        {
            Equals,
            StartsWith,
        }
    }
}