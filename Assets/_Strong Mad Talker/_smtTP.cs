using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using wawa.TwitchPlays;
using wawa.TwitchPlays.Domains;

public sealed class _smtTP:Twitch<_smtscript>{
	[Command("")]
    IEnumerable<Instruction> Press(params string[]command){
		for(int i=0;i<command.Length;i++){
        	command[i]=command[i].ToUpperInvariant().Trim();
		}
        if(command[0]=="PLAYALL"||command[0]=="PLAYWORDS"||command[0]=="PLAYEACH"){
            yield return null;
            yield return TwitchString.SendToChat("The words read: "+string.Join(", ",Module.fulllist.ToArray()));
            if(!Module.playing)Module.PS();
            for(int i=0;i<6;i++){
                yield return new[]{Module.buttons[i]};
                Module.buttons[i].OnHighlightEnded();
                yield return new WaitForSeconds(1.25f);
            } Module.PS();
            yield break;
        } if (command[0]=="R"||command[0]=="RESET"){
            yield return null;
            if (Module.playing || Module.stage == 0) yield return TwitchString.SendToChatError("The input currently cannot be reset.");
            else{
                Module.PS();
                Module.PS();
            } yield break;
        }
        List<KMSelectable> buttonlist = new List<KMSelectable>();
        if (Module.playing) Module.PS();
        bool typomade = false;
        int currentamount;
        foreach (string word in command){
            currentamount=buttonlist.Count;
            if (word == "TL") buttonlist.Add(Module.buttons[0]);
            if (word == "TR") buttonlist.Add(Module.buttons[1]);
            if (word == "CL" || word == "ML") buttonlist.Add(Module.buttons[2]);
            if (word == "CR" || word == "MR") buttonlist.Add(Module.buttons[3]);
            if (word == "BL") buttonlist.Add(Module.buttons[4]);
            if (word == "BR") buttonlist.Add(Module.buttons[5]);
            if (Module.fulllist.Contains(word)) buttonlist.Add(Module.buttons.First(b => b.GetComponentInChildren<TextMesh>().text == word));
            if (buttonlist.Count == currentamount){
                //if the sender makes a typo, such as "!# working proxmiity sweetycakes movie", it'll stop when it reaches the typo and not press the rest of the buttons
                yield return null;
                yield return TwitchString.SendToChatError("{0}, you made a typo.");
                typomade = true;
                break;
            }
        } yield return null;
        foreach (KMSelectable BB in buttonlist){
            yield return new[] {BB};
            BB.OnHighlightEnded();
        }
        if (Module.SMTSettings.SMT_TPResetOnTypo && typomade){
            Module.PS();
            Module.PS();
        }
    }

    public override IEnumerable<Instruction> ForceSolve(){
		Module.PS();//reset input if in Solving
        if(Module.playing)
			Module.PS();//switch to Solving
		foreach(string word in Module.wordlist){
			foreach(KMSelectable button in Module.buttons){
				if(button.GetComponentInChildren<TextMesh>().text==word){
					button.OnInteract();
					button.OnHighlightEnded();
					yield return Instruction.Pause;
				}
			}
		}
		Module.Log("Force solved by Twitch mod.");
    }
}
