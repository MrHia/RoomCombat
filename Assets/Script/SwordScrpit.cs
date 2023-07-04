using UnityEditor;
using UnityEngine;

namespace RoomCombat {
    public class SwordScrpit : MonoBehaviour {
        [SerializeField] private Collider m_Collider;
        [SerializeField] private HealthBar m_HealthBar;
        [SerializeField] private Character character;

        private void OnTriggerEnter(Collider other) {
            if (other == m_Collider) {
                return;
            }
            if (m_HealthBar.health <= 0) {

                return;
            }
            if (other.GetComponent<Character>()) {
                other.GetComponentInChildren<HealthBar>().TakeDame(character.attackDame, character);
                //PushEnemyInAttack(other.GetComponent<Rigidbody>());
                //Debug.Log("Call take dame");
            }
            
        }
        float pushFocre = 3;
        public void PushEnemyInAttack(Rigidbody rb) {
            Vector3 directionToEnenmy = rb.transform.position - transform.position;
            directionToEnenmy.y = 1;
            rb.AddForce(directionToEnenmy.normalized * pushFocre, ForceMode.Impulse);
        }

    }

}
