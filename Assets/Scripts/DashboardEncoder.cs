using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

using FullSerializer;

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public static class ExtensionMethod {
        public static Texture2D Decompress(this Texture2D source) {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
    }

public class DashboardEncoder : MonoBehaviour {

    public CustomizationManager c;

    public Dashboard EncodeDashboard(DashboardDisplay currentDashboard) {

        Dashboard encodedDash = new Dashboard();

        encodedDash.userPictures = DashboardData.currentUserDashboard.userPictures;
        encodedDash.username = currentDashboard.username.text;
        if (currentDashboard.bio != null) {
            encodedDash.bio = currentDashboard.bio.text;
            encodedDash.bioColor = currentDashboard.bio.GetComponent<Text>().color;
        }
        encodedDash.affinityNames = new List<string>();
        foreach (Affinity child in currentDashboard.affinityZone.GetComponentsInChildren<Affinity>()) {
            encodedDash.affinityNames.Add(child.affinityText.text);
        }
        encodedDash.usernameColor = currentDashboard.username.GetComponent<Text>().color;
        encodedDash.headerImages = new List<int>();

        //Once the header Images thing works, change this:

        if (currentDashboard.header.sprite != null) {
            encodedDash.headerImages.Add(currentDashboard.curData.headerImages[0]);
        } else {
            encodedDash.headerImages.Add(-1);
        }
        
        encodedDash.headerScales = new List<Vector3>();
        encodedDash.headerPositions = new List<Vector3>();
        encodedDash.headerColors = new List<Color>();
        encodedDash.headerScales.Add(currentDashboard.header.GetComponent<RectTransform>().localScale);
        encodedDash.headerPositions.Add(currentDashboard.header.GetComponent<RectTransform>().localPosition);
        encodedDash.headerColors.Add(currentDashboard.header.color);
        if (currentDashboard.dashboardBackground.sprite != null) {
            encodedDash.backgroundImage = currentDashboard.curData.backgroundImage;
        } else {
            encodedDash.backgroundImage = -1;
        }
        encodedDash.backgroundColor = currentDashboard.dashboardBackground.color;
        
        if (currentDashboard.profilePicture.sprite != null) {
            encodedDash.profileImage = currentDashboard.curData.profileImage;
        } else {
            encodedDash.profileImage = -1;
        }
        encodedDash.profileScale = currentDashboard.profilePicture.GetComponent<RectTransform>().localScale;
        encodedDash.profilePosition = currentDashboard.profilePicture.GetComponent<RectTransform>().localPosition;
        encodedDash.profileColor = currentDashboard.profilePicture.color;
        encodedDash.layoutType = currentDashboard.DashboardType;

        encodedDash.userID = DashboardData.currentUserDashboard.userID;
        encodedDash.currentLocation = DashboardData.currentUserDashboard.currentLocation;
        encodedDash.userPicturesAmount = DashboardData.currentUserDashboard.userPictures.Count;

        return encodedDash;
    }

    public void UploadDashboard() {
        DashboardData.currentUserDashboard = c.GetCurrentEditorLayoutDashboard();
        string jsonString = Serialize(typeof(Dashboard), (object) DashboardData.currentUserDashboard);
        FirebaseDatabase.PostJSON("dashboards/" + DashboardData.currentUserDashboard.userID + "/dashboard", jsonString, gameObject.name, "UploadSuccess", "UploadError");

        string jsonString2 = Serialize(typeof(string), (object) System.DateTime.Now);
        FirebaseDatabase.PostJSON("LastTimeUpdated", jsonString2, gameObject.name, "UploadSuccess", "UploadError");

        FirebaseDatabase.PostJSON("dashboard_history/" + DashboardData.currentUserDashboard.userID + "/" + (System.DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"), jsonString, gameObject.name, "UploadSuccess", "UploadError");

        UploadPictures(DashboardData.currentUserDashboard);
    }

    void UploadPictures(Dashboard d) {
        foreach (KeyValuePair<int, Texture2D> p in d.userPictures) {
            FirebaseStorage.UploadFile("dashboard_photos/" + DashboardData.currentUserDashboard.userID + "/" + p.Key, Convert.ToBase64String(p.Value.EncodeToJPG()), gameObject.name, "UploadSuccess", "UploadError");
        }
    }

    void UploadSuccess(string result) {
        Debug.Log(result);
    }

    void UploadError(string error) {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
        Debug.Log(parsedError);
    }

    private static readonly fsSerializer _serializer = new fsSerializer();

    public static string Serialize(Type type, object value) {
        // serialize the data
        fsData data;
        _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

        // emit the data via JSON
        return fsJsonPrinter.CompressedJson(data);
    }

}
