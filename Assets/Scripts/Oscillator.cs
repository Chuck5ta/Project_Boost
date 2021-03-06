﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f; // 2 seconds

 // [Range(0,1)] // sets a range of 1 to 1 for movementFactor
 // [SerializeField] // makes movementFactor available in the inspector (IDE)
    float movementFactor; // 0 for not moved, 1 for fully moved

    Vector3 startingPos;

    // Use this for initialization
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // set movement factor
        if (period <= Mathf.Epsilon) // guard against div by zero
            return;
        float cycles = Time.time / period; // grows continually from 0

        const float tau = Mathf.PI * 2f; // aboyt 6.28
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
