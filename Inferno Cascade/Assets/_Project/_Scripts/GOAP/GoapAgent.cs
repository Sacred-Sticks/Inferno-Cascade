using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Inferno_Cascade
{
    [SelectionBase]
    [RequireComponent(typeof(NavMeshAgent))]
    public class GoapAgent : MonoBehaviour
    {
        [Header("Sensors")]
        [SerializeField] private Sensor chaseSensor;
        [SerializeField] private Sensor attackSensor;

        [Header("Known Locations")]
        [SerializeField] private Transform healingStation;
        [SerializeField] private Transform restingStation;

        private NavMeshAgent navMeshAgent;
        private Rigidbody body;

        // TODO : Move this to stats system
        [Header("Stats")]
        [SerializeField] private float health = 100f;
        [SerializeField] private float stamina = 100f;

        private CountdownTimer statsTimer;
        private AgentGoal lastGoal;
        private AgentGoal currentGoal;
        private AgentAction currentAction;
        private ActionPlan actionPlan;

        public Dictionary<string, AgentBelief> beliefs;
        public HashSet<AgentAction> actions;
        public HashSet<AgentGoal> goals;

        private IGoapPlanner gPlanner;

        #region UnityEvents
        private void Awake()
        {
            navMeshAgent = gameObject.GetOrAdd<NavMeshAgent>();
            body = gameObject.GetOrAdd<Rigidbody>();
            body.freezeRotation = true;

            gPlanner = new GoapPlanner();
        }

        private void Start()
        {
            SetupTimers();
            SetupBeliefs();
            SetupActions();
            SetupGoals();
        }

        private void Update()
        {
            statsTimer.Tick(Time.deltaTime);

            if (currentAction == null)
            {
                Debug.Log("Calculating a new action plan");
                CalculatePlan();

                if (actionPlan != null && actionPlan.Actions.Count > 0)
                {
                    navMeshAgent.ResetPath();

                    currentGoal = actionPlan.AgentGoal;
                    currentAction = actionPlan.Actions.Pop();
                    currentAction.Start();
                }
            }

            if (actionPlan != null && currentAction != null)
            {
                currentAction.Update(Time.deltaTime);

                if (currentAction.Complete)
                {
                    currentAction.Stop();
                    currentAction = null;

                    if (actionPlan.Actions.Count == 0)
                    {
                        lastGoal = currentGoal;
                        currentGoal = null;
                    }
                }
            }
        }

        private void OnEnable()
        {
            chaseSensor.OnTargetChanged += HandleTargetChange;
        }

        private void OnDisable()
        {
            chaseSensor.OnTargetChanged -= HandleTargetChange;
        }
        #endregion

        private void SetupTimers()
        {
            statsTimer = new CountdownTimer(2f);
            statsTimer.OnTimerStop += () =>
            {
                UpdateStats();
                statsTimer.Start();
            };
            statsTimer.Start();
        }

        private void SetupBeliefs()
        {
            beliefs = new Dictionary<string, AgentBelief>();
            var factory = new BeliefFactory(this, beliefs);

            factory.AddBelief("Nothing", () => false);
            factory.AddBelief("AgentIdle", () => !navMeshAgent.hasPath);
            factory.AddBelief("AgentMoving", () => navMeshAgent.hasPath);
        }

        private void SetupActions()
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

        private void SetupGoals()
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

        private void HandleTargetChange()
        {
            currentAction = null;
            currentGoal = null;
        }

        // TODO : Move this to stats system
        private void UpdateStats()
        {
            health += InRangeOf(healingStation.position, 2f) ? 10f : 0f;
            stamina += InRangeOf(restingStation.position, 2f) ? 20f : -1f;
            health = Mathf.Clamp(health, 0f, 100f);
            stamina = Mathf.Clamp(stamina, 0f, 100f);
        }

        private bool InRangeOf(Vector3 position, float range)
            => Vector3.Distance(transform.position, position) < range;

        private void CalculatePlan()
        {
            var priorityLevel = currentGoal?.Priority ?? 0;

            var goalsToCheck = goals;

            if (currentGoal != null)
            {
                goalsToCheck = new HashSet<AgentGoal>(goals.Where(g => g.Priority > priorityLevel));
            }

            var potentialPlan = gPlanner.Plan(this, goalsToCheck, lastGoal);
            if (potentialPlan != null)
            {
                actionPlan = potentialPlan;
            }
        }
    }
}
