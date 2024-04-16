using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    [SelectionBase]
    public class HealingEnemy : GoapAgent
    {
        [Header("Sensors")]
        [SerializeField] private Sensor healSensor;
        [SerializeField] private Sensor chaseSensor;

        public List<Transform> SafePositions { get; } = new List<Transform>();

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
            healSensor.OnTargetChanged += HandleTargetChange;
            chaseSensor.OnTargetChanged += HandleTargetChange;
        }

        private void OnDisable()
        {
            healSensor.OnTargetChanged -= HandleTargetChange;
            chaseSensor.OnTargetChanged -= HandleTargetChange;
        }
        #endregion

        #region GOAP
        protected override void SetupBeliefs()
        {
            beliefs = new Dictionary<string, AgentBelief>();
            var factory = new BeliefFactory(this, beliefs);

            factory.AddBelief("Nothing", () => false);
            factory.AddBelief("SafeFromHarm", () => false);

            factory.AddSensorBelief("EnemyInChaseRange", chaseSensor);
            factory.AddSensorBelief("EnemyInHealRange", healSensor);
            factory.AddBelief("HealingEnemy", () => false);
        }

        protected override void SetupActions()
        {
            actions = new HashSet<AgentAction>();

            actions.Add(new AgentAction.Builder("No Movement")
                .WithStrategy(new IdleStrategy(5, animationController))
                .AddEffect(beliefs["Nothing"])
                .Build());

            actions.Add(new AgentAction.Builder("Go To Safety")
                .WithStrategy(new MoveStrategy(navMeshAgent, () => SafePositions[Random.Range(0, SafePositions.Count)].position, animationController))
                .AddEffect(beliefs["SafeFromHarm"])
                .Build());

            actions.Add(new AgentAction.Builder("Chase after Enemy")
                .WithStrategy(new MoveStrategy(navMeshAgent, () => beliefs["EnemyInChaseRange"].Location, animationController))
                .AddPrecondition(beliefs["EnemyInChaseRange"])
                .AddEffect(beliefs["EnemyInHealRange"])
                .Build());

            actions.Add(new AgentAction.Builder("Heal Enemy")
                .WithStrategy(new HealStrategy(() => healSensor.Target, animationController))
                .AddPrecondition(beliefs["EnemyInHealRange"])
                .AddEffect(beliefs["HealingEnemy"])
                .Build());
        }

        protected override void SetupGoals()
        {
            goals = new HashSet<AgentGoal>();

            goals.Add(new AgentGoal.Builder("Don't Move")
                 .WithPriority(2)
                 .WithDesiredEffect(beliefs["Nothing"])
                 .Build());

            goals.Add(new AgentGoal.Builder("Go to a Safe Area")
                .WithPriority(2)
                .WithDesiredEffect(beliefs["SafeFromHarm"])
                .Build());

            goals.Add(new AgentGoal.Builder("Seek and Heal Enemies")
                .WithPriority(3)
                .WithDesiredEffect(beliefs["HealingEnemy"])
                .Build());
        }

        protected override void UpdateStats()
        {
            // noop
        }
        #endregion
    }
}
