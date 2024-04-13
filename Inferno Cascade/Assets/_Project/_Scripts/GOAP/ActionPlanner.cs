using System.Collections.Generic;
using System.Linq;

namespace Inferno_Cascade
{
    public interface IGoapPlanner
    {
        public ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentgoal = null);
    }

    public class GoapPlanner : IGoapPlanner
    {
        public ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentgoal = null)
        {
            // order goals by priority
            var orderedGoals = goals
                .Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
                .OrderByDescending(g => g == mostRecentgoal ? g.Priority - 0.01 : g.Priority)
                .ToList();

            foreach (var goal in orderedGoals)
            {
                var goalNode = new Node(null, null, goal.DesiredEffects, 0);

                // If we can find the goal, return the plan
                if (FindPath(goalNode, agent.actions))
                {
                    if (goalNode.IsLeafDead())
                        continue;

                    Stack<AgentAction> actionStack = new Stack<AgentAction>();
                    while (goalNode.Leaves.Count > 0)
                    {
                        var cheapestLeaf = goalNode.Leaves.OrderBy(n => n.Cost).First();
                        goalNode = cheapestLeaf;
                        actionStack.Push(cheapestLeaf.Action);
                    }

                    return new ActionPlan(goal, actionStack, goalNode.Cost);
                }
            }

            UnityEngine.Debug.LogWarning("No Plan Found");
            return null;
        }

        private bool FindPath(Node parent, HashSet<AgentAction> actions)
        {
            foreach (var action in actions)
            {
                var requiredEffects = parent.RequiredEffects;

                // Remove beliefs that evaluate to true, there is no action to take
                requiredEffects.RemoveWhere(b => b.Evaluate());
                
                if (requiredEffects.Count == 0)
                    return true;

                if (action.Effects.Any(requiredEffects.Contains))
                {
                    var newRequiredEffects = new HashSet<AgentBelief>(requiredEffects);
                    newRequiredEffects.ExceptWith(action.Effects);
                    newRequiredEffects.UnionWith(action.Preconditions);

                    var newAvailableActions = new HashSet<AgentAction>(actions);
                    newAvailableActions.Remove(action);

                    var newNode = new Node(parent, action, newRequiredEffects, parent.Cost + action.Cost);

                    if (FindPath(newNode, newAvailableActions))
                    {
                        parent.Leaves.Add(newNode);
                        newRequiredEffects.ExceptWith(newNode.Action.Preconditions);
                    }

                    // If all effects are satisfied, return true
                    if (newRequiredEffects.Count == 0)
                        return true;
                }
            }

            return false;
        }
    }

    public class Node
    {
        public Node Parent { get; }
        public AgentAction Action { get; }
        public HashSet<AgentBelief> RequiredEffects { get; }
        public List<Node> Leaves { get; }
        public float Cost { get; }

        public bool IsLeafDead() => Leaves.Count == 0 && Action == null;

        public Node(Node parent, AgentAction action, HashSet<AgentBelief> effects, float cost)
        {
            Parent = parent;
            Action = action;
            RequiredEffects = effects;
            Leaves = new List<Node>();
            Cost = cost;
        }
    }

    public class ActionPlan
    {
        public AgentGoal AgentGoal { get; }
        public Stack<AgentAction> Actions { get; }
        public float TotalCost { get; }

        public ActionPlan(AgentGoal goal, Stack<AgentAction> actions, float totalCost)
        {
            AgentGoal = goal;
            Actions = actions;
            TotalCost = totalCost;
        }
    }
}
