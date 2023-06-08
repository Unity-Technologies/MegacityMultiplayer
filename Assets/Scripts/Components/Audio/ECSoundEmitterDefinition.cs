﻿using System;
using Unity.Entities;

namespace Unity.MegaCity.Audio
{
    [Serializable]
    public struct ECSoundEmitterDefinition : IComponentData
    {
        public int definitionIndex;
        public int soundPlayerIndexMin;
        public int soundPlayerIndexMax;
        public float volume;
        public float maxDist;
    }
}