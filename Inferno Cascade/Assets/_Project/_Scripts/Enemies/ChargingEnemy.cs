using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Inferno_Cascade
{
    [SelectionBase]
    public class ChargingEnemy : GoapAgent
    {
        [Header("Sensors")]
        [SerializeField] private Sensor chaseSensor;
        [SerializeField] private Sensor attackSensor;

        private Health health;
        private AnimationController animationController;

        #region UnityEvents
        protected override void Start()
        {
            health = GetComponent<Health>();
            animationController = GetComponent<AnimationController>();
            base.Start();
        }

        private void OnEnable()
        {
            chaseSensor.OnTargetChanged += HandleTargetChange;
            attackSensor.OnTargetChanged += HandleTargetChange;
        }

        private void OnDisable()
        {
            chaseSensor.OnTargetChanged -= HandleTargetChange;
            attackSensor.OnTargetChanged -= HandleTargetChange;
        }
        #endregion

        #region GOAP
        protected override void SetupBeliefs()
        {
            beliefs = new Dictionary<string, AgentBelief>();
            var factory = new BeliefFactory(this, beliefs);

            factory.AddBelief("Nothing", () => false);
            factory.AddBelief("AgentIdle", () => !navMeshAgent.hasPath);
            factory.AddBelief("AgentMoving", () => navMeshAgent.hasPath);

            factory.AddBelief("Healthy", () => health.HealthPercentage > 0.75f);
            factory.AddBelief("Hurt", () => health.HealthPercentage < 0.25f);

            factory.AddSensorBelief("PlayerInChaseRange", chaseSensor);
            factory.AddSensorBelief("PlayerInAttackRange", attackSensor);
            factory.AddBelief("AttackingPlayer", () => false);
        }

        protected override void SetupActions()
        {
            actions = new HashSet<AgentAction>();

            actions.Add(new AgentAction.Builder("No Movement")
                .WithStrategy(new IdleStrategy(5, animationController))
                .AddEffect(beliefs["Nothing"])
                .Build());

            actions.Add(new AgentAction.Builder("Move to random point within a radius")
                .WithStrategy(new WanderStrategy(navMeshAgent, 10, animationController))
                .WithCost(2)
                .AddEffect(beliefs["AgentMoving"])
                .Build());

            actions.Add(new AgentAction.Builder("Chase after Player")
                .WithStrategy(new MoveStrategy(navMeshAgent, () => beliefs["PlayerInChaseRange"].Location, animationController))
                .AddPrecondition(beliefs["PlayerInChaseRange"])
                .AddEffect(beliefs["PlayerInAttackRange"])
                .Build());

            actions.Add(new AgentAction.Builder("Attack Player")
                .WithStrategy(new AttackStrategy(() => attackSensor.Target, animationController))
                .AddPrecondition(beliefs["PlayerInAttackRange"])
                .AddEffect(beliefs["AttackingPlayer"])
                .Build());
        }

        protected override void SetupGoals()
        {
            goals = new HashSet<AgentGoal>();

            goals.Add(new AgentGoal.Builder("Don't Move")
                 .WithPriority(1)
                 .WithDesiredEffect(beliefs["Nothing"])
                 .Build());

            goals.Add(new AgentGoal.Builder("Move Randomly")
                .WithPriority(1)
                .WithDesiredEffect(beliefs["AgentMoving"])
                .Build());

            goals.Add(new AgentGoal.Builder("Seek And Destroy")
                .WithPriority(3)
                .WithDesiredEffect(beliefs["AttackingPlayer"])
                .Build());
        }

        protected override void UpdateStats()
        {
            // noop
        }
        #endregion
    }
}
