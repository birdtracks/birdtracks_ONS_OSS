using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public abstract class ServiceAsset : ScriptableObject, IService
    {
        [SerializeField] private LogAsset m_Log = default;
        private ServiceHost _host;
        private bool _hasRunInitialize;


        public bool HasRunInitialize
        {
            get { return _hasRunInitialize; }
            set { _hasRunInitialize = value; }
        }

        public ServiceInitializeResult InitializeResult { get; private set; }

        protected ServiceHost Host { get { return _host; } }

        public LogAsset Log { get { return m_Log; } }


        public event Action<ServiceInitializeResult> OnInitialize;


        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                InitializeServiceAsset();
            }
            else if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Action<UnityEditor.PlayModeStateChange> playModeStateChangedHandler = null;
                playModeStateChangedHandler = (s) =>
                {
                    UnityEditor.EditorApplication.playModeStateChanged -= playModeStateChangedHandler;

                    if (s == UnityEditor.PlayModeStateChange.EnteredPlayMode)
                    {
                        InitializeServiceAsset();
                    }
                };

                UnityEditor.EditorApplication.playModeStateChanged += playModeStateChangedHandler;
            }
#else
            InitializeServiceAsset();
#endif
        }

        private void InitializeServiceAsset()
        {
            if (!Application.isEditor &&
                HasRunInitialize)
            {
                return;
            }

            var serviceName = $"ServiceHost ({name})";
            var go = GameObject.Find(serviceName);

            if (go == null)
            {
                go = new GameObject(serviceName);
                DontDestroyOnLoad(go);
                _host = go.AddComponent<ServiceHost>();

            }
            else
            {
                _host = go.GetComponent<ServiceHost>();
            }

            ((IService)this).Initialize();
        }

        async Task IService.Initialize()
        {
            try
            {
                await Initialize();
                InitializeResult = new ServiceInitializeResult(this);
            }
            catch (Exception e)
            {
                InitializeResult = new ServiceInitializeResult(this, e);
                m_Log.LogException(e);
            }

            HasRunInitialize = true;
            OnInitialize?.Invoke(InitializeResult);
        }

        protected virtual Task Initialize()
        {
            return Task.CompletedTask;
        }

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            InitializeServiceAsset();
            return Host.StartCoroutine(routine);
        }

        protected void StopCoroutine(Coroutine routine)
        {
            InitializeServiceAsset();
            Host.StopCoroutine(routine);
        }

        protected void StopAllCoroutines()
        {
            if (Host != null)
            {
                Host.StopAllCoroutines();
            }
        }

        protected TComponent AddComponent<TComponent>()
            where TComponent : Component
        {
            InitializeServiceAsset();
            return Host.gameObject.AddComponent<TComponent>();
        }

        protected TComponent GetComponent<TComponent>()
            where TComponent : Component
        {
            InitializeServiceAsset();
            return Host.GetComponent<TComponent>();
        }
    }
}