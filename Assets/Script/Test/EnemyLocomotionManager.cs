using RoomCombat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine {


    public class EnemyLocomotionManager : MonoBehaviour {

        private EnemyManager enemyManager;
        Character currentTarget;
        public Rigidbody enemyRigitbody;
        private void Awake() {
            enemyManager = GetComponent<EnemyManager>();
        }
        public void HandleDetection() {
            
            
        }



    }
}