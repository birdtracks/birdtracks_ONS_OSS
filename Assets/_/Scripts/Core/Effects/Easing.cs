using System;
using UnityEngine;

namespace BirdTracks.Game.Core
{
public class EasingFunction
{
    public enum Ease
    {
        EaseInQuad = 0,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        Linear,
        Spring,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
    }

    private const float NATURAL_LOG_OF_2 = 0.693147181f;

    //
    // Easing functions
    //

    public static float Linear(float value)
    {
        return Mathf.Lerp(0f, 1f, value);
    }

    public static float Spring(float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return 0f + (1f - 0f) * value;
    }

    public static float EaseInQuad(float value)
    {
        
        return 1f * value * value + 0f;
    }

    public static float EaseOutQuad(float value)
    {
        
        return -1f * value * (value - 2) + 0f;
    }

    public static float EaseInOutQuad(float value)
    {
        value /= .5f;
        
        if (value < 1) return 1f * 0.5f * value * value + 0f;
        value--;
        return -1f * 0.5f * (value * (value - 2) - 1) + 0f;
    }

    public static float EaseInCubic(float value)
    {
        
        return 1f * value * value * value + 0f;
    }

    public static float EaseOutCubic(float value)
    {
        value--;
        return 1f * (value * value * value + 1) + 0f;
    }

    public static float EaseInOutCubic(float value)
    {
        value /= .5f;
        if (value < 1) return 1f * 0.5f * value * value * value + 0f;
        value -= 2;
        return 1f * 0.5f * (value * value * value + 2) + 0f;
    }

    public static float EaseInQuart(float value)
    {
        return 1f * value * value * value * value + 0f;
    }

    public static float EaseOutQuart(float value)
    {
        value--;
        return -1f * (value * value * value * value - 1) + 0f;
    }

    public static float EaseInOutQuart(float value)
    {
        value /= .5f;
        if (value < 1) return 1f * 0.5f * value * value * value * value + 0f;
        value -= 2;
        return -1f * 0.5f * (value * value * value * value - 2) + 0f;
    }

    public static float EaseInQuint(float value)
    {
        return 1f * value * value * value * value * value + 0f;
    }

    public static float EaseOutQuint(float value)
    {
        value--;
        return 1f * (value * value * value * value * value + 1) + 0f;
    }

    public static float EaseInOutQuint(float value)
    {
        value /= .5f;
        if (value < 1) return 1f * 0.5f * value * value * value * value * value + 0f;
        value -= 2;
        return 1f * 0.5f * (value * value * value * value * value + 2) + 0f;
    }

    public static float EaseInSine(float value)
    {
        return -1f * Mathf.Cos(value * (Mathf.PI * 0.5f)) + 1f + 0f;
    }

    public static float EaseOutSine(float value)
    {
        return 1f * Mathf.Sin(value * (Mathf.PI * 0.5f)) + 0f;
    }

    public static float EaseInOutSine(float value)
    {
        return -1f * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + 0f;
    }

    public static float EaseInExpo(float value)
    {
        return 1f * Mathf.Pow(2, 10 * (value - 1)) + 0f;
    }

    public static float EaseOutExpo(float value)
    {
        
        return 1f * (-Mathf.Pow(2, -10 * value) + 1) + 0f;
    }

    public static float EaseInOutExpo(float value)
    {
        value /= .5f;
        
        if (value < 1) return 1f * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + 0f;
        value--;
        return 1f * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + 0f;
    }

    public static float EaseInCirc(float value)
    {
        
        return -1f * (Mathf.Sqrt(1 - value * value) - 1) + 0f;
    }

    public static float EaseOutCirc(float value)
    {
        value--;
        
        return 1f * Mathf.Sqrt(1 - value * value) + 0f;
    }

    public static float EaseInOutCirc(float value)
    {
        value /= .5f;
        
        if (value < 1) return -1f * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + 0f;
        value -= 2;
        return 1f * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + 0f;
    }

    public static float EaseInBounce(float value)
    {
        
        float d = 1f;
        return 1f - EaseOutBounce(d - value) + 0f;
    }

    public static float EaseOutBounce(float value)
    {
        value /= 1f;
        
        if (value < (1 / 2.75f))
        {
            return 1f * (7.5625f * value * value) + 0f;
        }
        else if (value < (2 / 2.75f))
        {
            value -= (1.5f / 2.75f);
            return 1f * (7.5625f * (value) * value + .75f) + 0f;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25f / 2.75f);
            return 1f * (7.5625f * (value) * value + .9375f) + 0f;
        }
        else
        {
            value -= (2.625f / 2.75f);
            return 1f * (7.5625f * (value) * value + .984375f) + 0f;
        }
    }

