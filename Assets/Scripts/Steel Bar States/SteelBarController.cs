using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SteelBarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem sparkParticlePrefab;
    [SerializeField] private Slider heatProgressSlider;
    [SerializeField] private RectTransform hotStateLine;
    [SerializeField] private RectTransform progressBar;

    [Header("Components")]
    [SerializeField] private MeshRenderer meshRender;
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip hammerSmashSFX;

    [Header("Recipe Settings")]
    [SerializeField] private ForgeRecipe part;


    [Header("Heatup Settings")]
    [SerializeField] private static float heatUpRate = 10f;
    [SerializeField] private static float heatDownRate = 5f;
    [SerializeField] private float heatDownAfterSeconds = 2f;
    [SerializeField] private bool isInTheForge = false;
    [SerializeField] private float heatUpTimer = 0f;
    [SerializeField] private const float maxHeatProgress = 100f;
    [SerializeField] private const float maxOverheatProgress = 200f;
    [SerializeField] private float heatProgress = 0f;
    private float heatTimer = 0f;

    [Header("Hammering Settings")]
    [SerializeField] private int hammerHits = 0;
    [SerializeField] private bool isOnAnvil = false;

    [Header("States color settings")]
    [SerializeField] private Dictionary<string, Color> stateColor = new Dictionary<string, Color>
    {
        { "Cold", new Color32(74, 74, 74, 255) },
        { "Hot", new Color32(255, 122, 0, 255) },
        { "Overheat", new Color32(255, 0, 0, 255) }
    };

    // SETTERS
    public static void SetHeatUpRate(float rate) => heatUpRate = rate;
    public static void SetHeatDownRate(float rate) => heatDownRate = rate;
    public void SetRecipe(ForgeRecipe recipe) => part = recipe;
    public void SetHammerHits(int hits) => hammerHits = hits;
    public void SetHeatProgress(float progress)
    {
        heatProgress = Mathf.Clamp(progress, 0f, maxOverheatProgress);
        if (heatProgressSlider != null)
        {
            heatProgressSlider.value = heatProgress / maxOverheatProgress;
        }
    }

    public void SetHotStateLine(float target)
    {
        target = Mathf.Clamp01(target);

        float width = progressBar.rect.width;
        float x = (target - 0.5f) * width;

        Vector2 position = hotStateLine.anchoredPosition;
        position.x = x;
        hotStateLine.anchoredPosition = position;
    }

    // GETTERS
    public Dictionary<string, Color> GetStateColor() => stateColor;
    public MeshRenderer GetMeshRenderer() => meshRender;
    public ISteelBarBaseState currentState { get; private set; }
    public ForgeRecipe GetCurrentRecipe() => part;
    public bool IsInTheForge() => isInTheForge;
    public bool IsOnAnvil() => isOnAnvil;
    public ParticleSystem GetSparkParticle() => sparkParticlePrefab;
    public float GetMaxOverheatProgress() => maxOverheatProgress;
    public float GetMaxHeatProgress() => maxHeatProgress;
    public static float GetHeatUpRate() => heatUpRate;
    public static float GetHeatDownRate() => heatDownRate;
    public int GetHammerHits() => hammerHits;


    private void Awake()
    {
        part = part ?? Resources.Load<ForgeRecipe>("ForgeRecipes/Long Sword Blade");
    }

    private void Start()
    {
        // Set the initial state to IdleState
        if (ConfigurationUI.IsInstantHeatingEnabled())
        {
            heatProgress = maxHeatProgress;
            ChangeState(new HotState());
        }
        else
        {
            ChangeState(new ColdState());
        }

        if (ConfigurationUI.IsOneHitHammerEnabled())
        {
            hammerHits = part.requiredHammerHits - 1;
        }
        else
        {
            hammerHits = 0;
        }

        SetHotStateLine(maxHeatProgress / maxOverheatProgress);
        SetHeatProgress(heatProgress);

        currentState?.EnterState(this);
    }

    private void Update()
    {
        HeatDownSteelBar();
        currentState?.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdateState(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Anvil"))
        {
            isOnAnvil = true;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hammer"))
        {
            ContactPoint contact = collision.contacts[0];
            ParticleSystem sparkParticle = GameObject.Instantiate(sparkParticlePrefab, contact.point, Quaternion.LookRotation(-contact.normal));
            sparkParticle.Play();
            SoundManager.PlaySoundAtPosition(hammerSmashSFX, contact.point, 1f);
            DestroyParticle(sparkParticle);
        }

        currentState?.OnCollisionEnter(this, collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        currentState?.OnCollisionStay(this, collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isOnAnvil && collision.gameObject.layer == LayerMask.NameToLayer("Anvil"))
        {
            isOnAnvil = false;
        }
        currentState?.OnCollisionExit(this, collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Heat Collider"))
        {
            isInTheForge = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isInTheForge && other.gameObject.layer == LayerMask.NameToLayer("Heat Collider"))
        {
            isInTheForge = false;
        }
    }

    public void ChangeState(ISteelBarBaseState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }

    public void HeatUpSteelBar()
    {
        heatUpTimer += Time.deltaTime;

        if (heatUpTimer < 1f) { return; }
        heatProgress += heatUpRate;
        heatUpTimer = 0f;
        SetHeatProgress(heatProgress);

        if (heatProgress >= maxOverheatProgress)
        {
            ChangeState(new OverHeatState());
        }
        else if (heatProgress >= maxHeatProgress && heatProgress < maxOverheatProgress)
        {
            ChangeState(new HotState());
        }
    }

    private void HeatDownSteelBar()
    {
        if (!isInTheForge && heatProgress > 0)
        {
            heatTimer += Time.deltaTime;
            if (heatTimer >= heatDownAfterSeconds)
            {
                heatProgress -= heatDownRate;
                SetHeatProgress(heatProgress);
                heatTimer = 0f;
            }

            if (heatProgress < maxHeatProgress && heatProgress > 0)
            {
                if (currentState is ColdState) { return; }
                ChangeState(new ColdState());
            }
        }
    }

    private async void DestroyParticle(ParticleSystem particle)
    {
        if (particle == null) { return; }
        while (particle.IsAlive())
        {
            await Task.Yield();
        }
        GameObject.Destroy(particle.gameObject);
    }

    public void DropHotItem()
    {
        if (ConfigurationUI.IsInvulnerableHandsEnabled()) { return; }
        if (currentState is HotState)
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
