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
        //图片的宽高、单位像素量等信息，暂时没用上
        this.height = (int)this.gameObject.GetComponent<SpriteRenderer>().sprite.rect.height;
        this.width = (int)this.gameObject.GetComponent<SpriteRenderer>().sprite.rect.width;
        this.pixPerUnit = this.gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

        //获取材质，设置Shader参数
        mt = this.gameObject.GetComponent<SpriteRenderer>().material;

        //设置各个图层
        //其中this.gameobject中的SpriteRender设置的Sprite图片作为静止的最底层背景
        //subLayer[0]是向上一层的背景，subLayer[1]是再向上一层的背景，以此类推
        mt.SetTexture("_SecondTex", subLayers[0]);
        mt.SetTexture("_ThirdTex", subLayers[1]);
        mt.SetTexture("_FourthTex", subLayers[2]);

        //设置各个图层的偏移速度
        //FirstSpeed对应subLayer[0]的移动，SecondSpeed对应subLayer[1]的移动
        mt.SetVector(
            "_Speed",
            new Vector4(this.FirstSpeed, this.SecondSpeed, this.ThirdSpeed, this.FouthSpeed)
        );

    }

    private void Update()
    {
        float newX = transform.TransformPoint(this.gameObject.transform.position).x;
        mt.SetFloat("_Bias", newX/200);
    }
}