    public static float EaseInOutBounce(float value)
    {
        
        float d = 1f;
        if (value < d * 0.5f) return EaseInBounce(value * 2) * 0.5f + 0f;
        else return EaseOutBounce(value * 2 - d) * 0.5f + 1f * 0.5f + 0f;
    }

    public static float EaseInBack(float value)
    {
        
        value /= 1;
        float s = 1.70158f;
        return 1f * (value) * value * ((s + 1) * value - s) + 0f;
    }

    public static float EaseOutBack(float value)
    {
        float s = 1.70158f;
        
        value = (value) - 1;
        return 1f * ((value) * value * ((s + 1) * value + s) + 1) + 0f;
    }

    public static float EaseInOutBack(float value)
    {
        float s = 1.70158f;
        
        value /= .5f;
        if ((value) < 1)
        {
            s *= (1.525f);
            return 1f * 0.5f * (value * value * (((s) + 1) * value - s)) + 0f;
        }
        value -= 2;
        s *= (1.525f);
        return 1f * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + 0f;
    }

    public static float EaseInElastic(float value)
    {
        

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (value == 0) return 0f;

        if ((value /= d) == 1) return 0f + 1f;

        if (a == 0f || a < Mathf.Abs(1f))
        {
            a = 1f;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(1f / a);
        }

        return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + 0f;
    }

