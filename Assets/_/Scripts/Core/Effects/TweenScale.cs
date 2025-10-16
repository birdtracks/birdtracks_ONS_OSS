using System;
using System.Collections;
using SweetEngine.Routine;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public struct TweenScale
    {
        private static readonly EasingFunction.Function _DefaultEase = AnimationCurve.Linear(0f, 0f, 1f, 1f).Evaluate;
        private Transform _target;
        private float _time;
        private Vector3 _from;
        private Vector3 _to;
        private EasingFunction.Function _ease;
        private Action _callback;
        private bool _loop;


        public TweenScale(Transform target, float time)
        {
            _target = target;
            _time = time;
            _from = Vector3.zero;
            _to = Vector3.zero;
            _ease = _DefaultEase;
            _callback = null;
            _loop = false;
        }


        public TweenScale WithFrom(Vector3 from)
        {
            _from = from;
            return this;
        }

        public TweenScale WithTo(Vector3 to)
        {
            _to = to;
            return this;
        }

        public TweenScale WithEase(AnimationCurve curve)
        {
            _ease = curve.Evaluate;
            return this;
        }

        public TweenScale WithEase(EasingFunction.Ease easeType)
        {
            _ease = EasingFunction.GetEasingFunction(easeType);
            return this;
        }
        public TweenScale WithLoop(bool loop)
        {
            _loop = loop;
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


        private IEnumerator DoRoutine(TweenScale args)
        {
            float time = Time.deltaTime;

            do
            {
                while (time < args._time && args._target != null)
                {
                    args._target.localScale = Vector3.LerpUnclamped(args._from, args._to, args._ease(time/args._time));
                    yield return null;
                    time += Time.deltaTime;
                }

                time -= args._time;
            } while (false);

            if (args._target != null)
            {
                args._target.localScale = args._to;
            }

            if (_callback != null)
            {
                _callback();
            }
        }

        public TweenScale Then(Action callback)
        {
            _callback = callback;
            return this;
        }
    }
}