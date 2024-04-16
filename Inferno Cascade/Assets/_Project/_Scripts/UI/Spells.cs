using Kickstarter.DependencyInjection;
using Kickstarter.Observer;
using UnityEngine;
using UnityEngine.UIElements;

namespace Inferno_Cascade
{
    [RequireComponent(typeof(UIDocument))]
    public class Spells : MonoBehaviour, IObserver<SpellManager.CycleNotification>
    {
        [Inject]
        private SpellManager spellManager
        {
            set
            {
                value.AddObserver(this);
            }
        }

        [SerializeField] private StyleSheet stylesheet;
        [SerializeField] private Sprite[] waterSpellSprites;
        [SerializeField] private Sprite[] fireSpellSprites;

        private VisualElement[] waterSpells = new VisualElement[2];
        private VisualElement[] fireSpells = new VisualElement[2];

        #region UnityEvents
        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root?.styleSheets.Add(stylesheet);
            BuildDocument(root);
        }

        private void OnValidate()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root?.styleSheets.Add(stylesheet);
            BuildDocument(root);
        }
        #endregion

        private void BuildDocument(VisualElement root)
        {
            if (root == null)
                return;
            root.Clear();
            root.AddToClassList("root");

            var waterContainer = root.CreateChild<VisualElement>("container");
            waterSpells[0] = waterContainer.CreateChild<VisualElement>("water_jet", "spell", "selected");
            waterSpells[1] = waterContainer.CreateChild<VisualElement>("water_heal", "spell");

            var fireContainer = root.CreateChild<VisualElement>("container");
            fireSpells[0] = fireContainer.CreateChild<VisualElement>("fire_ball", "spell", "selected");
            fireSpells[1] = fireContainer.CreateChild<VisualElement>("fire_javelin", "spell");

            for (int i = 0; i < fireSpells.Length; i++)
            {
                waterSpells[i].style.backgroundImage = waterSpellSprites[i].texture;
                fireSpells[i].style.backgroundImage = fireSpellSprites[i].texture;
            }
        }

        #region Notifications
        public void OnNotify(SpellManager.CycleNotification argument)
        {
            var spells = argument.SpellType switch
            {
                SpellManager.SpellType.Water => waterSpells,
                SpellManager.SpellType.Fire => fireSpells,
                _ => throw new System.NotImplementedException()
            };
            spells[argument.SpellIndex].AddToClassList("selected");
            spells[(argument.SpellIndex + 1) % 2].RemoveFromClassList("selected");
        }
        #endregion
    }
}
