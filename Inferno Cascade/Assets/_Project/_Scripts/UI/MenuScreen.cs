using Kickstarter.Bootstrapper;
using Kickstarter.DependencyInjection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Inferno_Cascade
{
    [RequireComponent(typeof(UIDocument))]
    public class MenuScreen : MonoBehaviour
    {
        [Inject] private SceneLoader sceneLoader;

        [SerializeField] private StyleSheet styleSheet;
        [Header("")]
        [SerializeField] private ButtonType buttonType;
        [SerializeField] private string buttonText;
        [SerializeField] private string labelText;

        #region UnityEvents
        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root.styleSheets.Add(styleSheet);
            BuildDocument(root);
        }

        private void OnValidate()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root?.styleSheets.Add(styleSheet);
            BuildDocument(root);
        }
        #endregion

        private void BuildDocument(VisualElement root)
        {
            if (root == null) 
                return;
            root.Clear();

            var container = root.CreateChild<VisualElement>("container");
            container.CreateChild<Label>("win_label").text = labelText;
            var button = container.CreateChild<Button>("main_menu_button");
            button.text = buttonText;
            System.Action action = buttonType switch
            {
                ButtonType.MainMenu => () => sceneLoader.LoadSceneGroup("Main Menu"),
                ButtonType.Start => () => sceneLoader.LoadSceneGroup("Level"),
                _ => null
            };
            button.clickable.clicked += action;
            ;
        }

        private enum ButtonType
        {
            MainMenu,
            Start
        }
    }
}
