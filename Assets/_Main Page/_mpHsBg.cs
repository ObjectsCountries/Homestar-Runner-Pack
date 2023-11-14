using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

///<summary>This script handles Homestar and the background.</summary>
public class _mpHsBg : MonoBehaviour{

    internal List<Material> animMats;
    public Renderer bgcube;
    public Renderer hsBody;
    public Renderer hsHead;
    public Renderer pagemenu;
    internal int HSnumber;
    internal int BGnumber;
    internal Material bgmat;
    internal Material hsmat;
    internal Material menumat;
    internal Material bluemat;
    internal Material redmat;
    public _mainpagescript MP;
    _mpAnims ANIMS;
    public Shader semi;
    public GameObject oldtimeysay;
    public GameObject bl;
    public _mpTextures TXTRs;
    internal char lines;
    internal bool done=false;

    ///<summary>Chooses which shader (semitransparent or not) to use for Homestar.</summary>
    ///<param name="t">The textures to assign to Homestar.</param>
    ///<param name="semitransparency">Whether the instance of Homestar requires semitransparency.</param>
    ///<remarks>The shader that allows for semitransparency has errors where objects will appear over each other when looked at at certain angles, but the other shader makes semitransparent parts of a texture look weird.</remarks>
    void chooseShader(Texture[] t, bool semitransparency){
        for (int i = 0; i < t.Length; i++){
            animMats.Add(semitransparency?new Material(semi):new Material(MP.transparentshader));
            animMats[i].mainTexture = t[i];
        }
    }
    public IEnumerator blink(){
        while (!MP.blinkstop){
            bl.SetActive(true);
            yield return new WaitForSeconds(.1f);
            bl.SetActive(false);
            yield return new WaitForSeconds(3);
        }
    }

