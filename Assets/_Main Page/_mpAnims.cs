using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

///<summary>This script is assigned to each of the 6 effect buttons, and handles the animation of background objects.</summary>
public class _mpAnims:MonoBehaviour{
    ///<value>The <c>GameObject</c> in the background to control.</value>
    public GameObject b;
    public _mainpagescript MP;
    public _mpTextures TXTRs;
    public _mpHsBg HSBG;
    KMAudio Audio;
    private Material bmat;
    ///<value>The letter assigned to each of the six menu buttons (T,G,C,D,S,E).</value>
    internal char Letter;
    internal bool speaking = false;
    ///<value>The number assigned to each of the six menu buttons (0-5).</value>
    public int num;
    internal bool disappear = false;
    internal bool running;
    ///<value>The number of the assigned animation to each button.</value>
    internal int animNum;
    ///<value>The assigned animation to each button.</value>
    internal IEnumerator assignedAnim;
    ///<value>The assigned Homestar lip-sync and voice line to each button.</value>
    internal IEnumerator assignedSay;
    ///<value>The array of animations for background objects.</value>
    private static IEnumerator[]animations;
    ///<value>The array of animations for Homestar speaking.</value>
    private static IEnumerator[]sayAnims;
    ///<value>The array of configurations for positioning the animations correctly.</value>
    private static IEnumerator[]setups;
    private Texture[]FRs;

    void Awake(){
        Audio = MP.GetComponent<KMAudio>();
        setups=new IEnumerator[]{
            setup(0      , .01f  ,.16f  ,.064f ,true                             ),
            setup(0      , .0455f,.16f  ,.02f  ,false                            ),
            setup(0      , .05f  ,.16f  ,.015f ,false, .5f ,1    , .25f          ),
            setup( .0085f,-.0025f,.04f  ,.06f  ,false,1    , .75f,0    ,.25f     ),
            setup(0      , .004f ,.0625f,.0725f,true ,1    , .9f ,0    ,.1f      ),
            setup( .0075f, .032f ,.11f  ,.1f   ,true                             ),
            setup( .009f ,-.016f , 1/18f,1/30f ,false,1    , .56f,0    ,.43f     ),
            setup(0      ,0      ,.16f  ,.05f  ,false, 2/3f,1    , .15f          ),
            setup( .02f  , .005f ,.12f  ,.06f  ,true , .88f                      ),
            setup(-.03f  , .0275f,.1f   ,.08f  ,false,  yCrop:.7f                ),
            setup(-.02f  , .025f ,.05f  ,.0425f,true ,                   dis:true),
            setup( .008f ,1/1500f,.25f  ,.25f  ,false                            ),
            setup( .0085f, .016f ,.175f ,.0225f,false                            ),
            setup( .004f , .02f  ,.15f  ,.1f   ,true ,                   dis:true),
            setup(0      , .015f ,.1575f,.09f  ,false, .75f, .75f                ),
            setup(0      , .025f ,.1575f,.1f   ,false,                   dis:true),
            setup(0      , .0235f,.16f  ,.075f ,false, .85f                      ),
            setup(0      , .0235f,.1575f,.035f ,false, .9f                       ),
            setup(0      , .0235f,.1575f,.1f   ,false                            ),
            setup(-.0233f, .01f  ,.2063f,.075f ,false, .85f,xOffset:.1f          ),
            setup( .01f  , .04f  ,.1f   ,.1f   ,false                            ),
            setup( .003f , .015f ,.264f ,.154f ,false                            ),
            setup( .0042f, .0224f,.1505f,.11f  ,false, .6f , 2/3f, .25f,.3f      ),
            setup(0      , .024f , 1/15f,.045f ,true,                    dis:true),
            setup(0      , .024f ,.16f  ,.113f ,false                            ),
            setup(0      , .024f ,.16f  ,.113f ,false,2/3f ,xOffset:.2f          )
        };
        b.SetActive(false);
        animations = new IEnumerator[]{
            A(TXTRs.FR1,true,1/12f),
            A(TXTRs.FR2,false,.1f),
            A(TXTRs.FR3,true),
            A(TXTRs.FR4,false),
            A(TXTRs.FR5,true),
            A(TXTRs.FR6,true),
            A(TXTRs.FR7,false),
            A(TXTRs.FR8,false),
            A(TXTRs.FR9,false,.075f),
            A(TXTRs.FR10,false,.075f),
  stillImages(TXTRs.FR11),
            A(TXTRs.FR12,true,.085f),
            A(TXTRs.FR13,true),
            A(TXTRs.FR14,false),
            A(TXTRs.FR15,false),
        aLoop(TXTRs.FR16,true),
            A(TXTRs.FR17,true),
            A(TXTRs.FR18,true),
            A(TXTRs.FR19,false),
            A(TXTRs.FR20,true),
            A(TXTRs.FR21,true),
            A(TXTRs.FR22,true,.075f),
            A(TXTRs.FR23,true,.075f),
  stillImages(TXTRs.FR24),
            A(TXTRs.FR25,true),
            A(TXTRs.FR26,true)
        };
        StartCoroutine(san());
    }

