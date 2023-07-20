using System.Net.Sockets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

namespace RoomCombat {
    public class EnemyAI1 : Character {
        //bot stats
        private NavMeshAgent _agent;
        private Collider _collider;
        private Rigidbody _rb;
        //ragdoll
        public Rigidbody[] m_rigidbodysRagdoll;
        public Collider[] m_collidersRagdoll;

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
        private float timerAttack;
        public bool hasInEscape;
        //Escape
        public float timeDowntEscape = 5f;
        private float timerEscape = 5;
        public float timeRandomMax = 5f;
        private float timerRandom = 5;
        //hurt
        public float cooldowntHurt = 1.1f;
        public float hurtTimer = 0;

        //States
        enum StatesBot {
            Patrol,
            Chase,
            Attack,
            Escape,
            Hurt
        }
        [SerializeField] StatesBot stateBot = StatesBot.Patrol;
        public float attackRadius;
        public bool playerInchaseRadius, playerInattackRadius;
        private void Start() {
            _rb = GetComponent<Rigidbody>();
            m_rigidbodysRagdoll = GetComponentsInChildren<Rigidbody>();
            m_collidersRagdoll = GetComponentsInChildren<Collider>();
            animator = GetComponentInChildren<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            centrePoint = this.transform;
            _collider = GetComponent<Collider>();
            _agent.speed = speed;
            foreach (Rigidbody _rigidbody in m_rigidbodysRagdoll) {
                if (_rigidbody != _rb) {
                    _rigidbody.isKinematic = true;
                }
            }
            foreach (Collider collider in m_collidersRagdoll) {
                if (collider != _collider) {
                    collider.enabled = false;
                }

            }
        }
        /*public void SetState( string nameState) {

        } */
        private void Update() {
            if (health <= 0) {
                return;
            }
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

                    if (timerAttack <= 0f ) {
                        stateBot = StatesBot.Chase;
                        _agent.speed = 3.5f;
                    }
                    else {
                        timerAttack -= Time.deltaTime;
                        transform.LookAt(targetChase);
                        //animator.SetBool("isRun", false);
                    }
                    

                    break;
                case StatesBot.Escape:
                    _agent.speed = 3.5f;
                    Escape();
                    animator.SetBool("isFollow", true);
                    //change state
                    break;
                case StatesBot.Hurt:
                    _agent.speed = 0;
                    Hurt();
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
            if(health<=0) {
                return;
            }
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.y);
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
        public void HurtStart() {
            stateBot = StatesBot.Hurt;
        }
        public void Hurt() {
            if (health <= 30 && GetHpEnemy() >= health && hurtTimer >= cooldowntHurt) {
                stateBot = StatesBot.Escape;
                return;
            }
            if (hurtTimer >= cooldowntHurt) {
                
                animator.SetBool("isHurt", false);
                hurtTimer = 0f;
                _agent.enabled = true;
                _rb.mass = 10f;
                stateBot = StatesBot.Patrol;
            }
            else {
                animator.SetBool("isFollow", false);
                animator.SetBool("isPatrolling", false);
                animator.SetBool("isHurt", true);
                _agent.enabled = false;
                hurtTimer += Time.deltaTime;
            }
        }
        public override void Dead() {
            Invoke("OnRagDoll", 0.5f);
        }
        private void OnRagDoll() {
            animator.enabled = false;
            _rb.detectCollisions = false;
            _collider.enabled = false;
            foreach (Rigidbody _rigidbody in m_rigidbodysRagdoll) {
                if (_rigidbody != _rb) {
                    _rigidbody.mass = 100f;
                    _rigidbody.isKinematic = false;
                }
            }
            foreach (Collider collider in m_collidersRagdoll) {
                if (collider != _collider) {
                    collider.enabled = true;
                    collider.isTrigger = false;
                }

            }

            Invoke("DestroyOnDead", 2f);
            _agent.enabled = false;
            this.enabled = false;
        }
        public void DestroyOnDead() {
            foreach (Collider collider in m_collidersRagdoll) {
                collider.isTrigger = true;
            }
            Destroy(gameObject, 3f);
        }

    }

}