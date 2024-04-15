using Kickstarter.DependencyInjection;
using Kickstarter.Observer;
using UnityEngine;
using UnityEngine.UIElements;

namespace Inferno_Cascade
{
    [RequireComponent(typeof(UIDocument))]
    public class ManaBar : MonoBehaviour, IObserver<SpellManager.ManaNotification>
    {
        [Inject] private SpellManager spellManager
        {
            set
            {
                value.AddObserver(this);
            }
        }
        [SerializeField] private StyleSheet stylesheet;
        [SerializeField] private SpellManager.SpellType spellType;

        private ProgressBar progressBar;

        #region UnityEvents
        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root.styleSheets.Add(stylesheet);

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
            var container = root.CreateChild<VisualElement>("container");

            string manaBarClass = spellType switch
            {
                SpellManager.SpellType.Fire => "fire_mana",
                SpellManager.SpellType.Water => "water_mana",
                _ => throw new System.NotImplementedException()
            };

            progressBar = container.CreateChild<ProgressBar>(manaBarClass);
            progressBar.highValue = 100;
            progressBar.lowValue = 0;
            progressBar.value = 100;
        }

        #region Observations
        public void OnNotify(SpellManager.ManaNotification argument)
        {
            if (argument.SpellType != spellType)
                return;

            progressBar.value = argument.Mana;
        }
        #endregion
    }
}
