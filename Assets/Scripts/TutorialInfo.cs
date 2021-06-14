using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TutorialInfo : MonoBehaviour
    {
        // allow user to choose whether to show this menu 
        public bool showAtStart = true;

        // store the GameObject which renders the overlay info
        public GameObject overlay;

        // store a reference to the audio listener in the scene, allowing for muting of the scene during the overlay
        public AudioListener mainListener;

        // string to store Prefs Key with name of preference for showing the overlay info
        public static string showAtStartPrefsKey = "showLaunchScreen";

        // used to ensure that the launch screen isn't more than once per play session if the project reloads the main scene
        private static bool alreadyShownThisSession = false;

        // we store user input in here for number of columns in level
        public InputField columnsInputField;

        // we store user input in here for number of rows in level
        public InputField rowsInputField;

        // we store user input in here for min number of obstacles in level
        public Text minObstaclesInputField;

        // we store user input in here for max number of obstacles in level
        public Text maxObstaclesInputField;

        void Awake()
        {
            // have we already shown this once?
            if (alreadyShownThisSession)
            {
                return;
            }

            alreadyShownThisSession = true;

            // Check player prefs for show at start preference
            if (PlayerPrefs.HasKey(showAtStartPrefsKey))
            {
                showAtStart = PlayerPrefs.GetInt(showAtStartPrefsKey) == 1;
            }

            // show the overlay info or continue to play the game
            if (showAtStart)
            {
                ShowLaunchScreen();
            }
        }

        private void Start()
        {
            columnsInputField.onEndEdit.AddListener((str) =>
            {
                OnInputFieldValueChanged();
            });

            rowsInputField.onEndEdit.AddListener((str) =>
            {
                OnInputFieldValueChanged();
            });
        }

        // show overlay info, pausing game time, disabling the audio listener 
        // and enabling the overlay info parent game object
        public void ShowLaunchScreen()
        {
            Time.timeScale = 0f;
            mainListener.enabled = false;
            overlay.SetActive(true);
        }

        // continue to play, by ensuring the preference is set correctly, the overlay is not active, 
        // and that the audio listener is enabled, and time scale is 1 (normal)
        public void StartGame()
        {
            overlay.SetActive(false);
            mainListener.enabled = true;
            Time.timeScale = 1f;
            if (int.TryParse(columnsInputField.text, out int columns) && int.TryParse(rowsInputField.text, out int rows)
                && int.TryParse(minObstaclesInputField.text, out int minObstacles) && int.TryParse(maxObstaclesInputField.text, out int maxObstacles))
            {
                Managers.GameManager.instance.InitGame(columns, rows, minObstacles, maxObstacles);
            }
        }

        //Keeps user input for columns and rows at set min/max range
        public void OnInputFieldValueChanged()
        {
            var minColumns = Managers.GameManager.instance.minColumns;
            var maxColumns = Managers.GameManager.instance.maxColumns;
            var minRows = Managers.GameManager.instance.minRows;
            var maxRows = Managers.GameManager.instance.maxRows;

            if (int.TryParse(columnsInputField.text, out int columns))
            {
                if (columns < minColumns)
                {
                    columnsInputField.text = minColumns.ToString();
                }
                else if (columns > maxColumns)
                {
                    columnsInputField.text = maxColumns.ToString();
                }
            }

            if (int.TryParse(rowsInputField.text, out int rows))
            {
                if (rows < minRows)
                {
                    rowsInputField.text = minRows.ToString();
                }
                else if (rows > maxRows)
                {
                    rowsInputField.text = maxRows.ToString();
                }
            }
        }
    }
}