    internal void startup(int anim){
        animNum = anim;
        assignedAnim = animations[animNum];
        StartCoroutine(setups[animNum]);
    }

    ///<summary>A method that handles animation in the background.</summary>
    ///<param name="sound">Whether the animation requires sound. The names of the sound files are standardized for convenience.</param>
    ///<param name="FRs">The frames used in the animation.</param>
    ///<param name="wait">How long to wait between each frame in seconds. A higher value leads to a slower frame rate.</param>
    ///<returns>An animation that is played when hovering over a side button.</returns>
    private IEnumerator A(Texture[] FRs,bool sound,float wait=.05f){
        assignedAnim = A(FRs,sound,wait);
        running = true;
        b.SetActive(true);
        if (sound) Audio.PlaySoundAtTransform("fx" + (animNum + 1).ToString(), transform);
        for (int i = 0; i < FRs.Count(); i++){
            bmat.mainTexture = FRs[i];
            yield return new WaitForSeconds(wait);
        }
        b.SetActive(false);
        running = false;
    }

    ///<summary>A method that handles looping animation in the background.</summary>
    ///<param name="sound">Whether the animation requires sound. The names of the sound files are standardized for convenience.</param>
    ///<param name="FRs">The frames used in the animation.</param>
    ///<param name="wait">How long to wait between each frame in seconds. A higher value leads to a slower frame rate.</param>
    ///<returns>An animation that is played when hovering over a side button.</returns>
    public IEnumerator aLoop(Texture[] FRs,bool sound,float wait=.05f){
        assignedAnim = aLoop(FRs,sound,wait);
        running = true;
        b.SetActive(true);
        if (sound) Audio.PlaySoundAtTransform("fx" + (animNum + 1).ToString(), transform);
        while (running){
            for (int i = 0; i < FRs.Count(); i++){
                if (!running) break;
                bmat.mainTexture = FRs[i];
                yield return new WaitForSeconds(wait);
            }
        }
    }

    int img = -1;

    public IEnumerator stillImages(Texture[] imgs){
        assignedAnim = stillImages(imgs);
        running = true;
        img++;
        if (img >= imgs.Length) img = 0;
        bmat.mainTexture = imgs[img];
        b.SetActive(true);
        yield return null;
    }

    ///<summary>Sets up the background objects to be the right size and be in the right position.</summary>
    ///<param name="xPos">X position of the object.</param>
    ///<param name="zPos">Z position of the object.</param>
    ///<param name="xScale">X scale of the object.</param>
    ///<param name="zScale">Z scale of the object.</param>
    ///<param name="semi">Whether the object will use a shader that allows for semitransparency.</param>
    ///<param name="xCrop">X value for cropping the texture.</param>
    ///<param name="yCrop">Y value for cropping the texture.</param>
    ///<param name="xOffset">X value for off-setting the material.</param>
    ///<param name="yOffset">Y value for off-setting the material.</param>
    ///<param name="dis">If <c>true</c>, the object will disappear once the button stops being highlighted. If <c>false</c>, the object will play out its full animation even when the button is no longer highlighted.</param>
    ///<returns>The properly-configured positioning of the chosen animation.</returns>
    private IEnumerator setup(float xPos, float zPos, float xScale, float zScale, bool semi,
                            float xCrop=1, float yCrop=1, float xOffset=0, float yOffset=0,bool dis=false){
        disappear = dis;
        b.transform.localScale = new Vector3(xScale, .0001f, zScale);
        b.transform.localPosition = new Vector3(xPos, 0.0105f, zPos);
        bmat = semi ? new Material(HSBG.semi) : new Material(MP.transparentshader);
        bmat.mainTextureScale = new Vector2(xCrop, yCrop);
        bmat.mainTextureOffset = new Vector2(xOffset, yOffset);
        b.GetComponent<Renderer>().material = bmat;
        yield return null;
    }

