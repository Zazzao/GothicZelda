using System;
using System.Collections.Generic;
using UnityEngine;


namespace DensetsuEngine.GOAP {


    public class BeliefFactory {
        readonly GoapAgent agent; //GoapAgent.cs not created yet
        readonly Dictionary<string, AgentBelief> beliefs;

        public BeliefFactory(GoapAgent agent, Dictionary<string, AgentBelief> beliefs) { 
            this.agent = agent;
            this.beliefs = beliefs;
        }

        public void AddBelief(string key, Func<bool> condition) {
            beliefs.Add(key, new AgentBelief.Builder(key)
                .WithCondition(condition)
                .Build());
        }

        public void AddSensorBelief(string key, Sensor sensor) {
            beliefs.Add(key, new AgentBelief.Builder(key)
                .WithCondition(() => sensor.IsTargetInRange)
                .WithLocation(() => sensor.TargetPosition)
                .Build());
        }

        public void AddLocationBelief(string key, float distance, Transform locationCondition)
        {
            AddLocationBelief(key, distance, locationCondition.position);
        }

        public void AddLocationBelief(string key, float distance, Vector2 locationCondition) {
            beliefs.Add(key, new AgentBelief.Builder(key)
                .WithCondition(() => InRangeOf(locationCondition, distance))
                .WithLocation(() => locationCondition)
                .Build());
        }


        bool InRangeOf(Vector2 pos, float range) => Vector2.Distance(agent.transform.position, pos) < range;
    
    }



    public class AgentBelief 
    {
        public string Name { get; }

        Func<bool> condition = () => false;
        Func<Vector2> observedLocation = () => Vector2.zero;

        public Vector2 Location => observedLocation();

        private AgentBelief(string name) { 
            Name = name;
        }

        public bool Evaluate() => condition();

        public class Builder { 
            readonly AgentBelief belief;

            public Builder(string name) {
                belief = new AgentBelief(name);
            }

            public Builder WithCondition(Func<bool> condition) { 
                belief.condition = condition;
                return this;
            
            }

            public Builder WithLocation(Func<Vector2> observedLocation) { 
                belief.observedLocation = observedLocation;
                return this;
            }

            public AgentBelief Build() { 
                return belief;
            }


        }

    }
}

