using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RoomCombat {
    public class LoadScene : MonoBehaviour {

        [SerializeField] private Transform loadScenePanel;
        [SerializeField] private string nameScene;
        [SerializeField] private int index;
        [SerializeField] private Slider sliderLoad;
        void Start() {
            StartCoroutine(LoadAsynchromously(index));
            //LoadSceneInGame();
        }

        private void LoadSceneInGame() {
            SceneManager.LoadScene(nameScene);
        }

        IEnumerator LoadAsynchromously(int sceneIndex) {

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

            while (!operation.isDone) {
                float progess = Mathf.Clamp01(operation.progress / 0.9f);
                sliderLoad.value = progess;
                yield return null;
            }
        }
    }
}
