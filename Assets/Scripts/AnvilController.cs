using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnvilController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Text descText;
    [SerializeField] private Dropdown recipeDropdown;

    [Header("Audio Settings")]

    [SerializeField] private AudioClip hammerSmashSFX;
    [SerializeField] private Rigidbody rb;



    [SerializeField] private Dictionary<int, ForgeRecipe> recipes = new Dictionary<int, ForgeRecipe>();
    private ForgeRecipe selectedRecipe;
    private SteelBarController currentSteelBar;
    private static List<SwordPartsController> spawnedSwordParts = new List<SwordPartsController>();

    // SETTERS
    public void SetDescText(string text) => descText.text = text;
    public Transform GetSpawnPoint() => spawnPoint;
    public static void AddSpawnedSwordPart(SwordPartsController swordPart) => spawnedSwordParts.Add(swordPart);
    public static void RemoveSpawnedSwordPart(SwordPartsController swordPart) => spawnedSwordParts.Remove(swordPart);

    // GETTERS
    public static List<SwordPartsController> GetSpawnedSwordParts() => spawnedSwordParts;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ForgeRecipe[] loadedRecipes = Resources.LoadAll<ForgeRecipe>("ForgeRecipes");
        for (int i = 0; i < loadedRecipes.Length; i++)
        {
            recipes.Add(i, loadedRecipes[i]);
            recipeDropdown.options.Add(new Dropdown.OptionData(loadedRecipes[i].name.ToString()));
        }
        recipeDropdown.RefreshShownValue();

        recipeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void Start()
    {
        selectedRecipe = recipes[recipeDropdown.value];
        descText.text = "Place a steel bar on the anvil to see its recipe.";
    }

    private void Update()
    {
        if (currentSteelBar == null) { return; }
        if (currentSteelBar.IsOnAnvil())
        {
            UpdateRecipe();
        }
    }

    private void OnDropdownValueChanged(int value)
    {
        selectedRecipe = recipes[value];
        UpdateRecipe();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hammer"))
        {
            ContactPoint contact = collision.contacts[0];
            SoundManager.PlaySoundAtPosition(hammerSmashSFX, contact.point, 1f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Steel Bar"))
        {
            Debug.Log("Steel bar is on the anvil.");
            SteelBarController steelBar = collision.gameObject.GetComponentInParent<SteelBarController>();

            if (steelBar == null) { return; }

            currentSteelBar = steelBar;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Steel Bar"))
        {
            SteelBarController steelBar = collision.gameObject.GetComponentInParent<SteelBarController>();

            if (currentSteelBar == steelBar)
            {
                currentSteelBar = null;
                descText.text = "Place a steel bar on the anvil to see its recipe.";
            }
        }
    }

    private void UpdateRecipe()
    {
        if (currentSteelBar == null || selectedRecipe == null) { return; }

        if (currentSteelBar.GetCurrentRecipe() != selectedRecipe)
        {
            currentSteelBar.SetRecipe(selectedRecipe);
        }

        if (currentSteelBar.GetCurrentRecipe() != null)
        {
            if (currentSteelBar.currentState is HotState)
            {
                ForgePart recipe = currentSteelBar.GetCurrentRecipe().recipePart;
                descText.text = $"{recipe}";
            }
            else if (currentSteelBar.currentState is ColdState)
            {
                descText.text = "Put the steel bar in the forge to heat it up.";
            }
            else if (currentSteelBar.currentState is OverHeatState)
            {
                descText.text = "The steel bar is overheated! Wait for it to cool down.";
            }
        }
    }
}
