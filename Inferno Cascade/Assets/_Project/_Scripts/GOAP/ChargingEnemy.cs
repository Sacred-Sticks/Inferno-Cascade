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

        [Header("Known Locations")]
        [SerializeField] private Transform healingStation;
        [SerializeField] private Transform restingStation;

        #region UnityEvents
        private void OnEnable()
        {
            chaseSensor.OnTargetChanged += HandleTargetChange;
        }

        private void OnDisable()
        {
            chaseSensor.OnTargetChanged -= HandleTargetChange;
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

            factory.AddBelief("AttackingPlayer", () => false); // Always can attack player
        }

        protected override void SetupActions()
        {
            actions = new HashSet<AgentAction>();

            actions.Add(new AgentAction.Builder("Relax")
                .WithStrategy(new IdleStrategy(5))
                .AddEffect(beliefs["Nothing"])
                .Build());

            actions.Add(new AgentAction.Builder("Wander")
                .WithStrategy(new WanderStrategy(navMeshAgent, 10))
                .AddEffect(beliefs["AgentMoving"])
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
        }
        #endregion

        protected override void UpdateStats()
        {
            // noop
        }
    }
}
