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

        #region UnityEvents
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

            factory.AddSensorBelief("PlayerInChaseRange", chaseSensor);
            factory.AddSensorBelief("PlayerInAttackRange", attackSensor);
        }

        protected override void SetupActions()
        {
            actions = new HashSet<AgentAction>();

            actions.Add(new AgentAction.Builder("Relax")
                    .WithStrategy(new IdleStrategy(3))
                    .AddEffect(beliefs["Nothing"])
                    .Build());

            actions.Add(new AgentAction.Builder("Wander")
                    .WithStrategy(new WanderStrategy(navMeshAgent, 10))
                    .WithCost(2)
                    .AddEffect(beliefs["AgentMoving"])
                    .Build());

            actions.Add(new AgentAction.Builder("ChasePlayer")
                .WithStrategy(new MoveStrategy(navMeshAgent, () => beliefs["PlayerInChaseRange"].Location))
                .AddPrecondition(beliefs["PlayerInChaseRange"])
                .AddEffect(beliefs["PlayerInAttackRange"])
                .Build());

            actions.Add(new AgentAction.Builder("AttackPlayer")
                .WithStrategy(new AttackStrategy(attackSensor.Target))
                .AddPrecondition(beliefs["PlayerInAttackRange"])
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
