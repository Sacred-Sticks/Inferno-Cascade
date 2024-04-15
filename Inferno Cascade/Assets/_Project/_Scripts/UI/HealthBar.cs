using Kickstarter.DependencyInjection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Inferno_Cascade
{
    public class HealthBar : MonoBehaviour, IDependencyProvider
    {
        [Provide] private HealthBar healthBarClass => this;

        [SerializeField] private StyleSheet styleSheet;

        private ProgressBar healthBar;

        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root.styleSheets.Add(styleSheet);

            BuildDocument(root);
        }

        private void OnValidate()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            if (root == null)
                return;

            root.styleSheets.Add(styleSheet);
            BuildDocument(root);
        }

        private void BuildDocument(VisualElement root)
        {
            root.Clear();

            healthBar = root.CreateChild<ProgressBar>();
            healthBar.lowValue = 0;
            healthBar.highValue = 1;
            healthBar.value = 1;
        }

        public void SetHealth(float health, float maxHealth)
        {
            healthBar.value = health / maxHealth;
        }
    }
}
