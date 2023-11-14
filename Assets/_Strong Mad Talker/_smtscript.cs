using Wawa.Modules;
using Wawa.Extensions;
using Wawa.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class _smtscript:ModdedModule{
    public KMSelectable[] buttons;
    public KMSelectable PSToggle;
    public KMRuleSeedable rs;
    public Material[] PSColors;
    public TextMesh PSText;
    public Renderer lipsync;
    public Material[]mouthShapes;
    public GameObject eyesclosed;
    internal bool playing=true;
    public List<string>onesylwords=new List<string>(){"JUICE","PULSE","CAKE","CHARM","NIGHT","CURLS"};
    private string onesylword;
    public List<string>multisylwords=new List<string>(){"SWEETYCAKES","CHEDDAR","DOUGLAS","GARBLEDINA","MANTIS","DIAPER","PROXIMITY","MOVIE","HORSES","WORKING","CASSEROLE","AROUND","DELUISE","BATTLESHIP","MOTORCYCLES","TRAINWRECK"};
    private List<string>allWords=new List<string>(){"JUICE","PULSE","CAKE","CHARM","NIGHT","CURLS","SWEETYCAKES","CHEDDAR","DOUGLAS","GARBLEDINA","MANTIS","DIAPER","PROXIMITY","MOVIE","HORSES","WORKING","CASSEROLE","AROUND","DELUISE","BATTLESHIP","MOTORCYCLES","TRAINWRECK"};
    private int onesylposition;
    private string TheDecoy;
    List<int>usedWords=new List<int>();
    internal List<string>wordlist=new List<string>();
    internal List<string>fulllist=new List<string>();
    private string mantis;
    internal int stage=0;
    public GameObject[] stars;
    public Color[] buttonColors;
    private int decoyPos;
    internal settings SMTSettings;
    private bool doneSpeaking=true;
    private List<int>[]checkOrders=new List<int>[]{
        new List<int>(){0,1,2,3,4,5},
        new List<int>(){0,1,2,3,4,5},
        new List<int>(){0,1,2,3,4,5},
        new List<int>(){0,1,2,3,4,5}
    };
    
    private string[]absolutePositions=new string[]{"TL","TR","ML","MR","BL","BR"};
    private string[]relativePositions=new string[]{"L","R","A","B"};

    public sealed class settings{
        public bool SMT_LogPlayingModeInteractions=true;
        public bool SMT_LogPlayingSolvingSwitch=true;
        public bool SMT_TPResetOnTypo=false;
    }

    public static Dictionary<string,object>[]TweaksEditorSettings=new Dictionary<string,object>[]{
    new Dictionary<string,object>{
        {"Filename","strongmadtalker-settings.json"},
        {"Name","Strong Mad Talker"},
        {"Listings",new List<Dictionary<string,object>>{
            new Dictionary<string,object>{{"Key","SMT_LogPlayingModeInteractions"},{"Text","Log Interactions in Playing Mode"}},
            new Dictionary<string,object>{{"Key","SMT_LogPlayingSolvingSwitch"},{"Text","Log Switching Between Playing and Solving Modes"}},
            new Dictionary<string,object>{{"Key","SMT_TPResetOnTypo"},{"Text","Twitch Plays: Reset Input on Typo"}}
            }
        }}
    };

    protected override void OnActivate(){
        foreach(KMSelectable button in buttons){
            button.Add(onHighlight:()=>button.GetComponentInChildren<TextMesh>().color=buttonColors[1],
                onHighlightEnded:()=>button.GetComponentInChildren<TextMesh>().color=buttonColors[0],
                onInteract:()=>{
                    Press(button);
                    button.GetComponentInChildren<TextMesh>().color=buttonColors[2];
                },
                onInteractEnded:()=>button.GetComponentInChildren<TextMesh>().color=buttonColors[1]);
        }
        PSToggle.Set(onInteract:PS);
    }

    

    void Start(){
        var rnd=rs.GetRNG();
        if(rnd.Seed==1){
            onesylwords=new List<string>(){"JUICE","PULSE","CAKE","CHARM"};
            checkOrders=new List<int>[]{
                new List<int>(){0,1,2,3,4,5},
                new List<int>(){0,3,1,4,2,5},
                new List<int>(){2,3,4,1,0,5},
                new List<int>(){2,3,1,0,5,4}
            };
            multisylwords=new List<string>(){"SWEETYCAKES","PROXIMITY","CHEDDAR","MOVIE","DOUGLAS","CASSEROLE","GARBLEDINA","WORKING","MANTIS","AROUND","DIAPER","HORSES"};
        }else{
            rnd.ShuffleFisherYates(onesylwords);
            foreach(List<int>order in checkOrders)
                rnd.ShuffleFisherYates(order);
            rnd.ShuffleFisherYates(multisylwords);
        }
        rnd.ShuffleFisherYates(allWords);
        SMTSettings=new Config<settings>("strongmadtalker-settings.json").Read();
        foreach(GameObject star in stars)
            star.SetActive(false);
        StartCoroutine(blink());
        onesylposition=UnityEngine.Random.Range(0,6);
        PlaceWords();
        foreach(KMSelectable button in buttons)
            button.GetComponentInChildren<TextMesh>().fontSize=resize(button.GetComponentInChildren<TextMesh>().text);
        TheDecoy=Decoy(OrderCheck(onesylword));
        Log("{0} is the decoy.",TheDecoy);
        if(TheDecoy=="MANTIS")
            pressingOrder(mantis);
        else pressingOrder(TheDecoy);
    }

    private List<string>pickWordSelectors(MonoRandom r,bool can1Syl, bool canThis){
        List<string>selectors=new List<string>(){absolutePositions[r.Next(0,6)],relativePositions[r.Next(0,4)],(r.Next(1,7)-1).ToString()};
        if(can1Syl)
            selectors.Add("1SYL");
        if(canThis)
            selectors.Add("THIS");
        return r.ShuffleFisherYates(selectors);
    }
    
    private string decideWordsForContainsRule(MonoRandom r){
        int contains=r.Next(0,2);
        int mode=r.Next(0,3);
        int numWords=r.Next(3,6);
        switch(mode){
            case 0:
                if(contains==0)
                    return "ONLY "+allWords[0];
                else
                    return "nONLY "+allWords[0];
            case 1:
                if(contains==0)
                    return "ANY "+string.Join(" ", allWords.GetRange(0,numWords+1).ToArray());
                else
                    return "nANY "+string.Join(" ", allWords.GetRange(0,numWords+1).ToArray());
            case 2:
            default:
                List<string>newMultiSylWords=new List<string>();
                foreach(string word in multisylwords)
                    newMultiSylWords.Add(word);
                if(contains==0)
                    return "ALL "+newMultiSylWords.GetRange(0,numWords+1).ToArray();
                else
                    return "nALL "+newMultiSylWords.GetRange(0,numWords+1).ToArray();
        }
    }
    
    private bool CHECKadjacent(MonoRandom r){
        List<string>w=pickWordSelectors(r,true,true);
        string[]adjacentType=new string[]{"O","D","OD"};
        string adj=adjacentType[r.Next(0,3)];
        string[]looping=new string[]{"L","NL"};
        string loop=looping[r.Next(0,2)];
        List<string>tempWordList=new List<string>();
        for(int i=0;i<6;i++){
            tempWordList.Add(buttons[i].GetComponentInChildren<TextMesh>().text);
        }
        int word0=tempWordList.IndexOf(w[0]);
        int word1=tempWordList.IndexOf(w[1]);
        switch(adj){
            case "O":
                if(loop=="L")
                    return (
                                (word0==(word1+2)%6)
                              ||(word0==(word1+4)%6)
                              ||(word0%2==0&&word0==word1-1)
                              ||(word0%2==1&&word0==word1+1)
                           );
                else
                    return (
                                (word1<4&&word0==word1+2)
                              ||(word1>1&&word0==word1-2)
                              ||(word0%2==0&&word0==word1-1)
                              ||(word0%2==1&&word0==word1+1)
                           );
            case "D":
                if(loop=="L")
                    return (
                                (word0%2==0&&word1==(word0+3)%6)
                              ||(word0%2==0&&word1==(word0+5)%6)
                              ||(word0%2==1&&word1==(word0+1)%6)
                              ||(word0%2==1&&word1==(word0+3)%6)
                           );
                else
                    return (
                                (word0==0&&word1==3)
                              ||(word0==1&&word1==2)
                              ||(word0==2&&word1==1)
                              ||(word0==2&&word1==5)
                              ||(word0==3&&word1==0)
                              ||(word0==3&&word1==4)
                              ||(word0==4&&word1==3)
                              ||(word0==5&&word1==2)
                           );
            case "OD":
            default:
                if(loop=="L")
                    return true;
                else
                    return (
                                (word0!=0&&word1!=5)
                              ||(word0!=1&&word1!=4)
                              ||(word0!=4&&word1!=1)
                              ||(word0!=5&&word1!=0)
                           );
        }
    }

    internal void PS(){
        if(Status.IsSolved)
            return;
        playing=!playing;
        if(playing){
            PSToggle.GetComponent<Renderer>().material=PSColors[0];
            PSText.text="Playing";
            if(!Status.IsSolved){
                stage=0;
                foreach(GameObject star in stars)star.SetActive(false);
            }
        }else{
            PSToggle.GetComponent<Renderer>().material=PSColors[1];
            PSText.text="Solving";
        }
        if(SMTSettings.SMT_LogPlayingSolvingSwitch)
            Log("Switched to {0} mode.",PSText.text);
    }

    void PlaceWords(){
        for(int i=0;i<6;i++){
            if(i!=onesylposition){
                int wordPos=UnityEngine.Random.Range(0,12);
                while(usedWords.Contains(wordPos))
                    wordPos=UnityEngine.Random.Range(0,12);
                usedWords.Add(wordPos);
                buttons[i].GetComponentInChildren<TextMesh>().text=multisylwords[wordPos];
            }else{
                onesylword=onesylwords[UnityEngine.Random.Range(0,4)];
                buttons[i].GetComponentInChildren<TextMesh>().text=onesylword;
            }
        }
    }

    List<int>OrderCheck(string word){
        return checkOrders[onesylwords.IndexOf(word)];
    }

    string Decoy(List<int>Order){
        for(int i=0;i<6;i++){
            wordlist.Add(buttons[i].GetComponentInChildren<TextMesh>().text);
            fulllist.Add(buttons[i].GetComponentInChildren<TextMesh>().text);
        }
        Log("The words are, in reading order: {0}",string.Join(", ",wordlist.ToArray()));
        for(int i=0;i<6;i++){
            string word=buttons[Order[i]].GetComponentInChildren<TextMesh>().text;
            decoyPos=DecoyChecking(word,Order[i],i);
            if(decoyPos!=6){
                if(decoyPos==onesylposition){
                    if(onesylposition%2==0)
                        return mantising(buttons[(onesylposition+1)%6].GetComponentInChildren<TextMesh>().text,word,false);
                    else return mantising(buttons[(onesylposition-1)%6].GetComponentInChildren<TextMesh>().text,word,false);
                }
                return mantising(buttons[decoyPos%6].GetComponentInChildren<TextMesh>().text,word,false);
            }
        }
        Log("None of the conditions were true.");
        if(onesylposition%2==0)
            return mantising(buttons[(onesylposition+1)%6].GetComponentInChildren<TextMesh>().text,"SWEETYCAKES",true);
        else return mantising(buttons[(onesylposition-1)%6].GetComponentInChildren<TextMesh>().text,"SWEETYCAKES",true);
    }

    string mantising(string mantistest,string word,bool sweetyfakes){
        if(mantistest=="MANTIS"){
            mantis=word;
            //wordlist defaults to reading order so i didn't need to specifically use "SWEETYCAKES" but. come on. how could i pass up this pun
            if(sweetyfakes)
                Log("Because MANTIS is the decoy and it was not declared the decoy by another word, the buttons must be pressed in reading order.");
            else Log("Because MANTIS is the decoy, the order from the word that declared it the decoy will be used, which is {0}.",mantis);
            decoyPos=wordlist.IndexOf("MANTIS");
        }
        return mantistest;
    }

    int DecoyChecking(string word,int currentPos,int count){
        switch(word){
            case"SWEETYCAKES":
                return Get<KMBombInfo>().GetOnIndicators().ToArray().Length>=2
                       ?1:6;
            case"CHEDDAR":
                return (usedWords.Contains(0)
                      ||usedWords.Contains(10)
                      ||onesylword=="CAKE"
                      ||onesylword=="JUICE")
                      ?currentPos:6;
            case"DOUGLAS":return buttons[0].GetComponentInChildren<TextMesh>().text.Length
                               >=buttons[5].GetComponentInChildren<TextMesh>().text.Length
                               ?6:OrderCheck(onesylword)[Get<KMBombInfo>().GetSerialNumberNumbers().Sum()%6];
            case"GARBLEDINA":
                return currentPos;
            case"MANTIS":
                     if(currentPos%2==0&&multisylwords.Take(6).Contains(buttons[currentPos+1].GetComponentInChildren<TextMesh>().text))
                        return currentPos+1;
                else if(currentPos%2==1&&multisylwords.Take(6).Contains(buttons[currentPos-1].GetComponentInChildren<TextMesh>().text))
                        return currentPos-1;
                else return 6;
            case"DIAPER":
                return count>=2&&count<=4
                       ?2:6;
            case"PROXIMITY":
                return ((onesylposition<4&&currentPos==onesylposition+2)
                      ||(onesylposition>1&&currentPos==onesylposition-2)
                      ||(currentPos%2==0&&currentPos==onesylposition-1)
                      ||(currentPos%2==1&&currentPos==onesylposition+1))
                      ?5:6;
            case"MOVIE":
                return Get<KMBombInfo>().GetSerialNumberLetters().Any(onesylword.Contains)
                       ?(currentPos+4)%6:6;
            case"HORSES":
                return onesylposition%2==0
                       ?OrderCheck(onesylword)[3]:6;
            case"WORKING":return buttons[0].GetComponentInChildren<TextMesh>().text.Length
                               ==buttons[4].GetComponentInChildren<TextMesh>().text.Length
                                 ?2:6;
            case"CASSEROLE":
                if((currentPos+2)%6!=onesylposition){
                    for(int i=0;i<count;i++){
                        if((currentPos+2)%6==OrderCheck(onesylword)[i])
                            return currentPos;
                    }
                }
                return 6;
            case"AROUND":
                    if(usedWords.Contains(0)||usedWords.Contains(3)){
                        if(count==0
                        ||(count==1&&OrderCheck(onesylword)[0]==onesylposition))
                            return currentPos;
                        if(OrderCheck(onesylword)[count-1]==onesylposition)
                            return OrderCheck(onesylword)[count-2];
                        return OrderCheck(onesylword)[count-1];
                }
                return 6;
            default:
                return 6;
        }
    }

    internal void Press(KMSelectable button){
        Shake(button,.5f,Sound.BigButtonPress);
        string word=button.GetComponentInChildren<TextMesh>().text;
        StartCoroutine(mouthAnimation(word));
        string message="Pressed "+word;
        if(Status.IsSolved||playing){
            if(SMTSettings.SMT_LogPlayingModeInteractions)Log(message+" in Playing mode.");
            return;
        }
        message+=", which was ";
        if(word==wordlist[stage]){
            Log(message+"correct.");
            stars[stage].SetActive(true);
            stage++;
            if(stage==4){
                Solve("SOLVED!");
                PS();
                foreach(GameObject star in stars)
                    star.SetActive(true);
            }
            return;
        }
        if(word==onesylword)
            Log(message+"the one-syllable word.");
        else if(word==TheDecoy)
            Log(message+"the decoy.");
        else Log(message+"incorrect.");
        wrong();
    }
    int resize(string word){
        switch(word){
            case "MOTORCYCLES":
            case "SWEETYCAKES":
                return 16;
            case "BATTLESHIP":
            case "CASSEROLE":
            case "GARBLEDINA":
            case "PROXIMITY":
            case "TRAINWRECK":
                return 20;
            case "DELUISE":
            case "WORKING":
                return 24;
            case "AROUND":
            case "DOUGLAS":
            case "MANTIS":
                return 26;
            case "CHEDDAR":
            case "DIAPER":
            case "HORSES":
            case "MOVIE":
                return 28;
            default:
                return 30;
        }
    }
    void wrong(){
        Strike("STRIKE!");
        stage=0;
        foreach(GameObject star in stars)
            star.SetActive(false);
    }
    void pressingOrder(string decoy){
        List<int>specialList=new List<int>();
        if(decoy=="HORSES")
            specialList=new List<int>{5,4,3,1,0,2};
        if(decoy=="AROUND")
            specialList=new List<int>{0,1,3,5,4,2};
        if(decoy=="CASSEROLE")
            specialList=OrderCheck(onesylword);
        if(decoy=="GARBLEDINA"){
            List<int>rearrange=new List<int>{1,5,3,4,2,0};
            wordlist.Clear();
            for(int i=0;i<6;i++)
                wordlist.Add(buttons[OrderCheck(onesylword)[rearrange[i]]]
                        .GetComponentInChildren<TextMesh>()
                        .text);
        }
        if(decoy=="MOVIE"){
            specialList=new List<int>{0,1,3,5,4,2};
            int offset=specialList.IndexOf(decoyPos);
            List<int>clockwise=new List<int>();
            for(int i=0;i<6;i++)
                clockwise.Add(specialList[(i+offset)%6]);
            specialList=clockwise;
        }
        if(specialList.ToArray().Length!=0){
            wordlist.Clear();
            for(int i=0;i<6;i++)
                wordlist.Add(buttons[specialList[i]]
                        .GetComponentInChildren<TextMesh>()
                        .text);
        }
        wordlist.Remove(TheDecoy);
        wordlist.Remove(onesylword);
        if(decoy=="PROXIMITY")
            wordlist.Sort((x,y)=>string.Compare(x,y));
        if(decoy=="WORKING"){
            wordlist.Sort((x,y)=>string.Compare(x,y));
            wordlist.Reverse();
        }
        if(decoy=="CHEDDAR"||decoy=="DIAPER"){
            wordlist.Sort((x,y)=>string.Compare(x,y));
            wordlist=wordlist.OrderBy(x=>x.Length).ToList();
            if(decoy=="DIAPER"){
                List<string>templist=new List<string>();
                for(int i=0;i<4;i++)
                    templist.Add(wordlist[(5-i)%4]);
                wordlist=templist;
            }
        }
        if(decoy=="DOUGLAS"){
            List<string>templist=new List<string>();
            for(int i=6;i<18;i++){
                if(wordlist.Contains(multisylwords[i%12]))
                    templist.Add(multisylwords[i%12]);
            }
            wordlist=templist;
        }
        Log("Correct order: {0}",string.Join(", ",wordlist.ToArray()));
    }

    IEnumerator blink(){
        while(true){
            yield return new WaitForSeconds(4.9f);
            eyesclosed.SetActive(true);
            yield return new WaitUntil(()=>doneSpeaking);
            yield return new WaitForSeconds(.1f);
            eyesclosed.SetActive(false);
        }
    }

    IEnumerator mouthAnimation(string word){
        doneSpeaking=false;
        Play(new Sound(word));
        eyesclosed.SetActive(true);
        switch(word){
            case "AROUND":
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.2f);
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.9f);
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[0];
                break;
            case "BATTLESHIP":
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.3f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(.2f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.8f);
                lipsync.material=mouthShapes[0];
                break;
            case "CASSEROLE":
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.3f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.3f);
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(1.5f);
                lipsync.material=mouthShapes[0];
                break;
            case "CAKE":
            case "CHARM":
            case "NIGHT":
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(1);
                lipsync.material=mouthShapes[0];
                break;
            case "CHEDDAR":
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.35f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.5f);
                lipsync.material=mouthShapes[0];
                break;
            case "DELUISE":
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.5f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(.2f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(2);
                lipsync.material=mouthShapes[0];
                break;
            case "DIAPER":
            case "MANTIS":
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.5f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.85f);
                lipsync.material=mouthShapes[0];
                break;
            case "DOUGLAS":
            case "HORSES":
            case "MOVIE":
            case "WORKING":
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(.4f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(1);
                lipsync.material=mouthShapes[0];
                break;
            case "GARBLEDINA":
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.3f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(.2f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.3f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.8f);
                lipsync.material=mouthShapes[0];
                break;
            case "JUICE":
            case "PULSE":
            case "CURLS":
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(1);
                lipsync.material=mouthShapes[0];
                break;
            case "MOTORCYCLES":
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(.2f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(.2f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.2f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(.8f);
                lipsync.material=mouthShapes[0];
                break;
            case "PROXIMITY":
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(.4f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.1f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.1f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.1f);
                lipsync.material=mouthShapes[0];
                break;
            case "SWEETYCAKES":
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(.2f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.2f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.1f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.8f);
                lipsync.material=mouthShapes[0];
                break;
            case "TRAINWRECK":
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.3f);
                lipsync.material=mouthShapes[0];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[2];
                yield return new WaitForSeconds(1/12f);
                lipsync.material=mouthShapes[1];
                yield return new WaitForSeconds(.6f);
                lipsync.material=mouthShapes[0];
                break;
            default:
                break;
        }
        eyesclosed.SetActive(false);
        doneSpeaking=true;
    }
}
