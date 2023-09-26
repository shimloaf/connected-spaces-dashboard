using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FullSerializer;

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public class HelpRequest {
    public int location;
    public int affinity;
    public bool request;
}

public class ButtonManager : MonoBehaviour
{
    
    public GameObject ButtonMessage;
    public TMP_Text affinityText;
    public TMP_Text originText;

    public AudioSource buttonAlertSound;

    void Start() {
        if (!Application.isEditor) {
            ListenForValueChanged();
        }
    }

    public void ListenForValueChanged() =>
        FirebaseDatabase.ListenForValueChanged("helpButton/request", gameObject.name, "RetrieveButtonJSON", "LogError");

    public void ButtonDataReceived(string JSONResult) {
        
        HelpRequest request = (HelpRequest) Deserialize(typeof(HelpRequest), JSONResult);

        if (request.request) {
            DisplayHelpRequest(request);
        } else {
            ClearHelpRequest();
        }
    }

    //public List<GameObject> dashboards;
    
    void LightUpDashboards() {
        //foreach (GameObject dashboard in dashboards) {
            //dashboard.GetComponent<DashboardDisplay>().ToggleAlert(curAffinity);
        //}
    }

    void ClearHelpRequest() {
        ButtonMessage.SetActive(false);
    }

    void DisplayHelpRequest(HelpRequest request) {
        originText.text = "ORIGIN: " + DashboardData.locationNumbers[request.location];
        affinityText.text = "AFFINITY: " + DashboardData.affinityTypeNumbers[request.affinity];
        ButtonMessage.SetActive(true);
        buttonAlertSound.Play();
    }

    public void RetrieveButtonJSON() =>
        FirebaseDatabase.GetJSON("helpButton", gameObject.name, "ButtonDataReceived", "LogError");

    public void StopListeningForValueChanged() => 
        FirebaseDatabase.StopListeningForValueChanged("helpButton/request", gameObject.name, "LogInfo", "LogError");

    public void LogError(string error) {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
        Debug.Log(parsedError);
    }

    public void LogInfo(string info) {
        Debug.Log(info);
    }
    
    private static readonly fsSerializer _serializer = new fsSerializer();

    public static object Deserialize(Type type, string serializedState) {
        // step 1: parse the JSON data
        fsData data = fsJsonParser.Parse(serializedState);

        // step 2: deserialize the data
        object deserialized = null;
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

        return deserialized;
    }
}
