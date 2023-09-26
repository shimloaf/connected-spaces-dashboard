using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandedMenu : MonoBehaviour
{

    public Image MenuBackground;
    public RectTransform DashboardsGroup;
    public GameObject[] menus;

    public MenuSelectionIndicator menuSelection;

    int MenuOpen = -1;


    public void ToggleMenu(int menu) {
        
        if (MenuOpen == menu) {
            CloseMenu();
            menuSelection.SelectButton(-1);
            return;
        }

        menuSelection.SelectButton(menu);

        foreach (GameObject m in menus) {
            m.SetActive(false);
        }
        menus[menu].SetActive(true);
        MenuOpen = menu;

        MenuBackground.enabled = true;
        DashboardsGroup.GetComponent<HorizontalLayoutGroup>().padding.left = 200;
        DashboardsGroup.localPosition += new Vector3(100, 0, 0);
        LayoutRebuilder.MarkLayoutForRebuild(DashboardsGroup);

    }

    public void CloseMenu() {

        if (MenuOpen == -1) {
            return;
        }

        MenuOpen = -1;
        MenuBackground.enabled = false;
        foreach (GameObject m in menus) {
            m.SetActive(false);
        }
        DashboardsGroup.GetComponent<HorizontalLayoutGroup>().padding.left = 100;
        DashboardsGroup.localPosition -= new Vector3(100, 0, 0);
        LayoutRebuilder.MarkLayoutForRebuild(DashboardsGroup);
    }


    void Start() {
        MenuBackground.enabled = false;
        
        foreach (GameObject m in menus) {
            m.SetActive(false);
        }
    }

    void Update() {
        if(Input.GetKey("escape")) {
            ToggleMenu(MenuOpen);
        }
    }
}
