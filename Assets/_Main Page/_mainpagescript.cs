
using Wawa.Modules;
using Wawa.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

///<summary>Main script and interactions.</summary>
public class _mainpagescript:ModdedModule{
    public KMSelectable[] numberButtons;
    public KMSelectable[] menuButtons;
    public Renderer messageButton;
    public Material[] messageColors;
    private string[] messageColorNames=new string[]{"blue","green","red","yellow"};
    public Shader transparentshader;
    public _mpHsBg HSBG;
    public _mpTextures TXTRs;
    public KMRuleSeedable rs;
    internal bool speaking;
    internal bool moduleSolved = false;
    internal string[] buttonNames={"Toons","Games","Characters","Downloads","Store","E-mail"};
    internal char[] buttonLetters={'T','G','C','D','S','E'};
    internal bool blinkstop = false;
    internal int message1,message2,message3,color1,color2,color3;
    string colorNotPresent;
    private enum colorCondition{
        COLOR_PRESENT,
        COLOR_NOT_PRESENT,
        EITHER_COLOR_PRESENT,
        NEITHER_COLOR_PRESENT
    }
    private enum turnCondition{
        DEFAULT_VOICE_LINES,
        NOT_DEFAULT_VOICE_LINES,
        ANY_BUTTON_ORIGIN_MATCHES_HOMESTAR,
        ANY_BUTTON_ORIGIN_MATCHES_BACKGROUND
    }
    private enum turnType{
        REVERSE,
        SWAP_PAIRS,
        SWAP_HALVES
    }
    private string firstColorInCond,secondColorInCond;
    private colorCondition cond;
    private turnCondition turnCond;
    private turnType turnIf,turnElse;
    private bool caesarHS;
    private int correctMainPage;
    private string[,]messages=new string[,]
        {
            {"play a game", "latest toon", "latest merch"},
            {"new strong bad email", "new sbemail a-comin'", "email soon"},
            {"new toon soon", "new cartoon!", "hey, a new toon!!"},
            {"more biz cas fri", "biz cas fri", "new biz cas fri!"},
            {"short shorts!", "new short shortly", "new short!"}
        };
    private string[,]threeLetterCodes=new string[,]
        {
             {"SPC","OON","AST"},
             {"ALP","MTN","YDL"},
             {"MBD","SND","HWI"},
             {"BWL","STX","SPR"},
             {"WFH","SSC","NWH"},
             {"ANG","HVN","HRP"},
             {"RIP","DED","GRV"},
             {"SKY","FLL","DVE"},
             {"ART","NTE","SKC"},
             {"OLD","RBH","SLN"},
             {"NVA","NWS","RPT"},
             {"JLY","FWK","USA"},
             {"QBT","TRO","PXL"},
             {"WSH","SHW","TLE"},
             {"FLS","PBC","DRW"},
             {"SNW","SLD","DWN"},
             {"IUP","STK","ANM"},
             {"EHS","SCC","COW"},
             {"SHW","PRZ","TRP"},
             {"STR","BSC","PGE"},
             {"ASP","VCT","GRN"},
             {"VRS","DGR","IIB"},
             {"RVZ","BTS","WOD"},
             {"BLR","GLS","MPS"},
             {"BCF","TXS","PPT"},
             {"XRF","GOS","SPK"},
             {"TRG","PST","QST"}
            };
    private string[]chosenFirstMessages=new string[5];
    private string[]chosenCodes=new string[27];
    private char[,]lettersToChoose=new char[27,4];
    void Start(){
        var RND=rs.GetRNG();
        for(int i=0;i<5;i++){
            chosenFirstMessages[i]=messages[i,RND.Next(3)];
        }
        for(int i=0;i<27;i++){
            chosenCodes[i]=threeLetterCodes[i,RND.Next(3)];
        }
        char[,]firstSet=firstTableLetters(RND);
        char[,]secondSet=firstTableLetters(RND);
        char[,]thirdSet=firstTableLetters(RND);
        for(int i=0;i<27;i++){
            for(int j=0;j<4;j++){
                if(i<9)
                    lettersToChoose[i,j]=firstSet[i,j];
                else if(i<18)
                    lettersToChoose[i,j]=secondSet[i-9,j];
                else
                    lettersToChoose[i,j]=thirdSet[i-18,j];
            }
        }
        int c1=RND.Next(4);
        int c2;
        do{
            c2=RND.Next(4);
        }while(c2==c1);
        firstColorInCond=messageColorNames[c1];
        secondColorInCond=messageColorNames[c2];
        cond=(colorCondition)RND.Next(4);
        turnCond=(turnCondition)RND.Next(4);
        int t1=RND.Next(3);
        int t2;
        do{
            t2=RND.Next(3);
        }while(t2==t1);
        turnIf=(turnType)t1;
        turnElse=(turnType)t2;
        caesarHS=RND.Next(2)==1;
        correctMainPage=UnityEngine.Random.Range(0,27);
        Log("The correct menu is menu {0}.",correctMainPage+1);
        string code=chosenCodes[correctMainPage];
        Log("The decrypted three-letter code is {0}.",code);
        Log("The background is from menu {0}.", (HSBG.BGnumber + 1).ToString());
        Log("Homestar is from menu {0}.", (HSBG.HSnumber + 1).ToString());
        string alphabet="ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if(caesarHS){
            for(int i=0;i<3;i++)
                code+=alphabet[(alphabet.IndexOf(code[i])+HSBG.HSnumber+27)%26];
            code=code.Substring(3);
            Log("Code caesar-shifted according to Homestar's number: "+code);
        }else{
            for(int i=0;i<3;i++)
                code+=alphabet[(alphabet.IndexOf(code[i])+HSBG.BGnumber+27)%26];
            code=code.Substring(3);
            Log("Code caesar-shifted according to the background's number: "+code);
        }
        message1=UnityEngine.Random.Range(0,5);
        do{
            message2=UnityEngine.Random.Range(0,15);
        }while(chosenFirstMessages.Contains(messages[message2%5,message2/5]));

