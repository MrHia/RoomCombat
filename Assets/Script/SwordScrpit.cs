using Unity.VisualScripting;
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
                if (other.GetComponentInChildren<HealthBar>())
                    other.GetComponentInChildren<HealthBar>().TakeDame(character.attackDame, character);
                if (other.GetComponent<StateController>())
                    other.GetComponent<StateController>().OnHurt();
                //other.GetComponent<Character>().GetComponent().HurtStart();
                if (other.GetComponent<EnemyAI1>())
                    other.GetComponent<EnemyAI1>().HurtStart();
                if (other.GetComponent<PlayerController>())
                    other.GetComponent<PlayerController>().HurtStart();
                PushEnemyInAttack(other.GetComponent<Rigidbody>());
            }

        }
        float pushFocre = 7.5f;
        public void PushEnemyInAttack(Rigidbody rb) {
            Vector3 directionToEnenmy = rb.transform.position - m_Collider.transform.position;
            directionToEnenmy.y = 0f;
            rb.AddForce(directionToEnenmy.normalized * pushFocre, ForceMode.Impulse);
        }

    }

}
