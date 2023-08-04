// Copr. (c) Nexus 2023. All rights reserved.
#define USE_SYSTEM_RANDOM

using Openlabs.Mgcxm.Internal.SystemObjects;

namespace Openlabs.Mgcxm.Internal;

public static class Random
{
    #if (!USE_SYSTEM_RANDOM)
    private static int PcgHash(long input)
    {
        var state = (int)input * 747796405 + 2891336453;
        var word = ((int)state >> (((int)state >> 28) + 4)) * 277803737;
        return (word >> 22) ^ word;
    }
    #endif

    public static void Init(long state)
    {
#if (!USE_SYSTEM_RANDOM)
        _state = state;
#elif (USE_SYSTEM_RANDOM)
        _rng = new System.Random((int)state);
#endif
    }

    public static int Range(int min, int max)
    {
#if (!USE_SYSTEM_RANDOM)
        int range = max - min + 1;
        int random = Math.Abs(PcgHash(_state));
        
        random %= range;
        random += min;
        
        return random;
#elif (USE_SYSTEM_RANDOM)
        return _rng.Next(min, max);
#endif
    }

    public static void FillBuffer(byte[] buffer)
    {
        for (uint size = 0; size < buffer.Length; size++)
            buffer[size] = (byte)Range(byte.MinValue, byte.MaxValue);
    }
    
#if (!USE_SYSTEM_RANDOM)
    private static MgcxmLong _state;
#elif (USE_SYSTEM_RANDOM)
    private static System.Random _rng;
#endif
}