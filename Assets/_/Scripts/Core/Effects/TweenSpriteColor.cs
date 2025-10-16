using System;
using System.Collections;
using SweetEngine.Routine;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public struct TweenSpriteColor
    {
        private static readonly EasingFunction.Function _DefaultEase = AnimationCurve.Linear(0f, 0f, 1f, 1f).Evaluate;
        private SpriteRenderer _target;
        private float _time;
        private Color _from;
        private Color _to;
        private EasingFunction.Function _ease;
        private bool _loop;


        public TweenSpriteColor(SpriteRenderer target, float time)
        {
            _target = target;
            _time = time;
            _from = new Color(1f,1f,1f,1f);
            _to = new Color(1f,1f,1f,0f);
            _ease = _DefaultEase;
            _loop = false;
        }


        public TweenSpriteColor WithFrom(Color from)
        {
            _from = from;
            return this;
        }

        public TweenSpriteColor WithTo(Color to)
        {
            _to = to;
            return this;
        }

        public TweenSpriteColor WithEase(AnimationCurve curve)
        {
            _ease = curve.Evaluate;
            return this;
        }

        public TweenSpriteColor WithEase(EasingFunction.Ease easeType)
        {
            _ease = EasingFunction.GetEasingFunction(easeType);
            return this;
        }

        public TweenSpriteColor WithLoop(bool loop)
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

        private static IEnumerator DoRoutine(TweenSpriteColor args)
        {
            float time = Time.deltaTime;

            do
            {
                while (time < args._time)
                {
                    args._target.color = Color.Lerp(args._from, args._to, args._ease(time/args._time));
                    yield return null;
                    time += Time.deltaTime;
                }

                time -= args._time;
            } while (args._loop);

            args._target.color = args._to;
        }
    }
}