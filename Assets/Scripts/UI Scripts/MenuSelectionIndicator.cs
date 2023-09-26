using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelectionIndicator : MonoBehaviour
{

    public Image doohicky;
    public Transform[] buttonPositions;

    public int buttonSelected = 0;

    void Start() { doohicky.enabled = false; }

    public void SelectButton(int button) {
        doohicky.enabled = (button != -1); 
        buttonSelected = button;
    }

    void Update() {
        
        if (doohicky.enabled) {
            doohicky.transform.position = buttonPositions[buttonSelected].position;
        }
    }

}
