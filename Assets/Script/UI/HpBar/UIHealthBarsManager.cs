using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RoomCombat {
    public class UIHealthBarsManager : MonoBehaviour {
        [SerializeField] private Slider healthBarPrefab;
        [SerializeField] private Transform uiHealthBarTransform;
        private Camera mainCamera;
        [SerializeField] public Dictionary<int, Slider> healthBarStorage = new Dictionary<int, Slider>();
        private int index = 0;

        private void Awake() {
            mainCamera = Camera.main;
        }

        public int CreateHealthBar() {
            var healthBar = Instantiate(healthBarPrefab, uiHealthBarTransform);
            healthBarStorage[index] = healthBar;
            index++;
            return index - 1;
        }

        public void DeleteHealthBar(int id) {
            if (!healthBarStorage.ContainsKey(id)) {
                Debug.LogError($"RedFlag healthBarStorage doesn't contain key!!!");
                return;
            }
            if (healthBarStorage[id] != null)
                Destroy(healthBarStorage[id].gameObject);
            healthBarStorage.Remove(id);
            //Debug.Log($"Destroy id[{id}]");
        }

        public void UpdateValueHealthBar(int id, float value) {
            if (!healthBarStorage.ContainsKey(id)) {
                Debug.LogError($"RedFlag healthBarStorage doesn't contain key!!!");
                return;
            }
            healthBarStorage[id].value = value;
        }

        public void UpdatePositionHealthBar(int id, Vector3 position) {
            if (!healthBarStorage.ContainsKey(id)) {
                Debug.LogError($"RedFlag healthBarStorage doesn't contain key!!!");
                return;
            }
            healthBarStorage[id].gameObject.transform.position = mainCamera.WorldToScreenPoint(position);
        }
    }
}