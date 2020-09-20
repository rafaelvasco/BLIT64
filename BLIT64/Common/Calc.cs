using System;
using System.Runtime.CompilerServices;

namespace BLIT64
{
    public static class Calc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            return (value > max) ? max : ((value < min) ? min : value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastCeilToInt(float value)
        {
            return 32768 - (int)(32768f - value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastFloorToInt(float x)
        {
            return (int)(x + 32768f) - 32768;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float value)
        {
            return (float)Math.Round(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Snap(float value, float increment)
        {
            return (float) (Math.Floor(value / increment) * increment);
        }

        public static int SnapToInt(float value, float increment)
        {
            if (increment < 2.0f)
            {
                return Calc.FastFloorToInt(value);
            }

            return (int) (Calc.FastFloorToInt(value / increment) * increment);
        }
    }
}
