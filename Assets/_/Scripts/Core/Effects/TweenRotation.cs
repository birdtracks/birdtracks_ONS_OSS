using System;
using System.Collections;
using SweetEngine.Routine;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public struct TweenRotation
    {
        private static readonly EasingFunction.Function _DefaultEase = AnimationCurve.Linear(0f, 0f, 1f, 1f).Evaluate;
        private Transform _target;
        private float _time;
        private Quaternion _from;
        private Quaternion _to;
        private EasingFunction.Function _ease;
        private Action _callback;
        private bool _loop;


        public TweenRotation(Transform target, float time)
        {
            _target = target;
            _time = time;
            _from = Quaternion.identity;
            _to = Quaternion.identity;
            _ease = _DefaultEase;
            _callback = null;
            _loop = false;
        }


        public TweenRotation WithFrom(Quaternion from)
        {
            _from = from;
            return this;
        }

        public TweenRotation WithTo(Quaternion to)
        {
            _to = to;
            return this;
        }

        public TweenRotation WithEase(AnimationCurve curve)
        {
            _ease = curve.Evaluate;
            return this;
        }

        public TweenRotation WithEase(EasingFunction.Ease easeType)
        {
            _ease = EasingFunction.GetEasingFunction(easeType);
            return this;
        }
        public TweenRotation WithLoop(bool loop)
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


        private IEnumerator DoRoutine(TweenRotation args)
        {
            float time = Time.deltaTime;

            do
            {
                while (time < args._time)
                {
                    args._target.localRotation = Quaternion.LerpUnclamped(args._from, args._to, args._ease(time/args._time));
                    yield return null;
                    time += Time.deltaTime;
                }
                time -= args._time;
            } while (args._loop);

            args._target.localRotation = args._to;

            if (_callback != null)
            {
                _callback();
            }
        }

        public TweenRotation Then(Action callback)
        {
            _callback = callback;
            return this;
        }
    }
}