    ///<summary>Sets up the positioning of Homestar and the background, as well as voice lines. This is used both in initial setup and in changing the menu.</summary>
    ///<param name="HS">The menu number that Homestar comes from.</param>
    ///<param name="BG">The menu number that the background comes from.</param>
    public void BgHsSetup(int HS, int BG){
        animMats = new List<Material>() { };
        switch (HS){
            case 5:
                chooseShader(TXTRs.aHalo,true);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(.04401f, .0108f, -.017f);
                hsHead.transform.localPosition = new Vector3(.0473f, .0106f, .0265f);
                bl.transform.localPosition = new Vector3(.187f, 1, .06f);
                hsBody.transform.localScale = new Vector3(.0525f, .0001f, .031f);
                hsHead.transform.localScale = new Vector3(.06863344f, .0001f, .06774098f);
                bl.transform.localScale = new Vector3(0.1865793f, 1, 0.1502714f);
                break;
            case 6:
                chooseShader(TXTRs.aGrave,false);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(.0286f, .0108f, -.0224f);
                hsHead.transform.localPosition = new Vector3(.025f, .0106f,-.0025f);
                bl.transform.localPosition = new Vector3(.069f,1, -.169f);
                hsBody.transform.localScale = new Vector3(.03623563f, .0001f, .01409163f);
                hsHead.transform.localScale = new Vector3(0.04328765f, .0001f, 0.03246574f);
                bl.transform.localScale = new Vector3(0.2178927f, 1, 0.2374313f);
                break;
            case 7:
                chooseShader(TXTRs.aUpsD,false);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(.0535f,.0108f,.05f);
                hsHead.transform.localPosition = new Vector3(.0557f,.0106f,.01f);
                bl.transform.localPosition = new Vector3(.121f,1,.039f);
                hsBody.transform.localScale = new Vector3(.043f,.0001f,2/45f);
                hsHead.transform.localScale = new Vector3(.045f,.0001f,.042f);
                bl.transform.localScale = new Vector3(.1958605f, 1,.1934432f);
                break;
            case 8:
                chooseShader(TXTRs.aDraw,false);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(.05f,.0108f,-.0135f);
                hsHead.transform.localPosition = new Vector3(.047f,.0106f,.0245f);
                bl.transform.localPosition = new Vector3(0,1,0);
                hsBody.transform.localScale = new Vector3(.05f,.0001f,.04f);
                hsHead.transform.localScale = new Vector3(.05f,.0001f,.0475f);
                bl.transform.localScale = new Vector3(.01f,1,.01f);
                break;
            case 9:
                chooseShader(TXTRs.aOld,false);
                hsBody.transform.localPosition = new Vector3(.043f,.0108f,-.015f);
                hsHead.transform.localPosition = new Vector3(.04f,.0106f,.024f);
                bl.transform.localPosition = new Vector3(0,1,0);
                hsBody.transform.localScale = new Vector3(.05f,.0001f,.035f);
                hsHead.transform.localScale = new Vector3(.054f,.0001f,.054f);
                bl.transform.localScale = new Vector3(.01f,1,.01f);
                break;
            case 11:
                chooseShader(TXTRs.aJuly,false);
                hsBody.transform.localPosition = new Vector3(.043f,.0108f,-.015f);
                hsHead.transform.localPosition = new Vector3(.0462f,.0106f,.0275f);
                bl.transform.localPosition = new Vector3(0,1,0);
                hsBody.transform.localScale = new Vector3(.05f,.0001f,.035f);
                hsHead.transform.localScale = new Vector3(.06f,.0001f,.06f);
                bl.transform.localScale = new Vector3(.01f,1,.01f);
                break;
            case 12:
                chooseShader(TXTRs.aAtari,false);
                lines = 'a';
                hsBody.transform.localPosition = new Vector3(.0435f,.0108f,-.01f);
                hsHead.transform.localPosition = new Vector3(.0435f,.0106f,.0345f);
                bl.transform.localPosition = new Vector3(0,1,0);
                hsBody.transform.localScale = new Vector3(.045f,.0001f,.045f);
                hsHead.transform.localScale = new Vector3(.045f,.0001f,.045f);
                bl.transform.localScale = new Vector3(.01f,1,.01f);
                break;
            case 13:
                chooseShader(TXTRs.aShower,false);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(.045f,.0108f,-.014f);
                hsHead.transform.localPosition = new Vector3(.0412f,.0106f,.0258f);
                bl.transform.localPosition = new Vector3(.122f,1,-.024f);
                hsBody.transform.localScale = new Vector3(0.06322949f,.0001f, 0.03793771f);
                hsHead.transform.localScale = new Vector3(1/18f,.0001f,1/18f);
                bl.transform.localScale = new Vector3(0.2162404f, 1, 0.1833194f);
                break;
            case 14:
                chooseShader(TXTRs.aPBTC,false);
                lines = 'c';
                hsBody.transform.localPosition = new Vector3(2/45f,.0108f,-.015f);
                hsHead.transform.localPosition = new Vector3(.0475f,.0106f,.025f);
                bl.transform.localPosition = new Vector3(.122f,1,-.024f);
                hsBody.transform.localScale = new Vector3(0.05f,.0001f, 0.035f);
                hsHead.transform.localScale = new Vector3(.0525f,.0001f,.0525f);
                bl.transform.localScale = new Vector3(0.01f, 1, 0.01f);
                break;
            case 15:
                chooseShader(TXTRs.aWinter,false);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(2/45f,.0108f,-.015f);
                hsHead.transform.localPosition = new Vector3(.0235f,.0106f,.02f);
                bl.transform.localPosition = new Vector3(-.115f,1,-.065f);
                hsBody.transform.localScale = new Vector3(.05f,.0001f,.035f);
                hsHead.transform.localScale = new Vector3(.145f,.0001f,.06f);
                bl.transform.localScale = new Vector3(.0725f,1,.175f);
                break;
            case 16:
                chooseShader(TXTRs.aAnime,false);
                lines = 'j'; //j for japanese cartoon
                hsBody.transform.localPosition = new Vector3(2/45f,.0106f,-.01f);
                hsHead.transform.localPosition = new Vector3(7/150f,.0108f,.025f);
                bl.transform.localPosition = new Vector3(.01f,1,.01f);
                hsBody.transform.localScale = new Vector3(.05f,.0001f,.045f);
                hsHead.transform.localScale = new Vector3(.045f,.0001f,.05f);
                bl.transform.localScale = new Vector3(.01f,1,.01f);
                break;
            case 18:
                chooseShader(TXTRs.aNormal,false);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(.0353f,.0108f,-.01f);
                hsHead.transform.localPosition = new Vector3(.0412f,.0106f,.0242f);
                bl.transform.localPosition = new Vector3(.122f,1,-.024f);
                hsBody.transform.localScale = new Vector3(0.06574735f,.0001f,0.04646398f);
                hsHead.transform.localScale = new Vector3(1/18f,.0001f,1/18f);
                bl.transform.localScale = new Vector3(0.2162404f, 1, 0.1833194f);
                break;
            case 19:
                chooseShader(TXTRs.aStory,false);
                lines = 'b';
                hsBody.transform.localPosition = new Vector3(.04f,.0108f,-.0125f);
                hsHead.transform.localPosition = new Vector3(.04f,.0106f,.0254f);
                bl.transform.localPosition = new Vector3(.122f,1,-.024f);
                hsBody.transform.localScale = new Vector3(.05f,.0001f,.04f);
                hsHead.transform.localScale = new Vector3(1/18f,.0001f,1/18f);
                bl.transform.localScale = new Vector3(0.2162404f, 1, 0.1833194f);
                break;
            case 20:
                chooseShader(TXTRs.aVector,false);
                lines = 'v';
                hsBody.transform.localPosition = new Vector3(.04f,.0108f,-.0125f);
                hsHead.transform.localPosition = new Vector3(.0441f,.0106f,.0254f);
                bl.transform.localPosition = new Vector3(.122f,1,-.024f);
                hsBody.transform.localScale = new Vector3(.05f,.0001f,.04f);
                hsHead.transform.localScale = new Vector3(.05f,.0001f,.05f);
                bl.transform.localScale = new Vector3(0.2162404f, 1, 0.1833194f);
                break;
            case 22:
                chooseShader(TXTRs.aBack,false);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(.04f,.0108f,-.0167f);
                hsHead.transform.localPosition = new Vector3(.04f,.0106f,.0153f);
                bl.transform.localPosition = new Vector3(.122f,1,-.024f);
                hsBody.transform.localScale = new Vector3(0.045f,.0001f, 0.032f);
                hsHead.transform.localScale = new Vector3(.045f,.0001f,.0425f);
                bl.transform.localScale = new Vector3(0.2162404f, 1, 0.1833194f);
                break;
            case 24:
                chooseShader(TXTRs.aPuppet,false);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(.04f,.0108f,-.0125f);
                hsHead.transform.localPosition = new Vector3(.0441f,.0106f,.01f);
                bl.transform.localPosition = new Vector3(.122f,1,-.024f);
                hsBody.transform.localScale = new Vector3(.05f,.0001f,.04f);
                hsHead.transform.localScale = new Vector3(.05f,.0001f,.085f);
                bl.transform.localScale = new Vector3(0.2162404f, 1, 0.1833194f);
                break;
            case 25:
                chooseShader(TXTRs.aXforxe,false);
                lines = 'x';
                hsBody.transform.localPosition = new Vector3(.04f,.0108f,-.0125f);
                hsHead.transform.localPosition = new Vector3(.04f,.0106f,.02f);
                bl.transform.localPosition = new Vector3(.122f,1,-.024f);
                hsBody.transform.localScale = new Vector3(.05f,.0001f,.04f);
                hsHead.transform.localScale = new Vector3(.075f,.0001f,1/15f);
                bl.transform.localScale = new Vector3(0.2162404f, 1, 0.1833194f);
                break;
            case 26:
                chooseShader(TXTRs.aTrog,false);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(.04f,.0108f,-.0125f);
                hsHead.transform.localPosition = new Vector3(.04f,.0106f,.02f);
                bl.transform.localPosition = new Vector3(.122f,1,-.165f);
                hsBody.transform.localScale = new Vector3(.045f,.0001f,.04f);
                hsHead.transform.localScale = new Vector3(.045f,.0001f,.045f);
                bl.transform.localScale = new Vector3(0.2162404f, 1, 0.1833194f);
                break;
            default:
                chooseShader(TXTRs.aNormal,false);
                lines = 'n';
                hsBody.transform.localPosition = new Vector3(.045f,.0108f,-.014f);
                hsHead.transform.localPosition = new Vector3(.0412f,.0106f,.0258f);
                bl.transform.localPosition = new Vector3(.122f,1,-.024f);
                hsBody.transform.localScale = new Vector3(0.06322949f,.0001f, 0.03793771f);
                hsHead.transform.localScale = new Vector3(1/18f,.0001f,1/18f);
                bl.transform.localScale = new Vector3(0.2162404f, 1, 0.1833194f);
                break;
        }
        switch (BG){
            case 9:
                menumat.mainTexture = TXTRs.menus[1]; //old timey
                bluemat.mainTexture = TXTRs.blueButtons[1];
                redmat.mainTexture = TXTRs.redButtons[1];
                break;
            case 14:
                menumat.mainTexture = TXTRs.menus[2]; //pbtc
                bluemat.mainTexture = TXTRs.blueButtons[2];
                redmat.mainTexture = TXTRs.redButtons[2];
                break;
            case 22:
                menumat.mainTexture = TXTRs.menus[3]; //rearview
                bluemat.mainTexture = TXTRs.blueButtons[0];
                redmat.mainTexture = TXTRs.redButtons[0];
                hsHead.transform.localScale = new Vector3(-1 * hsHead.transform.localScale.x, hsHead.transform.localScale.y, hsHead.transform.localScale.z);
                hsBody.transform.localScale = new Vector3(-1 * hsBody.transform.localScale.x, hsBody.transform.localScale.y, hsBody.transform.localScale.z);
                bl.transform.localScale = new Vector3(-1 * bl.transform.localScale.x, bl.transform.localScale.y, bl.transform.localScale.z);
                hsHead.transform.localPosition = new Vector3(-1 * hsHead.transform.localPosition.x, hsHead.transform.localPosition.y, hsHead.transform.localPosition.z);
                hsBody.transform.localPosition = new Vector3(-1 * hsBody.transform.localPosition.x, hsBody.transform.localPosition.y, hsBody.transform.localPosition.z);
                bl.transform.localPosition = new Vector3(-1 * bl.transform.localPosition.x, bl.transform.localPosition.y, bl.transform.localPosition.z);
                break;
            case 23:
            case 24:
                menumat.mainTexture = TXTRs.menus[4]; //shiny
                bluemat.mainTexture = TXTRs.blueButtons[3];
                redmat.mainTexture = TXTRs.redButtons[3];
                break;
            default:
                menumat.mainTexture = TXTRs.menus[0]; //normal
                bluemat.mainTexture = TXTRs.blueButtons[0];
                redmat.mainTexture = TXTRs.redButtons[0];
                break;
        }
        bgmat.mainTexture = TXTRs.backgrounds[BG];
        hsmat.mainTexture = TXTRs.homestars[HS];
        hsHead.material = animMats[0];
        bgcube.material = bgmat;
        hsBody.material = hsmat;
        pagemenu.material = menumat;
        bl.GetComponent<Renderer>().material = animMats[1];
        if(BG==22){
            float[]buttonXpositions=new float[]{.059f,.0572f,.0537f,.0445f,.03f,.0038f};
            for(int i=0;i<6;i++){
                MP.menuButtons[i].transform.localPosition=new Vector3(buttonXpositions[i], MP.menuButtons[i].transform.localPosition.y, MP.menuButtons[i].transform.localPosition.z);
            }
        }else{
            float[]buttonXpositions=new float[]{-.059f,-.0608f,-.0591f,-.0535f,-.0459f,-.0355f};
            for(int i=0;i<6;i++){
                MP.menuButtons[i].transform.localPosition=new Vector3(buttonXpositions[i], MP.menuButtons[i].transform.localPosition.y, MP.menuButtons[i].transform.localPosition.z);
            }
        }
        StartCoroutine(blink());
        MP.blinkstop = false;
    }
    private void Awake(){
        bl.SetActive(false);
        oldtimeysay.SetActive(false);
        HSnumber = Random.Range(0, TXTRs.homestars.Length);
        BGnumber = Random.Range(0, TXTRs.homestars.Length);
        bgmat = new Material(MP.transparentshader);
        hsmat = new Material(MP.transparentshader);
        menumat = new Material(MP.transparentshader);
        bluemat = new Material(MP.transparentshader);
        redmat = new Material(MP.transparentshader);
        BgHsSetup(HSnumber, BGnumber);
        done=true;
    }
}