using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Affinity : MonoBehaviour
{
    public string affinityName;
    public string categoryName;

    public Image affinityBackground;
    public Image affinityImage;
    public Text affinityText;

    public bool Editable = false;

    public void UpdateAffinity() {
        categoryName = DashboardData.categoryNames[affinityName];
        affinityBackground.color = DashboardData.categoryColors[categoryName];
        Texture2D affinityTex = Resources.Load<Texture2D>("Affinity Icons/" + affinityName);
        affinityImage.sprite = Sprite.Create(affinityTex, new Rect(0, 0, affinityTex.width, affinityTex.height), new Vector2(0.5f, 0.5f));
        if (affinityText != null) {
            affinityText.text = affinityName;
        }
    }

    public void RemoveAffinity() {
        if (Editable) {
            Destroy(gameObject);
        }
    }

    public void LogAffinityRemoved() {
        FindObjectOfType<CustomizationManager>().LogChange();
    }
    
    void Start() {
        UpdateAffinity();
    }
    
}
