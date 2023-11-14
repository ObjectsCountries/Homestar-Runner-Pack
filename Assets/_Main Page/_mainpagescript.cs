
using Wawa.Modules;
using Wawa.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

///<summary>Main script and interactions.</summary>
public class _mainpagescript:ModdedModule{
    public KMSelectable[] numberButtons;
    public KMSelectable[] menuButtons;
    public Renderer messageButton;
    public Material[] messageColors;
    public string[] messageColorNames=new string[]{"blue","green","red","yellow"};//public for Souvenir
    public Shader transparentshader;
    public _mpHsBg HSBG;
    public _mpTextures TXTRs;
    public KMRuleSeedable rs;
    public KMBombInfo bomb;
    public KMColorblindMode colorblind;
    public MeshRenderer[]colorblindBubbles;
    public Material colorblindUNHL,colorblindHL;
    internal bool speaking;
    internal bool moduleSolved = false;
    internal string[] buttonNames={"Toons","Games","Characters","Downloads","Store","E-mail"};
    internal char[] buttonLetters={'T','G','C','D','S','E'};
    internal bool blinkstop = false;
    public int message1,message2,message3,color1,color2,color3;//public for Souvenir
    string colorNotPresent;
    private enum colorCondition{
        COLOR_PRESENT,
        COLOR_NOT_PRESENT,
        BOTH_COLORS_PRESENT,
        EITHER_COLOR_NOT_PRESENT
    }
    private enum turnCondition{
        DEFAULT_VOICE_LINES,
        NOT_DEFAULT_VOICE_LINES
    }
    private enum turnType{
        REVERSE,
        SWAP_PAIRS,
        SWAP_HALVES
    }
    private string firstColorInCond,secondColorInCond;
    private colorCondition cond;
    private turnCondition turnCond;
    private turnType turnIf,turnElse;
    private bool caesarHS;
    public int correctMainPage;//public for Souvenir
    public int[]effects=new int[6]{-1,-1,-1,-1,-1,-1};//public for Souvenir
    public string[,]messages=new string[,]//public for Souvenir
        {
            {"play a game", "latest toon", "latest merch"},
            {"new strong bad email", "new sbemail a comin", "email soon"},
            {"new toon soon", "new cartoon!", "hey, a new toon!!"},
            {"more biz cas fri", "biz cas fri", "new biz cas fri!"},
            {"short shorts!", "new short shortly", "new short!"}
        };
    private string[,]messagesLettersOnly=new string[,]
        {
            {"playagame","latesttoon","latestmerch"},
            {"newstrongbademail","newsbemailacomin","emailsoon"},
            {"newtoonsoon","newcartoon","heyanewtoon"},
            {"morebizcasfri","bizcasfri","newbizcasfri"},
            {"shortshorts","newshortshortly","newshort"}
        };
    private string[,]threeLetterCodes=new string[,]
        {
             {"SPC","OON","AST"},
             {"ALP","MTN","YDL"},
             {"MBD","SND","HWI"},
             {"BWL","STX","SPR"},
             {"WFH","SSC","NWH"},
             {"ANG","HVN","HRP"},
             {"RIP","DED","GRV"},
             {"SKY","FLL","DVE"},
             {"ART","NTE","SKC"},
             {"OLD","RBH","SLN"},
             {"NVA","NWS","RPT"},
             {"JLY","FWK","USA"},
             {"QBT","TRO","PXL"},
             {"WSH","SHW","TLE"},
             {"FLS","PBC","DRW"},
             {"SNW","SLD","DWN"},
             {"IUP","STK","ANM"},
             {"EHS","SCC","COW"},
             {"SHW","PRZ","TRP"},
             {"STR","BSC","PGE"},
             {"ASP","VCT","GRN"},
             {"VRS","DGR","IIB"},
             {"RVZ","BTS","WOD"},
             {"BLR","GLS","MPS"},
             {"BCF","TXS","PPT"},
             {"XRF","GOS","SPK"},
             {"TRG","PST","QST"}
            };
    private string[]chosenFirstMessages=new string[5];
    private string[]chosenFirstMessagesLettersOnly=new string[5];
    private string[]chosenCodes=new string[27];
    private char[,]lettersToChoose=new char[27,4];
    void Start(){
        var RND=rs.GetRNG();
        for(int i=0;i<5;i++){
            int j=RND.Next(3);
            chosenFirstMessages[i]=messages[i,j];
            chosenFirstMessagesLettersOnly[i]=messagesLettersOnly[i,j];
        }
        for(int i=0;i<27;i++){
            chosenCodes[i]=threeLetterCodes[i,RND.Next(3)];
        }
        char[,]firstSet=firstTableLetters(RND);
        char[,]secondSet=firstTableLetters(RND);
        char[,]thirdSet=firstTableLetters(RND);
        for(int i=0;i<27;i++){
            for(int j=0;j<4;j++){
                if(i<9)
                    lettersToChoose[i,j]=firstSet[i,j];
                else if(i<18)
                    lettersToChoose[i,j]=secondSet[i-9,j];
                else
                    lettersToChoose[i,j]=thirdSet[i-18,j];
            }
        }
        int c1=RND.Next(4);
        int c2;
        do{
            c2=RND.Next(4);
        }while(c2==c1);
        firstColorInCond=messageColorNames[c1];
        secondColorInCond=messageColorNames[c2];
        cond=(colorCondition)RND.Next(4);
        turnCond=(turnCondition)RND.Next(2);
        int t1=RND.Next(3);
        int t2;
        do{
            t2=RND.Next(3);
        }while(t2==t1);
        turnIf=(turnType)t1;
        turnElse=(turnType)t2;
        caesarHS=RND.Next(2)==1;
        correctMainPage=UnityEngine.Random.Range(0,27);
        Log("The correct main page is main page {0}.",correctMainPage+1);
        string code=chosenCodes[correctMainPage];
        Log("The decrypted three-letter code is {0}.",code);
        Log("The background is from main page {0}.", (HSBG.BGnumber + 1).ToString());
        Log("Homestar is from main page {0}.", (HSBG.HSnumber + 1).ToString());
        string alphabet="ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if(caesarHS){
            for(int i=0;i<3;i++)
                code+=alphabet[(alphabet.IndexOf(code[i])+HSBG.HSnumber+27)%26];
            code=code.Substring(3);
            Log("Code caesar-shifted according to Homestar's number: "+code);
        }else{
            for(int i=0;i<3;i++)
                code+=alphabet[(alphabet.IndexOf(code[i])+HSBG.BGnumber+27)%26];
            code=code.Substring(3);
            Log("Code caesar-shifted according to the background's number: "+code);
        }
        message1=UnityEngine.Random.Range(0,5);
        do{
            message2=UnityEngine.Random.Range(0,15);
        }while(chosenFirstMessages.Contains(messages[message2%5,message2/5]));

