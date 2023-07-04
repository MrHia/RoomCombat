using UnityEngine;
using UnityEngine.AI;

namespace RoomCombat {
    public class EnemyAI : Character {
        //bot stats
        private NavMeshAgent m_Agent;
        private Collider m_Collider;

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
        /*    bool alreadfyAttack;*/
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
            m_Agent = GetComponent<NavMeshAgent>();
            centrePoint = this.transform;
            m_Collider = GetComponent<Collider>();
            m_Agent.speed = speed;
        }
        private void Update() {
            switch (stateBot) {
                case StatesBot.Patrol:
                    
                    //change state
                    if (CheckisEnemyinSphere(transform.position, chaseRadius)) {
                        stateBot = StatesBot.Chase;
                        return;
                    }
                    Patroling();
                    if (cooldownChangeDestinationTimer < 0.7f) {
                        animator.SetBool("isFollow", false);
                        m_Agent.speed = 0;
                    }
                    else {
                        animator.SetBool("isFollow", true);
                        m_Agent.speed = 6;
                    }
                    break;
                case StatesBot.Chase:
                    m_Agent.speed = 6;
                    animator.SetBool("isFollow", true);
                    Chase();
                    //change state
                    if (CheckisEnemyinSphere(transform.position, attackRadius)) {
                        //stateBot = StatesBot.Attack;
                        Attack();
                        return;
                    }

                    break;
                case StatesBot.Attack:
                    if (health <= 30 && GetHpEnemy() >= health && timerAttack <= 0f) {
                        Vector3 point;
                        while(!RandomPoint(centrePoint.position, patrolRadius, out point)) {

                            Debug.DrawRay(point, Vector3.up, Color.red, 2.0f);
                            m_Agent.SetDestination(point);
                        }
                        stateBot = StatesBot.Escape;
                        return;
                    }
                    if (timerAttack <= 0f) {
                        stateBot = StatesBot.Chase;
                        m_Agent.speed = 6;
                    }
                    else {
                        timerAttack -= Time.deltaTime;
                        transform.LookAt(targetChase);
                        //animator.SetBool("isRun", false);
                    }
                    
                    break;
                case StatesBot.Escape:
                    m_Agent.speed = 6;
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
            if (m_Agent != null && m_Agent.hasPath) {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(m_Agent.destination, 0.5f);
            }
        }

        private bool CheckisEnemyinSphere(Vector3 center, float range) {
            enemys = Physics.OverlapSphere(center, range);

            foreach (Collider collider in enemys) {
                if (collider != m_Collider && collider.gameObject.GetComponent<Character>()) {
                    return true;
                }
            }
            return false;
        }
        private void Patroling() {

            if (m_Agent.remainingDistance <= m_Agent.stoppingDistance) {
                Vector3 point;
                if (RandomPoint(centrePoint.position, patrolRadius, out point) && cooldownChangeDestinationTimer <= 0) {
                    Debug.DrawRay(point, Vector3.up, Color.red, 2.0f);
                    cooldownChangeDestinationTimer = cooldownChangeDestination;
                    m_Agent.SetDestination(point);
                }
                cooldownChangeDestinationTimer -= Time.deltaTime;
            }
        }
        bool RandomPoint(Vector3 center, float range, out Vector3 result) {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            //Vector3 distanceToWalkPoint = transform.position - randomPoint;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                result = hit.position;
                return true;
            }
/*            while (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                result = hit.position;
                return true;
            }*/
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
                m_Agent.SetDestination(targetChase.position);
        }
        public void FindingClosestDistance(Collider[] m_enemys, ref Transform targetChase) {
            Vector3 m_Position = transform.position;
            float closestDistance = Mathf.Infinity;
            foreach (Collider targetEnemy in m_enemys) {
                if (targetEnemy != m_Collider && targetEnemy.gameObject.GetComponent<Character>()) {
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
                m_Agent.speed = 0;
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
            if (m_Agent.remainingDistance <= m_Agent.stoppingDistance ) {
                Vector3 point;
                if (RandomPoint(centrePoint.position, patrolRadius, out point)) {
                    Debug.DrawRay(point, Vector3.up, Color.red, 2.0f);
                    m_Agent.SetDestination(point);
                    //Debug.Log($"ê có điểm đến nè {point}");
                }
            }
            if (CheckisEnemyinSphere(transform.position, chaseRadius)) {
                randomStateWhenEscape();
            }
            if (timerEscape <= 0) {
                timerEscape = timeDowntEscape;
                stateBot = StatesBot.Patrol;
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