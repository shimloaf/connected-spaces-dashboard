using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public static class Extensions
{
    public static Dictionary<V, K> Reverse<K, V>(this IDictionary<K, V> dict)
    {
        var inverseDict = new Dictionary<V, K>();
        foreach (var kvp in dict)
        {
            if (!inverseDict.ContainsKey(kvp.Value)) {
                inverseDict.Add(kvp.Value, kvp.Key);
            }
        }
        return inverseDict;
    }
}

public class DashboardDisplay : MonoBehaviour
{
    public Dashboard curData;
    public int DashboardType;

    public GameObject LargeAffinityPrefab;
    public GameObject affinityPrefab;

    public GameObject affinityZone;

    public Text username;
    public Text bio;

    public Image dashboardBackground;
    public Image header;
    public Image profilePicture;

    public Image selectedCover;
    public Image AlertBackground;

    public bool ShowAlert = false;

    public bool CustomizationProfile = false;
    public bool isEnabled = false;

    public void ClearDashboard() {
        curData = new Dashboard();
        UpdateDisplay();
        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
    }

    void Update() {

        if (!CustomizationProfile) {
            if (ShowAlert) {
                AlertBackground.color = Color.red;

                if (dashboardBackground.sprite == null) {
                    dashboardBackground.color = new Color(dashboardBackground.color.r, dashboardBackground.color.g, dashboardBackground.color.b, 0.8f);
                }

            } else {
                AlertBackground.color = Color.black;

                if (dashboardBackground.sprite == null) {
                    dashboardBackground.color = new Color(dashboardBackground.color.r, dashboardBackground.color.g, dashboardBackground.color.b, 1.0f);
                }
            }
        }
    }

    public void ClearAffinities() {
        int children = affinityZone.transform.childCount;
        for (int i = children - 1; i >= 0; i--) {
            GameObject.Destroy(affinityZone.transform.GetChild(i).gameObject);
        }
    }

    public void ToggleAlert(string AffinityType) {
        foreach (string affinityName in curData.affinityNames) {
            if (affinityName == AffinityType) {
                ShowAlert = true;
                return;
            }
        }
        ShowAlert = false;
    }

    public void AddAffinity(string s, bool large) {

        GameObject go;

        if (large) {
            go = Instantiate(LargeAffinityPrefab, affinityZone.transform);
            go.GetComponent<Affinity>().affinityName = s;
            go.GetComponent<Affinity>().UpdateAffinity();
        } else {
            go = Instantiate(affinityPrefab, affinityZone.transform);
            go.GetComponent<Affinity>().affinityName = s;
            go.GetComponent<Affinity>().UpdateAffinity();
        }
        
        if (go.GetComponent<DraggablePreview>() != null) { 
            go.GetComponent<DraggablePreview>().enabled = false;
        }

        if (CustomizationProfile) {
            go.GetComponent<Affinity>().Editable = true;
        }
    }

    public void UpdateInfo() {
        username.text = curData.username;

        if (CustomizationProfile) {
            username.GetComponent<InputField>().text = curData.username;
        }

        if (bio != null) {
            bio.text = curData.bio;
            bio.color = curData.bioColor;

            if (CustomizationProfile) {
                bio.GetComponent<InputField>().text = curData.bio;
            }
        }
        username.color = curData.usernameColor;
        header.sprite = null;
        header.color = curData.headerColors[0];

        profilePicture.sprite = null;
        profilePicture.color = curData.profileColor;
            
        dashboardBackground.sprite = null;
        dashboardBackground.color = curData.backgroundColor;

        if (curData.affinityNames.Count <= (3 * (curData.layoutType + 1)) && !CustomizationProfile) {
            affinityZone.GetComponent<GridLayoutGroup>().spacing = new Vector2(0, 40);
        } else if (!CustomizationProfile) {
            affinityZone.GetComponent<GridLayoutGroup>().spacing = new Vector2(0, 0);
        }

        ClearAffinities();
        foreach (string affinityName in curData.affinityNames) {

            if (curData.affinityNames.Count > (3 * (curData.layoutType + 1)) || CustomizationProfile) {
                AddAffinity(affinityName, false);
            } else {
                AddAffinity(affinityName, true);
            }
        }
    }

