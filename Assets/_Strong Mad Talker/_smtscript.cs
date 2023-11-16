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
    public List<string>onesylwords;
    private string onesylword;
    public List<string>multisylwords;
    private List<string>allWords=new List<string>();
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
    private List<int>[]checkingOrders=new List<int>[]{};
    private bool[]checks=new bool[]{};
    private string[]decoys=new string[]{};
    internal settings SMTSettings;
    private bool doneSpeaking=true;
    private bool noneTrue=false;
    private List<string>tempWordList=new List<string>();
    private List<int>[]checkOrders=new List<int>[]{
        new List<int>(){0,1,2,3,4,5},
        new List<int>(){0,1,2,3,4,5},
        new List<int>(){0,1,2,3,4,5},
        new List<int>(){0,1,2,3,4,5}
    };
    private bool useDetDecoy=false;
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
        onesylwords=new List<string>(){"JUICE","PULSE","CAKE","CHARM","NIGHT","CURLS"};
        rnd.ShuffleFisherYates(onesylwords);
        checkOrders=Enumerable.Range(0,4).Select(_ => rnd.ShuffleFisherYates(Enumerable.Range(0,6).ToList())).ToArray();
        multisylwords=new List<string>(){"SWEETYCAKES","CHEDDAR","DOUGLAS","GARBLEDINA","MANTIS","DIAPER","PROXIMITY","MOVIE","HORSES","WORKING","CASSEROLE","AROUND","DELUISE","BATTLESHIP","MOTORCYCLES","TRAINWRECK"};
        rnd.ShuffleFisherYates(multisylwords);
        foreach(string word in onesylwords)
            allWords.Add(word);
        foreach(string word in multisylwords)
            allWords.Add(word);
        rnd.ShuffleFisherYates(allWords);
        onesylwords.RemoveAt(5);
        onesylwords.RemoveAt(4);
        multisylwords.RemoveAt(15);
        multisylwords.RemoveAt(14);
        multisylwords.RemoveAt(13);
        multisylwords.RemoveAt(12);
        SMTSettings=new Config<settings>("strongmadtalker-settings.json").Read();
        foreach(GameObject star in stars)
            star.SetActive(false);
        StartCoroutine(blink());
        onesylposition=UnityEngine.Random.Range(0,6);
        PlaceWords();
        foreach(KMSelectable button in buttons)
            button.GetComponentInChildren<TextMesh>().fontSize=resize(button.GetComponentInChildren<TextMesh>().text);
        for(int i=0;i<6;i++)
            tempWordList.Add(buttons[i].GetComponentInChildren<TextMesh>().text);
        List<int>checkShuffle=new List<int>(){0,1,2,3,4,5,6,7,8,9,10,11};
        rnd.ShuffleFisherYates(checkShuffle);
        List<bool>tempChecks=new List<bool>();
        tempChecks.Add(router         (rnd,tempWordList.IndexOf(multisylwords[0]),checkShuffle[0]));
        string decoy0 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[0]));
        List<int>checkingOrder0=new List<int>(pickOrder(rnd));
        tempChecks.Add(         router(rnd,tempWordList.IndexOf(multisylwords[1]),checkShuffle[1]));
        string decoy1 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[1]));
        List<int>checkingOrder1=new List<int>(pickOrder(rnd));
        tempChecks.Add(         router(rnd,tempWordList.IndexOf(multisylwords[2]),checkShuffle[2]));
        string decoy2 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[2]));
        List<int>checkingOrder2=new List<int>(pickOrder(rnd));
        tempChecks.Add(         router(rnd,tempWordList.IndexOf(multisylwords[3]),checkShuffle[3]));
        string decoy3 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[3]));
        List<int>checkingOrder3=new List<int>(pickOrder(rnd));
        tempChecks.Add(         router(rnd,tempWordList.IndexOf(multisylwords[4]),checkShuffle[4]));
        string decoy4 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[4]));
        List<int>checkingOrder4=new List<int>(pickOrder(rnd));
        tempChecks.Add(         router(rnd,tempWordList.IndexOf(multisylwords[5]),checkShuffle[5]));
        string decoy5 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[5]));
        List<int>checkingOrder5=new List<int>(pickOrder(rnd));
        tempChecks.Add(         router(rnd,tempWordList.IndexOf(multisylwords[6]),checkShuffle[6]));
        string decoy6 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[6]));
        List<int>checkingOrder6=new List<int>(pickOrder(rnd));
        tempChecks.Add(         router(rnd,tempWordList.IndexOf(multisylwords[7]),checkShuffle[7]));
        string decoy7 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[7]));
        List<int>checkingOrder7=new List<int>(pickOrder(rnd));
        tempChecks.Add(         router(rnd,tempWordList.IndexOf(multisylwords[8]),checkShuffle[8]));
        string decoy8 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[8]));
        List<int>checkingOrder8=new List<int>(pickOrder(rnd));
        tempChecks.Add(         router(rnd,tempWordList.IndexOf(multisylwords[9]),checkShuffle[9]));
        string decoy9 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[9]));
        List<int>checkingOrder9=new List<int>(pickOrder(rnd));
        tempChecks.Add(          router(rnd,tempWordList.IndexOf(multisylwords[10]),checkShuffle[10]));
        string decoy10 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[10]));
        List<int>checkingOrder10=new List<int>(pickOrder(rnd));
        tempChecks.Add(          router(rnd,tempWordList.IndexOf(multisylwords[11]),checkShuffle[11]));
        string decoy11 =pickDeclaration(rnd,tempWordList.IndexOf(multisylwords[11]));
        List<int>checkingOrder11=new List<int>(pickOrder(rnd));
        checks=tempChecks.ToArray();
        decoys=new string[]{decoy0,decoy1,decoy2,decoy3,decoy4,decoy5,decoy6,decoy7,decoy8,decoy9,decoy10,decoy11};
        checkingOrders=new List<int>[]{checkingOrder0,checkingOrder1,checkingOrder2,checkingOrder3,checkingOrder4,checkingOrder5,checkingOrder6,checkingOrder7,checkingOrder8,checkingOrder9,checkingOrder10,checkingOrder11};
        TheDecoy=Decoy(OrderCheck(onesylword));
        Log("{0} is the decoy.",TheDecoy);
        if(checkingOrders[multisylwords.IndexOf(TheDecoy)][0]==-3)
            pressingOrder(mantis);
        else pressingOrder(TheDecoy);
    }

    private bool router(MonoRandom r,int pos,int index){
        switch(index){
            case 0:
                return CHECKadjacent(r,pos);
            case 1:
                return CHECKcontains(r);
            case 2:
                return CHECKserialshare(r,pos);
            case 3:
                return CHECKlength(r,pos);
            case 4:
                return CHECKbeenChecked(r,pos);
            case 5:
                return CHECKmanualTable(r,pos);
            case 6:
                return CHECKlengthRanges(r,pos);
            case 7:
                return CHECKseqIndex(r,pos);
            case 8:
                return CHECKmoduleTable(r,pos);
            case 9:
                return CHECKalphOrder(r,pos);
            case 10:
                return CHECK2Lit();
            case 11:
            default:
                return CHECKemptyPortPlate();
        }
    }

    private List<string>pickWordSelectors(MonoRandom r,bool can1Syl, bool canThis){
        List<string>selectors=new List<string>(){absolutePositions[r.Next(0,6)],relativePositions[r.Next(0,4)],OrderCheck(onesylword).IndexOf(r.Next(1,7)-1).ToString()};
        if(can1Syl)
            selectors.Add("1SYL");
        if(canThis)
            selectors.Add("THIS");
        return r.ShuffleFisherYates(selectors);
    }

    private string decideWordsForContainsRule(MonoRandom r){
        int contains=r.Next(0,2);
        int mode=r.Next(0,3);
        int numWords=r.Next(2,5);
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
                    return "ALL "+string.Join(" ", newMultiSylWords.GetRange(0,numWords+1).ToArray());
                else
                    return "nALL "+string.Join(" ", newMultiSylWords.GetRange(0,numWords+1).ToArray());
        }
    }

    private string pickDeclaration(MonoRandom r,int index){
        switch(r.Next(0,4)){
            case 0:
                List<string>w=pickWordSelectors(r,false,true);
                return w[0];
            case 1:
                int[]edgeworkNumbers=new int[]{Get<KMBombInfo>().GetSerialNumberNumbers().Sum(x=>Convert.ToInt32(x.ToString())),Get<KMBombInfo>().GetOnIndicators().ToArray().Length};
                int edgeworkNumber=edgeworkNumbers[r.Next(0,2)]%6;
                return OrderCheck(onesylword).IndexOf(edgeworkNumber).ToString();
            case 2:
                if(index>=0&&index<=5){
                    if(OrderCheck(onesylword)[index]==0)
                        return index.ToString();
                    else
                        return OrderCheck(onesylword).IndexOf(OrderCheck(onesylword)[index]-1).ToString();
                }
                return "";
            case 3:
            default:
                string[]order=new string[]{"R","G","C"};
                string[]table=new string[]{"M","T"};
                string[]check=new string[]{"B","N"};
                return order[r.Next(0,3)]+table[r.Next(0,2)]+check[r.Next(0,2)];
        }
    }
    int order=0;
    private List<int>pickOrder(MonoRandom r){
        if(useDetDecoy)
            order=r.Next(0,5);
        else
            order=r.Next(0,6);
        switch(order){
            case 0:
                int order=r.Next(0,6);
                int moduleOrTable=r.Next(0,2);
                List<int>[]ordersModule=new List<int>[]{
                    new List<int>(){0,1,2,3,4,5},
                    new List<int>(){4,5,2,3,0,1},
                    new List<int>(){1,3,5,0,2,4},
                    new List<int>(){5,4,3,2,1,0},
                    new List<int>(){1,0,3,2,5,4},
                    new List<int>(){4,2,0,5,3,1}
                };
                List<int>[]ordersTable=new List<int>[]{
                    new List<int>(){tempWordList.IndexOf(multisylwords[ 0]),
                                    tempWordList.IndexOf(multisylwords[ 1]),
                                    tempWordList.IndexOf(multisylwords[ 2]),
                                    tempWordList.IndexOf(multisylwords[ 3]),
                                    tempWordList.IndexOf(multisylwords[ 4]),
                                    tempWordList.IndexOf(multisylwords[ 5]),
                                    tempWordList.IndexOf(multisylwords[ 6]),
                                    tempWordList.IndexOf(multisylwords[ 7]),
                                    tempWordList.IndexOf(multisylwords[ 8]),
                                    tempWordList.IndexOf(multisylwords[ 9]),
                                    tempWordList.IndexOf(multisylwords[10]),
                                    tempWordList.IndexOf(multisylwords[11])},

                    new List<int>(){tempWordList.IndexOf(multisylwords[10]),
                                    tempWordList.IndexOf(multisylwords[11]),
                                    tempWordList.IndexOf(multisylwords[ 8]),
                                    tempWordList.IndexOf(multisylwords[ 9]),
                                    tempWordList.IndexOf(multisylwords[ 6]),
                                    tempWordList.IndexOf(multisylwords[ 7]),
                                    tempWordList.IndexOf(multisylwords[ 4]),
                                    tempWordList.IndexOf(multisylwords[ 5]),
                                    tempWordList.IndexOf(multisylwords[ 2]),
                                    tempWordList.IndexOf(multisylwords[ 3]),
                                    tempWordList.IndexOf(multisylwords[ 0]),
                                    tempWordList.IndexOf(multisylwords[ 1])},

                    new List<int>(){tempWordList.IndexOf(multisylwords[ 1]),
                                    tempWordList.IndexOf(multisylwords[ 3]),
                                    tempWordList.IndexOf(multisylwords[ 5]),
                                    tempWordList.IndexOf(multisylwords[ 7]),
                                    tempWordList.IndexOf(multisylwords[ 9]),
                                    tempWordList.IndexOf(multisylwords[11]),
                                    tempWordList.IndexOf(multisylwords[ 0]),
                                    tempWordList.IndexOf(multisylwords[ 2]),
                                    tempWordList.IndexOf(multisylwords[ 4]),
                                    tempWordList.IndexOf(multisylwords[ 6]),
                                    tempWordList.IndexOf(multisylwords[ 8]),
                                    tempWordList.IndexOf(multisylwords[10])},

                    new List<int>(){tempWordList.IndexOf(multisylwords[11]),
                                    tempWordList.IndexOf(multisylwords[10]),
                                    tempWordList.IndexOf(multisylwords[ 9]),
                                    tempWordList.IndexOf(multisylwords[ 8]),
                                    tempWordList.IndexOf(multisylwords[ 7]),
                                    tempWordList.IndexOf(multisylwords[ 6]),
                                    tempWordList.IndexOf(multisylwords[ 5]),
                                    tempWordList.IndexOf(multisylwords[ 4]),
                                    tempWordList.IndexOf(multisylwords[ 3]),
                                    tempWordList.IndexOf(multisylwords[ 2]),
                                    tempWordList.IndexOf(multisylwords[ 1]),
                                    tempWordList.IndexOf(multisylwords[ 0])},

                    new List<int>(){tempWordList.IndexOf(multisylwords[ 1]),
                                    tempWordList.IndexOf(multisylwords[ 0]),
                                    tempWordList.IndexOf(multisylwords[ 3]),
                                    tempWordList.IndexOf(multisylwords[ 2]),
                                    tempWordList.IndexOf(multisylwords[ 5]),
                                    tempWordList.IndexOf(multisylwords[ 4]),
                                    tempWordList.IndexOf(multisylwords[ 7]),
                                    tempWordList.IndexOf(multisylwords[ 6]),
                                    tempWordList.IndexOf(multisylwords[ 9]),
                                    tempWordList.IndexOf(multisylwords[ 8]),
                                    tempWordList.IndexOf(multisylwords[11]),
                                    tempWordList.IndexOf(multisylwords[10])},

                    new List<int>(){tempWordList.IndexOf(multisylwords[10]),
                                    tempWordList.IndexOf(multisylwords[ 8]),
                                    tempWordList.IndexOf(multisylwords[ 6]),
                                    tempWordList.IndexOf(multisylwords[ 4]),
                                    tempWordList.IndexOf(multisylwords[ 2]),
                                    tempWordList.IndexOf(multisylwords[ 0]),
                                    tempWordList.IndexOf(multisylwords[11]),
                                    tempWordList.IndexOf(multisylwords[ 9]),
                                    tempWordList.IndexOf(multisylwords[ 7]),
                                    tempWordList.IndexOf(multisylwords[ 5]),
                                    tempWordList.IndexOf(multisylwords[ 3]),
                                    tempWordList.IndexOf(multisylwords[ 1])}
                };
                if(moduleOrTable==0)
                    return ordersModule[order];
                else
                    return ordersTable[order];
            case 1:
                List<int>result1=new List<int>();
                List<string>tempTempWordList1=new List<string>(tempWordList);
                tempTempWordList1.Sort((x,y)=>string.Compare(x,y));
                for(int i=0;i<6;i++)
                    result1.Add(tempWordList.IndexOf(tempTempWordList1[i]));
                int reverse=r.Next(0,2);
                if(reverse==1)
                    result1.Reverse();
                return result1;
            case 2:
                List<int>result2=new List<int>();
                List<string>tempTempWordList2=new List<string>(tempWordList);
                int incOrDec2=r.Next(0,2);
                tempTempWordList2.Sort((x,y)=>string.Compare(x,y));
                if(incOrDec2==1)
                    tempTempWordList2=tempTempWordList2.OrderBy(x=>-1*x.Length).ToList();
                else
                    tempTempWordList2=tempTempWordList2.OrderBy(x=>x.Length).ToList();
                for(int i=0;i<6;i++)
                    result2.Add(tempWordList.IndexOf(tempTempWordList2[i]));
                return result2;
            case 3:
                List<int>result3=new List<int>();
                List<int>rearrange=r.ShuffleFisherYates(Enumerable.Range(0,4).ToList());
                List<string>tempTempWordList3=new List<string>(tempWordList);
                int incOrDec3=r.Next(0,2);
                tempTempWordList3.Sort((x,y)=>string.Compare(x,y));
                if(incOrDec3==1)
                    tempTempWordList3=tempTempWordList3.OrderBy(x=>-1*x.Length).ToList();
                else
                    tempTempWordList3=tempTempWordList3.OrderBy(x=>x.Length).ToList();
                for(int i=0;i<6;i++)
                    result3.Add(tempWordList.IndexOf(tempTempWordList3[i]));
                result3.Add(-2);//to separate rearrangement numbers
                foreach(int rea in rearrange)
                    result3.Add(rea);
                return result3;
            case 4:
                List<int>temp4=r.ShuffleFisherYates(Enumerable.Range(0,6).ToList());
                List<int>order4=new List<int>();
                for(int i=0;i<6;i++)
                    order4.Add(temp4.IndexOf(i));
                return order4;
            case 5:
            default:
                useDetDecoy=true;
                return new List<int>(){-3};
        }
    }

    private int wToWord(string input,int pos,int index=-1){
        int[]geometricModule=new int[]{4,5,2,3,0,1};
        int[]geometricTable =new int[]{10,11,8,9,6,7,4,5,2,3,0,1};
        int[]chineseModule  =new int[]{1,3,5,0,2,4};
        int[]chineseTable   =new int[]{1,3,5,7,9,11,0,2,4,6,8,10};
        if(input.Length==3&&(index==0||(index==1&&OrderCheck(onesylword)[onesylposition]==0)))
            return pos;
        switch(input){
            case "RMB":
                for(int i=0;i<6;i++){
                    if(OrderCheck(onesylword)[i]<index)
                        return i;
                }
                return -1;
            case "RMN":
                for(int i=0;i<6;i++){
                    if(OrderCheck(onesylword)[i]>index)
                        return i;
                }
                return -1;
            case "RTB":
                for(int i=0;i<12;i++){
                    if(tempWordList.Contains(multisylwords[i])&&OrderCheck(onesylword)[tempWordList.IndexOf(multisylwords[i])]<index)
                        return tempWordList.IndexOf(multisylwords[i]);
                }
                return -1;
            case "RTN":
                for(int i=0;i<12;i++){
                    if(tempWordList.Contains(multisylwords[i])&&OrderCheck(onesylword)[tempWordList.IndexOf(multisylwords[i])]>index)
                        return tempWordList.IndexOf(multisylwords[i]);
                }
                return -1;
            case "GMB":
                for(int i=0;i<6;i++){
                    if(OrderCheck(onesylword)[geometricModule[i]]<index)
                        return geometricModule[i];
                }
                return -1;
            case "GMN":
                for(int i=0;i<6;i++){
                    if(OrderCheck(onesylword)[geometricModule[i]]>index)
                        return geometricModule[i];
                }
                return -1;
            case "GTB":
                for(int i=0;i<12;i++){
                    if(tempWordList.Contains(multisylwords[geometricTable[i]])&&OrderCheck(onesylword)[tempWordList.IndexOf(multisylwords[geometricTable[i]])]<index)
                        return tempWordList.IndexOf(multisylwords[geometricTable[i]]);
                }
                return -1;
            case "GTN":
                for(int i=0;i<12;i++){
                    if(tempWordList.Contains(multisylwords[geometricTable[i]])&&OrderCheck(onesylword)[tempWordList.IndexOf(multisylwords[geometricTable[i]])]>index)
                        return tempWordList.IndexOf(multisylwords[geometricTable[i]]);
                }
                return -1;
            case "CMB":
                for(int i=0;i<6;i++){
                    if(OrderCheck(onesylword)[chineseModule[i]]<index)
                        return chineseModule[i];
                }
                return -1;
            case "CMN":
                for(int i=0;i<6;i++){
                    if(OrderCheck(onesylword)[chineseModule[i]]>index)
                        return chineseModule[i];
                }
                return -1;
            case "CTB":
                for(int i=0;i<12;i++){
                    if(tempWordList.Contains(multisylwords[chineseTable[i]])&&OrderCheck(onesylword)[tempWordList.IndexOf(multisylwords[chineseTable[i]])]<index)
                        return tempWordList.IndexOf(multisylwords[chineseTable[i]]);
                }
                return -1;
            case "CTN":
                for(int i=0;i<12;i++){
                    if(tempWordList.Contains(multisylwords[chineseTable[i]])&&OrderCheck(onesylword)[tempWordList.IndexOf(multisylwords[chineseTable[i]])]>index)
                        return tempWordList.IndexOf(multisylwords[chineseTable[i]]);
                }
                return -1;
            case "TL":
            case "TR":
            case "ML":
            case "MR":
            case "BL":
            case "BR":
                return absolutePositions.ToList().IndexOf(input);
            case "L":
            case "R":
                if(pos%2==0)
                    return pos+1;
                else
                    return pos-1;
            case "A":
                return (pos+4)%6;
            case "B":
                return (pos+2)%6;
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
                return int.Parse(input);
            case "1SYL":
                return onesylposition;
            case "THIS":
            default:
                return pos;
        }
    }

    private bool CHECKadjacent(MonoRandom r,int pos){
        List<string>w=pickWordSelectors(r,true,true);
        string[]adjacentType=new string[]{"O","D","OD"};
        string adj=adjacentType[r.Next(0,3)];
        string[]looping=new string[]{"L","NL"};
        string loop=looping[r.Next(0,2)];
        int word0=wToWord(w[0],pos);
        int word1=wToWord(w[1],pos);
        if(pos<0||word0==word1)
            return false;
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

    private bool CHECKcontains(MonoRandom r){
        string[]contains=decideWordsForContainsRule(r).Split(' ');
        switch(contains[0]){
            case "ONLY":
                return tempWordList.Contains(contains[1]);
            case "nONLY":
                return !tempWordList.Contains(contains[1]);
            case "ANY":
                for(int i=1;i<contains.Length;i++){
                    if(tempWordList.Contains(contains[i]))
                        return true;
                }
                return false;
            case "nANY":
                for(int i=1;i<contains.Length;i++){
                    if(!tempWordList.Contains(contains[i]))
                        return true;
                }
                return false;
            case "ALL":
                for(int i=1;i<contains.Length;i++){
                    if(!tempWordList.Contains(contains[i]))
                        return false;
                }
                return true;
            case "nALL":
            default:
                for(int i=1;i<contains.Length;i++){
                    if(tempWordList.Contains(contains[i]))
                        return false;
                }
                return true;
        }
    }

    private bool CHECKserialshare(MonoRandom r,int pos){
        List<string>w=pickWordSelectors(r,true,true);
        if(pos<0)
            return false;
        string word0=tempWordList[wToWord(w[0],pos)];
        foreach(char c in word0){
            if(Get<KMBombInfo>().GetSerialNumberLetters().Contains(c))
                return true;
        }
        return false;
    }

    private bool CHECKlength(MonoRandom r,int pos){
        List<string>w=pickWordSelectors(r,true,true);
        string[]lengthType=new string[]{">","<","="};
        string lt=lengthType[r.Next(0,3)];
        if(pos<0)
            return false;
        string word0=tempWordList[wToWord(w[0],pos)];
        string word1=tempWordList[wToWord(w[1],pos)];
        switch(lt){
            case ">":
                return word0.Length>word1.Length;
            case "<":
                return word0.Length<word1.Length;
            case "=":
            default:
                return word0.Length==word1.Length;
        }
    }

    private bool CHECKbeenChecked(MonoRandom r,int pos){
        List<string>w=pickWordSelectors(r,false,false);
        string[]checkedType=new string[]{"C","NC"};
        string ct=checkedType[r.Next(0,2)];
        int word0=wToWord(w[0],pos);
        if(pos<0||pos>5||word0<0||word0>5)
            return false;
        switch(ct){
            case "C":
                return OrderCheck(onesylword)[word0]<OrderCheck(onesylword)[pos];
            case "NC":
            default:
                return OrderCheck(onesylword)[word0]>OrderCheck(onesylword)[pos];
        }
    }

    private bool CHECKmanualTable(MonoRandom r,int pos){
        List<string>w=pickWordSelectors(r,true,true);
        string[]tableType=new string[]{"L","R","T","B"};
        string tt=tableType[r.Next(0,4)];
        if(pos<0)
            return false;
        string word0=tempWordList[wToWord(w[0],pos)];
        switch(tt){
            case "L":
                return multisylwords.IndexOf(word0)%2==0;
            case "R":
                return multisylwords.IndexOf(word0)%2==1;
            case "T":
                return multisylwords.IndexOf(word0)<6;
            case "B":
            default:
                return multisylwords.IndexOf(word0)>=6;
        }
    }

    private bool CHECKlengthRanges(MonoRandom r,int pos){
        List<string>w=pickWordSelectors(r,false,false);
        int rtN=r.Next(0,2);
        int rangeN=r.Next(0,3);
        if(pos<0)
            return false;
        string word0=tempWordList[wToWord(w[0],pos)];
        string[]rangeType=new string[]{word0,"ANY"};
        string rt=rangeType[rtN];
        string[]ranges=new string[]{">=6","==7","<=4"};
        string range=ranges[rangeN];
        if(rt==word0){
            switch(range){
                case ">=6":
                    return word0.Length>=6;
                case "==7":
                    return word0.Length==7;
                case "<=4":
                default:
                    return word0.Length<=4;
            }
        }else{
            switch(range){
                case ">=10":
                    foreach(string word in tempWordList){
                        if(word.Length>=10)
                            return true;
                    }
                    return false;
                case "==8":
                    foreach(string word in tempWordList){
                        if(word.Length==8)
                            return true;
                    }
                    return false;
                case "<=6":
                default:
                    foreach(string word in tempWordList){
                        if(word.Length<=6)
                            return true;
                    }
                    return false;
            }
        }
    }

    private bool CHECKseqIndex(MonoRandom r,int pos){
        List<string>w=pickWordSelectors(r,true,true);
        int word0=wToWord(w[0],pos);
        int[]indices=r.ShuffleFisherYates(Enumerable.Range(0,6).ToArray());
        int[]indicesSlice=new int[r.Next(1,4)];
        if(pos<0)
            return false;
        for(int i=0;i<indicesSlice.Length;i++)
            indicesSlice[i]=indices[i];
        foreach(int index in indicesSlice){
            if(OrderCheck(onesylword).IndexOf(index)==word0)
                return true;
        }
        return false;
    }

    private bool CHECKmoduleTable(MonoRandom r,int pos){
        List<string>w=pickWordSelectors(r,true,true);
        string[]tableType=new string[]{"L","R","T","M","B"};
        string tt=tableType[r.Next(0,5)];
        int word0=wToWord(w[0],pos);
        if(pos<0)
            return false;
        switch(tt){
            case "L":
                return word0%2==0;
            case "R":
                return word0%2==1;
            case "T":
                return word0==0||word0==1;
            case "M":
                return word0==2||word0==3;
            case "B":
            default:
                return word0==4||word0==5;
        }
    }

    private bool CHECKalphOrder(MonoRandom r,int pos){
        List<string>w=pickWordSelectors(r,true,false);
        string[]beforeAfter=new string[]{"B","A"};
        string ba=beforeAfter[r.Next(0,2)];
        if(pos<0)
            return false;
        string word0=tempWordList[wToWord(w[0],pos)];
        string word1=tempWordList[wToWord(w[1],pos)];
        switch(ba){
            case "B":
                return string.Compare(word0,word1)<0;
            case "A":
            default:
                return string.Compare(word0,word1)>0;
        }
    }

    private bool CHECK2Lit(){
        return Get<KMBombInfo>().GetOnIndicators().ToArray().Length>=2;
    }

    private bool CHECKemptyPortPlate(){
        return Get<KMBombInfo>().GetPortPlates().Any(p=>p.Length==0);
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
        string word="";
        for(int i=0;i<6;i++){
            word=buttons[Order.IndexOf(i)].GetComponentInChildren<TextMesh>().text;
            decoyPos=DecoyChecking(word,Order.IndexOf(i),i);
            if(decoyPos!=6){
                Log(word+" declared the decoy.");
                if(decoyPos==onesylposition){
                    if(onesylposition%2==0)
                        return mantising(buttons[(onesylposition+1)%6].GetComponentInChildren<TextMesh>().text,word,false);
                    else return mantising(buttons[(onesylposition+5)%6].GetComponentInChildren<TextMesh>().text,word,false);
                }
                return mantising(buttons[(decoyPos+6)%6].GetComponentInChildren<TextMesh>().text,word,false);
            }
        }
        Log("None of the conditions were true.");
        noneTrue=true;
        if(onesylposition%2==0)
            return mantising(buttons[(onesylposition+1)%6].GetComponentInChildren<TextMesh>().text,word,true);
        else return mantising(buttons[(onesylposition-1)%6].GetComponentInChildren<TextMesh>().text,word,true);
    }

    string mantising(string mantistest,string word,bool sweetyfakes){
        if(multisylwords.Contains(mantistest)&&checkingOrders[multisylwords.IndexOf(mantistest)][0]==-3){
            mantis=word;
            //wordlist defaults to reading order so i didn't need to specifically use "SWEETYCAKES" but. come on. how could i pass up this pun
            if(sweetyfakes)
                Log("Because "+mantistest+" is the decoy and it was not declared the decoy by another word, the buttons must be pressed in reading order.");
            else if(mantistest==word)
                Log("Because "+mantistest+" declared itself the decoy, the buttons must be pressed in reading order.");
            else
                Log("Because "+mantistest+" is the decoy, the order from the word that declared it the decoy will be used, which is {0}.",mantis);
            decoyPos=wordlist.IndexOf(mantistest);
        }
        return mantistest;
    }

    int DecoyChecking(string word,int currentPos,int count){
        if(onesylwords.Contains(word))
            return 6;
        if(checks[multisylwords.IndexOf(word)])
            return wToWord(decoys[multisylwords.IndexOf(word)],currentPos,count);
        else
            return 6;

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
        List<int>checkList=checkingOrders[multisylwords.IndexOf(decoy)];
        List<int>orderList=new List<int>();
        if(checkList.Contains(-2)){
            orderList=checkList.GetRange(checkList.IndexOf(-2)+1,checkList.Count()-checkList.IndexOf(-2)-1);
            checkList=checkList.GetRange(0,checkList.IndexOf(-2));
        }
        if(checkList.Contains(-3)&&((TheDecoy==mantis||noneTrue)))
            checkList=new List<int>(){0,1,2,3,4,5};
        if(checkList.Count()==12){
            for(int i=11;i>=0;i--){
                if(checkList[i]==-1)
                    checkList.RemoveAt(i);
            }
        }
        if(checkList.Contains(tempWordList.IndexOf(TheDecoy)))
            checkList.Remove(tempWordList.IndexOf(TheDecoy));
        if(checkList.Contains(tempWordList.IndexOf(onesylword)))
            checkList.Remove(tempWordList.IndexOf(onesylword));
        if(orderList.Count()!=0){
            List<int>checkListCopy=new List<int>();
            for(int i=0;i<4;i++)
                checkListCopy.Add(checkList[orderList[i]]);
            checkList=checkListCopy;
        }
        wordlist.Clear();
        foreach(int ch in checkList)
            wordlist.Add(tempWordList[ch]);
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
