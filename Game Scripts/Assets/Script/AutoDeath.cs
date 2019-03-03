using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeath : MonoBehaviour
{
    public float LifeSpan = 1f;
    private void Start()
    {
        Destroy(gameObject, LifeSpan);
    }
}
