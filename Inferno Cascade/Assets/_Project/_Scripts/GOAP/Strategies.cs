using UnityEngine;
using UnityEngine.AI;

namespace Inferno_Cascade
{
    public interface IActionStrategy
    {
        public bool CanPerform { get; }
        public bool Complete { get; }

        public void Start()
        {
            // noop
        }

        public void Update(float deltaTime)
        {
            // noop
        }

        public void Stop()
        {
            // noop
        }
    }

    public class IdleStrategy : IActionStrategy
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }

        private CountdownTimer timer;

        public IdleStrategy(float duration)
        {
            timer = new CountdownTimer(duration);
            timer.OnTimerStart += () => Complete = false;
            timer.OnTimerStop += () => Complete = true;
        }

        public void Start()
            => timer.Start();

        public void Update(float deltaTime)
            => timer.Tick(deltaTime);
    }

    public class WanderStrategy : IActionStrategy
    {
        readonly NavMeshAgent agent;
        readonly float wanderRadius;

        public bool CanPerform => !Complete;
        public bool Complete => agent.remainingDistance <= 2f && !agent.pathPending;

        public WanderStrategy(NavMeshAgent agent, float wanderRadius)
        {
            this.agent = agent;
            this.wanderRadius = wanderRadius;
        }

        public void Start()
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 randomDirection = (Random.insideUnitSphere * wanderRadius).With(y: 0);
                NavMeshHit hit;

                if (NavMesh.SamplePosition(agent.transform.position + randomDirection, out hit, wanderRadius, 1))
                {
                    agent.SetDestination(hit.position);
                    return;
                }
            }
        }
    }

    public class MoveStrategy : IActionStrategy
    {
        private readonly NavMeshAgent agent;
        private readonly System.Func<Vector3> destination;

        public bool CanPerform => !Complete;
        public bool Complete => agent.remainingDistance <= 2f && !agent.pathPending;

        public MoveStrategy(NavMeshAgent agent, System.Func<Vector3> destination) 
        {
            this.agent = agent;
            this.destination = destination;
        }

        public void Start()
            => agent.SetDestination(destination());

        public void Stop()
            => agent.ResetPath();
    }

    public class AttackStrategy : IActionStrategy
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }

        public AttackStrategy()
        {
            
        }

        
    }
}
