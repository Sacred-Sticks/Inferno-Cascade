using UnityEngine;
using Kickstarter.Inputs;
using Kickstarter.Observer;

namespace Inferno_Cascade
{
    public class SpellManager : Observable, IInputReceiver
    {
        [Header("Input")]
        [SerializeField] private FloatInput cycleWaterSpellInput;
        [SerializeField] private FloatInput useWaterSpellInput;
        [SerializeField] private FloatInput cycleFireSpellInput;
        [SerializeField] private FloatInput useFireSpellInput;

        private ISpell[] waterSpells;
        private ISpell[] fireSpells;
        private int waterSpellCount;
        private int fireSpellCount;
        private int waterSpellIndex;
        private int fireSpellIndex;

        #region UnityEvents
        private void Start()
        {
            waterSpells = new ISpell[]
            {
                new WaterJet(),
                new ExampleSpell("Heal Spell"),
                new ExampleSpell("Water Shield"),
            };
            waterSpellCount = waterSpells.Length;
            waterSpellIndex = 0;
            fireSpells = new ISpell[]
            {
                new FireBall(),
                new ExampleSpell("Fire Rune"),
                new ExampleSpell("Fire Javelin"),
            };
            fireSpellCount = fireSpells.Length;
            fireSpellIndex = 0;
        }
        #endregion

        #region InputHandler
        public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
        {
            cycleWaterSpellInput.RegisterInput(OnCycleWaterSpellInputChange, playerIdentifier);
            cycleFireSpellInput.RegisterInput(OnCycleFireSpellInputChange, playerIdentifier);
            useWaterSpellInput.RegisterInput(OnUseWaterSpellInputChange, playerIdentifier);
            useFireSpellInput.RegisterInput(OnUseFireSpellInputChange, playerIdentifier);
        }

        public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier)
        {
            cycleWaterSpellInput.DeregisterInput(OnCycleWaterSpellInputChange, playerIdentifier);
            cycleFireSpellInput.DeregisterInput(OnCycleFireSpellInputChange, playerIdentifier);
            useWaterSpellInput.DeregisterInput(OnUseWaterSpellInputChange, playerIdentifier);
            useFireSpellInput.DeregisterInput(OnUseFireSpellInputChange, playerIdentifier);
        }

        private void OnCycleWaterSpellInputChange(float input)
        {
            if (input == 0)
                return;
            waterSpellIndex++;
            if (waterSpellIndex >= waterSpellCount)
                waterSpellIndex = 0;
            NotifyObservers(new CycleNotification(SpellType.Water, waterSpellIndex));
        }

        private void OnCycleFireSpellInputChange(float input)
        {
            if (input == 0)
                return;
            fireSpellIndex++;
            if (fireSpellIndex >= fireSpellCount)
                fireSpellIndex = 0;
            NotifyObservers(new CycleNotification(SpellType.Fire, fireSpellIndex));
        }

        private void OnUseWaterSpellInputChange(float input)
        {
            var spell = waterSpells[waterSpellIndex];
            if (input == 1)
                spell.BeginSpell();
            else if (input == 0)
                spell.EndSpell();
        }

        private void OnUseFireSpellInputChange(float input)
        {
            var spell = fireSpells[fireSpellIndex];
            if (input == 1)
                spell.BeginSpell();
            else if (input == 0)
                spell.EndSpell();
        }
        #endregion

        #region SubTypes
        public enum SpellType
        {
            Water,
            Fire,
        }

        public struct CycleNotification : INotification
        {
            public CycleNotification(SpellType spellType, int spellIndex)
            {
                SpellType = spellType;
                SpellIndex = spellIndex;
            }

            public SpellType SpellType;
            public int SpellIndex;
        }
        #endregion
    }
}
