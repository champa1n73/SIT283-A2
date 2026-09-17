using System.Collections.Generic;
using UnityEngine;

public class SwordPartsController : MonoBehaviour
{
    public ForgeRecipe part;
    public ISwordPartsStates currentState { get; private set; }
    public Transform attachPoint;

    [SerializeField]
    private Dictionary<string, Color> stateColor = new Dictionary<string, Color>
    {
        { "Hot", new Color32(255, 122, 0, 255) },
        { "Quenched", new Color32(58, 58, 58, 255) },
        { "Grinded", new Color32(192, 192, 192, 255) }
    };

    public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

    public bool isInQuenchTank = false;
    public bool isGrinding = false;
    public int grindingAmount = 0;

    // GETTERS
    public Dictionary<string, Color> GetStateColor() => stateColor;


    private void Start()
    {
        meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        currentState = new AfterForgeState();
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
}
