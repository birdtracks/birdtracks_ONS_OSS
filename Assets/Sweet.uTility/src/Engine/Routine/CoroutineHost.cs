using System.Collections;
using UnityEngine;

namespace SweetEngine.Routine
{
    public class CoroutineHost : MonoBehaviour
    {
        private static bool _IsQuiting;
        private static CoroutineHost _Instance;


        private static CoroutineHost Instance
        {
            get
            {
                if (_IsQuiting)
                {
                    Debug.LogWarning("Attempting to access coroutine host instance while application is quiting");
                    return null;
                }

                if (_Instance == null)
                {
                    var go = new GameObject("CoroutineHost");
                    _Instance = go.AddComponent<CoroutineHost>();
                    DontDestroyOnLoad(go);
                }

                return _Instance;
            }
        }


        private void Awake()
        {
            Application.quitting += ApplicationQuittingHandler;
        }

        private void OnDestroy()
        {
            if (!_IsQuiting)
            {
                Debug.LogWarning("Destroying CoroutineHost without application quit.");
            }

            Application.quitting -= ApplicationQuittingHandler;
        }


        private void ApplicationQuittingHandler()
        {
            _IsQuiting = true;
        }

        public static Coroutine HostCoroutine(IEnumerator coroutine)
        {
            var instance = Instance;
            
            if (instance == null)
            {
                Debug.LogWarning("Coroutine host instance is null");
                return null;
            }

            return Instance.StartCoroutine(coroutine);
        }

        public static void CancelCoroutine(Coroutine coroutine)
        {
            var instance = Instance;
            
            if (instance == null)
            {
                Debug.LogWarning("Coroutine host instance is null");
                return;
            }

            Instance.StopCoroutine(coroutine);
        }
    }
}