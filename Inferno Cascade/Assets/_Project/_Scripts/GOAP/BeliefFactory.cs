using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    public class BeliefFactory
    {
        private readonly GoapAgent agent;
        private readonly Dictionary<string, AgentBelief> beliefs;

        public BeliefFactory(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
        {
            this.agent = agent;
            this.beliefs = beliefs;
        }

        public void AddBelief(string key, Func<bool> condition)
        {
            beliefs.Add(key, new AgentBelief.Builder(key)
                .WithCondition(condition)
                .Build());
        }

        public void AddSensorBelief(string key, Sensor sensor)
        {
            beliefs.Add(key, new AgentBelief.Builder(key)
                .WithCondition(() => sensor.IsTargetInRange)
                .WithLocation(() => sensor.TargetPosition)
                .Build());
        }

        public void AddLocationBelief(string key, float distance, Vector3 position)
        {
            beliefs.Add(key, new AgentBelief.Builder(key)
                .WithCondition(() => InRangeOf(position, distance))
                .WithLocation(() => position)
                .Build());
        }

        public void AddLocationBelief(string key, float distance, Transform transform)
        {
            AddLocationBelief(key, distance, transform.position);
        }

        private bool InRangeOf(Vector3 position, float range)
            => Vector3.Distance(agent.transform.position, position) < range;
    }
}
