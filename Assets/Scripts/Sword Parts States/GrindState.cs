using UnityEngine;

public class GrindState : ISwordPartsStates
{
    public void EnterState(SwordPartsController swordPartsController)
    {
        Debug.Log("Entered Grind State");
        foreach (MeshRenderer meshRenderer in swordPartsController.meshRenderers)
        {
            meshRenderer.material.color = swordPartsController.GetStateColor()["Grinded"];
        }
    }

    public void ExitState(SwordPartsController swordPartsController)
    {
    }

    public void FixedUpdateState(SwordPartsController swordPartsController)
    {
    }

    public void UpdateState(SwordPartsController swordPartsController)
    {
    }
}
