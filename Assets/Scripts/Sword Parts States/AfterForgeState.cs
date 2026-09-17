using UnityEngine;

public class AfterForgeState : ISwordPartsStates
{
    private float timer = 0f;
    private const float timeToQuench = 2f;
    public void EnterState(SwordPartsController swordPartsController)
    {
        Debug.Log("Entered After Forge State");
        foreach (MeshRenderer meshRenderer in swordPartsController.meshRenderers)
        {
            meshRenderer.material.color = swordPartsController.GetStateColor()["Hot"];
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
        if (swordPartsController.isInQuenchTank)
        {
            timer += Time.deltaTime;
            if (timer > timeToQuench )
            {
                swordPartsController.ChangeState(new QuenchState());
            }
        }

    }
}
