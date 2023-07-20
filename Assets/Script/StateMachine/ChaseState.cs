
using UnityEngine;
namespace RoomCombat {
    public class ChaseState : IState {
        public void OnEnter(StateController controller) {
            controller.m_rb.velocity = Vector3.zero;
            controller.animator.SetBool("isFollow", true);
            controller.agent.speed = 3.5f;
        }

        public void OnExit(StateController controller) {
            controller.animator.SetBool("isFollow", false);
        }

        public void OnHurt(StateController controller) {
            controller.ChangeState(controller.hurtState);
        }

        public void UpdateState(StateController controller) {
            if (controller.CheckisEnemyinSphere(controller.attackRadius)) {
                controller.ChangeState(controller.attackState);
                return;
            }
            controller.enemys = Physics.OverlapSphere(controller.centrePoint.position, controller.chaseRadius);
            controller.FindingClosestDistance(controller.enemys, ref controller.targetChase);
            if (controller.targetChase == null || controller.targetChase.GetComponent<Character>().GetHp() <= 0) {
                controller.ChangeState(controller.patrolState);
                return;
            }
            else {
                controller.agent.SetDestination(controller.targetChase.position);
                controller.agent.speed = 3.5f;
            }

        }

    }
}