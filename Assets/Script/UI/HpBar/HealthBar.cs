using System;
using UnityEngine;

namespace RoomCombat {
    public class HealthBar : MonoBehaviour {
        [SerializeField] private Character character;
        private UIManager uiManager;

        //hp bar
        private float maxHealth;
        public float health;
        public Transform hpBarPos;


        //player
        private Action<float> setHpDelegate = null;
        private Action deadDelegate = null;
        //UI hpbar
        private UIHealthBarsManager healthBarsManager;
        private int indexHealBar;

        private bool isPlayer;

        private void Awake() {
            healthBarsManager = FindObjectOfType<UIHealthBarsManager>();
            uiManager = FindObjectOfType<UIManager>();
        }
        private void Start() {
            //GetCharacter();
            health = maxHealth = character.GetHp();
            setHpDelegate = character.SetHp;
            deadDelegate = character.Dead;
            indexHealBar = healthBarsManager.CreateHealthBar();
            healthBarsManager.UpdatePositionHealthBar(indexHealBar, transform.position);
            isPlayer = character.GetComponent<PlayerController>();
        }
        public void TakeDame(float dame, Character attacker) {
            if (health <= dame) {
                if (attacker != null) {
                    attacker.SetCountKill();
                }
                health -= dame;
                setHpDelegate(health);
                uiManager.CountAlive();
                uiManager.CountKillPlayer();
                deadDelegate();
                Destroy(this.gameObject);
                return;
            }
            health -= dame;
            setHpDelegate(health);
            healthBarsManager.UpdateValueHealthBar(indexHealBar, health / maxHealth);
        }
        private void OnDestroy() {
            healthBarsManager.DeleteHealthBar(indexHealBar);
        }
        private void LateUpdate() {
/*            if (isPlayer) return;*/
            healthBarsManager.UpdatePositionHealthBar(indexHealBar, transform.position);
        }
    }

}
