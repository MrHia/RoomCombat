using RoomCombat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace StateMachine {

    public class EnemyManager : MonoBehaviour {

        EnemyLocomotionManager enemyLocomotionManager;
        EnemyAnimatorManager enemyAnimatorManager;
        Character character;

        public State currentState;

        [Header("A.I Setting")]
        public float detectionRadius;
        private void Awake() {
            
        }
        private void Update() {

        }
        public  void HandleStateMachine() {                  
            if (currentState != null) {
                /*State nextState = currentState.Tick(this, character, enemyAnimatorManager);
                if(nextState != null) {
                    SwitchToNextState(nextState);
                }*/
            }
        }

        private void SwitchToNextState(State State) {
            currentState = State;
        }
    }
}