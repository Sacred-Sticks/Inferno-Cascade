﻿using System;
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

        private Func<GameObject> target;
        private Health targetHealth;
        private GameObject targetGameObject;

        private float damagePerHit = 2;

        public AttackStrategy(Func<GameObject> getTarget)
        {
            target = getTarget;
            // Initialize the health component of the target
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

        public CautiousAttackStrategy(Func<GameObject> getTarget, Health agentHealth)
        {
            target = getTarget;
            this.agentHealth = agentHealth;
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

        public HealStrategy(Func<GameObject> getTarget)
        {
            target = getTarget;
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
        }
    }
}