    public IEnumerator toonsSay(){
        assignedSay = toonsSay();
        HSBG.hsHead.material = HSBG.animMats[3];
        yield return new WaitForSeconds(1);
        HSBG.hsHead.material = HSBG.animMats[0];
        yield return new WaitForSeconds(.3f);
    }

    public IEnumerator gamesSay(){
        assignedSay = gamesSay();
        HSBG.hsHead.material = HSBG.animMats[2];
        HSBG.bl.SetActive(true);
        yield return new WaitForSeconds(.5f);
        HSBG.hsHead.material = HSBG.animMats[0];
        yield return new WaitForSeconds(.1f);
        HSBG.bl.SetActive(false);
        yield return new WaitForSeconds(.3f);
    }

    public IEnumerator charsSay(){
        assignedSay = charsSay();
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material = HSBG.animMats[3];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material = HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material = HSBG.animMats[3];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material = HSBG.animMats[0];
        yield return new WaitForSeconds(.3f);
    }

    public IEnumerator downloadsSay(){
        assignedSay = downloadsSay();
        StartCoroutine(Nblink());
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(.4f);
        HSBG.hsHead.material = HSBG.animMats[0];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(.8f);
        HSBG.hsHead.material = HSBG.animMats[3];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material = HSBG.animMats[0];
        yield return new WaitForSeconds(.3f);
    }

    public IEnumerator gamesSayPBTC(){
        assignedSay = gamesSayPBTC();
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(.6f);
        HSBG.hsHead.material = HSBG.animMats[0];
    }

    public IEnumerator charsSayPBTC(){
        assignedSay = charsSayPBTC();
        for(int i=0;i<8;i++){
            HSBG.hsHead.material = HSBG.animMats[2];
            yield return new WaitForSeconds(.1f);
            HSBG.hsHead.material = HSBG.animMats[0];
            yield return new WaitForSeconds(.1f);
        }
    }

    public IEnumerator downloadsSayPBTC(){
        assignedSay = downloadsSayPBTC();
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material = HSBG.animMats[0];
        yield return new WaitForSeconds(.1f);
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(.6f);
        HSBG.hsHead.material = HSBG.animMats[3];
        yield return new WaitForSeconds(.1f);
        HSBG.hsHead.material = HSBG.animMats[0];
    }

    public IEnumerator toonsSayAnime(){
        assignedSay = toonsSayAnime();
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(.25f);
        HSBG.hsHead.material = HSBG.animMats[3];
        yield return new WaitForSeconds(1/12f);
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(.25f);
        HSBG.hsHead.material = HSBG.animMats[0];
    }

    public IEnumerator gamesSayAnime(){
        assignedSay = gamesSayAnime();
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(1/12f);
        HSBG.hsHead.material = HSBG.animMats[3];
        yield return new WaitForSeconds(1/12f);
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(1/12f);
        HSBG.hsHead.material = HSBG.animMats[0];
    }

    public IEnumerator charsSayAnime(){
        assignedSay = charsSayAnime();
        HSBG.hsHead.material = HSBG.animMats[3];
        yield return new WaitForSeconds(1/12f);
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(1/12f);
        HSBG.hsHead.material = HSBG.animMats[3];
        yield return new WaitForSeconds(1/12f);
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(1/6f);
        HSBG.hsHead.material = HSBG.animMats[0];
        yield return new WaitForSeconds(.25f);
        HSBG.hsHead.material = HSBG.animMats[3];
        yield return new WaitForSeconds(1/6f);
        HSBG.hsHead.material = HSBG.animMats[0];
    }

    public IEnumerator simpleMouthOpen(float openTime){
        assignedSay = simpleMouthOpen(openTime);
        if(HSBG.HSnumber==9)HSBG.oldtimeysay.GetComponentInChildren<TextMesh>().text = "\"" + MP.buttonNames[num].ToUpper() + "\"";
        if(HSBG.HSnumber==9)HSBG.oldtimeysay.SetActive(true);
        HSBG.hsHead.material = HSBG.animMats[2];
        yield return new WaitForSeconds(openTime);
        HSBG.hsHead.material = HSBG.animMats[0];
        if(HSBG.HSnumber==9)HSBG.oldtimeysay.SetActive(false);
    }


