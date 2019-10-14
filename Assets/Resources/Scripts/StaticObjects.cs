using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObjects : MonoBehaviour
{
    [SerializeField]
    private Material[] AttackBallMaterial;
    private static Material[] OutputMaterial;

    void Start()
    {
        OutputMaterial = new Material[AttackBallMaterial.Length];
        for (int i = 0; i < AttackBallMaterial.Length; i++)
            OutputMaterial[i] = AttackBallMaterial[i];
    }

    public static Material getMaterial(int i)
    {
        return OutputMaterial[i];
    }

}
