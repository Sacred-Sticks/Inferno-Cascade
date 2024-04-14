using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Inferno_Cascade
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class GoapAgent : MonoBehaviour
    {
        protected NavMeshAgent navMeshAgent;
        private Rigidbody body;

        protected CountdownTimer statsTimer;
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

        protected virtual void Start()
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
                CalculatePlan();

                if (actionPlan != null && actionPlan.Actions.Count > 0)
                {
                    navMeshAgent.ResetPath();

                    currentGoal = actionPlan.AgentGoal;
                    currentAction = actionPlan.Actions.Pop();
                    if (currentAction.Preconditions.All(b => b.Evaluate()))
                        currentAction.Start();
                    else
                    {
                        currentAction = null;
                        currentGoal = null;
                    }
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

        protected abstract void SetupBeliefs();
        protected abstract void SetupActions();
        protected abstract void SetupGoals();
        protected abstract void UpdateStats();

        protected void HandleTargetChange()
        {
            currentAction = null;
            currentGoal = null;
        }

        protected bool InRangeOf(Vector3 position, float range)
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