        do{
            message3=UnityEngine.Random.Range(0,15);
        }while(messages[message3%5,message3/5]==messages[message2%5,message2/5]||chosenFirstMessages.Contains(messages[message3%5,message3/5]));

        color1=UnityEngine.Random.Range(0,4);

        do{
            color2=UnityEngine.Random.Range(0,4);
        }while(color2==color1);

        do{
            color3=UnityEngine.Random.Range(0,4);
        }while(color3==color1||color3==color2);
        colorNotPresent=messageColorNames[6-color1-color2-color3];
        Log("The messages are:");
        Log("\"{0}\" in {1}.",chosenFirstMessages[message1],messageColorNames[color1]);
        Log("\"{0}\" in {1}.",messages[message2%5,message2/5],messageColorNames[color2]);
        Log("\"{0}\" in {1}.",messages[message3%5,message3/5],messageColorNames[color3]);
        StartCoroutine(coloredButtonCycle());
        foreach (KMSelectable button in menuButtons){
            button.GetComponent<Renderer>().material = HSBG.bluemat;
            _mpAnims fx = button.GetComponent<_mpAnims>();
            fx.startup(UnityEngine.Random.Range(0,26));
            Log("The {0} button has the menu {1} animation.", buttonNames[fx.num].ToString(), (fx.animNum+1).ToString());
            button.Set(onHighlight: () => {
                if (HSBG.HSnumber != 9 && HSBG.HSnumber != 11) Play(new Sound(HSBG.lines.ToString() + buttonLetters[fx.num].ToString()));
                button.GetComponent<Renderer>().material = HSBG.redmat;
                if (!fx.running) StartCoroutine(fx.assignedAnim);
                StartCoroutine(fx.assignedSay);
            },onHighlightEnded: () => {
                button.GetComponent<Renderer>().material = HSBG.bluemat;
                if (fx.disappear){
                    fx.b.SetActive(false);
                    fx.running = false;
                }});
        }

