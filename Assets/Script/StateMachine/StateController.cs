using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

namespace RoomCombat {
    public class StateController : Character {
        //bot stats
        public NavMeshAgent agent;
        public Collider m_collider;
        public Rigidbody m_rb;

        //ragdoll
        public Rigidbody[] m_rigidbodysRagdoll;
        public Collider[] m_collidersRagdoll;

        //Animation
        public Animator animator;

        //Patroling
        public Transform centrePoint;
        public float patrolRadius;
        public float cooldownChangeDestination;
        public float cooldownChangeDestinationTimer;
        //Chase
        public Collider[] enemys;
        public Transform targetChase;
        public float chaseRadius;

        //Attacking
        public Transform attackPos;
        public float timerAttack;
        public bool hasInEscape;
        /*    bool alreadfy Attack;*/
        //Escape
        public float timeDowntEscape = 5f;
        public float timerEscape = 5;
        public float timeRandomMax = 5f;
        public float timerRandom = 5;
        public float attackRadius;
        public bool playerInchaseRadius, playerInattackRadius;
        //State
        [SerializeField] string nameState;
        private IState currentState;
        public DeadState deadState = new DeadState();
        public ChaseState chaseState = new ChaseState();
        public PatrolState patrolState = new PatrolState();
        public EscapeState escapeState = new EscapeState();
        public AttackState attackState = new AttackState();
        public HurtState hurtState = new HurtState();
        private void Awake() {

        }
        private void Start() {

            m_rigidbodysRagdoll = GetComponentsInChildren<Rigidbody>();
            m_collidersRagdoll = GetComponentsInChildren<Collider>();

            m_rb = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            m_collider = GetComponent<Collider>();
            centrePoint = this.transform;
            agent.speed = speed;
            foreach (Rigidbody _rigidbody in m_rigidbodysRagdoll) {
                if (_rigidbody != m_rb) {
                    _rigidbody.isKinematic = true;
                }
            }
            foreach (Collider collider in m_collidersRagdoll) {
                if (collider != m_collider) {
                    collider.enabled = false;
                }

            }
            ChangeState(patrolState);
        }
        void Update() {
            if (currentState != null) {
                currentState.UpdateState(this);
            }

        }
        public void OnHurt() {
            m_rb.mass = 1f;
            currentState.OnHurt(this);
        }
        public void ChangeState(IState newState) {
            if (currentState != null) {
                currentState.OnExit(this);
            }
            currentState = newState;
            currentState.OnEnter(this);
            nameState = newState.ToString();


        }
/*        public bool RandomPoint(Vector3 center, float range, out Vector3 result) {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            var isTargetPointInNavMesh = NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas);
            *//*if (isTargetPointInNavMesh) {
                result = hit.position;
                return true;
            }
            result = hit.position;
            return false;*//*

            result = hit.position;
            return isTargetPointInNavMesh;
        }*/

        public Vector3 GetRandomPointInNavMesh() {
            bool isTargetPointInNavMesh;
            Vector3 nextPoint;
            do {
                var randomPoint = centrePoint.position + Random.insideUnitSphere * patrolRadius;
                isTargetPointInNavMesh = NavMesh.SamplePosition(randomPoint, out var hit, 1.0f, NavMesh.AllAreas);
                nextPoint = hit.position;
            }
            while (!isTargetPointInNavMesh);

            return nextPoint;
        }

        public bool CheckisEnemyinSphere( float range) {
            enemys = Physics.OverlapSphere(transform.position, range);
            foreach (Collider collider in enemys) {
                if (collider != m_collider && collider.gameObject.GetComponent<Character>()) {
                    return true;
                }
            }
            return false;
        }
        public void FindingClosestDistance(Collider[] m_enemys, ref Transform targetChase) {
            Vector3 m_Position = transform.position;
            float closestDistance = Mathf.Infinity;
            foreach (Collider targetEnemy in m_enemys) {
                if (targetEnemy != m_collider && targetEnemy.gameObject.GetComponent<Character>() ) {
                    Vector3 directionToTarget = targetEnemy.transform.position - m_Position;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistance && targetEnemy.gameObject.GetComponent<Character>().GetHp()>0) {
                        closestDistance = dSqrToTarget;
                        targetChase = targetEnemy.transform;
                    }
                }
            }
        }
        public float GetHpEnemy() {

            if (targetChase == null) {
                return 0;
            }
            if (targetChase.gameObject.GetComponent<Character>()) {
                return targetChase.gameObject.GetComponent<Character>().GetHp();
            }
            return 0;
        }
        public void lookAtEnemy() {
            transform.LookAt(targetChase);
        }
        public void SoundEnable(string nameSound) {
            FindObjectOfType<soundManager>().Play(nameSound);
        }
        public override void Dead() {
            Invoke("OnRagDoll", 0.5f);
            ChangeState(deadState);
        }
        private void OnRagDoll() {
            animator.enabled = false;
            m_rb.detectCollisions = false;
            m_collider.enabled = false;
            foreach (Rigidbody _rigidbody in m_rigidbodysRagdoll) {
                if (_rigidbody != m_rb) {
                    _rigidbody.mass = 100f;
                    _rigidbody.isKinematic = false;
                }
            }
            foreach (Collider collider in m_collidersRagdoll) {
                if (collider != m_collider) {
                    collider.enabled = true;
                    collider.isTrigger = false;
                }

            }
            Invoke("DestroyOnDead", 2f);
            agent.enabled = false;
            this.enabled = false;
        }
        public void DestroyOnDead() {
            foreach (Collider collider in m_collidersRagdoll) {
                collider.isTrigger = true;
            }
            Destroy(gameObject, 3f);
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
            if (agent != null && agent.hasPath) {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(agent.destination, 0.5f);
            }
        }
    }
    public interface IState {
        public void OnEnter(StateController controller);
        public void UpdateState(StateController controller);
        public void OnHurt(StateController controller);
        public void OnExit(StateController controller);
    }
}