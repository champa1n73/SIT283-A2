using UnityEngine;
using UnityEngine.InputSystem;

public class TongsController : MonoBehaviour
{
    [SerializeField] private AudioClip tongsGrabSound;
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private Transform triggerBox;
    private InputAction tongsAction;
    [SerializeField] private bool isHoldingTongs = false;
    private Rigidbody heldSteelBar;
    private bool grabSoundPlayed = false;

    public void SetIsHoldingTongs(bool value)
    {
        isHoldingTongs = value;
    }

    private void OnEnable()
    {
        inputActionAsset.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActionAsset.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        tongsAction = InputSystem.actions.FindAction("Tongs");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Steel Bar")
            && other.gameObject.layer != LayerMask.NameToLayer("Sword Parts")
            ) { return; }

        Rigidbody steelBarRigidbody = other.GetComponentInParent<Rigidbody>();
        if (steelBarRigidbody == null) { return; }

        if (tongsAction.IsPressed() && isHoldingTongs)
        {
            heldSteelBar = steelBarRigidbody;

            heldSteelBar.useGravity = false;
            heldSteelBar.isKinematic = true;

            heldSteelBar.transform.root.position = triggerBox.position;
            if (!grabSoundPlayed)
            {
                SoundManager.PlaySoundAtPosition(tongsGrabSound, triggerBox.position, 1f);
                grabSoundPlayed = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (heldSteelBar == null) { return; }

        if (tongsAction.IsPressed() && isHoldingTongs)
        {
            heldSteelBar.useGravity = false;
            heldSteelBar.isKinematic = true;

            heldSteelBar.transform.root.position = triggerBox.position;
        }
        else
        {
            ReleaseSteelBar();
        }
    }

    private void ReleaseSteelBar()
    {
        if (heldSteelBar == null) { return; }

        heldSteelBar.useGravity = true;
        heldSteelBar.isKinematic = false;

        heldSteelBar = null;
        grabSoundPlayed = false;
    }
}
