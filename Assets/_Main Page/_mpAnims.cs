//this script is assigned to each of the 6 effect buttons, and handles the animation (referred to as J in other scripts)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class _mpAnims : MonoBehaviour {
    
    public GameObject b;
    public _mainpagescript v;
    public _mpTextures t;
    public _mpHsBg h;
    KMAudio Audio;
    internal Material bmat;
    internal char Letter;
    internal bool speaking = false;
    public int num;
    internal bool disappear = false;
    internal bool running;
    internal int animNum;
    internal IEnumerator assignedAnim;
    internal IEnumerator assignedSay;
    internal List<IEnumerator> animations;
    internal IEnumerator[] sayAnims;
    internal List<IEnumerator> setups;
    internal Texture[] FRs;

    void Awake(){
        Audio = v.GetComponent<KMAudio>();
        b.SetActive(false);
        setups = new List<IEnumerator>{
            setup(0,.01f,.16f,0.06391645f,1,1,0,0,false,true),
            setup(0,0.0455f,.16f,0.02070288f,1,1,0,0,false,false),
            setup(0,.05f,.16f,.015f,.5f,1,.25f,0,false,false),
            setup(.0085f,-.0025f,.04f,.06f,1,.75f,0,.25f,false,false),
            setup(0,.004f,.0625f,.0725f,1,.9f,0,.1f,false,true),
            setup(.0075f,.032f,.10946f,.1f,1,1,0,0,false,true),
            setup(.009f,-.016f,1/18f,1/30f,1,.56f,0,.43f,false,false),
            setup(0,0,.16f,.05f,2/3f,1,.15f,0,false,false),
            setup(.02f,.005f,.12f,.06f,.88f,1,0,0,false,true),
            setup(-.03f,.0275f,.1f,.08f,1,.7f,0,0,false,false),
            setup(-.02f,.025f,.05f,.0425f,1,1,0,0,true,true)
        };

        animations = new List<IEnumerator>{
            A(1/12f,true,t.FR1),
            A(.1f,false,t.FR2),
            A(.05f,true,t.FR3),
            A(.05f,false,t.FR4),
            A(.05f,true,t.FR5),
            A(.05f,true,t.FR6),
            A(.05f,false,t.FR7),
            A(.05f,false,t.FR8),
            A(.075f,false,t.FR9),
            A(.075f,false,t.FR10),
            stillImages(t.FR11)
        };
        animNum = Random.Range(0, animations.Count);
        while (v.takenAnims.Contains(animNum)) animNum = Random.Range(0, animations.Count);
        v.takenAnims.Add(animNum);
        assignedAnim = animations[animNum];
        StartCoroutine(setups[animNum]);
        StartCoroutine(san());
    }

    public IEnumerator A(float wait, bool sound, Texture[] FRs){
        assignedAnim = A(wait, sound, FRs);
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

    public IEnumerator aLoop(float wait, bool sound, Texture[] FRs){
        assignedAnim = aLoop(wait, sound, FRs);
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

    public IEnumerator setup(float xPos, float zPos, float xScale, float zScale,
                             float xCrop, float yCrop, float xOffset, float yOffset, bool dis, bool semi){
    //starting x & z positions and scales,
    //cropping dimensions, offset for material, if the effect disappears when the button is unhighlighted
        disappear = dis;
        b.transform.localScale = new Vector3(xScale, .0001f, zScale);
        b.transform.localPosition = new Vector3(xPos, 0.0105f, zPos);
        bmat = semi ? new Material(h.semi) : new Material(v.transparentshader);
        bmat.mainTextureScale = new Vector2(xCrop, yCrop);
        bmat.mainTextureOffset = new Vector2(xOffset, yOffset);
        b.GetComponent<Renderer>().material = bmat;
        yield return null;
    }

    public IEnumerator toonsSay(){
        assignedSay = toonsSay();
        h.hsHead.material = h.animMats[3];
        yield return new WaitForSeconds(1);
        h.hsHead.material = h.animMats[0];
        yield return new WaitForSeconds(.3f);
    }

    public IEnumerator gamesSay(){
        assignedSay = gamesSay();
        h.hsHead.material = h.animMats[2];
        h.bl.SetActive(true);
        yield return new WaitForSeconds(.5f);
        h.hsHead.material = h.animMats[0];
        yield return new WaitForSeconds(.1f);
        h.bl.SetActive(false);
        yield return new WaitForSeconds(.3f);
    }

    public IEnumerator charsSay(){
        assignedSay = charsSay();
        h.hsHead.material = h.animMats[2];
        yield return new WaitForSeconds(.15f);
        h.hsHead.material = h.animMats[3];
        yield return new WaitForSeconds(.15f);
        h.hsHead.material = h.animMats[2];
        yield return new WaitForSeconds(.15f);
        h.hsHead.material = h.animMats[0];
        yield return new WaitForSeconds(.15f);
        h.hsHead.material = h.animMats[3];
        yield return new WaitForSeconds(.15f);
        h.hsHead.material = h.animMats[0];
        yield return new WaitForSeconds(.3f);
    }

    public IEnumerator downloadsSay(){
        assignedSay = downloadsSay();
        StartCoroutine(Nblink());
        h.hsHead.material = h.animMats[2];
        yield return new WaitForSeconds(.4f);
        h.hsHead.material = h.animMats[0];
        yield return new WaitForSeconds(.2f);
        h.hsHead.material = h.animMats[2];
        yield return new WaitForSeconds(.8f);
        h.hsHead.material = h.animMats[3];
        yield return new WaitForSeconds(.2f);
        h.hsHead.material = h.animMats[0];
        yield return new WaitForSeconds(.3f);
    }

    public IEnumerator oldTimeySay(float openTime){
        assignedSay = oldTimeySay(openTime);
        h.oldtimeysay.GetComponentInChildren<TextMesh>().text = "\"" + v.buttonNames[num].ToUpper() + "\"";
        h.oldtimeysay.SetActive(true);
        h.hsHead.material = h.animMats[2];
        yield return new WaitForSeconds(openTime);
        h.hsHead.material = h.animMats[0];
        h.oldtimeysay.SetActive(false);
    }

    public IEnumerator Nblink(){
        h.bl.SetActive(true);
        yield return new WaitForSeconds(1);
        h.bl.SetActive(false);
        yield return new WaitForSeconds(.15f);
        h.bl.SetActive(true);
        yield return new WaitForSeconds(.1f);
        h.bl.SetActive(false);
    }

    internal IEnumerator san(){
        yield return new WaitUntil(() => h.done);
        sayingAnims(h.HSnumber);
    }
    
    private IEnumerator nothing(){yield return null;}
    internal void sayingAnims(int hs){
        switch (hs){
            case 9:
                sayAnims = new IEnumerator[] { oldTimeySay(1), oldTimeySay(.5f), oldTimeySay(1), oldTimeySay(1), oldTimeySay(1), oldTimeySay(1) };
                break;
            case 11:
                sayAnims=new IEnumerator[]{nothing(),nothing(),nothing(),nothing(),nothing(),nothing()};
                break;
            default:
                //store and e-mail use the same mouth movements as toons and downloads respectively
                sayAnims = new IEnumerator[] { toonsSay(), gamesSay(), charsSay(), downloadsSay(), toonsSay(), downloadsSay() };
                break;
        } assignedSay = sayAnims[num];
    }
}