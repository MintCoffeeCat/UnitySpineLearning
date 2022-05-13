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
        //ͼƬ�Ŀ�ߡ���λ����������Ϣ����ʱû����
        this.height = (int)this.gameObject.GetComponent<SpriteRenderer>().sprite.rect.height;
        this.width = (int)this.gameObject.GetComponent<SpriteRenderer>().sprite.rect.width;
        this.pixPerUnit = this.gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

        //��ȡ���ʣ�����Shader����
        mt = this.gameObject.GetComponent<SpriteRenderer>().material;

        //���ø���ͼ��
        //����this.gameobject�е�SpriteRender���õ�SpriteͼƬ��Ϊ��ֹ����ײ㱳��
        //subLayer[0]������һ��ı�����subLayer[1]��������һ��ı������Դ�����
        mt.SetTexture("_SecondTex", subLayers[0]);
        mt.SetTexture("_ThirdTex", subLayers[1]);
        mt.SetTexture("_FourthTex", subLayers[2]);

        //���ø���ͼ���ƫ���ٶ�
        //FirstSpeed��ӦsubLayer[0]���ƶ���SecondSpeed��ӦsubLayer[1]���ƶ�
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
