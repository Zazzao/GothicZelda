using DensetsuEngine.Utils;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;




namespace DensetsuEngine.GOAP {

    public interface IActionStrategy { 
        bool CanPerform {  get; }
        bool Complete { get; }

        void Start() { 
            //noop
        }

        void Update(float deltaTime) { 
            //noop
        }

        void Stop() { 
            //noop
        }
    }

    //TO-DO: Might want to move all these to there own file
    public class IdleStrategy : IActionStrategy {
        public bool CanPerform => true; //agent can always idle
        public bool Complete {get; private set;}

        readonly CountdownTimer timer;


        public IdleStrategy(float duration) { 
            timer = new CountdownTimer(duration);
            timer.OnTimerStart += () => Complete = false;
            timer.OnTimerStop += () => Complete = true;
        }



        public void Start() => timer.Start();
        public void Update(float deltaTime) => timer.Tick(deltaTime);
    }

    public class WanderStrategy : IActionStrategy {
        
        //i dont have pathfind added yet
        //readonly NavMeshAgent agent;
        readonly Enemy enemy;
        readonly float wanderRadius;

        public bool CanPerform => !Complete;
        //public bool Complete => agent.remainingDistance <= 2f & !agent.pathPending;

        public bool Complete => enemy.ReachEndOfPath;

        public WanderStrategy(Enemy enemy, float wanderRadius) { 
            //this.agent = agent;
            this.enemy = enemy;
            this.wanderRadius = wanderRadius;
        }

        public void Start() {

            Vector2 randomDirection = (UnityEngine.Random.insideUnitCircle * wanderRadius);       
            enemy.SetPath((Vector2)enemy.transform.position + randomDirection);
            //TO-DO: this should prob have some kind of "back out timer" in case it cant reach the target pos

            // Code From tutorial that uses nav mesh
            /*
            for (int i = 0; i < 5; i++) {
                Vector2 randomDirection = (UnityEngine.Random.insideUnitCircle * wanderRadius);
                NavMeshHit hit;

                if (NavMesh.SamplePosition((Vector2)agent.transform.position + randomDirection, out hit, wanderRadius, 1)) {
                    agent.SetDestination(hit.position);
                    return;
                }

            }
            //*/
        }

    }


}

