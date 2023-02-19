using KeepCoding;
using System.Collections;

public class _sidTP : TPScript<_sidhoffrenchmanscript> {
    public override IEnumerator ForceSolve(){
        if (Module.active) yield return Module.correct()?Module.hoffButton:Module.frenchButton;
        else yield return "sendtochaterror {0}, the module isn't currently active.";
        yield return null;
    }
    public override IEnumerator Process(string command){
        command = command.ToLowerInvariant().Trim();
        yield return null;
        if (!Module.active){
            yield return "sendtochaterror {0}, the module isn't currently active.";
            yield break;
        }
        if (command.Equals("h")) yield return Module.Submitting(Module.correct(), true);
        else if (command.Equals("f")) yield return Module.Submitting(Module.correct(), false);
        else yield return "sendtochaterror {0}, your command must consist of only the letter h or f.";
    }
}