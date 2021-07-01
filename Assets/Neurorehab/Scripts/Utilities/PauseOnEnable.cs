using UnityEngine;

namespace Neurorehab.Scripts.Utilities
{
    
    public class PauseOnEnable : MonoBehaviour
    {
        public static PauseOnEnable Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                PauseUnPauseGame();
            }
        }

        public void PauseUnPauseGame()
        {
            if (Time.timeScale == 0)
            {
                print("Unpause");
                Time.timeScale = 1;
            }
            else
            {
                print("Pause");
                Time.timeScale = 0;
            }
        }

        public void PauseBasedOnScale(Behaviour component)
        {
            if (component.enabled == false)
            {
                print("Unpause");
                Time.timeScale = 1;
            }
            else
            {
                print("Pause");
                Time.timeScale = 0;
            }}
    }
}
