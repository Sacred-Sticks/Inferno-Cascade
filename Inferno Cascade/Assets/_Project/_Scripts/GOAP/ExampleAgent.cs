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
            BeliefFactory factory = new BeliefFactory(this, beliefs);

            factory.AddBelief("Nothing", () => false);

            factory.AddBelief("AgentIdle", () => !navMeshAgent.hasPath);
            factory.AddBelief("AgentMoving", () => navMeshAgent.hasPath);
            factory.AddBelief("AgentHealthLow", () => health < 30);
            factory.AddBelief("AgentIsHealthy", () => health >= 50);
            factory.AddBelief("AgentStaminaLow", () => stamina < 10);
            factory.AddBelief("AgentIsRested", () => stamina >= 50);

            factory.AddLocationBelief("AgentAtRestingPosition", 3f, restingStation);
            factory.AddLocationBelief("AgentAtHealingPosition", 3f, healingStation);

            factory.AddSensorBelief("PlayerInChaseRange", chaseSensor);
            factory.AddSensorBelief("PlayerInAttackRange", attackSensor);
            factory.AddBelief("AttackingPlayer", () => false); // Player can always be attacked, this will never become true
        }

        protected override void SetupActions()
        {
            actions = new HashSet<AgentAction>();

            actions.Add(new AgentAction.Builder("Relax")
                .WithStrategy(new IdleStrategy(5))
                .AddEffect(beliefs["Nothing"])
                .Build());

            actions.Add(new AgentAction.Builder("Wander Around")
                .WithStrategy(new WanderStrategy(navMeshAgent, 10))
                .AddEffect(beliefs["AgentMoving"])
                .Build());

            actions.Add(new AgentAction.Builder("MoveToEatingPosition")
                .WithStrategy(new MoveStrategy(navMeshAgent, () => healingStation.position))
                .AddEffect(beliefs["AgentAtFoodShack"])
                .Build());

            actions.Add(new AgentAction.Builder("Eat")
                .WithStrategy(new IdleStrategy(5))  // Later replace with a Command
                .AddPrecondition(beliefs["AgentAtFoodShack"])
                .AddEffect(beliefs["AgentIsHealthy"])
                .Build());

            actions.Add(new AgentAction.Builder("MoveFromDoorOneToRestArea")
                .WithCost(2)
                .WithStrategy(new MoveStrategy(navMeshAgent, () => restingStation.position))
                .AddPrecondition(beliefs["AgentAtDoorOne"])
                .AddEffect(beliefs["AgentAtRestingPosition"])
                .Build());

            actions.Add(new AgentAction.Builder("MoveFromDoorTwoRestArea")
                .WithStrategy(new MoveStrategy(navMeshAgent, () => restingStation.position))
                .AddPrecondition(beliefs["AgentAtDoorTwo"])
                .AddEffect(beliefs["AgentAtRestingPosition"])
                .Build());

            actions.Add(new AgentAction.Builder("Rest")
                .WithStrategy(new IdleStrategy(5))
                .AddPrecondition(beliefs["AgentAtRestingPosition"])
                .AddEffect(beliefs["AgentIsRested"])
                .Build());

            actions.Add(new AgentAction.Builder("ChasePlayer")
                .WithStrategy(new MoveStrategy(navMeshAgent, () => beliefs["PlayerInChaseRange"].Location))
                .AddPrecondition(beliefs["PlayerInChaseRange"])
                .AddEffect(beliefs["PlayerInAttackRange"])
                .Build());

            actions.Add(new AgentAction.Builder("AttackPlayer")
                .WithStrategy(new AttackStrategy())
                .AddPrecondition(beliefs["PlayerInAttackRange"])
                .AddEffect(beliefs["AttackingPlayer"])
                .Build());
        }

        protected override void SetupGoals()
        {
            goals = new HashSet<AgentGoal>();

            goals.Add(new AgentGoal.Builder("Chill Out")
                .WithPriority(1)
                .WithDesiredEffect(beliefs["Nothing"])
                .Build());

            goals.Add(new AgentGoal.Builder("Wander")
                .WithPriority(1)
                .WithDesiredEffect(beliefs["AgentMoving"])
                .Build());

            goals.Add(new AgentGoal.Builder("KeepHealthUp")
                .WithPriority(2)
                .WithDesiredEffect(beliefs["AgentIsHealthy"])
                .Build());

            goals.Add(new AgentGoal.Builder("KeepStaminaUp")
                .WithPriority(2)
                .WithDesiredEffect(beliefs["AgentIsRested"])
                .Build());

            goals.Add(new AgentGoal.Builder("SeekAndDestroy")
                .WithPriority(3)
                .WithDesiredEffect(beliefs["AttackingPlayer"])
                .Build());
        }
        #endregion

        // TODO : Move this to stats system
        protected override void UpdateStats()
        {
            health += InRangeOf(healingStation.position, 3f) ? 20 : -5;
            health = Mathf.Clamp(health, 0, 100);
            stamina += InRangeOf(restingStation.position, 3f) ? 20 : -10;
            stamina = Mathf.Clamp(stamina, 0, 100);
        }
    }
}
