using System;
using UnityEngine;

public partial class SimpleCardBattle2D : MonoBehaviour
{
    private void Start()
    {
        UnityEngine.Random.InitState(Environment.TickCount);
        BuildUi();
        InitializeStages();
        NewGame();
    }
}