        do{
            message3=UnityEngine.Random.Range(0,15);
        }while(messages[message3%5,message3/5]==messages[message2%5,message2/5]||chosenFirstMessages.Contains(messages[message3%5,message3/5]));

        color1=UnityEngine.Random.Range(0,4);

        do{
            color2=UnityEngine.Random.Range(0,4);
        }while(color2==color1);

        do{
            color3=UnityEngine.Random.Range(0,4);
        }while(color3==color1||color3==color2);
        colorNotPresent=messageColorNames[6-color1-color2-color3];
        Log("The messages are:");
        Log("\"{0}\" in {1}.",chosenFirstMessages[message1],messageColorNames[color1]);
        Log("\"{0}\" in {1}.",messages[message2%5,message2/5],messageColorNames[color2]);
        Log("\"{0}\" in {1}.",messages[message3%5,message3/5],messageColorNames[color3]);
        char effect1=lettersToChoose[HSBG.BGnumber,color1];
        char effect2=lettersToChoose[HSBG.BGnumber,color2];
        char effect3=lettersToChoose[HSBG.BGnumber,color3];
        char[]effectsChosen=new char[]{effect1,effect2,effect3};
        string eighteenLetterCode="";
        foreach(KMSelectable button in menuButtons){
            _mpAnims fx=button.GetComponent<_mpAnims>();
            fx.animNum=UnityEngine.Random.Range(0,26);
            if(!effectsChosen.Contains(buttonLetters[fx.num])){
                eighteenLetterCode+=chosenCodes[fx.animNum];
                effects[fx.num]=fx.animNum;
            }
        }
        eighteenLetterCode+=chosenCodes[HSBG.BGnumber]+chosenCodes[HSBG.HSnumber]+alphabet[(chosenFirstMessagesLettersOnly[message1].Length+25)%26]+alphabet[(messagesLettersOnly[message2%5,message2/5].Length+25)%26]+alphabet[(messagesLettersOnly[message3%5,message3/5].Length+25)%26];
        Log("The eighteen-letter code is "+eighteenLetterCode+".");
        bool condTrue;
        switch(cond){
            case colorCondition.COLOR_PRESENT:
                condTrue=color1==c1||color2==c1||color3==c1;
                if(condTrue)
                    Log(messageColorNames[c1].Substring(0,1).ToUpper()+messageColorNames[c1].Substring(1)+" is present, skewing left.");
                else
                    Log(messageColorNames[c1].Substring(0,1).ToUpper()+messageColorNames[c1].Substring(1)+" is not present, skewing right.");
                break;
            case colorCondition.COLOR_NOT_PRESENT:
                condTrue=colorNotPresent==messageColorNames[c1];
                if(condTrue)
                    Log(messageColorNames[c1].Substring(0,1).ToUpper()+messageColorNames[c1].Substring(1)+" is not present, skewing left.");
                else
                    Log(messageColorNames[c1].Substring(0,1).ToUpper()+messageColorNames[c1].Substring(1)+" is present, skewing right.");
                break;
            case colorCondition.BOTH_COLORS_PRESENT:
                condTrue=(color1==c1||color2==c1||color3==c1)&&(color1==c2||color2==c2||color3==c2);
                if(condTrue)
                    Log(messageColorNames[c1].Substring(0,1).ToUpper()+messageColorNames[c1].Substring(1)+" and "+messageColorNames[c2]+" are both present, skewing left.");
                else
                    Log("Either "+messageColorNames[c1]+" or "+messageColorNames[c2]+" is not present, skewing right.");
                break;
            case colorCondition.EITHER_COLOR_NOT_PRESENT:
            default:
                condTrue=!((color1==c1||color2==c1||color3==c1)&&(color1==c2||color2==c2||color3==c2));
                if(condTrue)
                    Log("Either "+messageColorNames[c1]+" or "+messageColorNames[c2]+" is not present, skewing left.");
                else
                    Log(messageColorNames[c1].Substring(0,1).ToUpper()+messageColorNames[c1].Substring(1)+" and "+messageColorNames[c2]+" are both present, skewing right.");
                break;
        }
        if(condTrue){
            eighteenLetterCode=""+eighteenLetterCode[0]+eighteenLetterCode[17]+eighteenLetterCode[1]+eighteenLetterCode[16]+eighteenLetterCode[2]+eighteenLetterCode[15]+eighteenLetterCode[3]+eighteenLetterCode[14]+eighteenLetterCode[4]+eighteenLetterCode[13]+eighteenLetterCode[5]+eighteenLetterCode[12]+eighteenLetterCode[6]+eighteenLetterCode[11]+eighteenLetterCode[7]+eighteenLetterCode[10]+eighteenLetterCode[8]+eighteenLetterCode[9];
        }else{
            eighteenLetterCode=""+eighteenLetterCode[13]+eighteenLetterCode[14]+eighteenLetterCode[12]+eighteenLetterCode[15]+eighteenLetterCode[11]+eighteenLetterCode[16]+eighteenLetterCode[10]+eighteenLetterCode[17]+eighteenLetterCode[9]+eighteenLetterCode[0]+eighteenLetterCode[8]+eighteenLetterCode[1]+eighteenLetterCode[7]+eighteenLetterCode[2]+eighteenLetterCode[6]+eighteenLetterCode[3]+eighteenLetterCode[5]+eighteenLetterCode[4];
        }
        Log("The rearranged 18-letter code is "+eighteenLetterCode+".");
        bool turnTrue;
        int[]notDefaultVoiceLines=new int[8];
        switch(turnCond){
            case turnCondition.DEFAULT_VOICE_LINES:
                notDefaultVoiceLines=new int[]{9,11,12,14,16,19,20,25};
                turnTrue=!notDefaultVoiceLines.Contains(HSBG.HSnumber);
                if(turnTrue)
                    Log("Homestar has the default voice lines.");
                else
                    Log("Homestar does not have the default voice lines.");
                break;
            case turnCondition.NOT_DEFAULT_VOICE_LINES:
            default:
                notDefaultVoiceLines=new int[]{9,11,12,14,16,19,20,25};
                turnTrue=notDefaultVoiceLines.Contains(HSBG.HSnumber);
                if(turnTrue)
                    Log("Homestar does not have the default voice lines.");
                else
                    Log("Homestar has the default voice lines.");
                break;
        }
        turnType resultingTurn=turnTrue?(turnType)t1:(turnType)t2;
        switch(resultingTurn){
            case turnType.REVERSE:
                char[]tempArray=eighteenLetterCode.ToCharArray();
                Array.Reverse(tempArray);
                eighteenLetterCode=new string(tempArray);
                Log("The eighteen-letter code has been reversed. It is now "+eighteenLetterCode+".");
                break;
            case turnType.SWAP_PAIRS:
                eighteenLetterCode=""+eighteenLetterCode[16]+eighteenLetterCode[17]+eighteenLetterCode[14]+eighteenLetterCode[15]+eighteenLetterCode[12]+eighteenLetterCode[13]+eighteenLetterCode[10]+eighteenLetterCode[11]+eighteenLetterCode[8]+eighteenLetterCode[9]+eighteenLetterCode[6]+eighteenLetterCode[7]+eighteenLetterCode[4]+eighteenLetterCode[5]+eighteenLetterCode[2]+eighteenLetterCode[3]+eighteenLetterCode[0]+eighteenLetterCode[1];
                Log("The eighteen-letter code has been swapped in pairs. It is now "+eighteenLetterCode+".");
                break;
            case turnType.SWAP_HALVES:
                eighteenLetterCode=eighteenLetterCode.Substring(9)+eighteenLetterCode.Substring(0,9);
                Log("The eighteen-letter code's first and second halves have been swapped. It is now "+eighteenLetterCode+".");
                break;
        }
        string tempCode="";
        foreach(char c in eighteenLetterCode){
            int letterIndex=alphabet.IndexOf(c);
            while(tempCode.Contains(alphabet[letterIndex])){
                letterIndex=(letterIndex+1)%26;
            }
            tempCode+=alphabet[letterIndex];
        }
        eighteenLetterCode=tempCode;
        Log("Making all letters unique, the eighteen-letter code is now "+eighteenLetterCode+".");
        char[]firstRow=new char[13];
        char[]secondRow=new char[13];
        for(int i=0;i<13;i++)
            firstRow[i]=eighteenLetterCode[i];
        for(int i=13;i<18;i++)
            secondRow[i-13]=eighteenLetterCode[i];
        string last8letters="";
        for(int i=0;i<26;i++){
            if(firstRow.Contains(alphabet[i])||secondRow.Contains(alphabet[i]))
                continue;
            last8letters+=alphabet[i];
        }
        for(int i=5;i<13;i++)
            secondRow[i]=last8letters[i-5];
        Log("The resulting table is as follows:");
        Log(firstRow);
        Log(secondRow);
        string tempEncryptedCode="";
        foreach(char c in code){
            if(firstRow.Contains(c))
                tempEncryptedCode+=""+secondRow[Array.IndexOf(firstRow,c)];
            else if(secondRow.Contains(c))
                tempEncryptedCode+=""+firstRow[Array.IndexOf(secondRow,c)];
        }
        code=tempEncryptedCode;
        Log("Using this table, the code has been encrypted to "+code+".");
        char[]serialLetters=bomb.GetSerialNumberLetters().Distinct().ToArray();
        char[]serialLettersAlphabetized=serialLetters.OrderBy(c=>c).ToArray();
        string serialLettersString="";
        foreach(char c in serialLetters)
            serialLettersString+=""+c;
        Log("The distinct letters in the serial number are "+serialLettersString+".");
        if(serialLetters.Length==3){
            Log("Because there are three distinct letters in the serial number, the code needs to be rearranged.");
            if(serialLetters[0]==serialLettersAlphabetized[0]){
                if(serialLetters[1]==serialLettersAlphabetized[1]){
                    Log("The serial number's distinct letters are already in alphabetical order, so the code has not been rearranged.");
                }else{
                    Log("The second and third letters need to be swapped.");
                    code=""+code[0]+code[2]+code[1];
                }
            }else if(serialLetters[0]==serialLettersAlphabetized[1]){
                if(serialLetters[1]==serialLettersAlphabetized[2]){
                    Log("The third letter should be placed at the beginning.");
                    code=""+code[1]+code[2]+code[0];
                }else{
                    Log("The first and second letters need to be swapped.");
                    code=""+code[1]+code[0]+code[2];
                }
            }else if(serialLetters[0]==serialLettersAlphabetized[2]){
                if(serialLetters[2]==serialLettersAlphabetized[0]){
                    Log("The first and third letters need to be swapped (the string should be reversed).");
                    code=""+code[2]+code[1]+code[0];
                }else{
                    Log("The first letter should be placed at the end.");
                    code=""+code[2]+code[0]+code[1];
                }
            }
            Log("The code has been rearranged to "+code+".");
        }else{
            Log("Because there are not three distinct letters in the serial number, the code does not need to be rearranged.");
        }
        foreach(KMSelectable button in menuButtons){
            _mpAnims fx=button.GetComponent<_mpAnims>();
            if(effects[fx.num]==-1){
                if(buttonLetters[fx.num]==effect1)
                    fx.animNum=alphabet.IndexOf(code[0]);
                else if(buttonLetters[fx.num]==effect2)
                    fx.animNum=alphabet.IndexOf(code[1]);
                else
                    fx.animNum=alphabet.IndexOf(code[2]);

            }
        }
        StartCoroutine(coloredButtonCycle());
        foreach (KMSelectable button in menuButtons){
            button.GetComponent<Renderer>().material = HSBG.bluemat;
            _mpAnims fx = button.GetComponent<_mpAnims>();
            fx.b.SetActive(false);
            fx.assignedAnim = fx.animations[fx.animNum];
            StartCoroutine(fx.setups[fx.animNum]);
            StartCoroutine(fx.san());
            Log("The {0} button has the main page {1} animation.", buttonNames[fx.num].ToString(), (fx.animNum+1).ToString());
            button.Set(onHighlight: () => {
                button.GetComponent<Renderer>().material = HSBG.redmat;
                if(!Status.IsSolved){
                    if (HSBG.HSnumber != 9 && HSBG.HSnumber != 11) Play(new Sound(HSBG.lines.ToString() + buttonLetters[fx.num].ToString()));
                    if (!fx.running) StartCoroutine(fx.assignedAnim);
                    StartCoroutine(fx.assignedSay);
                }
            },onHighlightEnded: () => {
                button.GetComponent<Renderer>().material = HSBG.bluemat;
                if(!Status.IsSolved){
                    if (fx.disappear){
                        fx.b.SetActive(false);
                        fx.running = false;
                }
                }});
        }