    public IEnumerator toonsSayStory(){
        assignedSay=toonsSayStory();
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.25f);
        HSBG.hsHead.material=HSBG.animMats[0];
    }

    public IEnumerator gamesSayStory(){
        assignedSay=gamesSayStory();
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.25f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.1f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.05f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material=HSBG.animMats[0];
    }


    public IEnumerator charsSayStory(){
        assignedSay=charsSayStory();
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.25f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.1f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.05f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.05f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.05f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[0];
    }

    public IEnumerator downloadsSayStory(){
        assignedSay=downloadsSayStory();
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.1f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.75f);
        HSBG.hsHead.material=HSBG.animMats[0];
    }

    public IEnumerator storeSayStory(){
        assignedSay=storeSayStory();
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.05f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.05f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.05f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[0];
    }

    public IEnumerator emailSayStory(){
        assignedSay=emailSayStory();
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.05f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.15f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.05f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.05f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.5f);
        HSBG.hsHead.material=HSBG.animMats[0];
    }

    public IEnumerator charsSayForxe(){
        assignedSay=charsSayForxe();
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.2f);
        HSBG.hsHead.material=HSBG.animMats[0];
    }

    public IEnumerator downloadsSayForxe(){
        assignedSay=downloadsSayForxe();
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.35f);
        HSBG.hsHead.material=HSBG.animMats[0];
        yield return new WaitForSeconds(.35f);
        HSBG.hsHead.material=HSBG.animMats[2];
        yield return new WaitForSeconds(.35f);
        HSBG.hsHead.material=HSBG.animMats[0];
    }

    public IEnumerator Nblink(){
        HSBG.bl.SetActive(true);
        yield return new WaitForSeconds(1);
        HSBG.bl.SetActive(false);
        yield return new WaitForSeconds(.15f);
        HSBG.bl.SetActive(true);
        yield return new WaitForSeconds(.1f);
        HSBG.bl.SetActive(false);
    }

    private IEnumerator san(){
        yield return new WaitUntil(() => HSBG.done);
        sayingAnims(HSBG.HSnumber);
    }
    
    private IEnumerator nothing(){yield return null;}
    internal void sayingAnims(int hs){
        switch (hs){
            case 9:
                sayAnims = new IEnumerator[] { simpleMouthOpen(1), simpleMouthOpen(.5f), simpleMouthOpen(1), simpleMouthOpen(1), simpleMouthOpen(1), simpleMouthOpen(1) };
                break;
            case 11:
                sayAnims=new IEnumerator[]{nothing(),nothing(),nothing(),nothing(),nothing(),nothing()};
                break;
            case 12:
                sayAnims = new IEnumerator[] { simpleMouthOpen(1), simpleMouthOpen(1), simpleMouthOpen(1), simpleMouthOpen(.5f), simpleMouthOpen(1), simpleMouthOpen(1) };
                break;
            case 14:
                sayAnims=new IEnumerator[]{toonsSay(), gamesSayPBTC(), charsSayPBTC(), downloadsSayPBTC(), toonsSay(), downloadsSayPBTC()};
                break;
            case 16:
                sayAnims=new IEnumerator[]{toonsSayAnime(),gamesSayAnime(),charsSayAnime(),charsSayAnime(),toonsSayAnime(),toonsSayAnime()};
                break;
            case 19:
                sayAnims=new IEnumerator[]{toonsSayStory(),gamesSayStory(),charsSayStory(),downloadsSayStory(),storeSayStory(),emailSayStory()};
                break;
            case 25:
                sayAnims=new IEnumerator[]{simpleMouthOpen(1),simpleMouthOpen(1),charsSayForxe(),downloadsSayForxe(),simpleMouthOpen(1),downloadsSayForxe()};
                break;
            default:
                //store and e-mail use the same mouth movements as toons and downloads respectively
                sayAnims = new IEnumerator[] { toonsSay(), gamesSay(), charsSay(), downloadsSay(), toonsSay(), downloadsSay() };
                break;
        } assignedSay = sayAnims[num];
    }
}