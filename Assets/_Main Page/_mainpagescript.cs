
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
    public KMSelectable messageButton;
    public Material[] messageColors;
    private string[] messageColorNames=new string[]{"blue","green","red","yellow"};
    public Shader transparentshader;
    public _mpHsBg HSBG;
    public _mpTextures TXTRs;
    internal bool speaking;
    internal bool moduleSolved = false;
    internal string[] buttonNames={"Toons","Games","Characters","Downloads","Store","E-mail"};
    internal char[] buttonLetters={'T','G','C','D','S','E'};
    internal bool blinkstop = false;
    internal int message1,message2,message3,color1,color2,color3;
    string colorNotPresent;
    ///<value>Messages to be displayed in the top-right bubble. For the purposes of solving the module, the messages in <c>messages[0]</c> are considered the "real" messages.</value>
    ///<remarks>how do i get rid of the backslashes</remarks>
    private string[][]messages=new string[][]
                                {new string[]{"play a game" ,"new strong\nbad email","new toon soon"    ,"more biz cas fri","short shorts!"    },
                                 new string[]{"latest toon" ,"new sbemail\na comin" ,"new cartoon!"     ,"biz cas fri"     ,"new short shortly"},
                                 new string[]{"latest merch","email soon"           ,"hey, a new toon!!","new biz cas fri!","new short!"       }};

    void Start(){
        Log("The background is from menu {0}.", (HSBG.BGnumber + 1).ToString());
        Log("Homestar is from menu {0}.", (HSBG.HSnumber + 1).ToString());
        foreach (KMSelectable button in menuButtons){
            button.GetComponent<Renderer>().material = HSBG.bluemat;
            _mpAnims fx = button.GetComponent<_mpAnims>();
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
                blinkstop = true;
                HSBG.HSnumber=int.Parse(button.GetComponentInChildren<TextMesh>().text)-1;
                HSBG.BGnumber=int.Parse(button.GetComponentInChildren<TextMesh>().text)-1;
                HSBG.BgHsSetup(HSBG.HSnumber,HSBG.BGnumber);
                foreach (KMSelectable bu in menuButtons)bu.GetComponent<_mpAnims>().sayingAnims(int.Parse(button.GetComponentInChildren<TextMesh>().text) - 1);});
        }
        message1=UnityEngine.Random.Range(0,5);

        do{
            message2=UnityEngine.Random.Range(0,5);
        }while(message2==message1);

        do{
            message3=UnityEngine.Random.Range(0,5);
        }while(message3==message1||message3==message2);

        color1=UnityEngine.Random.Range(0,4);

        do{
            color2=UnityEngine.Random.Range(0,4);
        }while(color2==color1);

        do{
            color3=UnityEngine.Random.Range(0,4);
        }while(color3==color1||color3==color2);
        colorNotPresent=messageColorNames[6-color1-color2-color3];
        Log("The messages are:");
        Log("\"{0}\" in {1}.",messages[0][message1],messageColorNames[color1]);
        Log("\"{0}\" in {1}.",messages[1][message2],messageColorNames[color2]);
        Log("\"{0}\" in {1}.",messages[2][message3],messageColorNames[color3]);
        StartCoroutine(coloredButtonCycle());
    }

    IEnumerator coloredButtonCycle(){
        while(true){
            messageButton.GetComponent<Renderer>().material=messageColors[color1];
            messageButton.GetComponentInChildren<TextMesh>().text=messages[0][message1];
            yield return new WaitForSeconds(2);
            messageButton.GetComponent<Renderer>().material=messageColors[color2];
            messageButton.GetComponentInChildren<TextMesh>().text=messages[1][message2];
            yield return new WaitForSeconds(2);
            messageButton.GetComponent<Renderer>().material=messageColors[color3];
            messageButton.GetComponentInChildren<TextMesh>().text=messages[2][message3];
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