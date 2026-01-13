
using System.Collections.Generic;
using DensetsuEngine.Utils;
using UnityEngine;
using UnityEngine.AI;


namespace DensetsuEngine.GOAP {
    public class GoapAgent : MonoBehaviour
    {
        //DEV NOTE: Tutorial has nav mesh and anim controller tied into it
        // i will NOT be doing that (Agent will send info to the Actor controller)


        [Header("Sensors")]
        [SerializeField] private Sensor chaseSensor;
        [SerializeField] private Sensor attackSensor;

        //This is for testing not REAL varibles i would want to use
        [Header("Known Locations")]
        [SerializeField] Transform restingPosition;
        [SerializeField] Transform foodPile;
        [SerializeField] Transform patrolPosOne;
        [SerializeField] Transform patrolPosTwo;

        // nav mesh
        // anim
        // rb

        //JUST TO TEST THE AGENT
        [Header("Stats")]
        public float health = 100.0f;
        public float stamina = 100.0f;
        CountdownTimer statsTimer;

        //testing
        public bool IsMoving = false;


        private GameObject target;
        private Vector2 destination;

        private AgentGoal lastGoal; // to make sure were not picking same goal over and over
        public AgentGoal currentGoal;
        //public ActionPlan actionPlan;
        public AgentAction currentAction;

        public Dictionary<string, AgentBelief> beliefs;
        public HashSet<AgentAction> actions;
        public HashSet<AgentGoal> goals;

        private bool InRangeOf(Vector2 pos, float range) => Vector2.Distance(transform.position, pos) < range;

        private void OnEnable()
        {
            chaseSensor.OnTargetChanged += HandleTargetChanged;
        }

        private void OnDisable()
        {
            chaseSensor.OnTargetChanged -= HandleTargetChanged;
        }

        private void Awake()
        {
            // get ref to all of the components 
            // we are NOT going to do this here
        }

        private void Start()
        {
            SetupTimers();
            SetupBeliefs();
            SetupActions();
            SetupGoals();
        }

        private void SetupBeliefs() { 
            beliefs = new Dictionary<string, AgentBelief>();
            BeliefFactory factory = new BeliefFactory(this,beliefs);

            factory.AddBelief("Nothing", () => false);
            factory.AddBelief("AgentIdle", () => !IsMoving);
            factory.AddBelief("AgentMoving", () => IsMoving);

        }

        private void SetupActions() { 
            actions = new HashSet<AgentAction>();

            actions.Add(new AgentAction.Builder("Relax")
                .WithStrategy(new IdleStrategy(5))
                .AddEffect(beliefs["Nothing"])
                .Build());

            actions.Add(new AgentAction.Builder("Wander Around")
                .WithStrategy(new WanderStrategy(new NavMeshAgent(), 10))
                .AddEffect(beliefs["AgentMoving"])
                .Build());

        }


        private void SetupGoals() { 
            goals = new HashSet<AgentGoal>();

            goals.Add (new AgentGoal.Builder("Chill Out")
                .WithPriority(1)
                .WithDesiredEffect(beliefs["Nothing"])
                .Build());

            goals.Add (new AgentGoal.Builder("Wander")
                .WithPriority(1)
                .WithDesiredEffect(beliefs["AgentMoving"])
                .Build());  


        }


        private void SetupTimers() {
            // this is to just test the agents 
            statsTimer = new CountdownTimer(2.0f);
            statsTimer.OnTimerStop += () =>
            {
                UpdateStats();
                statsTimer.Start();
            };
            statsTimer.Start();
        
        }

        void UpdateStats() {
            //the is just for testing
            stamina += InRangeOf(restingPosition.position, 2.0f) ? 20 : -10;
            health += InRangeOf(foodPile.position, 2.0f) ? 20 : -5;
            stamina = Mathf.Clamp(stamina, 0, 100);
            health = Mathf.Clamp(health, 0, 100);

        }

        private void HandleTargetChanged() {
            Debug.Log("Target changed, clearing current action and goal");
            //force planner to re-evaluate the plan
            currentAction = null;
            currentGoal = null;
        }

    }
}