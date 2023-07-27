﻿//main script and interactions are handled here (referred to as v in other scripts)
using Wawa.Modules;
using Wawa.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class _mainpagescript:ModdedModule{
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
            button.Set(onHighlight: () => {
                if (h.HSnumber != 9 && h.HSnumber != 11) Play(new Sound(h.lines.ToString() + buttonLetters[num].ToString()));
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
            button.Add(onInteract: () => {
                Log("Menu {0} selected.", button.GetComponentInChildren<TextMesh>().text);
                blinkstop = true;
                h.BgHsSetup(int.Parse(button.GetComponentInChildren<TextMesh>().text)-1,int.Parse(button.GetComponentInChildren<TextMesh>().text)-1);
                foreach (KMSelectable bu in menuButtons)bu.GetComponent<_mpAnims>().sayingAnims(int.Parse(button.GetComponentInChildren<TextMesh>().text) - 1);});
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