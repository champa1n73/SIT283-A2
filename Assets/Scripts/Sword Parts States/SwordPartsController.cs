using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SwordPartsController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] XRGrabInteractable grabInteractable;
    [Header("Sound Settings")]
    [SerializeField] private AudioSource grindingAudioSrc;
    [SerializeField] private AudioSource quenchAudioSrc;
    [SerializeField] private AudioClip quenchAudioClip;
    [SerializeField] private AudioClip grindingAudioClip;

    [Header("Particle Prefabs")]
    [SerializeField] private ParticleSystem sparkParticlePrefab;
    [SerializeField] private ParticleSystem steamParticlePrefab;

    [Header("Recipe Settings")]
    [SerializeField] private ForgeRecipe part;

    [Header("Quenching Settings")]
    [SerializeField] private float quenchTime = 0f;
    [Header("Grinding Settings")]
    [SerializeField] private float grindingTime = 0f;

    [Header("Attach Point")]
    [SerializeField] private Transform attachPoint;

    public ISwordPartsStates currentState { get; private set; }

    [SerializeField]
    private Dictionary<string, Color> stateColor = new Dictionary<string, Color>
    {
        { "Hot", new Color32(255, 122, 0, 255) },
        { "Quenched", new Color32(58, 58, 58, 255) },
        { "Grinded", new Color32(192, 192, 192, 255) }
    };

    [Header("References")]
    [SerializeField] private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

    [SerializeField] private bool isInQuenchTank = false;
    [SerializeField] private bool isGrinding = false;

    // SETTERS
    public void SetRecipe(ForgeRecipe recipe) => part = recipe;
    public void SetQuenchTime(float time) => quenchTime = time;
    public void SetGrindingTime(float time) => grindingTime = time;

    // GETTERS
    public Dictionary<string, Color> GetStateColor() => stateColor;
    public List<MeshRenderer> GetMeshRenderers() => meshRenderers;
    public bool IsInQuenchTank() => isInQuenchTank;
    public bool IsGrinding() => isGrinding;
    public ForgeRecipe GetCurrentRecipe() => part;
    public Transform GetAttachPoint() => attachPoint;
    public ParticleSystem GetSteamParticle() => steamParticlePrefab;
    public AudioSource GetQuenchAudioSrc() => quenchAudioSrc;
    public AudioSource GetGrindingAudioSrc() => grindingAudioSrc;
    public AudioClip GetQuenchAudioClip() => quenchAudioClip;
    public AudioClip GetGrindingAudioClip() => grindingAudioClip;
    public float GetQuenchTime() => quenchTime;
    public float GetGrindingTime() => grindingTime;


    private void Start()
    {
        if (ConfigurationUI.IsInstantQuenchingEnabled())
        {
            quenchTime = part.requiredQuenchTime;
        }

        if (ConfigurationUI.IsInstantGrindingEnabled())
        {
            grindingTime = part.requiredGrindTime;
        }

        meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        currentState = new AfterForgeState();
        currentState?.EnterState(this);
    }

    private void Update()
    {
        PlayGrindingSFX();
        currentState?.UpdateState(this);
    }

    private void PlayGrindingSFX()
    {
        if (isGrinding)
        {
            if (grindingAudioSrc.isPlaying) { return; }
            grindingAudioSrc.clip = grindingAudioClip;
            grindingAudioSrc.Play();
        }
        else
        {
            if (!grindingAudioSrc.isPlaying) { return; }
            grindingAudioSrc.Stop();
        }
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdateState(this);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Grinding Wheel"))
        {
            ContactPoint contact = collision.contacts[0];
            ParticleSystem sparkParticle = Instantiate(sparkParticlePrefab, contact.point, Quaternion.LookRotation(-contact.normal));
            sparkParticle.Play();
            DestroyParticle(sparkParticle);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Quench Tank"))
        {
            isInQuenchTank = true;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Grinding Wheel"))
        {
            isGrinding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isInQuenchTank && other.gameObject.layer == LayerMask.NameToLayer("Quench Tank"))
        {
            isInQuenchTank = false;
        }

        if (isGrinding && other.gameObject.layer == LayerMask.NameToLayer("Grinding Wheel"))
        {
            isGrinding = false;
        }
    }

    public void ChangeState(ISwordPartsStates newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }

    private async void DestroyParticle(ParticleSystem particle)
    {
        while (particle.IsAlive())
        {
            await Task.Yield();
        }
        Destroy(particle.gameObject);
    }

    public void DropHotItem()
    {
        if (ConfigurationUI.IsInvulnerableHandsEnabled()) { return; }
        if (currentState is AfterForgeState)
        {
            DropAfterGrab();
        }
    }

    private async void DropAfterGrab()
    {
        await Awaitable.WaitForSecondsAsync(0.1f);
        if (grabInteractable.isSelected)
        {
            grabInteractable.interactionManager.SelectExit(grabInteractable.firstInteractorSelecting, grabInteractable);
        }
    }
}
