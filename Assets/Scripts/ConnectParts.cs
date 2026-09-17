using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ConnectParts : MonoBehaviour
{
    public ForgePart connectToPart;

    private void OnTriggerEnter(Collider other)
    {
        SwordPartsController otherPart = other.GetComponentInParent<SwordPartsController>();

        if (otherPart == null) { return; }
        if (otherPart.currentState is not GrindState) { return; }
        if (otherPart.attachPoint == null) { return; }
        if (otherPart.part.recipePart != connectToPart) { return; }

        SwordPartsController thisPart = GetComponentInParent<SwordPartsController>();

        if (thisPart == null || thisPart == otherPart) { return; }
        if (thisPart.currentState is not GrindState) { return; }

        XRGrabInteractable thisGrab = thisPart.GetComponent<XRGrabInteractable>();

        XRGrabInteractable otherGrab = otherPart.GetComponent<XRGrabInteractable>();

        Rigidbody thisRigidbody = thisPart.GetComponent<Rigidbody>();

        Rigidbody otherRigidbody = otherPart.GetComponent<Rigidbody>();

        if (thisGrab != null)
        {
            Destroy(thisGrab);
        }

        if (otherGrab != null)
        {
            Destroy(otherGrab);
        }

        if (thisRigidbody != null)
        {
            Destroy(thisRigidbody);
        }
        
        if (otherRigidbody != null)
        {
            Destroy(otherRigidbody);
        }

        Debug.Log($"Connecting {otherPart.part.recipePart} to {connectToPart}");

        otherPart.transform.rotation = transform.rotation;

        Vector3 offset = otherPart.attachPoint.position - otherPart.transform.position;
        otherPart.transform.position = transform.position - offset;

        AddToParent(thisPart, otherPart);

        Destroy(otherPart.attachPoint.gameObject);
        Destroy(this);
    }

    private void AddToParent(SwordPartsController thisPart, SwordPartsController otherPart)
    {
        // Find an existing parent.
        Transform thisParent = thisPart.transform.parent;
        Transform otherParent = otherPart.transform.parent;

        Transform swordParent;

        if (thisParent != null)
        {
            swordParent = thisParent;
        }
        else if (otherParent != null)
        {
            swordParent = otherParent;
        }
        else
        {
            GameObject sword = new GameObject("Sword");
            swordParent = sword.transform;

            swordParent.position = thisPart.transform.position;
            swordParent.rotation = thisPart.transform.rotation;
        }

        thisPart.transform.SetParent(swordParent, true);
        otherPart.transform.SetParent(swordParent, true);

        if (swordParent.GetComponent<XRGrabInteractable>() == null)
        {
            XRGrabInteractable grabInteractable = swordParent.gameObject.AddComponent<XRGrabInteractable>();

            if (otherPart.part != null && otherPart.part.recipePart == ForgePart.LongSwordGuard)
            {
                grabInteractable.attachTransform = otherPart.transform;
                grabInteractable.farAttachMode = InteractableFarAttachMode.Near;
            }
        }

        Debug.Log($"Both parts are now children of {swordParent.name}");
    }
}
