using System.Collections;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SweetEngine.Extensions;
using UnityEngine;


namespace SweetEngine.SceneManagement
{
    public class EntryInvoker : MonoBehaviour
    {
        private static bool _Initialized;




        [UsedImplicitly]
        private void Awake()
        {
            EntryBehaviour entry;

            if (_Initialized ||
                (entry = GetComponent<EntryBehaviour>()) == null)
            {
                Destroy(gameObject);
                return;
            }

            _Initialized = true;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(EntryRoutine(entry));
        }


        private IEnumerator EntryRoutine(EntryBehaviour entry)
        {
            yield return entry.OnEntry().AsYieldable();
            Destroy(gameObject);
        }
    }
}
