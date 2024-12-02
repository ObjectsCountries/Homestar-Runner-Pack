using System;
using System.Collections.Generic;
using UnityEngine;
using wawa.TwitchPlays;
using wawa.TwitchPlays.Domains;

public sealed class _mpTP:Twitch<_mainpagescript>{
    [Command("")]
    IEnumerable<Instruction>process(string command){
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
            else yield return TwitchString.SendToChatError("{0}, the main page number must be from 1 to 27.");
            yield break;
        } bool containsatall = false;
        yield return null;
        foreach (char c in command){
            if (Array.Exists(Module.buttonLetters, x => x == char.ToUpperInvariant(c))){
                containsatall = true;
                Module.menuButtons[Array.IndexOf(Module.buttonLetters,char.ToUpperInvariant(c))].OnHighlight();
                Module.menuButtons[Array.IndexOf(Module.buttonLetters,char.ToUpperInvariant(c))].OnHighlightEnded();
                yield return new WaitForSeconds(1.5f);
            }
        } if (!containsatall) yield return TwitchString.SendToChatError("{0}, invalid command. Use the letters t, g, c, d, s, and e for each button.");
    }

    public override IEnumerable<Instruction>ForceSolve(){
        Module.numberButtons[Module.correctMainPage].OnInteract();
        yield return Instruction.Pause;
    }
}
