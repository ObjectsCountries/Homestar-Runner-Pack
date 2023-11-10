using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Wawa.Extensions;
using Wawa.Modules;
using Wawa.Optionals;

public class _sbemailsongs:ModdedModule{
    public KMSelectable playbutton;
    public KMSelectable[]hexButtons;
    public TextMesh display,stageNum;
    public AudioClip[]lines;
    public Material[]fontMats;
    public Font[]fonts;
    public KMAudio Audio;
    public KMBombModule thisModule;
    public KMBombInfo bomb;
    public KMBossModule boss;
    public KMRuleSeedable rs;
    private int[]sizes=new int[]{50,25,40,32,35};
    private int[]ranges=new int[]{41,119,202,203,210};
    private Dictionary<int,int>exceptionsToSizePattern=new Dictionary<int,int>();
    private bool currentlyPlaying;
    private bool submissionMode=false;
    private bool activated=false;
    private int chosenSong;
    private int totalNonIgnored=-1;
    private int numSolved=-1;
    private string[]ignoredModules;
    private string lastSolvedModule="";
    private List<int>stages;
    private string finalSequence="";
    private KMAudio.KMAudioRef lineRef;
    private string[]moduleNameHas=new string[]{"wire","maze","simon","morse","button"};
    private string[]orders=new string[]{
        "56781234",
        "21436587",
        "56341278",
        "12785634",
        "87654321",
        "12563478",
        "78123456",
        "45612378"
    };
    private string[]boolTypes=new string[]{
        "AND",
        "OR",
        "XOR",
        "NAND",
        "NOR",
        "XNOR"
    };
    private string[]operations=new string[]{
        "+",
        "-",
        "*",
        "%"
    };
    private string[]conditions=new string[]{
        "PREV DIGIT RANGE",
        "STAGE DIVISIBLE",
        "MAJORITY PARITY",
        "CURRENT DIGIT PARITY"
    };
    private const string hexDigits="0123456789ABCDEF";
    private int rangeLowerIndex,rangeUpperIndex;
    private bool majorityEven,currentDigitEven;
    private int divisibleby;
    private int[]operationDigits=new int[4];
    private string[]transcriptions=new string[]{
        "Oh, who is the guy that checks all\nhis emails? That's me, Strong Bad.",
        "I'm gonna check my email all of\nthe time, doo doo doo…",
        "Checkin' my email, checkin' my\nemail, checkin'-checkin'-checkin'-\ncheckin' my email.",
        "I check the email once, I check the\nemail twice, doo doo doo, doo doo\ndoo…",
        "Checkin' emails is like the best\nthing I do.",
        "I check, you check, we all check\nfor e… mail check… an email…",
        "I am going to check my email.",
        "I remember the time when I\nchecked my email.",
        "I've carefully set aside this time\nfor checking my email.",
        "I'm totally checking my email.\nTotal, man.",
        "Email, grumble grumble, email,\ngrumble…",
        "Initiate email check in 5, 4, 3, 2, 1.",
        "Email, dud-duh-doo-duh-dud-duh,\ndud-duh-doo-duh-dud-duh…",
        "Must… check… email…",
        "Everybody… check your email…",
        "Who's that giving Strong Bad a\nhand? Email… Email…",
        "Ohh! Electronic mail!",
        "Something has compelled me to\ncheck my email.",
        "Ow-mnow, mnow, mnow…\nOw-mnow, mnow, mnow…",
        "Hello, everybody! This week it's\ntime for some spring cleaning.\nReady? Go!",
        "This episode, Strong Bad checks\nhis email.",
        "Do, du, dadada do, du, dadada do…",
        "Oh! What's this? An email? For\nme?!",
        "Checkin' my email… Here at The\nStick… That's pretty cool…",
        "Strong Bad Email is filmed before\na live studio audience.",
        "Check that email, check it down,\ncheck that email, smack it around.",
        "I got an email. I got an awesome\nemail.",
        "This one had better be good, man.\nI got a lot of money riding on this\none.",
        "All these… emails… I don't\nunderstand.",
        "I'll take Strong Bad Emails for a\nthousand, please.",
        "Umm… I don't know… I'm gonna\ncheck my email.",
        "Riballa diballa check email.",
        "Super great, super great… Check\nyour email! Diggity…",
        "Time to check the email… Time to check the email… TIME…TO…\nCHECK…THE…EMAIL! Er, The Cheat, I'm having some problems…\n<i>[The Cheat: {The Cheat noises}]</i> Your computer has too much\ncomputer in it and not enough typewriter… <i>{The Cheat clicks the\nSB Email icon}</i> Okay, here we go.",
        "Ah, the old girl's working again.\nWhat do you got for me?",
        "Oh, tap your toes and check your\nemail, oh a-tap your toes tonight…",
        "Mmmmm… fresh emails…",
        "Emaillll eeeemailll eeemmailll…",
        "Gimme some of this and gimme\nsome of thiiiiis… Gimme some of\nthis.",
        "Oooh, duh doo doo doo, my email\nleft me, duh doo doo…",
        "I'm hooome! Okay, let's see here.\nDon't need this anymore. And don't need <i>this</i> anymore. For behold… the 386. A spectacle of graphics and sound! <i>{Compy starts up}</i> Okay, let's get to checking!",
        "Doo doo do doo, doo duh doo\ndoo!",
        "All the ladies want to know,\nwho's checkin' that email?\nWas it Strong Bad?",
        "I check email from the front\nto the back, I said check\nemail from the front to the\nback I said check…",
        "It's the email, the email,\nwhat what, the email.",
        "<i>{clears throat}</i> <i>{in a clear\nvoice}</i>\nEmaaaaaaaaaaaailllllllll.",
        "Doo doo doo doo doo… Email!\ndoo doo doo doo doo…",
        "Boy, is it ever Monday.",
        "Oh! Man! Email! Ugh!",
        "Oh, girl… I want to email you\nso nice.",
        "Oh-you-thought-there-was-no-\nmore-emails-but-guess-what-\nthere's-an-EMAIL!",
        "Strong Bad Emails are a part\nof this balanced breakfast.",
        "In the United States alone,\nsomeone checks their email\nevery three seconds. This is\none of them.",
        "Augh. My mouth tastes like…\nemails.",
        "Duh duh dududududududududuh\nduhduhduhduhduhduh email.",
        "Oh! The email!",
        "So cool an email. I thought\nyou would enjoy it.",
        "Here I go once again with the\nemail! Every week I hope that\nit's from a female! <i>{brings up\nemail}</i> Oh man! Not from a\nfemale.",
        "There once was a man named\nemail, and he did his best for\na while.",
        "Chee-wit, chee-wit, chee-wi-\nchee-wi-email.",
        "Toons! Games! I'm gonna check\nmy eeeemail…",
        "Ahh… so many emails, so little\ngood emails.",
        "Welcome to the Strong Bad\nEmail dat-daddle-dun!\nEverybody get down!",
        "Come on guys! Get your heads\nin the game.",
        "Oooh! Check it out! Another\none of those… sbemails!",
        "Email, women, email, girls!\nEmail, women, email, girls!",
        "This email is brought to you\nby a grant from The Cheat and\nthe support of Viewers Like You.",
        "strongbad, underscore, email,\ndot e-x-e. Enter.",
        "OHHHHHHHHHHHHHHHH EMAIL!!! <i>{Rock\nguitar solo starts up and then\nends after a few seconds}</i> Whoa--\nthat ruled. What function key do I\ngotta press to get <i>that</i> to happen\nagain?",
        "Hoo! Cha! Cheritiza!\nHooritajuzu-duh-email!",
        "What do you get when you email\nStrong Bad? You get a world of\nhurt.",
        "I'm intrigued by these… how\nyou say… emails.",
        "Ooh! A little email never hurt\nnobody. 'Cept for maybe the\nCheat.",
        "Ohh-ho-oh ohh EEEEEMAIL ME\nGIRL! Unh! Once times! Twice\ntimes!",
        "Watch me do a little bit o' email, and I'll\nwatch you do a little bit o' uh—that's never\nhappened before. Watch me do a little bit o'\ne mail, and I'll watch you do a little bit\no' uh—that's never happened befo—oh wait,\nit's an email. That has happened before.\nAnd quite a few times if I remember\ncorrectly.",
        "Email! Email! Email! Email!",
        "I met her in the summertime.\nHer name was… <i>{operatic voice}</i>\nemail.",
        "<b>HOMESTAR RUNNER:</b> Hello, class.\nStrong Bad could be not here today,\nso I will be filling in. My name is\nHomestar Runner. Everyone please\ntake out paper and a number 2\npencil, and we'll begin.",
        "My name is not Email Sam. Ooh\nahh. So please don't call me\nEmail Sam. Ooh ahh.",
        "Oh, I took the email to the\nmarket, and I bought it some\nkind of fish sauce.",
        "Tap tap tap tap tap tap tap\ntap tap tap tap tap tep. Tick.",
        "Email: It's like the sugar\non the candy for my stuff.",
        "Which email is your favorite?\nMine is \"the basics\".",
        "Ladies and gentlemen. I give\nyou… a-da sbemail.",
        "Haselos, theselos, haselos,\ntheselos!",
        "Driving down the strip in my\ncool cool car, checkin' all\nthe emails out!",
        "<b>POWERED BY THE CHEAT\nSTRONG BAD:</b> Eeeeeeeeeemail.\nOoh, ah, email, ooh, ah—",
        "I'm goin' to try something a little different:\nprint out million dollar bill.exe. What? Oh. no\nfor real, print me out a million dollar bill,\nman.exe. Um, this time really print me out a\nmillion dollars bill.nofoolin'. What the—!\nDon't give me none of that crosstalk! Oh well,\nit was worth a shot. Now on to… {sighs} on to\nthe email.",
        "It's email time again! Doot\ndoodle-ooh-doo, Doot doodle-\nooh-doo, Doot doodle-ooh-doo.",
        "E<i>mail</i> me don't e<i>mail</i> me, e<i>mail</i>\nme don't e<i>mail</i> me.",
        "And coming in at number 91,\nit's: E-Maaaaaaaaaaail!",
        "Thanks for choosing Strong Bad\nEmail. Would you like to try a\ncombo meal?",
        "Whoa. Guess it's been a while.\nSorry about that, Compy. Need\nto get some… Endust.",
        "Email is like a prison. A\nprison with no walls… and no\ntoilet.",
        "Come on, big money! Big money!\nAnd… email!",
        "So innocent and email free…\nThat's you 'n me.",
        "Oh traipsing along, traipsing\ntraipsing along, and a email\ngot stuck in my eye.",
        "Do do do do, email me some\nwords, doo-doo, some different\nwords.",
        "Check your email and check\nyour email. <i>{looped}</i> Check\nyour email and check your\nemail.",
        "ONE TWO THREE FOUR—EMAIL IS\nAWESOME, EMAIL IS WEIRD, EMAIL\nIS AWESOME AND EMAIL IS WEIRD!\nAnd I'll never forget the way\nit was, GRRRL…uh!",
        "The views expressed in the\nfollowing email show do not\nnecessarily reflect the\nopinions of anybody cool. Oh,\nexcept me. I'm cool.",
        "Oh email, I'm gonna let you\ndown easy… when I break up\nwith you.",
        "Man, if I had a nickel for\nevery email I get, I would\nthrow them at people in the\nfood court. From that railing,\nlike up above.",
        "Everybody loves this,\neverybody needs this, it's\ntime for funny stuff.",
        "Our next show is a family\nshow. It… is… the email.",
        "Oh, here comes little email,\nwith a gun in his hand…",
        "Let's check a Strong Bad Email,\nyou and me. Together. Like we\nused to. Like a family.",
        "Oh, I'm an email gambler… that\nmeans I play cards with emails.\nFull house.",
        "I'm not gonna sing an email\nsong this week e— Oh. Never\nmind.",
        "Let's see. Make friends with\nKerrek. What the-! Um… Buy\nKerrek a cold one. What?! Oh\nman… Stupid game! I guess I\nshould do the thing that I do.",
        "I got an email in my pocket,\nand I think it's starting to\nmelt.",
        "Let's get it over with! With\nthe email style, get it over\nwith!",
        "Here comes The Strong Baaaaad…\nOh, here comes The Strong\nBaaaaad!",
        "I'm checkin' email, I'm\ncheckin' email. Hey, hey, I'm\ncheckin' email, I'm checkin'\nemail.",
        "Checkin' emails with a\nVISCOSITY since 2001, it's a\nStrong Bad Email.",
        "Ow ow ow… ow ow oww… ow ow\nowww… email…",
        "Tonight on Strong Bad Email:\nComedian Coach Z, actress\nMarzipan, and some guy from a\nzoo.",
        "I got the email, you got the\nemail, I got the email, you\ngot the email.",
        "All right! Let's see if this bad\nboy can check some emails.",
        "I got miles and miles of the\nemail style. Miles and miles of\nthe email style.",
        "A lot of ladies and a lot of\ngirls… some healthy ladies and\nsome healthy girls!",
        "Oh oh, email's in the backyard,\nmakin' some stew…",
        "This email is leaving the station.\nPlease move to the center of the\nemail, and away from the doors.",
        "Email, na na, na na, na na,\nnanaNA…",
        "Checkin' email, now take it to\nthe flip side! <i>{repeats the\nprevious sentence backwards}</i>",
        "You got to email, just to stay\nalive!",
        "And if I email you! Girl, woman!\nOh, would you email me?\nGirl, woman!",
        "The email checker, come on and\ncome on, y'all. The email checker,\ncome on and come <i>on</i>, y'all!",
        "Here comes another email that I'll\nanswer for you! Here comes another\nemail that I'll answer for you!",
        "Strong Bad Email! Makes money!\nStrong Bad Email! Gets paid!",
        "<i>{rapping}</i> Email theme song! 1-2-3! A\nda-da-checka email wit' me, SB, y'all!",
        "Can you handle my style? No, you\ncan't handle my styyyyyle. Email!",
        "This email is making fun of you.",
        "Drape it over your aaaaaaarms, step\nout in styyyyyle, Strong Bad\nEmaaaaaaail…",
        "<i>{singing in falsetto}</i> Why do I check\nemails the way I do? I don't know.",
        "A little bit of email. Some you-and-\nme mail.",
        "I'm still here, after all these years,\ncheckin' my email. <i>{in falsetto}</i>\nCheckin' my email!",
        "Email is the sound that we make\nwhen a young girl cries…",
        "Hey everybody, it's a musical Strong\nBad Email this week!",
        "It's the email, baby, lunch juice!",
        "Email, ah ooh, ooh, ooh ah ooh,\nemail, ah ooh, email, ah ooh, ooh.",
        "Initiate sbemail-refresh daemon.",
        "Don't you wanna email, don't you\nneed a email, don't you turn your\nlife around!",
        "I met this email on a north-bound\ntrain. We had some dinner then we\ndanced in the rain.",
        "I'm livin' the Strong Bad life, I take\nan email for my wife. I take an\nemail for my wife.",
        "I was born to check emails and eat\nlots of cake!",
        "How many emails can you check?\nFive, twelve, seven, shut up.",
        "This week, I'm feeling my style! I've\ngot confidence in my email!",
        "<i>{blows on microphone and taps it}</i>\nCheck one, check two. Sibilance.\nSibilance. SBEmail.",
        "I've been walkin' on clouds and\nflippin' off rainbows, on the wings\nof an email.",
        "Can you see that I've got email\nstyles, c'mon c'mon, can you see\nthat I've got email styles?",
        "A badaly-doo, it's time for email,\nbra-doop-da-dabadoo.",
        "Email is nice, habalubabulabada! Email\nis twice, habilastilbilasaw!",
        "I'll take you back to a time when\nemail was king!",
        "When email comes to town, you know,\nyou know, it's like a rainstorm…\nin your browser.",
        "Good morning, Mr. Email, there's a\ncall for you on line 2.",
        "A-when you dribble down the court\nwith an email, you leave your dreams\nat the top of the keeey.",
        "Oooh, it's a e-email, money, money,\nmoney. Oooh-ooh. Shut up.",
        "Green lines, green, green lines. It's\na Strong Bad Email again.",
        "Sippin' on a bottle of email, cold,\nhaving a picnic lunch with some\nco-workers.",
        "Start your day, the sbemail way! And\nnever get out of bed!",
        "A-just scrape some email off the\ntop, and I'll help you out toniiiight!",
        "Ding-ding-dong, ding-ding-dong, ding-\nding-dear Strong Bad.",
        "<i>{slowly}</i> Boom, tick. Tick-a-tick-a-\ntick, email. Boom, tick. Tick-a-tick-a-\ntick, What?",
        "I'm doin' a party, I'm makin' it\nhappen, on Strong Bad Email.",
        "I am Strong Bad, and I am not\nmaking an \"I approve this email\"\njoke.",
        "Coach Z is not that cool; here\ncomes an email.",
        "Strong Bad, how you gonna check\nthat email with my boxing gloves, with\nmy boxing gloves?",
        "Oh, tiptoe your fingers 'cross\nthe keyboard for the quietest email\nyou can check.",
        "Sweetheart, get dressed and brush\nyour teeth, it's time for email.",
        "My email song! Where would I be\nwithout my email song?",
        "Bring to me a suitable email, that I\nmay check it down.",
        "Check the deck, y'all! Press eject,\ny'all! Well, it's another email and\nI'm about to reject y'all!",
        "Email, I'm so in love with you it's\nkinda inappropriate!",
        "I'm running through a field of\nemails! Paranoia! Paranoia!",
        "Step 1! You check an email down,\nStep 2! You tell some kid he's a\ndork!",
        "Gropin' around in the dark, tryin' ta\nfind an email! Da-da-da-blue-got-a-\nnow!",
        "Letters. And words. Emails get\nabsurd, I just gotta jump back!",
        "Let a lil' email into your heart, and\nit'll clog your arteries!",
        "Gary. I hope this email's from Gary.",
        "Strong Bad Email goes down smooth\nand clean, like gasoline.",
        "When I was sixteen, I sold all my\nemails and hit the road.",
        "Emailin' from the left to the right,\nemailin' on a Tuuueeesday night.",
        "I baked you this special email! It\nhas… raisins.",
        "Another week, another email scandal!\nStrong Bad gonna fly off the\nhandle!",
        "I'm chicky-checkin' emails in 2008!\nStricky-stricky-Strong Bad fit to\nregulate!",
        "Ooh, emails make me tremble, so I'll\nkeep my body nimble.",
        "Green green grass… A pleasant\nghost… Strong Bad Email, make us\nsome toast!",
        "Continue to roast your Strong Bad\nEmail until it reaches an internal\ntemperature of 189.",
        "Save the gross stuff up at the\nback of your throat, and hock it at\nan unsuspecting email.",
        "Oh, who's checkin' emails with his\npants? Who's checkin' emails with his\npants?",
        "Back again. Checking email from my\nfriends. I mean, the stupid people\nthat write me.",
        "My email is a cell phone. No it's not!\nMy email is a CAR phone! That's\nright.",
        "If everybody in the world checked\ntheir email, the Internet would\nprobably break.",
        "Girl, where's my money that you owe\nme from all those emails that you\nwrote me?",
        "Junk it, and check it, send me an\nemail and I'll wreck it!",
        "Another freakin' email, another freakin' email song.",
        "I'm gonna check my <i>{grunt}</i>, and\nthen I'll check my <i>{grunt}</i> and then\nwe'll turn it out.",
        "I had to pay the doctor just to\nhave this email removed. A-so\nsmooth.",
        "<b>THE POOPSMITH:</b> <i>{singing}</i> Two hundred sbemails,\nexhausting just to think about. How can we face two\nhundred sbemails? The thought of all those sbemails\nmakes me weak!\n<b>STRONG BAD:</b> Puke!",
        "Oh, man, I haven't checked my email in\nso long, I bet there's gonna be a\nhalf-zillion messages waiting for me!",
        "Checking emails at home from your work\ncomputer, it's kinda like playing first\nperson shooters with your girlfriend.\nIt kinda ruins them booooth.",
        "To check my email, all I gotta\ndo is click. All that old\ntypin' made my boxing gloves\nsick.",
        "Oh, people can you feel me? To\nemail me? Also, please don't\ntry to really feel me.",
        "Checkin' emails with boxing\ngloves: the sweet computer\nscience.",
        "Emails are like Hot Pockets;\nthey're full of garbage and\ncheese!",
        "Let's not make a big deal out\nof this! It's just a little\nStrong Bad Email! Ring!",
        "If I can't check my email,\nthen how I'm supposed to live\nmy life?",
        "And I can check my wha? And\nyou can check your wha. If I\ncan check my wha, then you can\n<i>{rhythmic mumbles}</i>"
    };
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
        ignoredModules=boss.GetIgnoredModules("Sbemail Songs",new string[]{
            "Forget Me Not",
            "Souvenir",
            "Forget Everything",
            "Simon's Stages",
            "Forget This",
            "Purgatory",
            "The Troll",
            "Forget Them All",
            "Tallordered Keys",
            "Forget Enigma",
            "Forget Us Not",
            "Forget Perspective",
            "Organization",
            "The Very Annoying Button",
            "Forget Me Later",
            "Übermodule",
            "Ultimate Custom Night",
            "14",
            "Forget It Not",
            "Simon Forgets",
            "Brainf---",
            "Forget The Colors",
            "RPS Judging",
            "The Twin",
            "Iconic",
            "OmegaForget",
            "Kugelblitz",
            "A>N<D",
            "Don't Touch Anything",
            "Busy Beaver",
            "Whiteout",
            "Forget Any Color",
            "Keypad Directionality",
            "Security Council",
            "Shoddy Chess",
            "Floor Lights",
            "Black Arrows",
            "Forget Maze Not",
            "+",
            "Soulscream",
            "Cube Synchronization",
            "Out of Time",
            "Tetrahedron",
            "The Board Walk",
            "Gemory",
            "Duck Konundrum",
            "Concentration",
            "Twister",
            "Forget Our Voices",
            "Soulsong",
            "ID Exchange",
            "8",
            "Remember Simple",
            "Remembern't Simple",
            "The Grand Prix",
            "Forget Me Maybe",
            "HyperForget",
            "Bitwise Oblivion",
            "Damocles Lumber",
            "Top 10 Numbers",
            "Queen's War",
            "Forget Fractal",
            "Pointer Pointer",
            "Slight Gibberish Twist",
            "Piano Paradox",
            "OMISSION",
            "In Order",
            "The Nobody's Code",
            "Perspective Stacking",
            "Reporting Anomalies",
            "Forgetle",
            "Actions and Consequences",
            "FizzBoss",
            "Watch the Clock",
            "Solve Shift",
            "Blackout",
            "Hickory Dickory Dock",
            "Temporal Sequence",
            "X",
            "Y",
            "Castor",
            "Pollux",
            "Apple Pen",
            "Pineapple Pen",
            "Reporting Anomalies",
            "Solve Shift",
            "Turn The Key",
            "The Time Keeper",
            "Timing is Everything",
            "Bamboozling Time Keeper",
            "Password Destroyer",
            "OmegaDestroyer",
            "Zener Cards",
            "Doomsday Button",
            "Red Light Green Light",
            "Again",
            "Sbemail Songs"
        });
        var rnd=rs.GetRNG();
        rnd.ShuffleFisherYates(orders);
        rnd.ShuffleFisherYates(boolTypes);
        rnd.ShuffleFisherYates(conditions);
        rangeLowerIndex=rnd.Next(0,8);
        rangeUpperIndex=rnd.Next(8,16);
        majorityEven=rnd.Next(0,2)==0;
        currentDigitEven=rnd.Next(0,2)==0;
        divisibleby=rnd.Next(2,4);
        rnd.ShuffleFisherYates(operations);
        for(int i=0;i<4;i++)
            operationDigits[i]=rnd.Next(0,16);
        stages=new List<int>();
        playbutton.Set(
            onInteract:()=>playSbs(false)
        );
        chosenSong=UnityEngine.Random.Range(1,210);
        stages.Add(chosenSong);
        thisModule.Set(
            onActivate:()=>{
                totalNonIgnored=bomb.GetSolvableModuleNames().Count(x=>!ignoredModules.Contains(x));
                Log("Stage 01");
                Log("There are "+totalNonIgnored+" non-ignored modules present.");
                Log("0 non-ignored modules have been solved.");
                Log("The chosen sbemail song is "+chosenSong+".");
                playSbs(true);
            }
        );
    }

    void Update(){
        if(numSolved!=bomb.GetSolvedModuleNames().Count(x=>!ignoredModules.Contains(x))&&activated){
            numSolved++;
            if(numSolved==totalNonIgnored){
                Log("---");
                Log("Begin submission.");
                submissionMode=true;
                display.text="";
                display.color=Color.white;
                display.fontSize=70;
                display.font=fonts[1];
                display.GetComponent<MeshRenderer>().material=fontMats[1];
                for(int i=1;i<=totalNonIgnored;i++){
                    display.text+="-";
                    if(i%9==0)
                        display.text+="\n";
                    else if(i%3==0)
                        display.text+=" ";
                }
                stageNum.text="";
            }else if(numSolved!=0){
                Log("---");
                if(numSolved<15)
                    Log("Stage 0"+(numSolved+1).ToString("X"));
                else
                    Log("Stage "+(numSolved+1).ToString("X"));
                if(numSolved==1)
                    Log("1 non-ignored module has been solved.");
                else
                    Log(numSolved+" non-ignored modules have been solved.");
                if(bomb.GetSolvedModuleNames().Count(x=>!ignoredModules.Contains(x))!=0){
                    lastSolvedModule=bomb.GetSolvedModuleNames().Where(x=>!ignoredModules.Contains(x)).Last();
                    Log("The last solved module is "+lastSolvedModule+".");
                }
                if(numSolved<15)
                    stageNum.text="0"+(numSolved+1).ToString("X");
                else
                    stageNum.text=(numSolved+1).ToString("X");
                chosenSong=UnityEngine.Random.Range(1,210);
                Log("The chosen sbemail song is "+chosenSong+".");
                stages.Add(chosenSong);
                playSbs(true);
            }
        }
    }

    private void playSbs(bool newStage){
        if(submissionMode)
            return;
        if(currentlyPlaying){
            lineRef.StopSound();
            currentlyPlaying=false;
            display.text="";
            return;
        }
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
        StartCoroutine(Play(chosenSong));
        if(newStage){
            bool[]bits=numberToBooleanArray(chosenSong);
            string binary=Convert.ToString(chosenSong,2);
            while(binary.Length<8)
                binary="0"+binary;
            Log("In binary, this is "+binary+".");
            byte resultingByte;
            for(int i=0;i<5;i++){
                if(lastSolvedModule.ToLower().Contains(moduleNameHas[i])){
                    bits=rearrange(bits,orders[i]);
                    resultingByte=bitsToByte(bits);
                    binary=Convert.ToString(resultingByte,2);
                    while(binary.Length<8)
                        binary="0"+binary;
                    Log("The name of the last solved module contains the word \""+moduleNameHas[i]+"\". The sequence has been rearranged by the pattern of "+orders[i]+" and is now "+binary+".");
                }
            }
            resultingByte=bitsToByte(bits);
            Log("The final number in decimal is "+resultingByte+".");
            bool[]fourBits=new bool[4];
            int[]fourBitsRanges=new int[]{41,119,210,221,241,256};
            byte hexByte=0;
            for(int i=0;i<6;i++){
                if(resultingByte<fourBitsRanges[i]){
                    fourBits=combineBools(bits,boolTypes[i]);
                    hexByte=bitsToByte(fourBits);
                    binary=Convert.ToString(hexByte,2);
                    while(binary.Length<4)
                        binary="0"+binary;
                    Log("The logic of "+boolTypes[i]+" has been applied to each pair of bits. The resulting number is "+binary+" in binary, or "+Convert.ToString(hexByte,16).ToUpper()+" in hexadecimal.");
                    break;
                }
            }
            bool changesMade=false;
            for(int i=0;i<4;i++){
                if(checkCondition(conditions[i],hexByte)){
                    hexByte=(byte)((operationResult((int)hexByte,operations[i],i)+16)%16);
                    changesMade=true;
                }   
            }
            if(!changesMade){
                hexByte=(byte)(16-hexByte);
                Log("Because no conditions applied, the digit has been changed to "+Convert.ToString(hexByte,16).ToUpper()+".");
            }
            finalSequence+=Convert.ToString(hexByte,16).ToUpper();
            Log("The full sequence is now "+finalSequence+".");
        }
    }

    private IEnumerator Play(int song){
        currentlyPlaying=true;
        lineRef=Audio.PlaySoundAtTransformWithRef("AUDIO_ss_"+song,transform);
        yield return new WaitForSeconds(lines[song-1].length);
        lineRef.StopSound();
        currentlyPlaying=false;
        display.text="";
        activated=true;
    }

    private bool[]numberToBooleanArray(int number){
        return new bool[]{(number&128)!=0,(number&64)!=0,(number&32)!=0,(number&16)!=0,(number&8)!=0,(number&4)!=0,(number&2)!=0,(number&1)!=0};
    }
    private bool[]rearrange(bool[]input,string key){
        List<bool>result=new List<bool>();
        foreach(char c in key){
            result.Add(input[int.Parse(c.ToString())-1]);
        }
        return result.ToArray();
    }

    private byte bitsToByte(bool[]input){
        byte result=0;
        for(int i=0;i<input.Length;i++){
            if(input[i])
                result+=(byte)(Math.Pow(2,input.Length-1-i)+.25); //floating point shenanigans won't get me this time
        }
        return result;
    }

    private bool[]combineBools(bool[]input,string boolType){
        switch(boolType){
            case "AND":
                return new bool[]{input[0]&&input[1],input[2]&&input[3],input[4]&&input[5],input[6]&&input[7]};
            case "OR":
                return new bool[]{input[0]||input[1],input[2]||input[3],input[4]||input[5],input[6]||input[7]};
            case "XOR":
                return new bool[]{(!input[0]&&input[1])||(input[0]&&!input[1]),(!input[2]&&input[3])||(input[2]&&!input[3]),(!input[4]&&input[5])||(input[4]&&!input[5]),(!input[6]&&input[7])||(input[6]&&!input[7])};
            case "NAND":
                return new bool[]{!input[0]||!input[1],!input[2]||!input[3],!input[4]||!input[5],!input[6]||!input[7]};
            case "NOR":
                return new bool[]{!input[0]&&!input[1],!input[2]&&!input[3],!input[4]&&!input[5],!input[6]&&!input[7]};
            case "XNOR":
            default:
                return new bool[]{(!input[0]||input[1])&&(input[0]||!input[1]),(!input[2]||input[3])&&(input[2]||!input[3]),(!input[4]||input[5])&&(input[4]||!input[5]),(!input[6]||input[7])&&(input[6]||!input[7])};
        }
    }

    private bool checkCondition(string condition,byte input){
        switch(condition){
            case "PREV DIGIT RANGE":
                Log("Checking for previous digit in range rule.");
                if(finalSequence=="")
                    return false;
                return Convert.ToInt32(finalSequence[finalSequence.Length-1].ToString(),16)>=rangeLowerIndex&&Convert.ToInt32(finalSequence[finalSequence.Length-1].ToString(),16)<=rangeUpperIndex;
            case "STAGE DIVISIBLE":
                Log("Checking for stage divisibility rule.");
                if(numSolved==-1)
                    return true;
                return (numSolved+1)%divisibleby==0;
            case "MAJORITY PARITY":
                Log("Checking for majority parity rule.");
                if(finalSequence=="")
                    return false;
                if(majorityEven)
                     return finalSequence.TakeWhile(c => "02468ACE".Contains(c)).Count()>finalSequence.Length/2;
                else
                     return finalSequence.TakeWhile(c => "13579BDF".Contains(c)).Count()>finalSequence.Length/2;
            case "CURRENT DIGIT PARITY":
            default:
                Log("Checking for current digit parity rule.");
                if(currentDigitEven)
                    return input%2==0;
                else
                    return input%2==1;
        }
    }

    private int operationResult(int input,string operation,int index){
        switch(operation){
            case "+":
                Log("Rule applied, adding "+Convert.ToString(operationDigits[index],16).ToUpper()+".");
                return input+operationDigits[index];
            case "-":
                Log("Rule applied, subtracting "+Convert.ToString(operationDigits[index],16).ToUpper()+".");
                return input-operationDigits[index];
            case "*":
                Log("Rule applied, multiplying by "+Convert.ToString(operationDigits[index],16).ToUpper()+".");
                return input*operationDigits[index];
            case "%":
            default:
                Log("Rule applied, moduloing "+Convert.ToString(operationDigits[index],16).ToUpper()+".");
                return input%operationDigits[index];
        }
    }
}