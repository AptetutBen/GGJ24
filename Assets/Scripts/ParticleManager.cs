using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    public GameObject pickupClothesParticlePrefab;

    private void Awake()
    {
        instance = this;
    }

    public GameObject PickUpClothesSpawn()
    {
        return Instantiate(pickupClothesParticlePrefab, Vector3.zero, Quaternion.identity);
    }
}
