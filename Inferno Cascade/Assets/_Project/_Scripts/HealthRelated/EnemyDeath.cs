using Kickstarter.Bootstrapper;
using Kickstarter.DependencyInjection;
using UnityEngine;

namespace Inferno_Cascade
{
    public class EnemyDeath : MonoBehaviour, IOnDeath
    {
        public void OnDeath()
        {
            var enemies = FindObjectsByType<EnemyDeath>(FindObjectsSortMode.None);
            if (enemies.Length == 1)
            {
                Debug.Log("All enemies are dead");
            }
            Destroy(gameObject);
        }
    }
}
