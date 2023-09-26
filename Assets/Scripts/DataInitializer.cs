using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using FullSerializer;

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public class DataInitializer : MonoBehaviour
{

    private static readonly fsSerializer _serializer = new fsSerializer();
    
    public string userID;
    public int curLocation;
    public void StartApplication() {
        SceneManager.LoadScene("Dashboard Customization", LoadSceneMode.Single);
    }

    public void TryPullUserData() {

        if (Application.isEditor) {
            NoUserFound();
        } else {
            string path = "dashboards/" + userID + "/dashboard";
            FirebaseDatabase.GetJSON(path, gameObject.name, "InitializeData", "LogError");
        }
    }

    void NoUserFound() {
        Debug.Log("No user found.");
        DashboardData.currentUserDashboard = new Dashboard(userID);

        AddDefaultImages();

        DashboardData.currentUserDashboard.currentLocation = curLocation;
        StartApplication();
    }

    void InitializeData(string data) {

        if (data == null || data == "null") {
            NoUserFound();
        } else {
            Dashboard d = (Dashboard) Deserialize(typeof(Dashboard), data);
            DashboardData.currentUserDashboard = d;
            DashboardData.currentUserDashboard.currentLocation = curLocation;
            DownloadImages();
        }
    }

    void AddDefaultImages() {
        DashboardData.currentUserDashboard.userPictures.Add(0, Resources.Load<Texture2D>("Default Images/dreaam"));
        DashboardData.currentUserDashboard.userPictures.Add(1, Resources.Load<Texture2D>("Default Images/well"));
        DashboardData.currentUserDashboard.userPictures.Add(2, Resources.Load<Texture2D>("Default Images/pixel"));
        DashboardData.currentUserDashboard.userPictures.Add(3, Resources.Load<Texture2D>("Default Images/snowday"));
        DashboardData.currentUserDashboard.userPictures.Add(4, Resources.Load<Texture2D>("Default Images/illinois"));
        DashboardData.currentUserDashboard.userPictures.Add(5, Resources.Load<Texture2D>("Default Images/bball"));
        DashboardData.currentUserDashboard.userPictures.Add(6, Resources.Load<Texture2D>("Default Images/flower"));
        DashboardData.currentUserDashboard.userPictures.Add(7, Resources.Load<Texture2D>("Default Images/usericon"));
    }

    int curImageDownloaded = 0;

    void DownloadImages() {
        curImageDownloaded = 0;
        if (DashboardData.currentUserDashboard.userPicturesAmount > 0) {
            DownloadCurrentImage();
        } else {
            AddDefaultImages();
        }
    }

    void DownloadCurrentImage() {
        if (curImageDownloaded == DashboardData.currentUserDashboard.userPicturesAmount) {
            StartApplication();
        } else {
            FirebaseStorage.DownloadFile("dashboard_photos/" + DashboardData.currentUserDashboard.userID + "/" + curImageDownloaded, gameObject.name, "ImageDownloaded", "ImageError");
        }
    }

    void ImageDownloaded(string imageData) {

        var bytes = Convert.FromBase64String(imageData);
        Texture2D tex = new Texture2D(6, 9);
        tex.LoadImage(bytes);

        DashboardData.currentUserDashboard.userPictures.Add(curImageDownloaded, tex);
        curImageDownloaded = curImageDownloaded + 1;
        DownloadCurrentImage();
    }

    void ImageError(string error) {
        Debug.Log("Error downloading image: " + error);
        DashboardData.currentUserDashboard.userPictures.Add(curImageDownloaded, null);
        curImageDownloaded = curImageDownloaded + 1;
        DownloadCurrentImage();
    }

    void LogError(string error) {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
        Debug.Log(parsedError);
    }

    public static object Deserialize(Type type, string serializedState) {
        // step 1: parse the JSON data
        fsData data = fsJsonParser.Parse(serializedState);

        // step 2: deserialize the data
        object deserialized = null;
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

        return deserialized;
    }
    
}
