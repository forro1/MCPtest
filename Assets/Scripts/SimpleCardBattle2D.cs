using System;
using UnityEngine;

public partial class SimpleCardBattle2D : MonoBehaviour
{
    private void Start()
    {
        UnityEngine.Random.InitState(Environment.TickCount);
        if (usePhaseOneLoop)
        {
            gameObject.AddComponent<PhaseOnePrototypeController>().Initialize();
            return;
        }

        BuildUi();
        InitializeStages();
        NewGame();
    }
}
