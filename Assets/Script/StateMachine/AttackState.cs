using UnityEngine;
namespace RoomCombat {
    public class AttackState : IState {
        private float timerAttack;
        public void OnEnter(StateController controller) {
            controller.m_rb.velocity = Vector3.zero;
            timerAttack = controller.cooldownAttack;
            controller.agent.speed = 0;
            controller.SoundEnable("Sword");
            controller.animator.SetTrigger("Attack");
        }

        public void OnExit(StateController controller) {  
        }

        public void OnHurt(StateController controller) {

            controller.ChangeState(controller.hurtState);
        }
        public void UpdateState(StateController controller) {

            if (controller.health <= 30 && controller.GetHpEnemy() >= controller.health && timerAttack <= 0f) {
                controller.ChangeState(controller.escapeState);
                return;
            }
            if (controller.GetHpEnemy() <= 0) {
                controller.ChangeState(controller.chaseState);
            }
            if (timerAttack <= 0f) {
                controller.ChangeState(controller.chaseState);
                return;
            }
            else {
                timerAttack -= Time.deltaTime;
                controller.lookAtEnemy();
            }

        }
    }
}