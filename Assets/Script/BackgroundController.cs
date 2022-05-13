using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField]
    private float FirstSpeed;
    [SerializeField]
    private float SecondSpeed;
    [SerializeField]
    private float ThirdSpeed;
    [SerializeField]
    private float FouthSpeed;

    [SerializeField]
    private Texture[] subLayers;

    private int height;
    private int width;
    private float pixPerUnit;
    private Material mt;

    private void Start()
    {
        this.height = (int)this.gameObject.GetComponent<SpriteRenderer>().sprite.rect.height;
        this.width = (int)this.gameObject.GetComponent<SpriteRenderer>().sprite.rect.width;
        this.pixPerUnit = this.gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        mt = this.gameObject.GetComponent<SpriteRenderer>().material;
        mt.SetVector(
            "_Speed",
            new Vector4(this.FirstSpeed, this.SecondSpeed, this.ThirdSpeed, this.FouthSpeed)
        );
        mt.SetTexture("_SecondTex", subLayers[0]);
        mt.SetTexture("_ThirdTex", subLayers[1]);
        mt.SetTexture("_FourthTex", subLayers[2]);

        //MyCamera cmr = (MyCamera)MyCamera.instance;
        //Vector2 camPos = cmr.transform.localPosition;
        //Vector2 selfPos = this.gameObject.transform.localPosition;

        //float bottomLineGap = cmr.getBottomLine() - (selfPos.y - this.height / pixPerUnit / 2f);
        //this.gameObject.transform.localPosition = new Vector2(camPos.x, selfPos.y + bottomLineGap);
    }

    private void Update()
    {
        float newX = transform.TransformPoint(this.gameObject.transform.position).x;
        mt.SetFloat("_Bias", newX/200);
    }
}
