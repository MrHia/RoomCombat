﻿using UnityEngine;
using UnityEngine.AI;

namespace RoomCombat {
    public class EnemyAI : Character {
        //bot stats
        private NavMeshAgent _agent;
        private Collider _collider;

        //Animation
        private Animator animator;

        //Patroling
        public Transform centrePoint;
        public float patrolRadius;
        public float cooldownChangeDestination;
        public float cooldownChangeDestinationTimer;
        //Chase
        private Collider[] enemys;
        public Transform targetChase;
        public float chaseRadius;

        //Attacking
        public Transform attackPos;
        private float timerAttack;
        public bool hasInEscape;
        /*    bool alreadfy Attack;*/
        //Escape
        public float timeDowntEscape = 5f;
        private float timerEscape = 5;
        public float timeRandomMax = 5f;
        private float timerRandom = 5;

        //States
        enum StatesBot {
            Patrol,
            Chase,
            Attack,
            Escape
        }
        [SerializeField] StatesBot stateBot = StatesBot.Patrol;
        public float attackRadius;
        public bool playerInchaseRadius, playerInattackRadius;
        private void Start() {
            animator = GetComponentInChildren<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            centrePoint = this.transform;
            _collider = GetComponent<Collider>();
            _agent.speed = speed;
        }
        private void Update() {
            switch (stateBot) {
                case StatesBot.Patrol:
                    animator.SetBool("isFollow", false);
                    //change state
                    if (CheckisEnemyinSphere(transform.position, chaseRadius)) {
                        stateBot = StatesBot.Chase;
                        return;
                    }
                    Patroling();
                    break;
                case StatesBot.Chase:
                    _agent.speed = 6;
                    animator.SetBool("isFollow", true);
                    animator.SetBool("isPatrolling", false);
                    Chase();
                    //change state
                    if (CheckisEnemyinSphere(transform.position, attackRadius)) {
                        //stateBot = StatesBot.Attack;
                        Attack();
                        return;
                    }

                    break;
                case StatesBot.Attack:
                    animator.SetBool("isFollow", false);
                    animator.SetBool("isPatrolling", false);
                    if (health <= 30 && GetHpEnemy() >= health && timerAttack <= 0f) {
                        Vector3 point;
                        while (!RandomPoint(centrePoint.position, patrolRadius, out point)) {
  
                        }
                        Debug.DrawRay(point, Vector3.up, Color.red, 2.0f);
                        _agent.SetDestination(point);
                        stateBot = StatesBot.Escape;
                        return;
                    }
                    if (timerAttack <= 0f) {
                        stateBot = StatesBot.Chase;
                        _agent.speed = 6;
                    }
                    else {
                        timerAttack -= Time.deltaTime;
                        transform.LookAt(targetChase);
                        //animator.SetBool("isRun", false);
                    }

                    break;
                case StatesBot.Escape:
                    _agent.speed = 6;
                    Escape();
                    animator.SetBool("isFollow", true);
                    //change state
                    break;
                default:
                    stateBot = StatesBot.Patrol;
                    break;
            }
        }
        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
            if (_agent != null && _agent.hasPath) {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_agent.destination, 0.5f);
            }
        }

        private bool CheckisEnemyinSphere(Vector3 center, float range) {
            enemys = Physics.OverlapSphere(center, range);

            foreach (Collider collider in enemys) {
                if (collider != _collider && collider.gameObject.GetComponent<Character>()) {
                    return true;
                }
            }
            return false;
        }
        private void FixedUpdate() {

        }
        private void Patroling() {

            if (_agent.remainingDistance <= _agent.stoppingDistance) {
                Vector3 point;
                if (cooldownChangeDestinationTimer >= cooldownChangeDestination) {
                    if (RandomPoint(centrePoint.position, patrolRadius, out point)) {
                        Debug.DrawRay(point, Vector3.up, Color.red, 2.0f);
                        _agent.SetDestination(point);
                        animator.SetBool("isPatrolling", true);
                    }
                    else {
                        return;
                    }
                    cooldownChangeDestinationTimer = 0;
                    _agent.speed = 2;
                }
                else {
                    cooldownChangeDestinationTimer += Time.deltaTime * 0.5f;
                    animator.SetBool("isPatrolling", false);
                    _agent.speed = 0;
                    return;
                }
            }


        }
        bool RandomPoint(Vector3 center, float range, out Vector3 result) {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                result = hit.position;
                return true;
            }
            result = hit.position;
            return false;
        }
        private void Chase() {
            enemys = Physics.OverlapSphere(transform.position, chaseRadius);
            FindingClosestDistance(enemys, ref targetChase);
            if (targetChase == null) {
                stateBot = StatesBot.Patrol;
                return;
            }
            else
                _agent.SetDestination(targetChase.position);
        }
        public void FindingClosestDistance(Collider[] m_enemys, ref Transform targetChase) {
            Vector3 m_Position = transform.position;
            float closestDistance = Mathf.Infinity;
            foreach (Collider targetEnemy in m_enemys) {
                if (targetEnemy != _collider && targetEnemy.gameObject.GetComponent<Character>()) {
                    Vector3 directionToTarget = targetEnemy.transform.position - m_Position;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistance) {
                        closestDistance = dSqrToTarget;
                        targetChase = targetEnemy.transform;
                    }
                }
            }
        }
        public override void Attack() {
            if (stateBot != StatesBot.Attack) {
                _agent.speed = 0;
                stateBot = StatesBot.Attack;
                FindObjectOfType<soundManager>().Play("Sword");
                timerAttack = cooldownAttack;
                animator.SetTrigger("Attack");
            }
        }
        public float GetHpEnemy() {

            if (targetChase == null) {
                return 0;
            }
            if (targetChase.gameObject.GetComponent<PlayerController>()) {
                return targetChase.gameObject.GetComponent<PlayerController>().health;
            }
            if (targetChase.gameObject.GetComponent<EnemyAI>()) {
                return targetChase.gameObject.GetComponent<EnemyAI>().health;
            }
            return 0;
        }
        public void Escape() {
            if (_agent.remainingDistance <= _agent.stoppingDistance) {
                Vector3 point;
                if (RandomPoint(centrePoint.position, patrolRadius, out point)) {
                    Debug.DrawRay(point, Vector3.up, Color.red, 2.0f);
                    _agent.SetDestination(point);
                }
            }
            if (CheckisEnemyinSphere(transform.position, chaseRadius)) {
                randomStateWhenEscape();
            }
            if (timerEscape <= 0) {
                timerEscape = timeDowntEscape;
                stateBot = StatesBot.Chase;
            }
            else {
                timerEscape -= Time.deltaTime;
            }
        }
        public void randomStateWhenEscape() {
            if (timerRandom < 0) {
                timerRandom = timeRandomMax;
                int rate = Random.Range(0, 100);
                if (rate < 30) {
                    stateBot = StatesBot.Chase;
                }
                else {
                    stateBot = StatesBot.Escape;
                }
            }
            else {
                timerRandom -= Time.deltaTime;
            }
        }
        public override void Dead() {
            Destroy(gameObject);
        }
    }

}