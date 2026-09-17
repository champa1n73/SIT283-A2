using UnityEngine;

public class ColdState : ISteelBarBaseState
{
    public void EnterState(SteelBarController steelBar)
    {
        Debug.Log("Steel bar is now in Cold State.");
        steelBar.GetMeshRenderer().material.color = steelBar.GetStateColor()["Cold"];
    }

    public void UpdateState(SteelBarController steelBar)
    {
        if (!steelBar.isInTheForge) { return; }
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

    public void OnTriggerEnter(SteelBarController steelBar, Collider other)
    {

    }

    public void OntriggerStay(SteelBarController steelBar, Collider other)
    {
    }

    public void OnTriggerExit(SteelBarController steelBar, Collider other)
    {

    }
}
