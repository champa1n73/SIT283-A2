using UnityEngine;
public interface ISteelBarBaseState
{
    public void EnterState(SteelBarController steelBar);
    public void UpdateState(SteelBarController steelBar);
    public void FixedUpdateState(SteelBarController steelBar);
    public void ExitState(SteelBarController steelBar);

    public void OnCollisionEnter(SteelBarController steelBar, Collision collision);
    public void OnTriggerEnter(SteelBarController steelBar, Collider other);
    public void OntriggerStay(SteelBarController steelBar, Collider other);
    public void OnTriggerExit(SteelBarController steelBar, Collider other);
}
