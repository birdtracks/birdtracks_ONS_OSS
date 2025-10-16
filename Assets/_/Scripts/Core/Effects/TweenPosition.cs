using System;
using System.Collections;
using SweetEngine.Routine;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public struct TweenPosition
    {
        private static readonly EasingFunction.Function _DefaultEase = AnimationCurve.Linear(0f, 0f, 1f, 1f).Evaluate;
        private Transform _target;
        private float _time;
        private Vector3 _from;
        private Vector3 _to;
        private EasingFunction.Function _ease;
        private bool _loop;
        private Action _callback;
        private bool _isLocal;


        public TweenPosition(Transform target, float time)
        {
            _target = target;
            _time = time;
            _from = Vector3.zero;
            _to = Vector3.zero;
            _ease = _DefaultEase;
            _loop = false;
            _callback = null;
            _isLocal = false;
        }


        public TweenPosition WithFrom(Vector3 from)
        {
            _from = from;
            return this;
        }

        public TweenPosition WithTo(Vector3 to)
        {
            _to = to;
            return this;
        }

        public TweenPosition WithEase(AnimationCurve curve)
        {
            _ease = curve.Evaluate;
            return this;
        }

        public TweenPosition WithEase(EasingFunction.Ease easeType)
        {
            _ease = EasingFunction.GetEasingFunction(easeType);
            return this;
        }
        public TweenPosition WithLoop(bool loop)
        {
            _loop = loop;
            return this;
        }

        public TweenPosition WithLocal(bool isLocal)
        {
            _isLocal = isLocal;
            return this;
        }

        public Coroutine Do()
        {
            return CoroutineHost.HostCoroutine(DoRoutine(this));
        }

        public void Do(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                CoroutineHost.CancelCoroutine(coroutine);
            }

            coroutine = Do();
        }


        private IEnumerator DoRoutine(TweenPosition args)
        {
            float time = Time.deltaTime;

            do
            {
                while (time < args._time)
                {
                    Vector3 value = Vector3.Lerp(args._from, args._to, args._ease(time/args._time));

                    if (_isLocal)
                    {
                        args._target.localPosition = value;
                    }
                    else
                    {
                        args._target.position = value;
                    }

                    yield return null;
                    time += Time.deltaTime;
                }
                time -= args._time;
            } while (args._loop);
            
            if (_isLocal)
            {
                args._target.localPosition = args._to;
            }
            else
            {
                args._target.position = args._to;
            }

            if (_callback != null)
            {
                _callback();
            }
        }

        public TweenPosition Then(Action callback)
        {
            _callback = callback;
            return this;
        }
    }
}