    public static float EaseOutElastic(float value)
    {
        

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (value == 0) return 0f;

        if ((value /= d) == 1) return 0f + 1f;

        if (a == 0f || a < Mathf.Abs(1f))
        {
            a = 1f;
            s = p * 0.25f;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(1f / a);
        }

        return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + 1f + 0f);
    }

    public static float EaseInOutElastic(float value)
    {
        

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (value == 0) return 0f;

        if ((value /= d * 0.5f) == 2) return 0f + 1f;

        if (a == 0f || a < Mathf.Abs(1f))
        {
            a = 1f;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(1f / a);
        }

        if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + 0f;
        return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + 1f + 0f;
    }

    //
    // These are derived functions that the motor can use to get the speed at a specific time.
    //
    // The easing functions all work with a normalized time (0 to 1) and the returned value here
    // reflects that. Values returned here should be divided by the actual time.
    //
    // TODO: These functions have not had the testing they deserve. If there is odd behavior around
    //       dash speeds then this would be the first place I'd look.

    public static float LinearD(float value)
    {
        return 1f - 0f;
    }

    public static float EaseInQuadD(float value)
    {
        return 2f * (1f - 0f) * value;
    }

    public static float EaseOutQuadD(float value)
    {
        
        return -1f * value - 1f * (value - 2);
    }

    public static float EaseInOutQuadD(float value)
    {
        value /= .5f;
        

        if (value < 1)
        {
            return 1f * value;
        }

        value--;

        return 1f * (1 - value);
    }

    public static float EaseInCubicD(float value)
    {
        return 3f * (1f - 0f) * value * value;
    }

    public static float EaseOutCubicD(float value)
    {
        value--;
        
        return 3f * 1f * value * value;
    }

    public static float EaseInOutCubicD(float value)
    {
        value /= .5f;
        

        if (value < 1)
        {
            return (3f / 2f) * 1f * value * value;
        }

        value -= 2;

        return (3f / 2f) * 1f * value * value;
    }

    public static float EaseInQuartD(float value)
    {
        return 4f * (1f - 0f) * value * value * value;
    }

    public static float EaseOutQuartD(float value)
    {
        value--;
        
        return -4f * 1f * value * value * value;
    }

    public static float EaseInOutQuartD(float value)
    {
        value /= .5f;
        

        if (value < 1)
        {
            return 2f * 1f * value * value * value;
        }

        value -= 2;

        return -2f * 1f * value * value * value;
    }

    public static float EaseInQuintD(float value)
    {
        return 5f * (1f - 0f) * value * value * value * value;
    }

    public static float EaseOutQuintD(float value)
    {
        value--;
        
        return 5f * 1f * value * value * value * value;
    }

    public static float EaseInOutQuintD(float value)
    {
        value /= .5f;
        

        if (value < 1)
        {
            return (5f / 2f) * 1f * value * value * value * value;
        }

        value -= 2;

        return (5f / 2f) * 1f * value * value * value * value;
    }

    public static float EaseInSineD(float value)
    {
        return (1f - 0f) * 0.5f * Mathf.PI * Mathf.Sin(0.5f * Mathf.PI * value);
    }

    public static float EaseOutSineD(float value)
    {
        
        return (Mathf.PI * 0.5f) * 1f * Mathf.Cos(value * (Mathf.PI * 0.5f));
    }

    public static float EaseInOutSineD(float value)
    {
        
        return 1f * 0.5f * Mathf.PI * Mathf.Cos(Mathf.PI * value);
    }
    public static float EaseInExpoD(float value)
    {
        return (10f * NATURAL_LOG_OF_2 * (1f - 0f) * Mathf.Pow(2f, 10f * (value - 1)));
    }

    public static float EaseOutExpoD(float value)
    {
        
        return 5f * NATURAL_LOG_OF_2 * 1f * Mathf.Pow(2f, 1f - 10f * value);
    }

    public static float EaseInOutExpoD(float value)
    {
        value /= .5f;
        

        if (value < 1)
        {
            return 5f * NATURAL_LOG_OF_2 * 1f * Mathf.Pow(2f, 10f * (value - 1));
        }

        value--;

        return (5f * NATURAL_LOG_OF_2 * 1f) / (Mathf.Pow(2f, 10f * value));
    }

    public static float EaseInCircD(float value)
    {
        return ((1f - 0f) * value) / Mathf.Sqrt(1f - value * value);
    }

    public static float EaseOutCircD(float value)
    {
        value--;
        
        return (-1f * value) / Mathf.Sqrt(1f - value * value);
    }

    public static float EaseInOutCircD(float value)
    {
        value /= .5f;
        

        if (value < 1)
        {
            return (1f * value) / (2f * Mathf.Sqrt(1f - value * value));
        }

        value -= 2;

        return (-1f * value) / (2f * Mathf.Sqrt(1f - value * value));
    }

    public static float EaseInBounceD(float value)
    {
        
        float d = 1f;

        return EaseOutBounceD(d - value);
    }

    public static float EaseOutBounceD(float value)
    {
        value /= 1f;
        

        if (value < (1 / 2.75f))
        {
            return 2f * 1f * 7.5625f * value;
        }
        else if (value < (2 / 2.75f))
        {
            value -= (1.5f / 2.75f);
            return 2f * 1f * 7.5625f * value;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25f / 2.75f);
            return 2f * 1f * 7.5625f * value;
        }
        else
        {
            value -= (2.625f / 2.75f);
            return 2f * 1f * 7.5625f * value;
        }
    }

    public static float EaseInOutBounceD(float value)
    {
        
        float d = 1f;

        if (value < d * 0.5f)
        {
            return EaseInBounceD(value * 2) * 0.5f;
        }
        else
        {
            return EaseOutBounceD(value * 2 - d) * 0.5f;
        }
    }

    public static float EaseInBackD(float value)
    {
        float s = 1.70158f;

        return 3f * (s + 1f) * (1f - 0f) * value * value - 2f * s * (1f - 0f) * value;
    }

    public static float EaseOutBackD(float value)
    {
        float s = 1.70158f;
        
        value = (value) - 1;

        return 1f * ((s + 1f) * value * value + 2f * value * ((s + 1f) * value + s));
    }

    public static float EaseInOutBackD(float value)
    {
        float s = 1.70158f;
        
        value /= .5f;

        if ((value) < 1)
        {
            s *= (1.525f);
            return 0.5f * 1f * (s + 1) * value * value + 1f * value * ((s + 1f) * value - s);
        }

        value -= 2;
        s *= (1.525f);
        return 0.5f * 1f * ((s + 1) * value * value + 2f * value * ((s + 1f) * value + s));
    }

    public static float EaseInElasticD(float value)
    {
        

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (a == 0f || a < Mathf.Abs(1f))
        {
            a = 1f;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(1f / a);
        }

        float c = 2 * Mathf.PI;

        // From an online derivative calculator, kinda hoping it is right.
        return ((-a) * d * c * Mathf.Cos((c * (d * (value - 1f) - s)) / p)) / p -
            5f * NATURAL_LOG_OF_2 * a * Mathf.Sin((c * (d * (value - 1f) - s)) / p) *
            Mathf.Pow(2f, 10f * (value - 1f) + 1f);
    }

    public static float EaseOutElasticD(float value)
    {
        

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (a == 0f || a < Mathf.Abs(1f))
        {
            a = 1f;
            s = p * 0.25f;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(1f / a);
        }

        return (a * Mathf.PI * d * Mathf.Pow(2f, 1f - 10f * value) *
            Mathf.Cos((2f * Mathf.PI * (d * value - s)) / p)) / p - 5f * NATURAL_LOG_OF_2 * a *
            Mathf.Pow(2f, 1f - 10f * value) * Mathf.Sin((2f * Mathf.PI * (d * value - s)) / p);
    }

    public static float EaseInOutElasticD(float value)
    {
        

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (a == 0f || a < Mathf.Abs(1f))
        {
            a = 1f;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(1f / a);
        }

        if (value < 1)
        {
            value -= 1;

            return -5f * NATURAL_LOG_OF_2 * a * Mathf.Pow(2f, 10f * value) * Mathf.Sin(2 * Mathf.PI * (d * value - 2f) / p) -
                a * Mathf.PI * d * Mathf.Pow(2f, 10f * value) * Mathf.Cos(2 * Mathf.PI * (d * value - s) / p) / p;
        }

        value -= 1;

        return a * Mathf.PI * d * Mathf.Cos(2f * Mathf.PI * (d * value - s) / p) / (p * Mathf.Pow(2f, 10f * value)) -
            5f * NATURAL_LOG_OF_2 * a * Mathf.Sin(2f * Mathf.PI * (d * value - s) / p) / (Mathf.Pow(2f, 10f * value));
    }

    public static float SpringD(float value)
    {
        value = Mathf.Clamp01(value);
        

        // Damn... Thanks http://www.derivative-calculator.net/
        return 1f * (6f * (1f - value) / 5f + 1f) * (-2.2f * Mathf.Pow(1f - value, 1.2f) *
            Mathf.Sin(Mathf.PI * value * (2.5f * value * value * value + 0.2f)) + Mathf.Pow(1f - value, 2.2f) *
            (Mathf.PI * (2.5f * value * value * value + 0.2f) + 7.5f * Mathf.PI * value * value * value) *
            Mathf.Cos(Mathf.PI * value * (2.5f * value * value * value + 0.2f)) + 1f) -
            6f * 1f * (Mathf.Pow(1 - value, 2.2f) * Mathf.Sin(Mathf.PI * value * (2.5f * value * value * value + 0.2f)) + value
            / 5f);

    }

    public delegate float Function(float v);

    /// <summary>
    /// Returns the function associated to the easingFunction enum. This value returned should be cached as it allocates memory
    /// to return.
    /// </summary>
    /// <param name="easingFunction">The enum associated with the easing function.</param>
    /// <returns>The easing function</returns>
    public static Function GetEasingFunction(Ease easingFunction)
    {
        if (easingFunction == Ease.EaseInQuad)
        {
            return EaseInQuad;
        }

        if (easingFunction == Ease.EaseOutQuad)
        {
            return EaseOutQuad;
        }

        if (easingFunction == Ease.EaseInOutQuad)
        {
            return EaseInOutQuad;
        }

        if (easingFunction == Ease.EaseInCubic)
        {
            return EaseInCubic;
        }

        if (easingFunction == Ease.EaseOutCubic)
        {
            return EaseOutCubic;
        }

        if (easingFunction == Ease.EaseInOutCubic)
        {
            return EaseInOutCubic;
        }

        if (easingFunction == Ease.EaseInQuart)
        {
            return EaseInQuart;
        }

        if (easingFunction == Ease.EaseOutQuart)
        {
            return EaseOutQuart;
        }

        if (easingFunction == Ease.EaseInOutQuart)
        {
            return EaseInOutQuart;
        }

        if (easingFunction == Ease.EaseInQuint)
        {
            return EaseInQuint;
        }

        if (easingFunction == Ease.EaseOutQuint)
        {
            return EaseOutQuint;
        }

        if (easingFunction == Ease.EaseInOutQuint)
        {
            return EaseInOutQuint;
        }

        if (easingFunction == Ease.EaseInSine)
        {
            return EaseInSine;
        }

        if (easingFunction == Ease.EaseOutSine)
        {
            return EaseOutSine;
        }

        if (easingFunction == Ease.EaseInOutSine)
        {
            return EaseInOutSine;
        }

        if (easingFunction == Ease.EaseInExpo)
        {
            return EaseInExpo;
        }

        if (easingFunction == Ease.EaseOutExpo)
        {
            return EaseOutExpo;
        }

        if (easingFunction == Ease.EaseInOutExpo)
        {
            return EaseInOutExpo;
        }

        if (easingFunction == Ease.EaseInCirc)
        {
            return EaseInCirc;
        }

        if (easingFunction == Ease.EaseOutCirc)
        {
            return EaseOutCirc;
        }

        if (easingFunction == Ease.EaseInOutCirc)
        {
            return EaseInOutCirc;
        }

        if (easingFunction == Ease.Linear)
        {
            return Linear;
        }

        if (easingFunction == Ease.Spring)
        {
            return Spring;
        }

        if (easingFunction == Ease.EaseInBounce)
        {
            return EaseInBounce;
        }

        if (easingFunction == Ease.EaseOutBounce)
        {
            return EaseOutBounce;
        }

        if (easingFunction == Ease.EaseInOutBounce)
        {
            return EaseInOutBounce;
        }

        if (easingFunction == Ease.EaseInBack)
        {
            return EaseInBack;
        }

        if (easingFunction == Ease.EaseOutBack)
        {
            return EaseOutBack;
        }

        if (easingFunction == Ease.EaseInOutBack)
        {
            return EaseInOutBack;
        }

        if (easingFunction == Ease.EaseInElastic)
        {
            return EaseInElastic;
        }

        if (easingFunction == Ease.EaseOutElastic)
        {
            return EaseOutElastic;
        }

        if (easingFunction == Ease.EaseInOutElastic)
        {
            return EaseInOutElastic;
        }

        return null;
    }

    /// <summary>
    /// Gets the derivative function of the appropriate easing function. If you use an easing function for position then this
    /// function can get you the speed at a given time (normalized).
    /// </summary>
    /// <param name="easingFunction"></param>
    /// <returns>The derivative function</returns>
    public static Function GetEasingFunctionDerivative(Ease easingFunction)
    {
        if (easingFunction == Ease.EaseInQuad)
        {
            return EaseInQuadD;
        }

        if (easingFunction == Ease.EaseOutQuad)
        {
            return EaseOutQuadD;
        }

        if (easingFunction == Ease.EaseInOutQuad)
        {
            return EaseInOutQuadD;
        }

        if (easingFunction == Ease.EaseInCubic)
        {
            return EaseInCubicD;
        }

        if (easingFunction == Ease.EaseOutCubic)
        {
            return EaseOutCubicD;
        }

        if (easingFunction == Ease.EaseInOutCubic)
        {
            return EaseInOutCubicD;
        }

        if (easingFunction == Ease.EaseInQuart)
        {
            return EaseInQuartD;
        }

        if (easingFunction == Ease.EaseOutQuart)
        {
            return EaseOutQuartD;
        }

        if (easingFunction == Ease.EaseInOutQuart)
        {
            return EaseInOutQuartD;
        }

        if (easingFunction == Ease.EaseInQuint)
        {
            return EaseInQuintD;
        }

        if (easingFunction == Ease.EaseOutQuint)
        {
            return EaseOutQuintD;
        }

        if (easingFunction == Ease.EaseInOutQuint)
        {
            return EaseInOutQuintD;
        }

        if (easingFunction == Ease.EaseInSine)
        {
            return EaseInSineD;
        }

        if (easingFunction == Ease.EaseOutSine)
        {
            return EaseOutSineD;
        }

        if (easingFunction == Ease.EaseInOutSine)
        {
            return EaseInOutSineD;
        }

        if (easingFunction == Ease.EaseInExpo)
        {
            return EaseInExpoD;
        }

        if (easingFunction == Ease.EaseOutExpo)
        {
            return EaseOutExpoD;
        }

        if (easingFunction == Ease.EaseInOutExpo)
        {
            return EaseInOutExpoD;
        }

        if (easingFunction == Ease.EaseInCirc)
        {
            return EaseInCircD;
        }

        if (easingFunction == Ease.EaseOutCirc)
        {
            return EaseOutCircD;
        }

        if (easingFunction == Ease.EaseInOutCirc)
        {
            return EaseInOutCircD;
        }

        if (easingFunction == Ease.Linear)
        {
            return LinearD;
        }

        if (easingFunction == Ease.Spring)
        {
            return SpringD;
        }

        if (easingFunction == Ease.EaseInBounce)
        {
            return EaseInBounceD;
        }

        if (easingFunction == Ease.EaseOutBounce)
        {
            return EaseOutBounceD;
        }

        if (easingFunction == Ease.EaseInOutBounce)
        {
            return EaseInOutBounceD;
        }

        if (easingFunction == Ease.EaseInBack)
        {
            return EaseInBackD;
        }

        if (easingFunction == Ease.EaseOutBack)
        {
            return EaseOutBackD;
        }

        if (easingFunction == Ease.EaseInOutBack)
        {
            return EaseInOutBackD;
        }

        if (easingFunction == Ease.EaseInElastic)
        {
            return EaseInElasticD;
        }

        if (easingFunction == Ease.EaseOutElastic)
        {
            return EaseOutElasticD;
        }

        if (easingFunction == Ease.EaseInOutElastic)
        {
            return EaseInOutElasticD;
        }

        return null;
    }
}
}