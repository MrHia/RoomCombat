using UnityEngine;
using UnityEngine.AI;

namespace RoomCombat {
    public class gameManager : MonoBehaviour {
        [SerializeField] GameObject playerPrefab;
        private GameObject player;
        Collider backgroundCollider;
        private void Awake() {
            backgroundCollider = GameObject.FindGameObjectWithTag("Ground").GetComponent<Collider>();
            player = Instantiate(playerPrefab);
            player.transform.position = RandomPosition();
        }
        private void Start() {
            
        }

        public Vector3 RandomPosition() {
            Vector3 sizeGround = backgroundCollider.bounds.size / 2;
            float posX;
            float posZ;
            Vector3 randomPoint;
            NavMeshHit hit;
            do {
                posX = Random.Range(-sizeGround.x, sizeGround.x);
                posZ = Random.Range(-sizeGround.z, sizeGround.z);
                randomPoint = new Vector3(posX, 0, posZ);
            } while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas));
            return hit.position;
        }
    }
}