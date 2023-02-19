//twitch plays support
using KeepCoding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _mpTP : TPScript<_mainpagescript>{
    public override IEnumerator ForceSolve() {throw new NotImplementedException("Todo: Implement the autosolver!");}

    public override IEnumerator Process(string command){
        command = command.ToLowerInvariant().Trim();
        if (command == "cycle"){
            yield return null;
            foreach (KMSelectable button in Module.menuButtons){
                button.OnHighlight();
                button.OnHighlightEnded();
                yield return new WaitForSeconds(1.5f);
            }
            yield break;
        } int i;
        if (int.TryParse(command, out i)){
            yield return null;
            if (int.Parse(command) < 28 && int.Parse(command) > 0) yield return new[] {Module.numberButtons[int.Parse(command) - 1]};
            else yield return "sendtochaterror The menu number must be from 1 to 27.";
            yield break;
        } bool containsatall = false;
        yield return null;
        foreach (char c in command){
            if (Array.Exists(Module.buttonLetters, x => x == char.ToUpperInvariant(c))){
                containsatall = true;
                Module.menuButtons[Module.buttonLetters.IndexOf(char.ToUpperInvariant(c))].OnHighlight();
                Module.menuButtons[Module.buttonLetters.IndexOf(char.ToUpperInvariant(c))].OnHighlightEnded();
                yield return new WaitForSeconds(1.5f);
            }
        } if (!containsatall) yield return "sendtochaterror Invalid command. Use the letters t, g, c, d, s, and e for each button.";
    }
}