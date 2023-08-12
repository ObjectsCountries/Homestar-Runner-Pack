using Wawa.Modules;
using Wawa.Extensions;
using Wawa.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class _broncoscript:ModdedModule{
    public GameObject button;
    public KMSelectable buttonpressing;
    public GameObject[] madeTrolleys;
    public GameObject score;
    public GameObject scoreCount;
    public Material[] cycle;
    private int matcycle = 0;
    private int scoreNum = 0;
    private settings broncoSettings;
    private int scoreNeeded;

    public sealed class settings{public int MinimumNumberRequired = 10;}
    
    protected override void Awake(){
        Get<KMNeedyModule>().OnNeedyActivation += OnNeedyActivation;
        Get<KMNeedyModule>().OnNeedyDeactivation += OnNeedyDeactivation;
        Get<KMNeedyModule>().OnTimerExpired += OnTimerExpired;
        buttonpressing.Add(onInteract: Press);
    }

    private void Start(){
        broncoSettings=new Config<settings>().Read();
        scoreNeeded = broncoSettings.MinimumNumberRequired;
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