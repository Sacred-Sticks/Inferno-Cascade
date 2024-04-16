using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    [SelectionBase]
    public class RangedEnemy : GoapAgent
    {
        [Header("Sensors")]
        [SerializeField] private Sensor attackSensor;
        [SerializeField] private Sensor chaseSensor;

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

            factory.AddBelief("NotHurt", () => health.HealthPercentage > 0.25f);

            factory.AddSensorBelief("PlayerInChaseRange", chaseSensor);
            factory.AddSensorBelief("PlayerInAttackRange", attackSensor);
            factory.AddBelief("AttackingPlayer", () => false);
        }

        protected override void SetupActions()
        {
            actions = new HashSet<AgentAction>();

            actions.Add(new AgentAction.Builder("Relax")
                    .WithStrategy(new IdleStrategy(3, animationController))
                    .AddEffect(beliefs["Nothing"])
                    .Build());

            actions.Add(new AgentAction.Builder("Wander")
                    .WithStrategy(new WanderStrategy(navMeshAgent, 10, animationController))
                    .WithCost(2)
                    .AddEffect(beliefs["AgentMoving"])
                    .Build());

            actions.Add(new AgentAction.Builder("ChasePlayer")
                .WithStrategy(new MoveStrategy(navMeshAgent, () => beliefs["PlayerInChaseRange"].Location, animationController))
                .AddPrecondition(beliefs["PlayerInChaseRange"])
                .AddPrecondition(beliefs["NotHurt"])
                .AddEffect(beliefs["PlayerInAttackRange"])
                .Build());

            actions.Add(new AgentAction.Builder("AttackPlayer")
                .WithStrategy(new CautiousAttackStrategy(() => attackSensor.Target, health, animationController))
                .AddPrecondition(beliefs["PlayerInAttackRange"])
                .AddPrecondition(beliefs["NotHurt"])
                .AddEffect(beliefs["AttackingPlayer"])
                .Build());
        }

        protected override void SetupGoals()
        {
            goals = new HashSet<AgentGoal>();

            goals.Add(new AgentGoal.Builder("Rest")
                 .WithPriority(1)
                 .WithDesiredEffect(beliefs["Nothing"])
                 .Build());

            goals.Add(new AgentGoal.Builder("Wander")
                .WithPriority(1)
                .WithDesiredEffect(beliefs["AgentMoving"])
                .Build());

            goals.Add(new AgentGoal.Builder("SeekAndDestroy")
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
