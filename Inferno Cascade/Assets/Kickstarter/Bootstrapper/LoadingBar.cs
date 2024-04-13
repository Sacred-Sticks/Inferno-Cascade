using UnityEngine;
using UnityEngine.UIElements;

namespace Kickstarter.Bootstrapper
{
    public interface ILoadingBar
    {
        public void SetProgress(float progress);
        public void Enable(bool enable = true);
    }

    [RequireComponent(typeof(UIDocument))]
    public class LoadingBar : MonoBehaviour, ILoadingBar
    {
        [SerializeField] private StyleSheet styleSheet;

        private VisualElement backdrop;
        private ProgressBar _loadingBar;

        private const float initialValue = 0;
        private const float targetValue = 1;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root.styleSheets.Add(styleSheet);
            BuildLoadingBar(root);
        }

        private void BuildLoadingBar(VisualElement root)
        {
            backdrop = root.CreateChild<VisualElement>("loading_backdrop");
            _loadingBar = backdrop.CreateChild<ProgressBar>();
            _loadingBar.value = initialValue;
            _loadingBar.lowValue = initialValue;
            _loadingBar.highValue = targetValue;
        }

        public void SetProgress(float progress)
        {
            _loadingBar.value = progress;
        }

        public void Enable(bool enable = true)
        {
            var display = enable ? DisplayStyle.Flex : DisplayStyle.None;
            backdrop.style.display = display;
            _loadingBar.style.display = display;
        }
    }
}