    public void UpdateDisplay() {

        UpdateInfo();

        if (!Application.isEditor && !CustomizationProfile) {
            LoadImages();
        } else {
            LoadLocalImages("");
        }

        UpdateImages();

        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
    }

    public void LoadLocalImages(string specificImage) {

        Texture2D tex = null;

        if (curData.headerImages[0] != -1 && (specificImage == "" || specificImage == "header")) {

            tex = curData.userPictures[curData.headerImages[0]];

            header.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            
            header.SetNativeSize();
            header.GetComponent<RectTransform>().localPosition = curData.headerPositions[0];
            header.GetComponent<RectTransform>().localScale = curData.headerScales[0];
        }

        if (curData.profileImage != -1 && (specificImage == "" || specificImage == "profile")) {
            tex = curData.userPictures[curData.profileImage];

            profilePicture.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            
            profilePicture.SetNativeSize();
            profilePicture.GetComponent<RectTransform>().localPosition = curData.profilePosition;
            profilePicture.GetComponent<RectTransform>().localScale = curData.profileScale;
        }
        
        if (curData.backgroundImage != -1 && (specificImage == "" || specificImage == "background")) {
            tex = curData.userPictures[curData.backgroundImage];

            dashboardBackground.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            dashboardBackground.color = new Color(dashboardBackground.color.r, dashboardBackground.color.g, dashboardBackground.color.b, 0.8f);
        }

        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
    }

    public void UpdateImages() {
        if (curData.backgroundImage != -1) {
            Color col = dashboardBackground.GetComponent<Image>().color;
            col.a = 0.8f;
            dashboardBackground.GetComponent<Image>().color = col;
        }
    }

    public void LoadImages() {

        if (curData.headerImages != null && curData.headerImages.Count > 0 && curData.headerImages[0] != -1) {
            FirebaseStorage.DownloadFile("dashboard_photos/" + curData.userID + "/" + curData.headerImages[0], gameObject.name, "AddHeaderImage", "HeaderImageError");
        }

        if (curData.profileImage != -1) {
            FirebaseStorage.DownloadFile("dashboard_photos/" + curData.userID + "/" + curData.profileImage, gameObject.name, "AddProfileImage", "ProfileImageError");
        }

        if (curData.backgroundImage != -1) {
            FirebaseStorage.DownloadFile("dashboard_photos/" + curData.userID + "/" + curData.backgroundImage, gameObject.name, "AddBackgroundImage", "BackgroundImageError");
        }
    }

    void AddHeaderImage(string data) {

        var bytes = Convert.FromBase64String(data);
        Texture2D tex = new Texture2D(6, 9);
        tex.LoadImage(bytes);

        header.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        
        header.GetComponent<RectTransform>().localPosition = curData.headerPositions[0];
        header.GetComponent<RectTransform>().localScale = curData.headerScales[0];
        header.SetNativeSize();
        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
    }

    void HeaderImageError(string error) {
        Debug.Log("Error downloading header Image: " + error);
    }

    void AddBackgroundImage(string data) {
        var bytes = Convert.FromBase64String(data);
        Texture2D tex = new Texture2D(6, 9);
        tex.LoadImage(bytes);
        
        dashboardBackground.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        dashboardBackground.color = new Color(dashboardBackground.color.r, dashboardBackground.color.g, dashboardBackground.color.b, 0.8f);
        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
    }

    void BackgroundImageError(string error) {
        Debug.Log("Error downloading background Image: " + error);
    }

    void AddProfileImage(string data) {
        var bytes = Convert.FromBase64String(data);
        Texture2D tex = new Texture2D(6, 9);
        tex.LoadImage(bytes);

        profilePicture.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        
        profilePicture.SetNativeSize();
        profilePicture.GetComponent<RectTransform>().localPosition = curData.profilePosition;
        profilePicture.GetComponent<RectTransform>().localScale = curData.profileScale;
        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
        LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
    }

    void ProfileImageError(string error) {
        Debug.Log("Error downloading profile Image: " + error);
    }

    public void EnableDashboard() {
        selectedCover.enabled = false;
        isEnabled = true;
    }

    public void DisableDashboard() {
        selectedCover.enabled = true;
        isEnabled = false;
    }

}
