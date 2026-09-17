using System.Collections.Generic;
using UnityEngine;

public class SteelBarController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MeshRenderer meshRender;


    // Forge Recipes
    public ForgeRecipe part;


    [Header("Heatup Settings")]
    public bool isInTheForge = false;
    public float heatUpTimer = 0f;
    public const float maxHeatProgress = 100f;
    public const float maxOverheatProgress = 200f;
    public float heatProgress { get; set; } = 0f;

    [Header("Hammering Settings")]
    public int hammerHits = 0;

    [Header("States color settings")]
    [SerializeField] private Dictionary<string, Color> stateColor = new Dictionary<string, Color>
    {
        { "Cold", new Color32(74, 74, 74, 255) },
        { "Hot", new Color32(255, 122, 0, 255) },
        { "Overheat", new Color32(255, 0, 0, 255) }
    };


    // GETTERS
    public Dictionary<string, Color> GetStateColor() => stateColor;
    public MeshRenderer GetMeshRenderer() => meshRender;
    public ISteelBarBaseState currentState { get; private set; }



    private void Start()
    {
        // Set the initial state to IdleState
        currentState = new ColdState();
        currentState?.EnterState(this);
    }

    private void Update()
    {
        currentState?.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdateState(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState?.OnCollisionEnter(this, collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Heat Collider"))
        {
            isInTheForge = true;
        }
        currentState?.OnTriggerEnter(this, other);
    }

    private void OnTriggerStay(Collider other)
    {
        currentState?.OntriggerStay(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isInTheForge && other.gameObject.layer == LayerMask.NameToLayer("Heat Collider"))
        {
            isInTheForge = false;
        }
        currentState?.OnTriggerExit(this, other);
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
        heatProgress += 10f;
        heatUpTimer = 0f;

        if (heatProgress >= maxOverheatProgress)
        {
            ChangeState(new OverHeatState());
        }
        else if (heatProgress >= maxHeatProgress && heatProgress < maxOverheatProgress)
        {
            ChangeState(new HotState());
        }
    }
}
