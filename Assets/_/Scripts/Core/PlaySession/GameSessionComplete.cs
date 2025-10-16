using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSessionComplete : MonoBehaviour
{
    [SerializeField] private bool _allowManualExit = false;
    [SerializeField] private CanvasGroup _canvasGroup;
    
    private void Start()
    {
        if (_allowManualExit)
        {
            StartCoroutine(ShowExitAppButton());
        }
    }

    private IEnumerator ShowExitAppButton()
    {
        while (_canvasGroup.alpha < 1f)
        {
            _canvasGroup.alpha += 0.05f;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
