using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ConnectParts : MonoBehaviour
{
    public ForgePart connectToPart;
    private bool isConnecting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isConnecting) { return; }

        SwordPartsController otherPart = other.GetComponentInParent<SwordPartsController>();
        if (otherPart == null) { return; }
        if (!(otherPart.currentState is GrindState)) { return; }

        Transform otherAttach = otherPart.GetAttachPoint();

        ForgeRecipe otherRecipe = otherPart.GetCurrentRecipe();
        if (otherRecipe == null) { return; }
        if (otherRecipe.recipePart != connectToPart) { return; }

        SwordPartsController thisPart = GetComponentInParent<SwordPartsController>();
        if (thisPart == null || thisPart == otherPart) { return; }
        if (!(thisPart.currentState is GrindState)) { return; }

        bool thisAlreadyAssembled = IsPartAlreadyAssembled(thisPart);
        bool otherAlreadyAssembled = IsPartAlreadyAssembled(otherPart);

        if (thisAlreadyAssembled && otherAlreadyAssembled) { return; }

        Vector3 connectorPosition = transform.position;
        Quaternion connectorRotation = transform.rotation;

        // Begin one-shot connection.
        isConnecting = true;
        Collider trigger = GetComponent<Collider>();
        if (trigger != null) { trigger.enabled = false; }

        // Cache and remove individual interactables / rigidbodies.
        XRGrabInteractable thisGrab = thisPart.GetComponent<XRGrabInteractable>();
        XRGrabInteractable otherGrab = otherPart.GetComponent<XRGrabInteractable>();
        Rigidbody thisRigidbody = thisPart.GetComponent<Rigidbody>();
        Rigidbody otherRigidbody = otherPart.GetComponent<Rigidbody>();

        if (thisGrab != null) Destroy(thisGrab);
        if (otherGrab != null) Destroy(otherGrab);
        if (thisRigidbody != null) Destroy(thisRigidbody);
        if (otherRigidbody != null) Destroy(otherRigidbody);

        Debug.Log($"Connecting {otherRecipe.recipePart} to {connectToPart}");

        AddToParent(thisPart, otherPart);

        if (otherAlreadyAssembled)
        {
            Transform targetPoint = otherAttach != null ? otherAttach : other.transform;

            AlignAttachPoint(thisPart.transform, transform, targetPoint.position, targetPoint.rotation);
        }
        else
        {
            AlignAttachPoint(otherPart.transform, otherAttach != null ? otherAttach : other.transform, connectorPosition, connectorRotation);
        }

        if (AnvilController.GetSpawnedSwordParts().Contains(thisPart))
        {
            AnvilController.RemoveSpawnedSwordPart(thisPart);
        }

        if (AnvilController.GetSpawnedSwordParts().Contains(otherPart))
        {
            AnvilController.RemoveSpawnedSwordPart(otherPart);
        }

        // Destroy the consumed attach point object (cached earlier)
        if (otherAttach != null)
        {
            Destroy(otherAttach.gameObject);
        }

        // Remove this connector component now that connection is complete.
        Destroy(this);
    }

    private bool IsPartAlreadyAssembled(SwordPartsController part)
    {
        Transform parent = part.transform.parent;

        return parent != null && parent.GetComponentInParent<XRGrabInteractable>() != null;
    }

    private void AlignAttachPoint(Transform partTransform, Transform movingPoint, Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 movingPointLocalPosition = partTransform.InverseTransformPoint(movingPoint.position);
        Quaternion movingPointLocalRotation = Quaternion.Inverse(partTransform.rotation) * movingPoint.rotation;

        partTransform.rotation = targetRotation * Quaternion.Inverse(movingPointLocalRotation);
        partTransform.position = targetPosition - partTransform.rotation * movingPointLocalPosition;
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

        XRGrabInteractable grabInteractable = swordParent.GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = swordParent.gameObject.AddComponent<XRGrabInteractable>();
        }

        // Ensure the sword root has a Rigidbody for XR interaction.
        Rigidbody rootRb = swordParent.GetComponent<Rigidbody>();
        if (rootRb == null)
        {
            rootRb = swordParent.gameObject.AddComponent<Rigidbody>();
            rootRb.isKinematic = true;
            rootRb.useGravity = false;
            rootRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // Use near attachment for the complete assembled sword.
        grabInteractable.farAttachMode = InteractableFarAttachMode.Near;

        if (otherPart.GetCurrentRecipe() != null && otherPart.GetCurrentRecipe().recipePart == ForgePart.LongSwordGuard)
        {
            grabInteractable.attachTransform = otherPart.transform;
        }

        Debug.Log($"Both parts are now children of {swordParent.name}");
    }
}
