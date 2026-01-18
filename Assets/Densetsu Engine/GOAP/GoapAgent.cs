
using System.Collections.Generic;
using System.Linq;
using DensetsuEngine.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;


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


        //JUST TO TEST THE AGENT
        [Header("Stats")]
        public float health = 100.0f;
        public float stamina = 100.0f;
        CountdownTimer statsTimer;

        //testing
        public bool IsMoving => enemy.HasPath; //to-do: this is not getting set so its always false
        Enemy enemy;

        private GameObject target;
        private Vector2 destination;

        private AgentGoal lastGoal; // to make sure were not picking same goal over and over
        public AgentGoal currentGoal;
        public ActionPlan actionPlan;
        public AgentAction currentAction;

        public Dictionary<string, AgentBelief> beliefs;
        public HashSet<AgentAction> actions;
        public HashSet<AgentGoal> goals;

        IGoapPlanner gPlanner;



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
           
            gPlanner = new GoapPlanner();
            enemy = GetComponent<Enemy>();


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
                .WithStrategy(new WanderStrategy(GetComponent<Enemy>(), 8))
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
            //Debug.Log("Target changed, clearing current action and goal");
            //force planner to re-evaluate the plan
            currentAction = null;
            currentGoal = null;
        }


        private void Update()
        {
            statsTimer.Tick(Time.deltaTime);

            //udate the plan and current action if there is one
            if (currentAction == null) {
                Debug.Log("calculating a potenial new plan");
                CalculatePlan();

                if (actionPlan != null && actionPlan.Actions.Count > 0) { 
                    //reset nav mesh
                    
                    currentGoal = actionPlan.AgentGoal;
                    currentAction = actionPlan.Actions.Pop();
                    currentAction.Start();

                    Debug.Log($"Goal: {currentGoal.Name} with {actionPlan.Actions.Count} actions in plan");
                    Debug.Log($"Popped action:{currentAction.Name}");

                }
            }

            // If we have a current action, execute it
            if (actionPlan != null && currentAction != null) { 
                currentAction.Update(Time.deltaTime);

                if (currentAction.Complete) {
                    //Debug.Log($"{currentAction.Name} complete");
                    currentAction.Stop();
                    currentAction = null;

                    if (actionPlan.Actions.Count == 0)
                    {
                        //Debug.Log("Plan Complete");
                        lastGoal = currentGoal;
                        currentGoal = null;
                    }

                }

            }

        }

      


        private void CalculatePlan() {
            var priorityLevel = currentGoal?.Priority ?? 0;

            HashSet<AgentGoal> goalsToCheck = goals;

            // If we have a current goal, we only want to check goals with higher priority
            if (currentGoal != null) {
                //Debug.Log("Current goal exist, checking goals with higher priority");
                goalsToCheck = new HashSet<AgentGoal>(goals.Where(g => g.Priority > priorityLevel));
            }

            var potentialPlan = gPlanner.Plan(this, goalsToCheck, lastGoal);
            if (potentialPlan != null) { 
                actionPlan = potentialPlan;
            }

        }


    }
}