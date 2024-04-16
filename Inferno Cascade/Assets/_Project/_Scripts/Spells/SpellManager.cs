using UnityEngine;
using Kickstarter.Inputs;
using Kickstarter.Observer;
using Kickstarter.DependencyInjection;

namespace Inferno_Cascade
{
    public class SpellManager : Observable, IInputReceiver, IDependencyProvider
    {
        [Provide] private SpellManager spellManager => this;

        [Header("Input")]
        [SerializeField] private FloatInput cycleWaterSpellInput;
        [SerializeField] private FloatInput useWaterSpellInput;
        [SerializeField] private FloatInput cycleFireSpellInput;
        [SerializeField] private FloatInput useFireSpellInput;
        [Header("Spells")]
        [SerializeField] private float WaterManaGainPerSecond = 5;
        [SerializeField] private float FireManaGainPerSecond = 5;

        private ISpell[] waterSpells;
        private ISpell[] fireSpells;
        private float waterMana = 100;
        private float fireMana = 100;
        private float WaterMana
        {
            get => waterMana;
            set 
            {
                waterMana = value;
                NotifyObservers(new ManaNotification(SpellType.Water, waterMana));
            }
        }
        private float FireMana
        {
            get => fireMana;
            set
            {
                fireMana = value;
                NotifyObservers(new ManaNotification(SpellType.Fire, fireMana));
            }
        }
        private int waterSpellCount;
        private int fireSpellCount;
        private int waterSpellIndex;
        private int fireSpellIndex;

        private AnimationController animationController;

        #region UnityEvents
        private void Start()
        {
            waterSpells = new ISpell[]
            {
                new ProjectileSpell("Prefabs/SpellPrefabs/WaterJet", 2.5f),
                new HealSpell(10, 5, 10),
            };
            waterSpellCount = waterSpells.Length;
            waterSpellIndex = 0;
            fireSpells = new ISpell[]
            {
                new ProjectileSpell("Prefabs/SpellPrefabs/FireBall", 5),
                new ProjectileSpell("Prefabs/SpellPrefabs/FireJavelin", 10),
            };
            fireSpellCount = fireSpells.Length;
            fireSpellIndex = 0;
            animationController = GetComponent<AnimationController>();
        }

        private void Update()
        {
            if (WaterMana < 100 || FireMana < 100)
                RegenerateMana();
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
            switch (input)
            {
                case 1:
                    if (WaterMana < spell.ManaCost)
                        return;
                    spell.BeginSpell();
                    AdjustMana(SpellType.Water, -spell.ManaCost);
                    animationController.Attack();
                    break;
                case 0:
                    spell.EndSpell();
                    break;
            }
        }

        private void OnUseFireSpellInputChange(float input)
        {
            var spell = fireSpells[fireSpellIndex];
            switch (input)
            {
                case 1:
                    if (FireMana < spell.ManaCost)
                        return;
                    spell.BeginSpell();
                    AdjustMana(SpellType.Fire, -spell.ManaCost);
                    animationController.Attack();
                    break;
                case 0:
                    spell.EndSpell();
                    break;
            }
        }
        #endregion

        #region Mana
        private void RegenerateMana() 
        {
            AdjustMana(SpellType.Water, WaterManaGainPerSecond * Time.deltaTime);
            AdjustMana(SpellType.Fire, FireManaGainPerSecond * Time.deltaTime);
        }

        private void AdjustMana(SpellType spellType, float manaAdjustment)
        {
            switch (spellType)
            {
                case SpellType.Water:
                    WaterMana += manaAdjustment;
                    WaterMana = Mathf.Clamp(WaterMana, 0, 100);
                    break;
                case SpellType.Fire:
                    FireMana += manaAdjustment;
                    FireMana = Mathf.Clamp(FireMana, 0, 100);
                    break;
            }
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

        public struct ManaNotification : INotification
        {
            public ManaNotification(SpellType spellType, float mana)
            {
                SpellType = spellType;
                Mana = mana;
            }

            public readonly SpellType SpellType;
            public readonly float Mana;
        }
        #endregion
    }
}
