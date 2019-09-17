using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface INoiseFilter {

    double Evaluate(double3 point);
}
