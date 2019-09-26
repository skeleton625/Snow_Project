using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObjects : MonoBehaviour
{
    [SerializeField]
    private Material[] AttackBallMaterial;
    private static Material[] outputMaterial;

    void Start()
    {
        outputMaterial = new Material[AttackBallMaterial.Length];
        for (int i = 0; i < AttackBallMaterial.Length; i++)
            outputMaterial[i] = AttackBallMaterial[i];
    }

    public static Material getMaterial(int i)
    {
        return outputMaterial[i];
    }

}
