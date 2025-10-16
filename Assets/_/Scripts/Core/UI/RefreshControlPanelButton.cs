using System.Collections;
using BirdTracks.Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BirdTracks.Game.Patterning.Editor
{
    public sealed class RefreshControlPanelButton : MonoBehaviour
    {
        public Button Button;


        public void OnButtonClicked()
        {
            StartCoroutine(RefreshRoutine());
        }

        private IEnumerator RefreshRoutine()
        {
            Button.interactable = false;
            yield return ControlPanelUtility.RefreshAllSheets();
            Button.interactable = true;
        }
    }
}
