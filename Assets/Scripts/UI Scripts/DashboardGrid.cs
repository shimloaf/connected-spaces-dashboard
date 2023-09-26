using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashboardGrid : MonoBehaviour
{
    void Update() {

        if (Input.GetKeyDown("1")) {
            ChangeLayout(1);
        } else if (Input.GetKeyDown("2")) {
            ChangeLayout(2);
        } else if (Input.GetKeyDown("3")) {
            ChangeLayout(3);
        } else if (Input.GetKeyDown("4")) {
            ChangeLayout(4);
        } else if (Input.GetKeyDown("5")) {
            ChangeLayout(5);
        } else if (Input.GetKeyDown("6")) {
            ChangeLayout(6);
        } else if (Input.GetKeyDown("7")) {
            ChangeLayout(7);
        } else if (Input.GetKeyDown("8")) {
            ChangeLayout(8);
        } else if (Input.GetKeyDown("9")) {
            ChangeLayout(9);
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (curColumns < 9) {
                ChangeLayout(curColumns + 1);
            }
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (curColumns > 0) {
                ChangeLayout(curColumns - 1);
            }
        }
    }

    void Start() {
        ChangeLayout(9);
    }

    public int curColumns = 3;

    void ChangeLayout(int columns) {
        curColumns = columns;
        if (columns == 1) {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.94f, 0.94f, 1f);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2 (1000, 0);
        }
        if (columns == 2) {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 1f);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2 (1500, 0);
        }
        if (columns == 3) {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1f);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2 (1900, 0);
        }
        if (columns == 4) {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.35f, 0.35f, 1f);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2 (2200, 0);
        }
        if (columns == 5) {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.275f, 0.275f, 1f);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2 (2800, 0);
        }
        if (columns == 6) {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.23f, 0.23f, 1f);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2 (3300, 0);
        }
        if (columns == 7) {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.2f, 0.2f, 1f);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2 (3800, 0);
        }
        if (columns == 8) {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.175f, 0.175f, 1f);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2 (4300, 0);
        }
        if (columns == 9) {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.15f, 0.15f, 1f);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2 (4900, 0);
        }
        
        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
    }
}