        foreach (KMSelectable button in numberButtons){
            button.Set(onInteract: () => {
                Log("Menu {0} selected.", button.GetComponentInChildren<TextMesh>().text);
                if(int.Parse(button.GetComponentInChildren<TextMesh>().text)==correctMainPage+1){
                    blinkstop = true;
                    HSBG.HSnumber=int.Parse(button.GetComponentInChildren<TextMesh>().text)-1;
                    HSBG.BGnumber=int.Parse(button.GetComponentInChildren<TextMesh>().text)-1;
                    HSBG.BgHsSetup(HSBG.HSnumber,HSBG.BGnumber);
                    foreach (KMSelectable bu in menuButtons)bu.GetComponent<_mpAnims>().sayingAnims(int.Parse(button.GetComponentInChildren<TextMesh>().text) - 1);
                    Solve("Correct menu selected!");
                }else{
                    Strike("Strike! Correct menu was {0}",correctMainPage+1);
                }
            });
        }
    }

    private char[,] firstTableLetters(MonoRandom rnd){
        char[][]rows=new char[9][]{
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'}
        };
        for(int i=0;i<6;i++){
            int letterCount=0;
            List<char[]>availRows=new List<char[]>();
            foreach(char[]row in rows){
                if(4-row.Count(s=>s!='X')==6-i){
                    letterCount++;
                    for(int j=0;j<4;j++){
                        if(row[j]=='X'){
                            row[j]=buttonLetters[i];
                            break;
                        }
                    }
                }
                else if(row.Count(s=>s!='X')<4){
                    availRows.Add(row);
                }
            }
            while(letterCount<6){
                char[]row=availRows[rnd.Next(availRows.Count)];
                for(int j=0;j<4;j++){
                    if(row[j]=='X'){
                        row[j]=buttonLetters[i];
                        break;
                    }
                }
                letterCount++;
                availRows.RemoveAt(availRows.IndexOf(row));
            }
        }
        char[,]result=new char[9,4];
        for(int i=0;i<rows.Length;i++){
            rnd.ShuffleFisherYates(rows[i]);
            for(int j=0;j<rows[i].Length;j++){
                result[i,j]=rows[i][j];
            }
        }
        return result;
    }

    IEnumerator coloredButtonCycle(){
        while(true){
            messageButton.material=messageColors[color1];
            messageButton.GetComponentInChildren<TextMesh>().text=chosenFirstMessages[message1];
            yield return new WaitForSeconds(2);
            messageButton.material=messageColors[color2];
            messageButton.GetComponentInChildren<TextMesh>().text=messages[message2%5,message2/5];
            yield return new WaitForSeconds(2);
            messageButton.material=messageColors[color3];
            messageButton.GetComponentInChildren<TextMesh>().text=messages[message3%5,message3/5];
            yield return new WaitForSeconds(2);
        }
    }

    IEnumerator ProcessTwitchCommand(string command){
        command = command.ToLowerInvariant().Trim();
        if (command == "cycle"){
            yield return null;
            foreach (KMSelectable button in menuButtons){
                button.OnHighlight();
                button.OnHighlightEnded();
                yield return new WaitForSeconds(1.5f);
            }
            yield break;
        } int i;
        if (int.TryParse(command, out i)){
            yield return null;
            if (int.Parse(command) < 28 && int.Parse(command) > 0) yield return new[] {numberButtons[int.Parse(command) - 1]};
            else yield return "sendtochaterror The menu number must be from 1 to 27.";
            yield break;
        } bool containsatall = false;
        yield return null;
        foreach (char c in command){
            if (Array.Exists(buttonLetters, x => x == char.ToUpperInvariant(c))){
                containsatall = true;
                menuButtons[Array.IndexOf(buttonLetters,char.ToUpperInvariant(c))].OnHighlight();
                menuButtons[Array.IndexOf(buttonLetters,char.ToUpperInvariant(c))].OnHighlightEnded();
                yield return new WaitForSeconds(1.5f);
            }
        } if (!containsatall) yield return "sendtochaterror Invalid command. Use the letters t, g, c, d, s, and e for each button.";
    }
}