using Kickstarter.Bootstrapper;
using Kickstarter.DependencyInjection;
using UnityEngine;

namespace Inferno_Cascade
{
    public class EnemyDeath : MonoBehaviour, IOnDeath
    {
        [Provide] private SceneLoader sceneLoader;
        
        public void OnDeath()
        {
            var enemies = FindObjectsByType<EnemyDeath>(FindObjectsSortMode.None);
            if (enemies.Length == 1)
            {
                Debug.Log("All enemies are dead");
                sceneLoader.LoadSceneGroup("Win");
            }
            Destroy(gameObject);
        }
    }
}
