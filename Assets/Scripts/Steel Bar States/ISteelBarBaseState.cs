using UnityEngine;
public interface ISteelBarBaseState
{
    public void EnterState(SteelBarController steelBar);
    public void UpdateState(SteelBarController steelBar);
    public void FixedUpdateState(SteelBarController steelBar);
    public void ExitState(SteelBarController steelBar);

    public void OnCollisionEnter(SteelBarController steelBar, Collision collision);
    public void OnCollisionStay(SteelBarController steelBar, Collision collision);
    public void OnCollisionExit(SteelBarController steelBar, Collision collision);
}
