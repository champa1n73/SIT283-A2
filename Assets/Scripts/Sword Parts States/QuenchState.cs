using UnityEngine;

public class QuenchState : ISwordPartsStates
{
    public void EnterState(SwordPartsController swordPartsController)
    {
        Debug.Log("Entered Quench State");
        foreach (MeshRenderer meshRenderer in swordPartsController.GetMeshRenderers())
        {
            meshRenderer.material.color = swordPartsController.GetStateColor()["Quenched"];
        }
    }

    public void ExitState(SwordPartsController swordPartsController)
    {
        if (swordPartsController.GetGrindingAudioSrc().isPlaying)
        {
            swordPartsController.GetGrindingAudioSrc().Stop();
        }
    }

    public void FixedUpdateState(SwordPartsController swordPartsController)
    {
    }

    public void UpdateState(SwordPartsController swordPartsController)
    {
        if (swordPartsController.IsGrinding())
        {
            swordPartsController.SetGrindingTime(swordPartsController.GetGrindingTime() + Time.deltaTime);
            if (swordPartsController.GetGrindingTime() >= swordPartsController.GetCurrentRecipe().requiredGrindTime)
            {
                swordPartsController.ChangeState(new GrindState());
            }
        }
    }
}
