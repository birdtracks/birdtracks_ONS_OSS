using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using MiniJSON;

namespace BirdTracks.Game.Core
{
    public class DeveloperInfoDialog : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text = default(TextMeshProUGUI);


        private void Awake()
        {
            /* if (!Debug.isDebugBuild)
            {
                gameObject.SetActive(false);
                return;
            }*/

            GetComponent<Canvas>().enabled = true;
            
            var manifest = (TextAsset) Resources.Load("UnityCloudBuildManifest.json");
            if (manifest != null)
            {
                var manifestDict = Json.Deserialize(manifest.text) as Dictionary<string,object>;
                StringBuilder sb = new StringBuilder(200);
                
                if (m_Text != null)
                {
                    if (manifestDict != null)
                    {
                        sb.AppendFormat("Version: {0} ({1}.{2})\n", manifestDict["buildNumber"].ToString(), manifestDict["scmBranch"].ToString(),
                            manifestDict["scmCommitId"].ToString().Substring(0, 7));

                        sb.AppendFormat("Date: {0}\n", manifestDict["buildStartTime"].ToString());
                    }

                    sb.AppendFormat("Platform: {0} ({1})\n", SystemInfo.operatingSystem, SystemInfo.deviceModel);

                    m_Text.text = sb.ToString();
                }
            }

        }
    }
}