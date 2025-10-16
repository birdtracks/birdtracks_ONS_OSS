using UnityEngine;
using UnityEngine.UI;

namespace BirdTracks.Game.Core
{
    public static class GameObjectExtensions
    {
        public static TweenPosition Move(this Transform target, Vector3 from, Vector3 to, float time)
        {
            return new TweenPosition
            (
                target,
                time
            )
            .WithFrom(from)
            .WithTo(to);
        }

        public static TweenPosition MoveTo(this GameObject target, Vector3 value, float time)
        {
            return target.GetComponentInChildren<Transform>().MoveTo(value, time);
        }

        public static TweenPosition MoveTo(this Transform target, Vector3 value, float time)
        {
            return new TweenPosition
            (
                target,
                time
            )
            .WithFrom(target.position)
            .WithTo(value);
        }

        public static TweenPosition MoveToLocal(this GameObject target, Vector3 value, float time)
        {
            return target.GetComponentInChildren<Transform>().MoveToLocal(value, time);
        }

        public static TweenPosition MoveToLocal(this Transform target, Vector3 value, float time)
        {
            return new TweenPosition
            (
                target,
                time
            )
            .WithFrom(target.localPosition)
            .WithTo(value);
        }

        public static TweenRotation RotateTo(this GameObject target, Quaternion value, float time)
        {
            return target.GetComponentInChildren<Transform>().RotateTo(value, time);
        }
        public static TweenRotation RotateTo(this Transform target, Quaternion value, float time)
        {
            return new TweenRotation
            (
                target,
                time
            )
            .WithFrom(target.localRotation)
            .WithTo(value);
        }


        public static TweenScale Scale(this GameObject target, Vector3 from, Vector3 value, float time)
        {
            return target.GetComponentInChildren<Transform>().Scale(from, value, time);
        }

        public static TweenScale Scale(this Transform target, Vector3 from, Vector3 value, float time)
        {
            return new TweenScale
            (
                target,
                time
            )
            .WithFrom(from)
            .WithTo(value);
        }
        public static TweenScale ScaleTo(this GameObject target, Vector3 value, float time)
        {
            return target.GetComponentInChildren<Transform>().ScaleTo(value, time);
        }

        public static TweenScale ScaleTo(this Transform target, Vector3 value, float time)
        {
            return new TweenScale
            (
                target,
                time
            )
            .WithFrom(target.localScale)
            .WithTo(value);
        }

        public static TweenSpriteColor SpriteColorTo(this GameObject target, Color value, float time)
        {
            return target.GetComponentInChildren<SpriteRenderer>().SpriteColorTo(value, time);
        }

        public static TweenSpriteColor SpriteColorTo(this SpriteRenderer target, Color color, float time)
        {
            return new TweenSpriteColor
            (
                target,
                time
            )
            .WithFrom(target.color)
            .WithTo(color);
        }

        public static TweenSpriteColor SpriteAlphaTo(this GameObject target, float value, float time)
        {
            return target.GetComponentInChildren<SpriteRenderer>().SpriteAlphaTo(value, time);
        }


        public static TweenSpriteColor SpriteAlphaTo(this SpriteRenderer target, float alpha, float time)
        {
            var to = target.color;
            to.a = alpha;

            return new TweenSpriteColor
            (
                target,
                time
            )
            .WithFrom(target.color)
            .WithTo(to);
        }

        public static TweenImageColor ImageColorTo(this GameObject target, Color value, float time)
        {
            return target.GetComponentInChildren<Image>().ImageColorTo(value, time);
        }

        public static TweenImageColor ImageColorTo(this Image target, Color color, float time)
        {
            return new TweenImageColor
            (
                target,
                time
            )
            .WithFrom(target.color)
            .WithTo(color);
        }

        public static TweenImageColor ImageAlphaTo(this GameObject target, float value, float time)
        {
            return target.GetComponentInChildren<Image>().ImageAlphaTo(value, time);
        }


        public static TweenImageColor ImageAlphaTo(this Image target, float alpha, float time)
        {
            var to = target.color;
            to.a = alpha;

            return new TweenImageColor
            (
                target,
                time
            )
            .WithFrom(target.color)
            .WithTo(to);
        }
    }
}