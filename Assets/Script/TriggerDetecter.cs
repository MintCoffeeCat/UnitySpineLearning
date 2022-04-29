using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate bool DetectCondition();
public delegate void TasksWhenDetected();

[System.Serializable]
public class TriggerDetecter
{

    [ReadOnly][SerializeReference] public DetectStateSet state;

    public LayerMask ground;
    public Collider2D footDetect;
    public Collider2D headDetect;

    protected DetectCondition onGroundCondition;
    protected TasksWhenDetected tasksOnGround;

    protected DetectCondition hitCeilCondition;
    protected TasksWhenDetected tasksHitCeil;

    public TriggerDetecter()
    {
        this.state = new DetectStateSet();
    }
    public TriggerDetecter(Collider2D footDetect, Collider2D headDetect, DetectCondition onGroundCondition, TasksWhenDetected tasksOnGround, DetectCondition hitCeilCondition, TasksWhenDetected tasksHitCeil)
    {
        this.footDetect = footDetect;
        this.headDetect = headDetect;
        this.onGroundCondition = onGroundCondition;
        this.tasksOnGround = tasksOnGround;
        this.hitCeilCondition = hitCeilCondition;
        this.tasksHitCeil = tasksHitCeil;
    }

    public virtual void addDelegates(DetectCondition onGroundCondition, TasksWhenDetected tasksOnGround, DetectCondition hitCeilCondition, TasksWhenDetected tasksHitCeil)
    {
        this.onGroundCondition = onGroundCondition;
        this.tasksOnGround = tasksOnGround;
        this.hitCeilCondition = hitCeilCondition;
        this.tasksHitCeil = tasksHitCeil;
    }
    public virtual void Update()
    {
        bool couldOnGround = footDetect.IsTouchingLayers(ground);
        bool couldHitCeil = headDetect.IsTouchingLayers(ground);
        bool bodyOnGround = onGroundCondition();
        bool bodyCeil = hitCeilCondition();
        if (!this.state.isOnGround && couldOnGround && bodyOnGround)
        {
            state.isOnGround = true;
            this.tasksOnGround();
        }
        else if(!couldOnGround || !bodyOnGround)
        {
            state.isOnGround = false;
        }

        if(!this.state.isHitCeil && couldHitCeil && bodyCeil)
        {
            state.isHitCeil = true;
            this.tasksHitCeil();
        }else if(!couldHitCeil || !bodyCeil)
        {
            state.isHitCeil = false;
        }

    }

    public bool getDetect(DetectType type)
    {
        return this.state.getDetect(type);
    }
}

[System.Serializable]
public class PlayerTriggerDetecter : TriggerDetecter
{
    public Collider2D faceDetect;

    protected DetectCondition hitFaceCondition;
    protected TasksWhenDetected tasksHitFace;


    public PlayerTriggerDetecter()
    {
        this.state = new PlayerDetectStateSet();
    }
    public PlayerTriggerDetecter(Collider2D faceDetect,
                                 Collider2D footDetect,
                                 Collider2D headDetect,
                                 DetectCondition onGroundCondition,
                                 TasksWhenDetected tasksOnGround,
                                 DetectCondition hitCeilCondition,
                                 TasksWhenDetected tasksHitCeil,
                                 DetectCondition hitFaceCondition,
                                 TasksWhenDetected tasksHitFace
                              ) : base(footDetect,headDetect,onGroundCondition,tasksOnGround,hitCeilCondition,tasksHitCeil)
    {
        this.faceDetect = faceDetect;
        this.hitFaceCondition = hitFaceCondition;
        this.tasksHitFace = tasksHitFace;
        this.state = new PlayerDetectStateSet();
    }
    public void addDelegates(DetectCondition onGroundCondition,
                             TasksWhenDetected tasksOnGround,
                             DetectCondition hitCeilCondition,
                             TasksWhenDetected tasksHitCeil,
                             DetectCondition hitFaceCondition,
                             TasksWhenDetected tasksHitFace)
    {
        base.addDelegates(onGroundCondition, tasksOnGround, hitCeilCondition, tasksHitCeil);
        this.hitFaceCondition = hitFaceCondition;
        this.tasksHitFace = tasksHitFace;
    }
    public override void Update()
    {
        base.Update();
        bool couldHitFace = faceDetect.IsTouchingLayers(ground);
        bool bodyHitFace = hitFaceCondition();

        PlayerDetectStateSet pState = (PlayerDetectStateSet)this.state;

        if (couldHitFace && bodyHitFace)
        {
            pState.isHitWall_Face = true;
            this.tasksHitFace();
        }
        else 
        {
            pState.isHitWall_Face = false;
        }
    }
}

public enum DetectType
{
    ON_GROUND,HIT_CEIL,HIT_WALL_FACE,HIT_WALL_BACK
};

[System.Serializable]
public class DetectStateSet
{
    public Dictionary<DetectType, DetectCondition> dict;


    [ReadOnly] public bool isOnGround;
    [ReadOnly] public bool isHitCeil;

    public DetectStateSet()
    {
        dict = new Dictionary<DetectType, DetectCondition>();
        dict.Add(DetectType.ON_GROUND, () => { return this.isOnGround; });
        dict.Add(DetectType.HIT_CEIL, () => { return this.isHitCeil; });
    }
    public virtual bool getDetect(DetectType type)
    {
        if (!dict.ContainsKey(type)) throw new KeyNotFoundException();

        return dict[type]();
    }
}

[System.Serializable]
public class PlayerDetectStateSet : DetectStateSet
{

    [ReadOnly]  public bool isHitWall_Face;
    [ReadOnly]  public bool isHitWall_Back;


    public PlayerDetectStateSet():base()
    {

        dict.Add(DetectType.HIT_WALL_FACE, () => { return this.isHitWall_Face; });
        dict.Add(DetectType.HIT_WALL_BACK, () => { return this.isHitWall_Back; });
    }
}

[System.Serializable]
public class EnemyDetectStateSet : DetectStateSet
{

}