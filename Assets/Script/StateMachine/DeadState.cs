using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace RoomCombat {
    public class DeadState : IState {
        public void OnEnter(StateController controller) {
            controller.m_rb.velocity = Vector3.zero;
            controller.animator.SetBool("isPatrolling", false);
            controller.animator.SetBool("isFollow", false);
            controller.animator.SetBool("isHurt", false);
        }

        public void OnExit(StateController controller) {

        }

        public void OnHurt(StateController controller) {

        }

        public void UpdateState(StateController controller) {

        }
    }
}