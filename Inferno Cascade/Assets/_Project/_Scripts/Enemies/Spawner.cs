using System.Collections;
using UnityEngine;

namespace Inferno_Cascade
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private Transform[] transforms;

        private CountdownTimer timer;
        
        private void Start()
        {
            timer = new CountdownTimer(60);
            timer.OnTimerStop += () =>
            {
                StartCoroutine(SpawnEnemies());
                timer.Start();
            };
            StartCoroutine(SpawnEnemies());
            timer.Start();
        }

        private void Update()
            => timer.Tick(Time.deltaTime);

        private IEnumerator SpawnEnemies()
        {
            const int enemySpawnCount = 2;
            const float spawnRate = 0.25f;
            var delay = new WaitForSeconds(1 / spawnRate);

            for (int i = 0; i < enemySpawnCount; i++)
            {
                var go = Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position, transform.rotation, transform);
                go.transform.parent = null;
                go.GetComponent<HealingEnemy>()?.SafePositions.AddRange(transforms);
                yield return delay;
            }
        }
    }
}
