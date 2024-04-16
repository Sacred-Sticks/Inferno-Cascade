using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Inferno_Cascade
{
    public class IdleStrategy : IActionStrategy
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }

        private CountdownTimer timer;
        private AnimationController animationController;

        public IdleStrategy(float duration, AnimationController animationController)
        {
            timer = new CountdownTimer(duration);
            timer.OnTimerStart += () => Complete = false;
            timer.OnTimerStop += () => Complete = true;
            this.animationController = animationController;
        }

        public void Start()
        {
            timer.Start();
            animationController.Locomotion();
            animationController.SetDirection(Vector2.zero);
        }

        public void Update(float deltaTime)
            => timer.Tick(deltaTime);
    }

    public class WanderStrategy : IActionStrategy
    {
        readonly NavMeshAgent agent;
        readonly float wanderRadius;

        public bool CanPerform => !Complete;
        public bool Complete => agent.remainingDistance <= 2f && !agent.pathPending;

        private AnimationController animationController;

        public WanderStrategy(NavMeshAgent agent, float wanderRadius, AnimationController animationController)
        {
            this.agent = agent;
            this.wanderRadius = wanderRadius;
            this.animationController = animationController;
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
            animationController.Locomotion();
            animationController.SetDirection(Vector2.zero);
        }
    }

    public class MoveStrategy : IActionStrategy
    {
        private readonly NavMeshAgent agent;
        private readonly Func<Vector3> destination;

        public bool CanPerform => !Complete;
        public bool Complete => agent.remainingDistance <= 2f && !agent.pathPending;

        private AnimationController animationController;

        public MoveStrategy(NavMeshAgent agent, Func<Vector3> destination, AnimationController animationController) 
        {
            this.agent = agent;
            this.destination = destination;
            this.animationController = animationController;
        }

        public void Start()
        {
            agent.SetDestination(destination());
            animationController.Locomotion();
            animationController.SetDirection(new Vector2(0, 1));
        }

        public void Stop()
            => agent.ResetPath();
    }

    public class AttackStrategy : IActionStrategy
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }

        private Func<GameObject> target;
        private Health targetHealth;
        private GameObject targetGameObject;

        private float damagePerHit = 2;

        private AnimationController animationController;

        public AttackStrategy(Func<GameObject> getTarget, AnimationController animationController)
        {
            target = getTarget;
            this.animationController = animationController;
        }

        private CountdownTimer timer;

        public void Start()
        {
            timer = new CountdownTimer(2);
            timer.OnTimerStop += () =>
            {
                AttackTarget();
                timer.Start();
            };
            timer.Start();

            targetHealth = target().GetComponent<Health>();
            targetGameObject = target();
        }

        public void Update(float deltaTime)
        {
            timer.Tick(deltaTime);

            if (targetGameObject == null)
                Complete = true;
        }

        private void AttackTarget()
        {
            targetHealth.ChangeHealth(-damagePerHit);
            animationController.Attack();
            animationController.SetDirection(Vector2.zero);
        }
    }

    public class CautiousAttackStrategy : IActionStrategy
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }

        private Func<GameObject> target;
        private Health agentHealth;
        private Health targetHealth;
        private GameObject targetGameObject;

        private float damagePerHit = 10;

        private float healthThreshold = 0.25f;
        private AnimationController animationController;

        public CautiousAttackStrategy(Func<GameObject> getTarget, Health agentHealth, AnimationController animationController)
        {
            target = getTarget;
            this.agentHealth = agentHealth;
            this.animationController = animationController;
        }

        private CountdownTimer timer;

        public void Start()
        {
            timer = new CountdownTimer(0.5f);
            timer.OnTimerStop += () =>
            {
                AttackTarget();
                timer.Start();
            };
            timer.Start();

            targetHealth = target().GetComponent<Health>();
            targetGameObject = target();
        }

        public void Update(float deltaTime)
        {
            timer.Tick(deltaTime);

            if (targetGameObject == null || agentHealth.HealthPercentage < healthThreshold)
                Complete = true;
        }

        private void AttackTarget()
        {
            targetHealth.ChangeHealth(-damagePerHit);
            animationController.Attack();
            animationController.SetDirection(Vector2.zero);
        }
    }

    public class HealStrategy : IActionStrategy
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }

        private Func<GameObject> target;
        private Health targetHealth;
        private GameObject targetObject;

        private float healthyThreshold = 0.75f;
        private float healAmount = 5;
        private AnimationController animationController;

        public HealStrategy(Func<GameObject> getTarget, AnimationController animationController)
        {
            target = getTarget;
            this.animationController = animationController;
        }

        private CountdownTimer timer;

        public void Start()
        {
            Complete = false;
            timer = new CountdownTimer(0.5f);
            timer.OnTimerStop += () =>
            {
                HealTarget();
                timer.Start();
            };
            timer.Start();

            targetHealth = target().GetComponent<Health>();
            targetObject = target();
        }

        public void Update(float deltaTime)
        {
            timer.Tick(deltaTime);

            if (targetObject == null || targetHealth.HealthPercentage >= healthyThreshold)
                Complete = true;
        }

        private void HealTarget()
        {
            targetHealth.ChangeHealth(healAmount);
            animationController.Attack();
            animationController.SetDirection(Vector2.zero);
        }
    }
}
