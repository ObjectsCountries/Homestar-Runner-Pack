using Wawa.Modules;
using Wawa.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _sidhoffrenchmanscript:ModdedModule{
    public Material[] chars;
    public Material empty;
    public Renderer[] bgChars;
    public KMSelectable hoffButton;
    public KMSelectable frenchButton;
    public KMRuleSeedable ruleseed;
    public TextMesh scoreCount;
    public Material[] hrAnim;
    public Renderer hrBackdrop;
    private int score = 0;
    private int hoffCount = 0;
    private int frenchCount = 0;
    public string[] charNames;
    private List<int> chosenChars = new List<int>();
    private List<int> chosenSpots = new List<int>();
    public GameObject hoffB;
    public GameObject frenchB;
    public GameObject blink;
    public Color[] colors;
    internal bool active;


    private void Start(){
        var RND=ruleseed.GetRNG();
        Get<KMNeedyModule>().OnNeedyActivation += OnNeedyActivation;
        Get<KMNeedyModule>().OnTimerExpired += OnTimerExpired;
        Get<KMNeedyModule>().OnNeedyDeactivation += HideChars;
        TextMesh hoffText = hoffButton.GetComponentInChildren<TextMesh>();
        TextMesh frenchText = frenchButton.GetComponentInChildren<TextMesh>();
        hoffButton.Add(
                onHighlight: () => ColorChange(hoffText, 1),
                onHighlightEnded: () => ColorChange(hoffText, 0),
                onInteract: () => ColorChange(hoffText, 2),
                onInteractEnded: () => { ColorChange(hoffText, 1); StartCoroutine(Submitting(correct(), true)); });
        frenchButton.Add(
                onHighlight: () => ColorChange(frenchText, 1),
                onHighlightEnded: () => ColorChange(frenchText, 0),
                onInteract: () => ColorChange(frenchText, 2),
                onInteractEnded: () => { ColorChange(frenchText, 1); StartCoroutine(Submitting(correct(), false)); });
        hrBackdrop.material = hrAnim[0];
        HideChars();
        StartCoroutine(Blink());
    }

    void ColorChange(TextMesh button, int j){button.color = colors[j];}

    void HideChars(){
        active = false;
        for (int i = 0; i < 4; i++) bgChars[i].material = empty;
        hoffB.SetActive(false);
        frenchB.SetActive(false);
    }

    IEnumerator Blink(){
        while (true){
            yield return new WaitForSeconds(3);
            blink.SetActive(true);
            yield return new WaitForSeconds(1f / 12);
            blink.SetActive(false);
        }
    }

    protected bool Solve(){
        GetComponent<KMNeedyModule>().OnPass();
        HideChars();
        playing = true;
        return false;
    }

    protected void OnNeedyActivation(){
        active = true;
        StartCoroutine(Asking());
        //to separate activations in the log
        Log("---\nThe score is {0}.", score);
        chosenChars.Clear();
        chosenSpots.Clear();
        hoffCount = 0;
        frenchCount = 0;
        chooseChars();
        DetermineOption();
    }

    protected void OnTimerExpired(){
        active = false;
        Log("I'm sorry, we're out of time. Ding board? *BUZZ*");
        GetComponent<KMNeedyModule>().OnStrike();
        Solve();
    }

    void chooseChars(){
        List<string> presentChars = new List<string>();
        for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++){
            int index = UnityEngine.Random.Range(0, 4);
            while (chosenSpots.Contains(index)) index = UnityEngine.Random.Range(0, 4);
            chosenSpots.Add(index);
            int characterPresent = UnityEngine.Random.Range(0, 10);
            while (chosenChars.Contains(characterPresent)) characterPresent = UnityEngine.Random.Range(0, 10);
            chosenChars.Add(characterPresent);
            bgChars[index].material = chars[characterPresent];
            presentChars.Add(charNames[characterPresent]);
        } Log("Characters present: {0}", string.Join(", ", presentChars.ToArray()));
    }

    void DetermineOption(){
        for (int i = 0; i < chosenChars.Count; i++){
            //the first 5 materials correspond to Hoff, the other 5 to French
            if (chosenChars[i] < 5) hoffCount++;
            else frenchCount++;
        }
        Log("Correct Answer: Sid {0}man",(hoffCount == frenchCount && score % 2 == 0) || hoffCount > frenchCount ? "Hoff" : "French");
    }

    internal bool correct(){
        if (hoffCount == frenchCount) return (score % 2 == 0);
        return (hoffCount > frenchCount);
    }

    private bool playing = true;

    IEnumerator SayHoff(){
        Play(new Sound("AUDIO_sid_hoffLine"));
        hrBackdrop.material = hrAnim[1];
        yield return new WaitForSeconds(.25f);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.25f);
        hrBackdrop.material = hrAnim[2];
        yield return new WaitForSeconds(.25f);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.25f);
        hrBackdrop.material = hrAnim[1];
        yield return new WaitForSeconds(.2f);
        hrBackdrop.material = hrAnim[0];
    }

    IEnumerator SayFrench(){
        Play(new Sound("AUDIO_sid_frenchLine"));
        hrBackdrop.material = hrAnim[1];
        yield return new WaitForSeconds(.25f);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.1f);
        hrBackdrop.material = hrAnim[2];
        yield return new WaitForSeconds(.1f);
        hrBackdrop.material = hrAnim[1];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.155f);
        hrBackdrop.material = hrAnim[1];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[0];
    }

    IEnumerator Asking(){
        Play(new Sound("AUDIO_sid_ask"));
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[1];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[2];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[1];
        yield return new WaitForSeconds(.15f);
        hoffB.SetActive(true);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.3f);
        hrBackdrop.material = hrAnim[2];
        yield return new WaitForSeconds(.1f);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[1];
        yield return new WaitForSeconds(.2f);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[2];
        yield return new WaitForSeconds(.1f);
        frenchB.SetActive(true);
        hrBackdrop.material = hrAnim[1];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[1];
        yield return new WaitForSeconds(.15f);
        hrBackdrop.material = hrAnim[0];
        yield return new WaitForSeconds(.15f);
        playing = false;
    }

    internal IEnumerator Submitting(bool hoff, bool chosen){
        if (!playing){
            if (Get<KMNeedyModule>().GetNeedyTimeRemaining() <= 2) Get<KMNeedyModule>().SetNeedyTimeRemaining(2);
            //hoff is true, french is false
            StartCoroutine(hoff?SayHoff():SayFrench());
            playing = true;
            yield return new WaitForSeconds(1.5f);
            playing = false;
            Log("Selected Sid {0}man.",chosen?"Hoff":"French");
            if (hoff != chosen){
                GetComponent<KMNeedyModule>().HandleStrike();
                Log("No, I'm sorry. The correct answer... was \"E-mail\".");
            }else{
                score++;
                scoreCount.text = "score:" + score;
                Play(new Sound("AUDIO_sid_ding"));
                Log("Hooway!");
            } Solve();
        }
    }

    IEnumerator TwitchHandleForcedSolve(){
        if (active) yield return correct()?hoffButton:frenchButton;
        else yield return "sendtochaterror {0}, the module isn't currently active.";
        yield return null;
    }
    IEnumerator ProcessTwitchCommand(string command){
        command = command.ToLowerInvariant().Trim();
        yield return null;
        if (!active){
            yield return "sendtochaterror {0}, the module isn't currently active.";
            yield break;
        }
        if (command.Equals("h")) yield return Submitting(correct(), true);
        else if (command.Equals("f")) yield return Submitting(correct(), false);
        else yield return "sendtochaterror {0}, your command must consist of only the letter h or f.";
    }
}