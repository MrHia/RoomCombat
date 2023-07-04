using UnityEngine;

namespace RoomCombat {

    public class AnimationEvent : MonoBehaviour {
        public Collider m_ColliderSw0rd;
        public void StartAttack() {
            m_ColliderSw0rd.enabled = true;
        }
        public void EndAttack() {
            m_ColliderSw0rd.enabled = false;
        }
    }
}
