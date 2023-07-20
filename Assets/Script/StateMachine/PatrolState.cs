using UnityEngine;
namespace RoomCombat {
    public class PatrolState : IState {

        public float cooldownChangeDestination = 1.5f;
        public float cooldownChangeDestinationTimer;
        public float speedPatrol = 2;
        private Vector3 point;
        public void OnEnter(StateController controller) {
            controller.m_rb.velocity = Vector3.zero;

            controller.animator.SetBool("isPatrolling", true);
            /*while (!controller.RandomPoint(controller.centrePoint.position, controller.patrolRadius, out point)) {
            }*/
            Vector3 nextPoint = controller.GetRandomPointInNavMesh();
            Debug.DrawRay(nextPoint, Vector3.up, Color.red, 2.0f);
            controller.agent.SetDestination(nextPoint);
            controller.agent.speed = speedPatrol;
        }

        public void UpdateState(StateController controller) {
            if (controller.CheckisEnemyinSphere(controller.chaseRadius)) {
                controller.ChangeState(controller.chaseState);
                return;
            }
            if (controller.agent.remainingDistance <= controller.agent.stoppingDistance) {
                if (cooldownChangeDestinationTimer >= cooldownChangeDestination) {
                    /*                    Debug.LogWarning("No chay de");*/
                    Vector3 nextPoint = controller.GetRandomPointInNavMesh();
                    Debug.DrawRay(point, Vector3.up, Color.red, 2.0f);
                    controller.agent.SetDestination(nextPoint);
                    /*                    controller.animator.SetBool("isPatrolling", true);*/
                    cooldownChangeDestinationTimer = 0;
                    /*                    controller.agent.speed = speedPatrol;*/
                }
                else {
                    /*                    Debug.LogError($"No dang dung ne ba oi" +
                                            $"cooldownChangeDestination: {cooldownChangeDestination}" +
                                            $"cooldownChangeDestinationTimer: {cooldownChangeDestinationTimer}");*/
                    cooldownChangeDestinationTimer += Time.deltaTime * 0.5f;
                    controller.animator.SetBool("isPatrolling", false);
                    controller.agent.speed = 0;
                    return;
                }
            }
            else {
                controller.animator.SetBool("isPatrolling", true);
                controller.agent.speed = speedPatrol;
            }
        }
        public void OnHurt(StateController controller) {
            controller.ChangeState(controller.hurtState);
        }
        public void OnExit(StateController controller) {
            controller.animator.SetBool("isPatrolling", false);
            cooldownChangeDestinationTimer = 0;
        }
    }

}