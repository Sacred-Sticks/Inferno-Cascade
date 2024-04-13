using UnityEngine;
using Kickstarter.Inputs;

public class SpellManager : MonoBehaviour, IInputReceiver
{
    [Header("Input")]
    [SerializeField] private FloatInput cycleWaterSpellInput;
    [SerializeField] private FloatInput useWaterSpellInput;
    [SerializeField] private FloatInput cycleFireSpellInput;
    [SerializeField] private FloatInput useFireSpellInput;

    #region InputHandler
    public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        cycleWaterSpellInput.RegisterInput(OnCycleWaterSpellInputChange, playerIdentifier);
        cycleFireSpellInput.RegisterInput(OnCycleFireSpellInputChange, playerIdentifier);
    }

    public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        cycleWaterSpellInput.DeregisterInput(OnCycleWaterSpellInputChange, playerIdentifier);
        cycleFireSpellInput.DeregisterInput(OnCycleFireSpellInputChange, playerIdentifier);
    }

    private void OnCycleWaterSpellInputChange(float input)
    {
        if (input == 0)
            return;
        Debug.Log("Water spell pressed");
    }
    
    private void OnCycleFireSpellInputChange(float input)
    {
        if (input == 0)
            return;
        Debug.Log("Fire spell pressed");
    }

    public void OnUseWaterSpellInput(float input)
    {
        if (input == 0)
            return;
        Debug.Log("Water spell used");
    }

    public void OnUseFireSpellInput(float input)
    {
        if (input == 0)
            return;
        Debug.Log("Fire spell used");
    }
    #endregion
}
