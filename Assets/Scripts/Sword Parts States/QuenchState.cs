using UnityEngine;

public class QuenchState : ISwordPartsStates
{
    private float timer = 0f;
    private const float timeToGrind = 2f;
    public void EnterState(SwordPartsController swordPartsController)
    {
        Debug.Log("Entered Quench State");
        foreach (MeshRenderer meshRenderer in swordPartsController.meshRenderers)
        {
            meshRenderer.material.color = swordPartsController.GetStateColor()["Quenched"];
        }
        swordPartsController.grindingAmount = 0;
    }

    public void ExitState(SwordPartsController swordPartsController)
    {
    }

    public void FixedUpdateState(SwordPartsController swordPartsController)
    {
    }

    public void UpdateState(SwordPartsController swordPartsController)
    {
        if (swordPartsController.isGrinding)
        {
            timer += Time.deltaTime;
            if (timer > timeToGrind)
            {
                swordPartsController.grindingAmount++;
                timer = 0f;
            }
            if (swordPartsController.grindingAmount >= swordPartsController.part.requiredGrindAmount)
            {
                swordPartsController.ChangeState(new GrindState());
            }
        }
    }
}
