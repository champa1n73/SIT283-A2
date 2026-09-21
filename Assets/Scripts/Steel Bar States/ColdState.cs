using UnityEngine;

public class ColdState : ISteelBarBaseState
{
    public void EnterState(SteelBarController steelBar)
    {
        steelBar.GetMeshRenderer().material.color = steelBar.GetStateColor()["Cold"];
    }

    public void UpdateState(SteelBarController steelBar)
    {
        if (!steelBar.IsInTheForge()) { return; }
        steelBar.HeatUpSteelBar();
    }

    public void FixedUpdateState(SteelBarController steelBar)
    {
    }

    public void ExitState(SteelBarController steelBar)
    {
    }

    public void OnCollisionEnter(SteelBarController steelBar, Collision collision)
    {

    }

    public void OnCollisionExit(SteelBarController steelBar, Collision collision)
    {
    }

    public void OnCollisionStay(SteelBarController steelBar, Collision collision)
    {
    }
}
