﻿using wawa.Modules;
using wawa.Extensions;
using wawa.IO;
using wawa.Schemas;
using UnityEngine;

public class _broncoscript:ModdedModule{
    public GameObject button;
    public KMSelectable buttonpressing;
    public GameObject[] madeTrolleys;
    public GameObject score;
    public GameObject scoreCount;
    public Material[] cycle;
    private int matcycle = 0;
    private int scoreNum = 0;
    private Config<settings>broncoSettings;
    private int scoreNeeded;

    public sealed class settings{
        [TweaksSetting.Number("Minimum Number of Bronco Trolleys Required", "Can range from 10 to 15.")]
        public int BT_MinimumNumberRequired = 10;

        public settings(){}

        public settings(int minimum){
            BT_MinimumNumberRequired = Mathf.Clamp(minimum, 10, 15);
        }
    }

    static readonly TweaksEditorSettings TweaksEditorSettings = TweaksEditorSettings.CreateListing("Bronco Trolleys", "broncotrolleys").Register<settings>().BuildAndClear();


    /*
    public static Dictionary<string,object>[]TweaksEditorSettings=new Dictionary<string,object>[]{
    new Dictionary<string,object>{
        {"Filename","broncotrolleys-settings.json"},
        {"Name","Bronco Trolleys"},
        {"Listings",new List<Dictionary<string,object>>{
            new Dictionary<string,object>{{"Key","BT_MinimumNumberRequired"},{"Text","Minimum Number of Bronco Trolleys Required"},{"Description","Can range from 10 to 15."}}
            }
        }}
    };
    */

    protected override void Awake(){
        Get<KMNeedyModule>().OnNeedyActivation += OnNeedyActivation;
        Get<KMNeedyModule>().OnNeedyDeactivation += OnNeedyDeactivation;
        Get<KMNeedyModule>().OnTimerExpired += OnTimerExpired;
        buttonpressing.Add(onInteract: Press);
    }

    private void Start(){
        broncoSettings=new Config<settings>("broncotrolleys-settings.json");
        scoreNeeded = Mathf.Clamp(broncoSettings.Read().BT_MinimumNumberRequired,10,15);
        broncoSettings.Write(new settings(broncoSettings.Read().BT_MinimumNumberRequired));
        if (scoreNeeded < 10) scoreNeeded = 10;
        if (scoreNeeded > 15) scoreNeeded = 15;
        score.SetActive(false);
        scoreCount.SetActive(false);
        foreach (GameObject trolley in madeTrolleys) trolley.SetActive(false);
        button.SetActive(false);
        button.transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.05f, .025f), .01525f, UnityEngine.Random.Range(-0.0275f, -.0575f));
        Log("You need to make at least {0} bronco trolleys each time.", scoreNeeded);
    }

    protected void OnNeedyActivation(){
        Log("---");//to separate activations in the log
        matcycle = 0;
        scoreNum = 0;
        scoreCount.GetComponent<TextMesh>().text = "0";
        score.SetActive(true);
        scoreCount.SetActive(true);
        button.SetActive(true);
    }

    protected void OnNeedyDeactivation(){
        score.SetActive(false);
        scoreCount.SetActive(false);
        button.SetActive(false);
        foreach (GameObject trolley in madeTrolleys) trolley.SetActive(false);
    }

    protected void OnTimerExpired(){
        Log("Time Up!");
        OnNeedyDeactivation();
        Log("Made " + scoreNum.ToString() + " bronco trolleys.");
        if (scoreNum < scoreNeeded) Strike("Not enough made.");
    }

    void Press(){
        Play(new Sound("AUDIO_bronco_"+(matcycle%3).ToString()));
        matcycle++;
        button.GetComponent<Renderer>().material = cycle[matcycle%3];
        if (matcycle%3 == 0){
            if (scoreNum < 13) madeTrolleys[scoreNum].SetActive(true);
            scoreNum++;
            scoreCount.GetComponent<TextMesh>().text = scoreNum.ToString();
        } button.transform.localPosition = new Vector3 (UnityEngine.Random.Range(-0.05f,.025f),.01525f, UnityEngine.Random.Range(-0.0275f,-.0575f));
    }
}
