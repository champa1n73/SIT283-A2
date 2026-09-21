using UnityEngine;

public class AfterForgeState : ISwordPartsStates
{
    private ParticleSystem particle;
    private bool runOnce = false;
    private float dropTime = 1f;
    public void EnterState(SwordPartsController swordPartsController)
    {
        Debug.Log("Entered After Forge State");
        DelayDrop(swordPartsController);

        foreach (MeshRenderer meshRenderer in swordPartsController.GetMeshRenderers())
        {
            meshRenderer.material.color = swordPartsController.GetStateColor()["Hot"];
        }
        swordPartsController.GetQuenchAudioSrc().clip = swordPartsController.GetQuenchAudioClip();
    }

    public void ExitState(SwordPartsController swordPartsController)
    {
        if (swordPartsController.GetQuenchAudioSrc().isPlaying)
        {
            swordPartsController.GetQuenchAudioSrc().Stop();
        }

        if (particle == null) { return; }
        particle.Stop();
        GameObject.Destroy(particle.gameObject);
    }

    public void FixedUpdateState(SwordPartsController swordPartsController)
    {
    }

    public void UpdateState(SwordPartsController swordPartsController)
    {
        if (swordPartsController.IsInQuenchTank())
        {
            if (!runOnce)
            {
                particle = GameObject.Instantiate(swordPartsController.GetSteamParticle(), swordPartsController.transform.position, Quaternion.identity);

                if (particle == null) { return; }
                runOnce = true;
                particle.Play();
                swordPartsController.GetQuenchAudioSrc().Play();
            }

            swordPartsController.SetQuenchTime(swordPartsController.GetQuenchTime() + Time.deltaTime);
            if (swordPartsController.GetQuenchTime() >= swordPartsController.GetCurrentRecipe().requiredQuenchTime)
            {
                swordPartsController.ChangeState(new QuenchState());
            }
        }
        else
        {
            runOnce = false;
            
            if (particle != null)
            {
                particle.Stop();
                GameObject.Destroy(particle.gameObject);
                particle = null;
            }

            if (swordPartsController.GetQuenchAudioSrc().isPlaying)
            {
                swordPartsController.GetQuenchAudioSrc().Stop();
            }
        }
    }

    private async void DelayDrop(SwordPartsController swordPartsController)
    {
        Rigidbody rb = swordPartsController.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        await Awaitable.WaitForSecondsAsync(dropTime);

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
