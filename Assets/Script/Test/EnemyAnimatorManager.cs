using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine {
    public class EnemyAnimatorManager : MonoBehaviour {
        EnemyManager enemyManager;
        Animator animator;
        private void Awake() {
            enemyManager = GetComponentInParent<EnemyManager>();
            animator = GetComponent<Animator>();

        }
        private void OnAnimatorMove() {
            float delta = Time.deltaTime;

        }
    }
}