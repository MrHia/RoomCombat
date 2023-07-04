using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
namespace RoomCombat {
    public class UIManager : MonoBehaviour {
        int countAlive;
        int countKill;
        [SerializeField] TextMeshProUGUI countKilltext;
        [SerializeField] TextMeshProUGUI countAlivetext;

        //public Transform settingDialog;
        public Transform settingDialog;
        public Transform WinnerPanel;
        public Transform LosePanel;
        [SerializeField] private PlayerController playerController;

        [SerializeField]  private string nameScene;

        private void Start() {
            countAlive = FindObjectsOfType<Character>().Length;
            countAlivetext.text = "Alive: " + countAlive;
            playerController = FindObjectOfType<PlayerController>();
            countKill = playerController.GetCountKill();
            countKilltext.text = "Kill: " + countKill;
        }
        public void Setting() {
            settingDialog.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        public void ExitSetting() {
            settingDialog.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
        [SerializeField] bool isOn = true;
        public void SoundSetting() {
            /*Debug.Log("Click");*/
            if (isOn) {

                FindObjectOfType<soundManager>().TurnOffSound();
                isOn = false;
            }
            else {
                FindObjectOfType<soundManager>().TurnOnSound();
                isOn = true;
            }
        }
        public void ShowKill() {

        }
        public void ReLoadScene() { 
            Time.timeScale = 1f;
            SceneManager.LoadScene(nameScene);
        }
        public void CountKillPlayer() {
            countKill = playerController.GetCountKill();
            countKilltext.text = "Kill: " + countKill.ToString();
        }
        public void CountAlive() {
            countAlive -= 1;
            countAlivetext.text = "Alive: " + countAlive.ToString();
            if(playerController == null) {
                return;
            }
            if(countAlive <= 1 && playerController.GetHp()>0) {
                //Debug.LogWarning(playerController.GetHp());
                Time.timeScale = 0f;
                WinnerPanel.gameObject.SetActive(true);
            }
        }
        public void LoseGame() {
            LosePanel.gameObject.SetActive(true);
            //Time.timeScale = 0f;
        }

    }
}