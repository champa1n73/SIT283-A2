using UnityEngine;

public interface ISwordPartsStates
{
    public void EnterState(SwordPartsController swordPartsController);
    public void UpdateState(SwordPartsController swordPartsController);
    public void FixedUpdateState(SwordPartsController swordPartsController);
    public void ExitState(SwordPartsController swordPartsController);
}
