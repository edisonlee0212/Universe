using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Universe
{
    public enum ShapeConstructionStageType
    {

        SimpleNoiseFilter,
        RidgidNoiseFilter
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
        public float weightMultiplier;
        public double Process(double3 pointOnUnitSphere, double previousElevation, ref Noise noise)
        {
            double elevation;
            switch (stageType)
            {
                case ShapeConstructionStageType.SimpleNoiseFilter:
                    elevation = SimpleNoiseFilterProcess(pointOnUnitSphere, previousElevation, ref noise);
                    break;
                case ShapeConstructionStageType.RidgidNoiseFilter:
                    elevation = RidgidNoiseFilterProcess(pointOnUnitSphere, previousElevation, ref noise);
                    break;
                default:
                    elevation = previousElevation;
                    break;
            }
            return elevation;
        }

        public double SimpleNoiseFilterProcess(double3 point, double previousElevation, ref Noise noise)
        {
            double elevation = previousElevation;

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

            noiseValue = math.max(0, noiseValue - minValue);
            elevation += noiseValue * strength;

            return elevation;
        }

        public double RidgidNoiseFilterProcess(double3 point, double previousElevation, ref Noise noise)
        {
            double elevation = previousElevation;

            double noiseValue = 0;
            double frequency = baseRoughness;
            double amplitude = 1;
            double weight = 1;

            for (int i = 0; i < numLayers; i++)
            {
                double v = 1 - math.abs(noise.Evaluate(point * frequency + center));
                v *= v;
                v *= weight;
                weight = math.clamp(v * weightMultiplier, 0, 1);

                noiseValue += v * amplitude;
                frequency *= roughness;
                amplitude *= persistence;
            }

            noiseValue = math.max(0, noiseValue - minValue);
            elevation += noiseValue * strength;

            return elevation;
        }
    }
}
