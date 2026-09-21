using UnityEngine;

public enum ForgePart { LongSwordBlade, LongSwordGuard, LongSwordPommel }

[CreateAssetMenu(fileName = "ForgeRecipe", menuName = "Forge/ForgeRecipe")]
public class ForgeRecipe : ScriptableObject
{
    public ForgePart recipePart;
    public int requiredHammerHits;
    public float requiredQuenchTime;
    public float requiredGrindTime;
    public GameObject finishedPrefab;
}
