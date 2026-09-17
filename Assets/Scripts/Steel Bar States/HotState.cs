using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HotState : ISteelBarBaseState
{
    private GameObject anvil;
    public void EnterState(SteelBarController steelBar)
    {
        Debug.Log("Steel bar is now hot.");
        steelBar.GetMeshRenderer().material.color = steelBar.GetStateColor()["Hot"];
        steelBar.hammerHits = 0;
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Anvil"))
        {
            anvil = collision.gameObject;
        }
    }

    public void OnTriggerEnter(SteelBarController steelBar, Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hammer"))
        {
            if (anvil == null)
            {
                Debug.LogWarning("Steel bar is not on the anvil. Cannot forge.");
                return;
            }
            steelBar.hammerHits += 1;
            if (steelBar.hammerHits >= steelBar.part.requiredHammerHits)
            {
                GameObject forgedPart = GameObject.Instantiate(steelBar.part.finishedPrefab, steelBar.transform.position, steelBar.transform.rotation);
                forgedPart.transform.forward = anvil.transform.forward;
                SwordPartsController swordPart = forgedPart.GetComponent<SwordPartsController>();
                if (swordPart != null)
                {
                    swordPart.part = steelBar.part;
                }
                GameObject.Destroy(steelBar.gameObject);
            }
        }
    }

    public void OntriggerStay(SteelBarController steelBar, Collider other)
    {
    }

    public void OnTriggerExit(SteelBarController steelBar, Collider other)
    {
    }
}
