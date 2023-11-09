using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Wawa.Extensions;
using Wawa.Modules;

public class _sbemailsongs:ModdedModule{
    public KMSelectable playbutton;
    public KMSelectable[]hexButtons;
    public TextMesh display;
    private string[]transcriptions;
    public Material[]fontMats;
    public Font[]fonts;
    private int[]sizes=new int[]{50,25,40,32,35};
    private int[]ranges=new int[]{41,119,202,203,210};
    Dictionary<int,int>exceptionsToSizePattern=new Dictionary<int,int>();
    void Start(){
        exceptionsToSizePattern.Add(34,27);
        exceptionsToSizePattern.Add(41,23);
        exceptionsToSizePattern.Add(69,23);
        exceptionsToSizePattern.Add(75,18);
        exceptionsToSizePattern.Add(78,22);
        exceptionsToSizePattern.Add(88,17);
        exceptionsToSizePattern.Add(123,43);
        exceptionsToSizePattern.Add(128,44);
        exceptionsToSizePattern.Add(200,27);
        exceptionsToSizePattern.Add(201,37);
        transcriptions=File.ReadAllLines("Assets/_Sbemail Songs/songs/transcriptions.txt",Encoding.UTF8);
        for(int i=0;i<transcriptions.Length;i++){
            while(transcriptions[i].Contains("\\n"))
                transcriptions[i]=transcriptions[i].Replace("\\n","\n");
        }
        playbutton.Set(
            onInteract:()=>playRandomSbS()
        );
    }
    
    private void playRandomSbS(){
        int chosenSong=UnityEngine.Random.Range(1,210);
        for(int i=0;i<5;i++){
            if(chosenSong<ranges[i]){
                display.fontSize=sizes[i];
                display.font=fonts[i];
                display.GetComponent<MeshRenderer>().material=fontMats[i];
                break;
            }
        }
        if(chosenSong<41)
            display.color=Color.green;
        else
            display.color=Color.white;
        if(exceptionsToSizePattern.ContainsKey(chosenSong))
            display.fontSize=exceptionsToSizePattern[chosenSong];
        display.text=transcriptions[chosenSong-1];
        Play(new Sound("AUDIO_ss_"+chosenSong));
        Log("The chosen sbemail song is "+chosenSong+".");
    }
}
