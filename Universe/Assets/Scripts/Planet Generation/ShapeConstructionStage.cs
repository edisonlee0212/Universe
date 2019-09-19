using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Universe
{
    public enum ShapeConstructionStageType
    {
        SimpleNoiseFilter,
        AmplifiedNoiseFilter,
        SeaLevel,
    }

    public struct ShapeConstructionStage
    {
        public ShapeConstructionStageType stageType;
        public double strength;
        public int numLayers;
        public double baseRoughness;
        public double roughness;
        public double persistence;
        public double3 center;
        public double minValue;
        public double weightMultiplier;
        public double Process(double3 pointOnUnitSphere, ref Noise noise, double previousResult, double previousElevation)
        {
            double result;
            switch (stageType)
            {
                case ShapeConstructionStageType.SeaLevel:
                    result = SeaLevelProcess(pointOnUnitSphere, previousElevation);
                    break;
                case ShapeConstructionStageType.SimpleNoiseFilter:
                    result = SimpleNoiseFilterProcess(pointOnUnitSphere, ref noise);
                    break;
                case ShapeConstructionStageType.AmplifiedNoiseFilter:
                    result = AmplifiedNoiseFilterProcess(pointOnUnitSphere, ref noise, previousResult);
                    break;
                default:
                    result = 0;
                    break;
            }
            return result;
        }

        public double SeaLevelProcess(double3 point, double previousElevation)
        {
            return math.max(minValue, previousElevation) - previousElevation;
        }

        public double SimpleNoiseFilterProcess(double3 point, ref Noise noise)
        {
            double noiseValue = 0;
            double frequency = baseRoughness;
            double amplitude = 1;

            for (int i = 0; i < numLayers; i++)
            {
                double v = noise.Evaluate(point * frequency + center);
                noiseValue += (v + 1) * .5f * amplitude;
                frequency *= roughness;
                amplitude *= persistence;
            }

            return (noiseValue - 1) * strength;
        }

        public double AmplifiedNoiseFilterProcess(double3 point, ref Noise noise, double previousResult)
        {
            double noiseValue = 0;
            double frequency = baseRoughness;
            double amplitude = 1;

            for (int i = 0; i < numLayers; i++)
            {
                double v = noise.Evaluate(point * frequency + center);
                noiseValue += (v + 1) * .5f * amplitude;
                frequency *= roughness;
                amplitude *= persistence;
            }

            noiseValue *= math.max(0, previousResult + 1);

            noiseValue = math.max(0, noiseValue - minValue);


            return noiseValue - previousResult;
        }

    }
}
