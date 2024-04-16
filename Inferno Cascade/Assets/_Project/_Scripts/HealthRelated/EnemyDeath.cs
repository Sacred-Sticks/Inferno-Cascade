using Kickstarter.Bootstrapper;
using UnityEngine;

namespace Inferno_Cascade
{
    public class EnemyDeath : MonoBehaviour, IOnDeath
    {
        public SceneLoader sceneLoader { private get; set; }
        public void OnDeath()
        {
            Destroy(gameObject);
            var enemies = FindObjectsByType<EnemyDeath>(FindObjectsSortMode.None);
            if (enemies.Length == 0 || (enemies.Length == 1 && enemies[0] == this))
                sceneLoader.LoadSceneGroup("Win");
        }
    }
}