        foreach (KMSelectable button in numberButtons){
            button.Set(onInteract: () => {
                Shake(button,.5f,Sound.BigButtonPress);
                if(!Status.IsSolved){
                    Log("Main page {0} selected.", button.GetComponentInChildren<TextMesh>().text);
                    if(int.Parse(button.GetComponentInChildren<TextMesh>().text)==correctMainPage+1){
                        blinkstop = true;
                        HSBG.HSnumber=int.Parse(button.GetComponentInChildren<TextMesh>().text)-1;
                        HSBG.BGnumber=int.Parse(button.GetComponentInChildren<TextMesh>().text)-1;
                        HSBG.BgHsSetup(HSBG.HSnumber,HSBG.BGnumber);
                        foreach (KMSelectable bu in menuButtons)
                            bu.GetComponent<_mpAnims>().sayingAnims(int.Parse(button.GetComponentInChildren<TextMesh>().text) - 1);
                        Solve("Correct main page selected!");
                        HSBG.oldtimeysay.SetActive(false);
                    }else{
                        Strike("Strike! Correct main page was {0}",correctMainPage+1);
                    }
                }
            });
        }
    }

    private char[,] firstTableLetters(MonoRandom rnd){
        char[][]rows=new char[9][]{
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'},
            new char[4]{'X','X','X','X'}
        };
        for(int i=0;i<6;i++){
            int letterCount=0;
            List<char[]>availRows=new List<char[]>();
            foreach(char[]row in rows){
                if(4-row.Count(s=>s!='X')==6-i){
                    letterCount++;
                    for(int j=0;j<4;j++){
                        if(row[j]=='X'){
                            row[j]=buttonLetters[i];
                            break;
                        }
                    }
                }
                else if(row.Count(s=>s!='X')<4){
                    availRows.Add(row);
                }
            }
            while(letterCount<6){
                char[]row=availRows[rnd.Next(availRows.Count)];
                for(int j=0;j<4;j++){
                    if(row[j]=='X'){
                        row[j]=buttonLetters[i];
                        break;
                    }
                }
                letterCount++;
                availRows.RemoveAt(availRows.IndexOf(row));
            }
        }
        char[,]result=new char[9,4];
        for(int i=0;i<rows.Length;i++){
            rnd.ShuffleFisherYates(rows[i]);
            for(int j=0;j<rows[i].Length;j++){
                result[i,j]=rows[i][j];
            }
        }
        return result;
    }

    private IEnumerator coloredButtonCycle(){
        if(colorblind.ColorblindModeActive){
            foreach(MeshRenderer bubble in colorblindBubbles){
                bubble.gameObject.SetActive(true);
                bubble.GetComponentInChildren<TextMesh>().color=Color.gray;
            }
        }
        var old=Time.time;
        while(!Status.IsSolved){
            old=Time.time;
            messageButton.material=messageColors[color1];
            colorblindColorToggle(color1);
            messageButton.GetComponentInChildren<TextMesh>().text=chosenFirstMessages[message1];
            yield return new WaitWhile(()=>old+2>Time.time&&!Status.IsSolved);
            if(Status.IsSolved)
                break;
            old=Time.time;
            messageButton.material=messageColors[color2];
            colorblindColorToggle(color2);
            messageButton.GetComponentInChildren<TextMesh>().text=messages[message2%5,message2/5];
            yield return new WaitWhile(()=>old+2>Time.time&&!Status.IsSolved);
            if(Status.IsSolved)
                break;
            old=Time.time;
            messageButton.material=messageColors[color3];
            colorblindColorToggle(color3);
            messageButton.GetComponentInChildren<TextMesh>().text=messages[message3%5,message3/5];
            yield return new WaitWhile(()=>old+2>Time.time&&!Status.IsSolved);
            if(Status.IsSolved)
                break;
        }
        messageButton.GetComponentInChildren<TextMesh>().text="solved!";
        messageButton.material=messageColors[1];
        colorblindColorToggle(1);

    }
    private void colorblindColorToggle(int index){
        foreach(MeshRenderer bubble in colorblindBubbles){
            bubble.material=colorblindUNHL;
            bubble.GetComponentInChildren<TextMesh>().color=Color.gray;
        }
        colorblindBubbles[index].material=colorblindHL;
        colorblindBubbles[index].GetComponentInChildren<TextMesh>().color=Color.white;
    }
}
