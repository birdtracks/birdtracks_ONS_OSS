using System.Collections;
using System.Threading.Tasks;
using UnityEngine;


namespace SweetEngine.SceneManagement
{
    [RequireComponent(typeof(EntryInvoker))]
    public abstract class EntryBehaviour : MonoBehaviour
    {
        public abstract Task OnEntry();
    }
}
