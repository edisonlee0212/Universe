using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RidgidNoiseFilter : INoiseFilter {

    NoiseSettings.RidgidNoiseSettings settings;
    Noise noise = new Noise();

    public RidgidNoiseFilter(NoiseSettings.RidgidNoiseSettings settings)
    {
        this.settings = settings;
    }

    public double Evaluate(double3 point)
    {
        double noiseValue = 0;
        double frequency = settings.baseRoughness;
        double amplitude = 1;
        double weight = 1;

        for (int i = 0; i < settings.numLayers; i++)
        {
            double v = 1 - math.abs(noise.Evaluate(point * frequency + settings.centre));
            v *= v;
            v *= weight;
            weight = math.clamp(v * settings.weightMultiplier, 0, 1);

            noiseValue += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseValue = math.max(0, noiseValue - settings.minValue); 
        return noiseValue * settings.strength;
    }
}
