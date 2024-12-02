using wawa.Modules;
using wawa.Extensions;
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
    private int tiebreakerSwapped=0;


    private void Start(){
        var RND=ruleseed.GetRNG();
        if(RND.Seed != 1) {
            int n = 10;
            while (n > 1) {
                n--;
                int k = RND.Next(n+1);
                Material m=chars[k];
                chars[k]=chars[n];
                chars[n]=m;
                string name=charNames[k];
                charNames[k]=charNames[n];
                charNames[n]=name;
            }
            tiebreakerSwapped = RND.Next(2);
        }
        Get<KMNeedyModule>().Add(
            onNeedyActivation: () => OnNeedyActivation(),
            onNeedyDeactivation: () => HideChars(),
            onPass: () => HideChars(),
            onStrike: () => HideChars(),
            onTimerExpired: () => StartCoroutine(OnTimerExpired()));
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
        playing = true;
    }

    IEnumerator Blink(){
        while (true){
            yield return new WaitForSeconds(3);
            blink.SetActive(true);
            yield return new WaitForSeconds(1f / 12);
            blink.SetActive(false);
        }
    }
    void OnNeedyActivation(){
        active = true;
        StartCoroutine(Asking());
        //to separate activations in the log
        Log("---");
        Log("The score is "+score+".");
        chosenChars.Clear();
        chosenSpots.Clear();
        hoffCount = 0;
        frenchCount = 0;
        chooseChars();
        DetermineOption();
    }

    IEnumerator OnTimerExpired(){
        active = false;
        yield return StartCoroutine(correct()?SayHoff():SayFrench());
        Strike("I'm sorry, we're out of time. Ding board? *BUZZ*");
    }

    void chooseChars(){
        List<string> presentChars = new List<string>();
        int index,characterPresent;
        int times = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < times; i++){
            do {
                index = UnityEngine.Random.Range(0, 4);
            } while (chosenSpots.Contains(index));
            chosenSpots.Add(index);
            do{
                characterPresent = UnityEngine.Random.Range(0, 10);
            } while (chosenChars.Contains(characterPresent));
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
        Log("Correct Answer: Sid {0}man",(hoffCount == frenchCount && score % 2 == tiebreakerSwapped) || hoffCount > frenchCount ? "Hoff" : "French");
    }

    internal bool correct(){
        if (hoffCount == frenchCount) return (score % 2 == tiebreakerSwapped);
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
            if (hoff == chosen){
                Log("Hooway!");
                score++;
                scoreCount.text = "score:" + score;
                Play(new Sound("AUDIO_sid_ding"));
            }else{
                Strike("No, I'm sorry. The correct answer... was \"E-mail\".");
            }
            Get<KMNeedyModule>().HandlePass();
            HideChars();
        }
    }
}
