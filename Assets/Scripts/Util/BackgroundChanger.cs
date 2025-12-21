using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    public int phaseToChange;
    private void OnTriggerEnter2D(Collider2D other)
    {
        BackGroundManager.Instance.SetPhase(phaseToChange);
    }
}
