using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationManager : MonoBehaviour
{
    public DashboardEncoder dashboardEncoder;

    public int currentEditorLayout;

    public DashboardDisplay[] dashboardDisplays;
    public Text UserIDDisplay;

    //Initialization
    public void Start() {
        
        if (DashboardData.currentUserDashboard == null) {
            DashboardData.currentUserDashboard = new Dashboard("nobody");
        }

        Dashboard curDash = DashboardData.currentUserDashboard;
        
        foreach(DashboardDisplay d in dashboardDisplays) {
            d.DisableDashboard();
        }

        currentEditorLayout = curDash.layoutType;

        dashboardDisplays[currentEditorLayout].EnableDashboard();
        dashboardDisplays[currentEditorLayout].curData = curDash;
        dashboardDisplays[currentEditorLayout].UpdateDisplay();

        FocusScrollView();
        InitializeImageGrid();
        
        UserIDDisplay.text = curDash.userID + "@" + DashboardData.locationNumbers[curDash.currentLocation];
    }

    public RectTransform scrollContent;

    void FocusScrollView() {
        Canvas.ForceUpdateCanvases();
        Vector2 anchor = scrollContent.anchoredPosition;
        anchor.x = currentEditorLayout * (-400f);
        scrollContent.anchoredPosition = anchor;
    }

    // Dashboard Customization Displays

    public Dashboard GetCurrentEditorLayoutDashboard() {
        return dashboardEncoder.EncodeDashboard(dashboardDisplays[currentEditorLayout]);
    }

    public void SwitchDashboardFocus(int layout) {
        Dashboard curEditorDashboard = GetCurrentEditorLayoutDashboard();
        dashboardDisplays[currentEditorLayout].DisableDashboard();

        if (layout == 0) {
            string tempBio = dashboardDisplays[currentEditorLayout].curData.bio;
            Color tempBioColor = dashboardDisplays[currentEditorLayout].curData.bioColor;
            dashboardDisplays[layout].curData = curEditorDashboard;
            dashboardDisplays[layout].curData.bio = tempBio;
            dashboardDisplays[layout].curData.bioColor = tempBioColor;
            dashboardDisplays[layout].UpdateInfo();
        } else {
            dashboardDisplays[layout].curData = curEditorDashboard;
            dashboardDisplays[layout].UpdateInfo();
        }

        dashboardDisplays[layout].header.sprite = dashboardDisplays[currentEditorLayout].header.sprite;
        dashboardDisplays[layout].header.SetNativeSize();
        dashboardDisplays[layout].header.GetComponent<DraggableMaskedImage>().UpdateAutoSize();

        dashboardDisplays[layout].dashboardBackground.sprite = dashboardDisplays[currentEditorLayout].dashboardBackground.sprite;

        dashboardDisplays[layout].profilePicture.sprite = dashboardDisplays[currentEditorLayout].profilePicture.sprite;
        dashboardDisplays[layout].profilePicture.SetNativeSize();
        dashboardDisplays[layout].profilePicture.GetComponent<DraggableMaskedImage>().UpdateAutoSize();

        currentEditorLayout = layout;
        
        dashboardDisplays[layout].EnableDashboard();
        LogChange();
    }

    // Clear Screen

    public GameObject ClearScreen;
    public bool changeMade = false;

    public void ClearEditorDashboard() {
        dashboardDisplays[currentEditorLayout].ClearDashboard();
        CloseClearScreen();
    }

    public void ShowClearScreen() {
        ClearScreen.SetActive(true);
    }

    public void CloseClearScreen() {
        ClearScreen.SetActive(false);
    }

    // Save Button

    public Image saveButton;
    public Text saveText;

    public void LogChange() {
        changeMade = true;
        saveButton.color = Color.white;
        saveText.GetComponent<Text>().color = Color.white;
        saveButton.GetComponent<Button>().enabled = true;
    }

    public void ResetSaveButton() {
        changeMade = false;
        saveButton.color = Color.gray;
        saveText.GetComponent<Text>().color = Color.gray;
        saveButton.GetComponent<Button>().enabled = false;
    }

    public void UploadCurrentEditorDashboard() {
        dashboardEncoder.UploadDashboard();
        ResetSaveButton();
    }

    // Image Grid
    
    public GameObject ImageGridItemPrefab;
    public GridLayoutGroup imageGrid;

    public void InitializeImageGrid() {

        foreach (KeyValuePair<int, Texture2D> p in DashboardData.currentUserDashboard.userPictures) {
            AddImage(p.Value, p.Key);
        }

    }

    public void AddImage(Texture2D texture, int imageNum) {

        if (texture != null)
        {
            // Create a Sprite from the Texture2D
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            GameObject obj = Instantiate(ImageGridItemPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            // Assign the Sprite to the Image component
            obj.GetComponent<Image>().sprite = sprite;
            obj.GetComponent<DraggablePreview>().draggedImage = imageNum;
            obj.GetComponent<RectTransform>().SetParent(imageGrid.GetComponent<RectTransform>());
            obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            obj.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Canvas.ForceUpdateCanvases();

        }
    }

}
