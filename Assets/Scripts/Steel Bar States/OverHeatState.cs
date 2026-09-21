using UnityEngine;

public class OverHeatState : ISteelBarBaseState
{
    private float timer = 0f;
    private const float destroyTime = 2f;
    public void EnterState(SteelBarController steelBar)
    {
        Debug.Log("Steel bar is now overheat.");
        steelBar.GetMeshRenderer().material.color = steelBar.GetStateColor()["Overheat"];
    }

    public void ExitState(SteelBarController steelBar)
    {
    }

    public void FixedUpdateState(SteelBarController steelBar)
    {
    }

    public void OnCollisionEnter(SteelBarController steelBar, Collision collision)
    {
    }

    public void OnCollisionExit(SteelBarController steelBar, Collision collision)
    {
    }

    public void UpdateState(SteelBarController steelBar)
    {
        timer += Time.deltaTime;
        if (timer >= destroyTime)
        {
            GameObject.Destroy(steelBar.gameObject);
        }
    }

    public void OnCollisionStay(SteelBarController steelBar, Collision collision)
    {
    }
}
