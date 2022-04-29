using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICursor : MonoBehaviour,SpineAnimatable
{
    public string animationState = "";
    public SkeletonAnimation anim;
    public AnimationReferenceAsset focus;
    public AnimationReferenceAsset unfocus;
    private IAnimationController animationController;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        Cursor.visible = false;

        animationController = new IAnimationController();
    }

    private void Awake()
    {
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        this.checkFocus();
        this.updatePosition();
    }

    private void updatePosition()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = Camera.main.transform.position.z;
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse.z = 0;
        this.transform.position = mouse;
    }

    private void checkFocus()
    {
        if(this.player == null)
        {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            if (player.Length == 0) return;

            this.player = player[0].GetComponent<Player>();
            if (this.player == null) return;
        }


        if(this.player.getInputState(InputState.AIM) == 1 && !this.animationState.Equals("Focus"))
        {
            this.animationController.setAnimation(this, this.focus,false);
        }
        else if(this.player.getInputState(InputState.AIM) != 1 && !this.animationState.Equals("Unfocus"))
        {
            this.animationController.setAnimation(this, this.unfocus, false);
        }
    }

    string SpineAnimatable.getCurrentAnimationName()
    {
        return this.animationState;
    }

    Spine.AnimationState SpineAnimatable.GetAnimatonState()
    {
        return this.anim.state;
    }

    public void setCurrentANimationName(string name)
    {
        this.animationState = name;
    }
}
