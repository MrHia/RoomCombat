using UnityEngine;

namespace RoomCombat {
    public class CameraFollow : MonoBehaviour {
        public Camera Camera;
        public Transform FollowTarget;
        public Vector3 offset;
        public Vector3 rotate;
        void Start() {
            if (FindObjectOfType<PlayerController>())
                FollowTarget = FindObjectOfType<PlayerController>().transform;
            this.transform.Rotate(rotate);
        }

        private void LateUpdate() {
            Follow(FollowTarget);

        }
        private void Follow(Transform FollowTarget) {
            if (FollowTarget == null) return;
            Vector3 desiredPosition = FollowTarget.position + offset;
            this.transform.position = desiredPosition;
        }
    }
}