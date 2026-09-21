using UnityEngine;

public class HotState : ISteelBarBaseState
{
    private AnvilController anvil;
    public void EnterState(SteelBarController steelBar)
    {
        Debug.Log("Steel bar is now hot.");
        steelBar.GetMeshRenderer().material.color = steelBar.GetStateColor()["Hot"];
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Anvil"))
        {
            anvil = collision.gameObject.GetComponent<AnvilController>();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Hammer"))
        {
            if (!steelBar.IsOnAnvil())
            {
                Debug.LogWarning("Steel bar is not on the anvil. Cannot forge.");
                return;
            }
            steelBar.SetHammerHits(steelBar.GetHammerHits() + 1);
            if (steelBar.GetHammerHits() >= steelBar.GetCurrentRecipe().requiredHammerHits)
            {
                GameObject forgedPart = GameObject.Instantiate(steelBar.GetCurrentRecipe().finishedPrefab, anvil.GetSpawnPoint().position, Quaternion.identity);
                forgedPart.transform.right = anvil.transform.up;
                anvil.SetDescText("Place a steel bar on the anvil to see its recipe.");
                SwordPartsController swordPart = forgedPart.GetComponent<SwordPartsController>();
                AnvilController.AddSpawnedSwordPart(swordPart);
                ObjectSpawner.RemoveSpawnedSteelBar(steelBar);
                if (swordPart != null)
                {
                    swordPart.SetRecipe(steelBar.GetCurrentRecipe());
                }
                GameObject.Destroy(steelBar.gameObject);
            }
        }
    }
}
