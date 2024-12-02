using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using wawa.TwitchPlays;
using wawa.TwitchPlays.Domains;

public sealed class _sidTP:Twitch<_sidhoffrenchmanscript>{
	private string[]hoffOptions=new string[]{"h","hoff","hoffman"};
	private string[]frenchOptions=new string[]{"f","french","frenchman"};
	[Command("")]
    public IEnumerator Select(string command){
        if (!Module.active){
            yield return TwitchString.SendToChatError("{0}, the module isn't currently active.");
            yield break;
        }
		command=command.ToLowerInvariant().Trim();
        yield return null;
        if (hoffOptions.Contains(command)) yield return Module.Submitting(Module.correct(), true);
        else if (frenchOptions.Contains(command)) yield return Module.Submitting(Module.correct(), false);
        else yield return TwitchString.SendToChatError("{0}, your command must be h/hoff/hoffman for \"Sid Hoffman\", or f/french/frenchman for \"Sid Frenchman\".");
    }
	public override IEnumerable<Instruction> ForceSolve(){
		KMSelectable button;
        if (Module.active){
			button=Module.correct()?Module.hoffButton:Module.frenchButton;
			button.OnInteract();
			button.OnHighlightEnded();
		}
        else yield return TwitchString.SendToChatError("{0}, the module isn't currently active.");
    }
}
