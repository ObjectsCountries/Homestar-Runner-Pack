//main script and interactions are handled here (referred to as v in other scripts)
using KeepCoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class _mainpagescript : ModuleScript{
    public KMSelectable[] numberButtons;
    public KMSelectable[] menuButtons;
    public Shader transparentshader;
    public _mpHsBg h;
    public _mpTextures t;
    internal bool speaking;
    internal bool moduleSolved = false;
    internal string[] buttonNames = { "Toons", "Games", "Characters", "Downloads", "Store", "E-mail" };
    internal char[] buttonLetters = { 'T', 'G', 'C', 'D', 'S', 'E' };
    internal bool blinkstop = false;
    internal List<int> takenAnims = new List<int>(){};

    void Start(){
        Log("The background is from menu {0}.", (h.BGnumber + 1).ToString());
        Log("Homestar is from menu {0}.", (h.HSnumber + 1).ToString());
        foreach (KMSelectable button in menuButtons){
            button.GetComponent<Renderer>().material = h.bluemat;
            _mpAnims fx = button.GetComponent<_mpAnims>();
            int num = fx.num;
            Log("The {0} button has the menu {1} animation.", buttonNames[num].ToString(), (fx.animNum+1).ToString());
            button.Assign(onHighlight: () => {
                if (h.HSnumber != 9 && h.HSnumber != 11) PlaySound(h.lines.ToString() + buttonLetters[num].ToString());
                button.GetComponent<Renderer>().material = h.redmat;
                if (!fx.running) StartCoroutine(fx.assignedAnim);
                StartCoroutine(fx.assignedSay);
            },onHighlightEnded: () => {
                button.GetComponent<Renderer>().material = h.bluemat;
                if (fx.disappear){
                    fx.b.SetActive(false);
                    fx.running = false;
                }});
        }
    
        foreach (KMSelectable button in numberButtons){
            button.Assign(onInteract: () => {
                Log("Menu {0} selected.", button.GetComponentInChildren<TextMesh>().text);
                blinkstop = true;
                h.BgHsSetup(int.Parse(button.GetComponentInChildren<TextMesh>().text)-1,int.Parse(button.GetComponentInChildren<TextMesh>().text)-1);
                foreach (KMSelectable bu in menuButtons)bu.GetComponent<_mpAnims>().sayingAnims(int.Parse(button.GetComponentInChildren<TextMesh>().text) - 1);});
        }
    }
}