using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FullSerializer;

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public class AmbientDashboardLoader : MonoBehaviour
{
    public ButtonManager buttonManager;

    public List<GameObject> dashboards;

    public GameObject gridlist;

    public GameObject[] dashboardGrids;

    public GameObject[] titles;

    public GameObject[] dashPrefabs;
    void Start() {
        if (!Application.isEditor) {
            ListenForValueChanged();
        } else {
            DashboardData.allUserDashboards = new List<Dashboard>();
            DashboardData.allUserDashboards.Add(new Dashboard());
            DashboardData.allUserDashboards.Add(new Dashboard());
            DashboardData.allUserDashboards.Add(new Dashboard());
            DashboardData.allUserDashboards.Add(new Dashboard());
            DashboardData.allUserDashboards.Add(new Dashboard());
            DashboardData.allUserDashboards.Add(new Dashboard());
            AddDashboards();
        }
    }

    void Update() {
        if(Input.GetKeyUp(KeyCode.A)) {
            ToggleDashboard(0);
        }
        if(Input.GetKeyUp(KeyCode.W)) {
            ToggleDashboard(1);
        }
        if(Input.GetKeyUp(KeyCode.D)) {
            ToggleDashboard(3);
        }
        if(Input.GetKeyUp(KeyCode.S)) {
            ToggleDashboard(2);
        }
        if(Input.GetKeyUp(KeyCode.M)) {
            ToggleDashboard(4);
        }
        if(Input.GetKeyUp(KeyCode.R)) {
            ToggleDashboard(5);
        }
        if(Input.GetKeyUp(KeyCode.Minus)) {
            ToggleDashboard(1);
            ToggleDashboard(3);
            ToggleDashboard(2);
            ToggleDashboard(4);
            ToggleDashboard(5);
            ToggleDashboard(0);
        }
        if (Input.GetKeyUp(KeyCode.Equals)) {
            GetDashboardUpdate();
        }
    }

    public void ToggleTitles() {
        foreach (GameObject j in titles) {
            j.SetActive(!j.activeSelf);
        }
    }

    public void ListenForValueChanged() =>
        FirebaseDatabase.ListenForValueChanged("LastTimeUpdated", gameObject.name, "GetDashboardUpdate", "LogError");

    void GetDashboardUpdate() {
        FirebaseDatabase.GetJSON("dashboards", gameObject.name, "InitializeDashboards", "LogError");
    }

    void InitializeDashboards(string data) {
        DashboardData.allUserDashboards = null;
        DashboardData.allUserDashboards = DeserializeDashboards(data);
        AddDashboards();
    }
    
    void LogError(string error) {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
        Debug.Log(parsedError);
    }

    void ClearDashboards() {
        for (int n = 0; n < dashboardGrids.Length; n++) {
            foreach (Transform child in dashboardGrids[n].transform) {
                GameObject.Destroy(child.gameObject);
            }
            LayoutRebuilder.MarkLayoutForRebuild(dashboardGrids[n].GetComponent<RectTransform>());
        }
        Canvas.ForceUpdateCanvases();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        
        //buttonManager.dashboards = new List<GameObject>();
    }

    public void ToggleDashboard(int dashboardNum) {
        dashboardGrids[dashboardNum].SetActive(!dashboardGrids[dashboardNum].activeSelf);
        if (dashboardNum > 0) {
            titles[dashboardNum - 1].SetActive(!titles[dashboardNum - 1].activeSelf);
        }
    }

    void RebuildDashboards() {
        for (int n = 0; n < dashboardGrids.Length; n++) {
            LayoutRebuilder.MarkLayoutForRebuild(dashboardGrids[n].GetComponent<RectTransform>());
        }
        LayoutRebuilder.MarkLayoutForRebuild(gridlist.GetComponent<RectTransform>());
    }

    void AddDashboards() {
        ClearDashboards();

        foreach (Dashboard d in DashboardData.allUserDashboards) {

            GameObject dashPrefab = dashPrefabs[d.layoutType];
            
            GameObject specificDashboardEntry = Instantiate(dashPrefab, dashboardGrids[d.currentLocation].transform);
            specificDashboardEntry.name = d.userID + "@" + d.currentLocation;
            specificDashboardEntry.GetComponent<DashboardDisplay>().curData = d;
            
            if (dashboardGrids[d.currentLocation].activeSelf) {
                specificDashboardEntry.GetComponent<DashboardDisplay>().UpdateDisplay();
            }

            GameObject allDashboardEntry = Instantiate(dashPrefab, dashboardGrids[0].transform);
            allDashboardEntry.name = d.userID;
            allDashboardEntry.GetComponent<DashboardDisplay>().curData = d;
            
            if (dashboardGrids[0].activeSelf) {
                allDashboardEntry.GetComponent<DashboardDisplay>().UpdateDisplay();
            }
        }

        RebuildDashboards();

        Canvas.ForceUpdateCanvases();
    }

    private static readonly fsSerializer _serializer = new fsSerializer();

    public static List<Dashboard> DeserializeDashboards(string serializedState) {
        // step 1: parse the JSON data
        fsData data = fsJsonParser.Parse(serializedState);

        List<Dashboard> dashboardList = new List<Dashboard>();

        foreach (KeyValuePair<string, fsData> entry in data.AsDictionary) {

            // step 2: deserialize the data
            object deserialized = null;
            try {
                _serializer.TryDeserialize(entry.Value.AsDictionary["dashboard"], typeof(Dashboard), ref deserialized).AssertSuccessWithoutWarnings();
                dashboardList.Add((Dashboard) deserialized);
            } catch (Exception e) {
                Debug.Log("Dashboard not found for user: " + entry.Key);
            }
        }

        return dashboardList;
    }
}
