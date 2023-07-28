//this script handles homestar and the background (referred to as h in other scripts)
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
    internal bool done = false;
    internal char lines;
    void chooseShader(Texture[] t, bool semitransparency){
        //the shader that allows for semitransparency has errors where objects will appear over each other when looked at at certain angles
        //but this one makes semitransparent parts of a texture look weird
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
                break;
            case 13:
                chooseShader(TXTRs.aShower,true);
                lines = 'n';
                break;
            case 14:
                chooseShader(TXTRs.aPBTC,false);
                lines = 'c';
                break;
            case 15:
                chooseShader(TXTRs.aWinter,false);
                lines = 'n';
                break;
            case 16:
                chooseShader(TXTRs.aAnime,false);
                lines = 'j'; //j for japanese cartoon
                break;
            case 19:
                chooseShader(TXTRs.aStory,false);
                lines = 'b';
                break;
            case 20:
                chooseShader(TXTRs.aVector,false);
                lines = 'v';
                break;
            case 21:
                chooseShader(TXTRs.aNormal,false);
                lines = 'r'; //virus, homestar still has normal head
                break;
            case 22:
                chooseShader(TXTRs.aBack,false);
                lines = 'n';
                break;
            case 23:
                chooseShader(TXTRs.aBlur,false);
                lines = 'n';
                break;
            case 24:
                chooseShader(TXTRs.aPuppet,false);
                lines = 'p';
                break;
            case 25:
                chooseShader(TXTRs.aXforxe,false);
                lines = 'x';
                break;
            case 26:
                chooseShader(TXTRs.aTrog,false);
                lines = 'n';
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
                /*hsBody.transform.localPosition = new Vector3(,.0108f,);
                hsHead.transform.localPosition = new Vector3(,.0106f,);
                bl.transform.localPosition = new Vector3(,1,);
                hsBody.transform.localScale = new Vector3(,.0001f,);
                hsHead.transform.localScale = new Vector3(,.0001f,);
                bl.transform.localScale = new Vector3(,1,);*/
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
                break;
            case 21:
                menumat.mainTexture = TXTRs.menus[3]; //virus
                break;
            case 22:
                menumat.mainTexture = TXTRs.menus[4]; //rearview
                if (HS == 7) hsBody.transform.localPosition = new Vector3(-1 * hsBody.transform.localPosition.x, hsBody.transform.localPosition.y, 0.0407f); //upside down
                else hsBody.transform.localPosition = new Vector3(-1 * hsBody.transform.localPosition.x, hsBody.transform.localPosition.y, hsBody.transform.localPosition.z);
                break;
            case 23:
                menumat.mainTexture = TXTRs.menus[5]; //shiny
                break;
            case 24:
                menumat.mainTexture = TXTRs.menus[5]; //shiny
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
        StartCoroutine(blink());
        MP.blinkstop = false;
    }
    private void Awake(){
        bl.SetActive(false);
        oldtimeysay.SetActive(false);
        HSnumber = Random.Range(0, TXTRs.backgrounds.Length);
        BGnumber = Random.Range(0, TXTRs.homestars.Length);
        bgmat = new Material(MP.transparentshader);
        hsmat = new Material(MP.transparentshader);
        menumat = new Material(MP.transparentshader);
        bluemat = new Material(MP.transparentshader);
        redmat = new Material(MP.transparentshader);
        BgHsSetup(HSnumber, BGnumber);
        done = true;
    }
}