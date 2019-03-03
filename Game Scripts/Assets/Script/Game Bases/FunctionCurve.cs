using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "My Data/Function Curve Data")]
public class FunctionCurve : ScriptableObject
{
    public FuncCurveName Name;
    public AnimationCurve Curve;
}

public enum FuncCurveName
{
    Arctan = 0,
    Sin = 1,
}