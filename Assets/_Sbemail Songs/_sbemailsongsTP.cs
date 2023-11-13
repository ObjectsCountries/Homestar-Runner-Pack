using System;
using System.Collections.Generic;
using UnityEngine;
using Wawa.TwitchPlays;
using Wawa.TwitchPlays.Domains;

public sealed class _sbemailsongsTP:Twitch<_sbemailsongs>{
    private readonly string hexDigits="0123456789ABCDEF";
    [Command("")]
    IEnumerable<Instruction>Input(string command){
        command=command.ToUpperInvariant();
        if(command=="PLAY"){
            yield return new[]{Module.playbutton};
            yield break;
        }
        bool allHexDigits=true;
        foreach(char c in command){
            if(!hexDigits.Contains(c.ToString())){
                yield return TwitchString.SendToChatError("{0}, your input contains a character that is not a hexadecimal digit.");
                allHexDigits=false;
                break;
            }
        }
        if(allHexDigits){
            if(!Module.submissionMode){
                yield return TwitchString.SendToChatError("{0}, you can't submit yet.");
                yield break;
            }else{
                List<KMSelectable>result=new List<KMSelectable>();
                foreach(char c in command)
                    result.Add(Module.hexButtons[hexDigits.IndexOf(c.ToString())]);
                yield return result.ToArray();
            }
        }
    }

    public override IEnumerable<Instruction>ForceSolve(){
        if(!Module.submissionMode){
            yield return TwitchString.SendToChatError("{0}, you can't solve yet.");
            yield break;
        }else{
            List<KMSelectable>result=new List<KMSelectable>();
            foreach(char c in Module.finalSequence)
                result.Add(Module.hexButtons[hexDigits.IndexOf(c.ToString())]);
            yield return result.ToArray();
        }
    }
}