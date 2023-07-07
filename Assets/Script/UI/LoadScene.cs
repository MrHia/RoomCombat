using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RoomCombat {
    public class LoadScene : MonoBehaviour {

        [SerializeField] private Transform loadScenePanel;
        [SerializeField] private string nameScene;
        [SerializeField] private int index;
        [SerializeField] private Slider slider;
        public float duration = 5f; // Thời gian tải scene mới (giây)
        void Start() {
            StartCoroutine(LoadSceneWithProgressBar(nameScene, duration));
            //LoadSceneInGame();
        }

        private void LoadSceneInGame() {
            SceneManager.LoadScene(nameScene);
        }

        IEnumerator LoadSceneWithProgressBar(string sceneName, float loadTime) {
            /*float delay = 5f;
            float count = 0;
            while (count <= 10) {
                Debug.Log(count);
                count = count + Time.deltaTime;
                float progess = count * 0.1f;
                sliderLoad.value = progess;
            }
            yield return new WaitForSeconds(delay);
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            while (!operation.isDone) {
                float progess = Mathf.Clamp01(operation.progress / 0.9f);
                sliderLoad.value = progess;
                yield return null;
            }*/
            float timer = 0f;
            float startValue = slider.value;
            float targetValue = 1f;

            while (timer < loadTime) {
                timer += Time.deltaTime;
                float progress = Mathf.Lerp(startValue, targetValue, timer / loadTime);
                slider.value = progress;
                yield return null;
            }

            // Thực hiện tải scene mới khi hoàn thành
            SceneManager.LoadScene(sceneName);
        }
    }
}
