using Kickstarter.Bootstrapper;
using Kickstarter.DependencyInjection;
using UnityEngine;

namespace Inferno_Cascade
{
    public class PlayerDeath : MonoBehaviour, IOnDeath
    {
        [Inject] private SceneLoader sceneLoader;

        public void OnDeath()
        {
            sceneLoader.LoadSceneGroup("Lose");
        }
    }
}
