using System;
//using System.Numerics;
using DensetsuEngine.Utils;
using UnityEngine;





namespace DensetsuEngine.GOAP {

    public interface IActionStrategy { 
        bool CanPerform {  get; }
        bool Complete { get; }

        // when we start the strataegy
        void Start() { 
            //noop
        }

        // when we need to update the strategy each frame
        void Update(float deltaTime) { 
            //noop
        }

        // when the startegy is finished 
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


    public class MoveStrategy : IActionStrategy
    {
        readonly Enemy enemy;
        readonly Vector2 destination;
        public bool CanPerform => !Complete;
        public bool Complete => enemy.ReachEndOfPath;

        public MoveStrategy(Enemy enemy, Vector2 destination)
        {
            this.enemy = enemy;
            this.destination = destination;
        }

        public void Start() => enemy.SetPath(destination);
       // public void Stop() => enemy.SetPath = null;

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
            
           


        }

        public void Stop() { 
            //enemy.ReachEndOfPath = false;
            //Debug.Log("wander stop()");
        }

    }


    public class AttackStrategy : IActionStrategy { 
        public bool CanPerform => true; //agent can always attack
        public bool Complete {  get; private set; }

        readonly CountdownTimer timer;

        public AttackStrategy() {
            //needs to tell the enemy to do the attack and base timer off of the animtion length

            timer = new CountdownTimer(1.0f);
            timer.OnTimerStart += () => Complete = false;
            timer.OnTimerStop += () => Complete = true; 
        }

        public void Start() {
            Debug.Log("Enemy attacking player");
            timer.Start();
        }

        public void Update(float deltaTime) => timer.Tick(deltaTime);

    }

}

