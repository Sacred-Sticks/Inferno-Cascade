using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    public class ExampleAgent : GoapAgent
    {
        [Header("Sensors")]
        [SerializeField] private Sensor chaseSensor;
        [SerializeField] private Sensor attackSensor;

        [Header("Known Locations")]
        [SerializeField] private Transform healingStation;
        [SerializeField] private Transform restingStation;

        // TODO : Move this to stats system
        [Header("Stats")]
        [SerializeField] private float health = 100f;
        [SerializeField] private float stamina = 100f;

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
            factory.AddBelief("LowHealth", () => health < 30f);
            factory.AddBelief("Healthy", () => health > 50f);
            factory.AddBelief("LowStamina", () => stamina < 20f);
            factory.AddBelief("WellRested", () => stamina > 50f);

            factory.AddLocationBelief("AtHealingStation", 1f, healingStation.position);
            factory.AddLocationBelief("AtRestingStation", 1f, restingStation.position);
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

            actions.Add(new AgentAction.Builder("MoveToHealingStation")
                .WithStrategy(new MoveStrategy(navMeshAgent, () => healingStation.position))
                .AddEffect(beliefs["AtHealingStation"])
                .Build());

            actions.Add(new AgentAction.Builder("Healing")
                .WithStrategy(new IdleStrategy(2))
                .AddEffect(beliefs["Healthy"])
                .AddPrecondition(beliefs["AtHealingStation"])
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

            goals.Add(new AgentGoal.Builder("KeepHealthUp")
                .WithPriority(2)
                .WithDesiredEffect(beliefs["Healthy"])
                .Build());
        }
        #endregion

        // TODO : Move this to stats system
        protected override void UpdateStats()
        {
            health += InRangeOf(healingStation.position, 5f) ? 20f : -5f;
            stamina += InRangeOf(restingStation.position, 5f) ? 20f : -1f;
            health = Mathf.Clamp(health, 0f, 100f);
            stamina = Mathf.Clamp(stamina, 0f, 100f);
        }
    }
}
