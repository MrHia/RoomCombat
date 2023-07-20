using UnityEngine;
namespace RoomCombat {

    public class EscapeState : IState {
        private float timeRandomMax = 5f;
        private float timerRandom = 5f;
        public void OnEnter(StateController controller) {
            controller.m_rb.velocity = Vector3.zero;
            
            controller.animator.SetBool("isFollow", true);
            Vector3 nextPoint = controller.GetRandomPointInNavMesh();
            Debug.DrawRay(nextPoint, Vector3.up, Color.red, 2.0f);
            controller.agent.SetDestination(nextPoint);
            controller.agent.speed = 3.5f;
        }

        public void OnExit(StateController controller) {
            controller.animator.SetBool("isFollow", false);
        }

        public void OnHurt(StateController controller) {
            controller.ChangeState(controller.hurtState);
        }

        public void UpdateState(StateController controller) {
            if (controller.agent.remainingDistance <= controller.agent.stoppingDistance) {
                /*Vector3 point;
                if (controller.RandomPoint(controller.centrePoint.position, controller.patrolRadius, out point)) {
                    Debug.DrawRay(point, Vector3.up, Color.red, 2.0f);
                    controller.agent.SetDestination(point);
                    Debug.DrawRay(point, Vector3.up, Color.red, 2.0f);
                    controller.agent.SetDestination(point);
                }*/
                Vector3 nextPoint = controller.GetRandomPointInNavMesh();
                Debug.DrawRay(nextPoint, Vector3.up, Color.red, 2.0f);
                controller.agent.SetDestination(nextPoint);
            }
            if (controller.CheckisEnemyinSphere(controller.chaseRadius)) {
                if (timerRandom < 0) {
                    timerRandom = timeRandomMax;
                    int rate = Random.Range(0, 100);
                    if (rate < 30) {
                        controller.ChangeState(controller.chaseState);
                    }
                    else {
                        controller.ChangeState(controller.chaseState);
                    }
                }
                else {
                    timerRandom -= Time.deltaTime;
                }
            }
            if (controller.timerEscape <= 0) {
                controller.ChangeState(controller.chaseState);
            }
            else {
                controller.timerEscape -= Time.deltaTime;
            }
        }
    }
}