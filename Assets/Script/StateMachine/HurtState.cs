using UnityEngine;
namespace RoomCombat {
    public class HurtState : IState {
        public float cooldowntHurt = 1.1f;
        public float hurtTimer = 0;
        public void OnEnter(StateController controller) {
            controller.animator.SetBool("isHurt", true);
            controller.agent.enabled = false;
        }

        public void OnExit(StateController controller) {
            controller.animator.SetBool("isHurt", false);
            hurtTimer = 0f;
            controller.agent.enabled = true;
            controller.m_rb.mass = 10f;
        }

        public void OnHurt(StateController controller) {
            controller.ChangeState(controller.hurtState);
        }
        public void UpdateState(StateController controller) {
            if (controller.health <= 30 && controller.GetHpEnemy() >= controller.health && hurtTimer >= cooldowntHurt) {
                controller.ChangeState(controller.escapeState);
                return;
            }
            if (hurtTimer >= cooldowntHurt) {
                controller.ChangeState(controller.patrolState);
            }
            else {
                hurtTimer += Time.deltaTime;
            }
        }
    }
}