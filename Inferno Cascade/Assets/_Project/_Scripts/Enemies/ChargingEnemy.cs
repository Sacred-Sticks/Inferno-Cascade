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
            factory.AddBelief("AttackingPlayer", () => false);
        }

        protected override void SetupActions()
        {
            actions = new HashSet<AgentAction>();

            actions.Add(new AgentAction.Builder("No Movement")
                .WithStrategy(new IdleStrategy(5))
                .AddEffect(beliefs["Nothing"])
                .Build());

            actions.Add(new AgentAction.Builder("Move to random point within a radius")
                .WithStrategy(new WanderStrategy(navMeshAgent, 10))
                .WithCost(2)
                .AddEffect(beliefs["AgentMoving"])
                .Build());

            actions.Add(new AgentAction.Builder("Chase after Player")
                .WithStrategy(new MoveStrategy(navMeshAgent, () => beliefs["PlayerInChaseRange"].Location))
                .AddPrecondition(beliefs["PlayerInChaseRange"])
                .AddEffect(beliefs["PlayerInAttackRange"])
                .Build());

            actions.Add(new AgentAction.Builder("Attack Player")
                .WithStrategy(new AttackStrategy(() => attackSensor.Target))
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
