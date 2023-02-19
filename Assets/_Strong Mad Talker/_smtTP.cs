using KeepCoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class _smtTP : TPScript<_smtscript>{
    public override IEnumerator ForceSolve(){
        //i just made it look like it's solved bc it really makes no difference
        Module.Solve("Force solved by Twitch mod.");
        if (!Module.playing) Module.PS();
        Module.moduleSolved = true;
        foreach (GameObject star in Module.stars){
            star.SetActive(true);
        }
        yield return null;
    }

    public override IEnumerator Process(string command){
        command = command.ToUpperInvariant().Trim();
        if (command == "PLAYALL" || command == "PLAYWORDS" || command == "PLAYEACH"){
            yield return null;
            yield return "sendtochat The words read: " + string.Join(", ", Module.fulllist.ToArray());
            if (!Module.playing) Module.PS();
            for (int i = 0; i < 6; i++){
                yield return new[] {Module.buttons[i]};
                Module.buttons[i].OnHighlightEnded();
                yield return new WaitForSeconds(1.25f);
            } Module.PS();
            yield break;
        } if (command == "R"){
            yield return null;
            if (Module.playing || Module.stage == 0) yield return "sendtochaterror The input currently cannot be reset.";
            else{
                Module.PS();
                Module.PS();
            } yield break;
        } string[] words = command.Split(' ');
        List<KMSelectable> buttonlist = new List<KMSelectable>();
        if (Module.playing) Module.PS();
        bool typomade = false;
        foreach (string word in words){
            int currentamount = buttonlist.LengthOrDefault();
            if (word == "TL") buttonlist.Add(Module.buttons[0]);
            if (word == "TR") buttonlist.Add(Module.buttons[1]);
            if (word == "CL" || word == "ML") buttonlist.Add(Module.buttons[2]);
            if (word == "CR" || word == "MR") buttonlist.Add(Module.buttons[3]);
            if (word == "BL") buttonlist.Add(Module.buttons[4]);
            if (word == "BR") buttonlist.Add(Module.buttons[5]);
            if (Module.fulllist.Contains(word)) buttonlist.Add(Module.buttons.First(b => b.GetComponentInChildren<TextMesh>().text == word));
            if (buttonlist.LengthOrDefault() == currentamount){
                //if the sender makes a typo, such as "!# working proxmiity sweetycakes movie", it'll stop when it reaches the typo and not press the rest of the buttons
                yield return null;
                yield return "sendtochaterror {0}, you made a typo.";
                typomade = true;
                break;
            }
        } yield return null;
        foreach (KMSelectable BB in buttonlist){
            yield return new[] {BB};
            BB.OnHighlightEnded();
        }
        if (Module.ModSettings.SMT_TPResetOnTypo && typomade){
            Module.PS();
            Module.PS();
        }
    }
}
