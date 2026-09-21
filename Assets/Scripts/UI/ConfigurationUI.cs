using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationUI : MonoBehaviour
{
    [SerializeField] private List<ForgeRecipe> recipes = new List<ForgeRecipe>();

    [SerializeField] private List<Toggle> toggles = new List<Toggle>();

    private float originalHeatUpRate;
    private float originalHeatDownRate;
    private static bool instantHeatingEnabled = false;
    private static bool oneHitHammerEnabled = false;
    private static bool instantQuenchingEnabled = false;
    private static bool instantGrindingEnabled = false;
    private static bool invulnerableHandsEnabled = false;

    private enum ToggleType
    {
        OneHitHammer,
        InstantHeating,
        InstantQuenching,
        InstantGrinding,
        InvulnerableHands,
    }



    // SETTERS
    public static void SetInstantHeatingEnabled(bool enabled) => instantHeatingEnabled = enabled;
    public static void SetOneHitHammerEnabled(bool enabled) => oneHitHammerEnabled = enabled;
    public static void SetInstantQuenchingEnabled(bool enabled) => instantQuenchingEnabled = enabled;
    public static void SetInstantGrindingEnabled(bool enabled) => instantGrindingEnabled = enabled;
    public static void SetInvulnerableHandsEnabled(bool enabled) => invulnerableHandsEnabled = enabled;

    // GETTERS
    public static bool IsInstantHeatingEnabled() => instantHeatingEnabled;
    public static bool IsOneHitHammerEnabled() => oneHitHammerEnabled;
    public static bool IsInstantQuenchingEnabled() => instantQuenchingEnabled;
    public static bool IsInstantGrindingEnabled() => instantGrindingEnabled;
    public static bool IsInvulnerableHandsEnabled() => invulnerableHandsEnabled;


    private void Awake()
    {
        originalHeatUpRate = SteelBarController.GetHeatUpRate();
        originalHeatDownRate = SteelBarController.GetHeatDownRate();
        recipes = new List<ForgeRecipe>(Resources.LoadAll<ForgeRecipe>("ForgeRecipes"));
        toggles = new List<Toggle>(GetComponentsInChildren<Toggle>());

        toggles[0].onValueChanged.AddListener(OnOneHitHammerToggleChanged);
        toggles[1].onValueChanged.AddListener(OnInstantHeatingToggleChanged);
        toggles[2].onValueChanged.AddListener(OnInstantQuenchingToggleChanged);
        toggles[3].onValueChanged.AddListener(OnInstantGrindingToggleChanged);
        toggles[4].onValueChanged.AddListener(OnInvulnerableHandsToggleChanged);
    }

    private void OnOneHitHammerToggleChanged(bool isOn)
    {
        oneHitHammerEnabled = isOn;
        MakingSwordSettings(ObjectSpawner.GetSpawnedSteelBars(), ToggleType.OneHitHammer, isOn);
    }

    private void OnInstantHeatingToggleChanged(bool isOn)
    {
        instantHeatingEnabled = isOn;
        MakingSwordSettings(ObjectSpawner.GetSpawnedSteelBars(), ToggleType.InstantHeating, isOn);
    }


    private void OnInstantQuenchingToggleChanged(bool isOn)
    {
        instantQuenchingEnabled = isOn;
        MakingSwordSettings(AnvilController.GetSpawnedSwordParts(), ToggleType.InstantQuenching, isOn);
    }


    private void OnInstantGrindingToggleChanged(bool isOn)
    {
        instantGrindingEnabled = isOn;
        MakingSwordSettings(AnvilController.GetSpawnedSwordParts(), ToggleType.InstantGrinding, isOn);
    }

    private void OnInvulnerableHandsToggleChanged(bool isOn)
    {
        invulnerableHandsEnabled = isOn;
    }

    private void MakingSwordSettings(List<SteelBarController> spawnedSteelBars, ToggleType toggleType, bool isEnabled)
    {
        switch (toggleType)
        {
            case ToggleType.OneHitHammer:
                foreach (SteelBarController steelBar in spawnedSteelBars)
                {
                    if (steelBar.GetCurrentRecipe() == null) { continue; }

                    if (isEnabled)
                    {
                        steelBar.SetHammerHits(steelBar.GetCurrentRecipe().requiredHammerHits - 1);
                    }
                    else
                    {
                        steelBar.SetHammerHits(0);
                    }
                }
                break;
            case ToggleType.InstantHeating:
                if (isEnabled)
                {
                    SteelBarController.SetHeatUpRate(0f);
                    SteelBarController.SetHeatDownRate(0f);

                    if (ObjectSpawner.GetSpawnedSteelBars() == null) { return; }

                    foreach (SteelBarController steelBar in ObjectSpawner.GetSpawnedSteelBars()) 
                    { 
                        steelBar.SetHeatProgress(steelBar.GetMaxHeatProgress()); 
                        steelBar.ChangeState(new HotState()); 
                    }
                }
                else
                {
                    SteelBarController.SetHeatUpRate(originalHeatUpRate); 
                    SteelBarController.SetHeatDownRate(originalHeatDownRate);
                }
                break;
            

        }
    }

    private void MakingSwordSettings(List<SwordPartsController> spawnedSwordParts, ToggleType toggleType, bool isEnabled)
    {
        switch (toggleType)
        {
            case ToggleType.InstantQuenching:
                foreach (SwordPartsController swordPart in spawnedSwordParts)
                {
                    if (swordPart.GetCurrentRecipe() == null) { continue; }

                    if (isEnabled)
                    {
                        swordPart.SetQuenchTime(swordPart.GetCurrentRecipe().requiredQuenchTime);
                    }
                    else
                    {
                        swordPart.SetQuenchTime(0f);
                    }
                }
                break;
            case ToggleType.InstantGrinding:
                foreach (SwordPartsController swordPart in spawnedSwordParts)
                {
                    if (swordPart.GetCurrentRecipe() == null) { continue; }

                    if (isEnabled)
                    {
                        swordPart.SetGrindingTime(swordPart.GetCurrentRecipe().requiredGrindTime);
                    }
                    else
                    {
                        swordPart.SetGrindingTime(0f);
                    }
                }
                break;
        }
    }
}
