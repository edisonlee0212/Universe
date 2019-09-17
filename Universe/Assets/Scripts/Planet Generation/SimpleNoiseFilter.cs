using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter {

    NoiseSettings.SimpleNoiseSettings settings;
    Noise noise = new Noise();

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
    {
        this.settings = settings;
    }

    public double Evaluate(double3 point)
    {
        double noiseValue = 0;
        double frequency = settings.baseRoughness;
        double amplitude = 1;

        for (int i = 0; i < settings.numLayers; i++)
        {
            double v = noise.Evaluate(point * frequency + settings.centre);
            noiseValue += (v + 1) * .5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseValue = math.max(0, noiseValue - settings.minValue);
        return noiseValue * settings.strength;
    }
}
