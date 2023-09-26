using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenChanger : MonoBehaviour
{
    
    public GameObject UnsavedChangesDialogue;
    public CustomizationManager c;

    void Update()
    {
        if (Input.GetKey(KeyCode.Keypad1)) {
            SceneManager.LoadScene("Dashboard Ambient Display", LoadSceneMode.Single);
        } else if (Input.GetKey(KeyCode.Keypad2)) {
            SceneManager.LoadScene("Dashboard Customization", LoadSceneMode.Single);
        } else if (Input.GetKey(KeyCode.Keypad3)) {
            SceneManager.LoadScene("Test Scene", LoadSceneMode.Single);
        } else if (Input.GetKey(KeyCode.Escape)) {
            SceneManager.LoadScene("Dashboard Customization", LoadSceneMode.Single);
        }
    }

    public void SwitchFromCustomization() {
        if (c.changeMade) {
            UnsavedChangesDialogue.SetActive(true);
        } else {
            SceneManager.LoadScene("Dashboard Ambient Display", LoadSceneMode.Single);
        }
    }

    public void CloseUnsavedDialogue() {
        UnsavedChangesDialogue.SetActive(false);
    }

    public void LoadScene(string scene) {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
