using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stage
{
    [Header("Stage Info")]
    [SerializeField] public int stageNumber;
    
    [Header("Ball Presets")]
    [SerializeField] public Sprite ballGraphics;
    [SerializeField] public AnimationCurve rotationSpeedCurve;
    [SerializeField] public float rotationSpeedCurveLength;

    [Header("Knife Presets")]
    [SerializeField] public int knifeQuantity;
    [SerializeField] public List<float> startingKnifePositions;
}
