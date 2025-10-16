using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public sealed class LifeExplorerLoadSceneButton : LoadSceneButton
    {
        private List<AssetBundle> AssetBundles = new List<AssetBundle>();
        [SerializeField] private string[] m_Bundles;


        protected override void OnBeforeLoadScene()
        {

#if !UNITY_EDITOR && USE_LIFE_EXPLORER_ASSET_BUNDLES
            for (int i = 0; i < m_Bundles.Length; i++)
            {
                AssetBundles.Add(AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "AssetBundles", m_Bundles[i])));
            }
#endif
        }


        protected override void OnAfterLoadScene()
        {
#if !UNITY_EDITOR && USE_LIFE_EXPLORER_ASSET_BUNDLES
            for (int i = 0; i < AssetBundles.Count; i++)
            {
                AssetBundles[i].Unload(false);
            }

            AssetBundles.Clear();
#endif
        }
    }
}