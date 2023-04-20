using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Action { STOP, MOVE, ATTACK, BLOCK, PARRY, CLASHENTER, CLASHSTAY, CLASHEXIT, PUSH, HURT };
    public enum State { VERTICAL, HORIZONTAL };

    [Header("Movement")]
    [SerializeField] protected float walkSpeed;
    [SerializeField] protected float dashSpeed;
    [SerializeField] protected float dashTime;
    [SerializeField] bool isDashing;

    [Header("Clash")]
    Vector3 clashPosition;
    [SerializeField] float clashDistance;
    [SerializeField] float clashStartSpeed;
    [SerializeField] protected float clashBackDashSpeed;
    [SerializeField] protected float clashBackDashTime;

    [Header("Push")]
    [SerializeField] float pushTime;
    [SerializeField] float pushForce;

    [Header("Facing")]
    [SerializeField] protected bool isFacingRight;
    [SerializeField] protected float face;
    [SerializeField] Vector2 direction;

    [Header("Wall")]
    Vector3 wallPosition;
    [SerializeField] bool isWalled;
    [SerializeField] float wallDistance;

    [Header("Enums")]
    [SerializeField] public Action action;
    [SerializeField] public State state;

    [Header("Layers")]
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] LayerMask whatIsBody;
    [SerializeField] LayerMask whatIsWall;

    [Header("Player")]
    public PlayerConfiguration configuration;
    [SerializeField] protected float atkTime;
    [SerializeField] Character character;
    [SerializeField] Skin currentSkin;
    [SerializeField] int skinIndex;

    [Header("Stats")]

    [SerializeField] float nPlayer;

    [SerializeField] float atk;

    [SerializeField] float def;

    [SerializeField] float hitStun;

    [SerializeField] float blockStun;

    [SerializeField] float parryTime;

    #region Properties
    public float AtkTime { get => atkTime; set => atkTime = value; }

    public float NPlayer { get => nPlayer; set => nPlayer = value; }

    public float Damage { get => atk; set => atk = value; }

    public float BlockStun { get => blockStun; set => blockStun = value; }

    public float HitStun { get => hitStun; set => hitStun = value; }

    public float ParryTime { get => parryTime; set => parryTime = value; }

    public float Def { get => def; set => def = value; }
    public bool IsWalled { get => isWalled; set => isWalled = value; }
    public Character Character { get => character; set => character = value; }
    #endregion

    Coroutine currentCo;
    [SerializeField] protected Player opponent;

    //Components
    Collider2D co;
    protected Rigidbody2D rb;
    protected Animator anim;

    //Child
    SpriteRenderer sprBody;
    SpriteRenderer sprLegs;

    //[SerializeField] int skin;

    Coroutine clashCo;

    public virtual void Awake()
    {
        co = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        sprBody = transform.Find("Sprites").Find("Body").GetComponent<SpriteRenderer>();
        sprLegs = transform.Find("Sprites").Find("Legs").GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        //Assign opponent
        foreach (Player p in FindObjectsOfType<Player>())
            if (nPlayer != p.nPlayer)
                opponent = p;

        if (configuration != null)
        {
            Character = configuration.character;
            skinIndex = configuration.skinIndex;
        }

        AssignScriptableObject();

    }

    // Update is called once per frame
    public virtual void Update()
    {
        Anim();

        Clash();

        FaceToOpponent();

        CheckBackWall();
    }

    public virtual void FixedUpdate()
    {
        if (action == Action.BLOCK || action == Action.HURT)
            rb.velocity = Vector2.zero;
    }

    void AssignScriptableObject()
    {
        walkSpeed = Character.walkSpeed;
        dashSpeed = Character.dashSpeed;
        dashTime = Character.dashTime;

        clashDistance = Character.clashDistance;
        clashStartSpeed = Character.clashStartSpeed;
        clashBackDashSpeed = Character.clashBackDashSpeed;
        clashBackDashTime = Character.clashBackDashTime;

        pushTime = Character.pushTime;
        pushForce = Character.pushForce;
        wallDistance = Character.wallDistance;

        atk = Character.atk;
        def = Character.def;

        hitStun = Character.hitStun;
        blockStun = Character.blockStun;
        parryTime = Character.parryTime;

        anim.runtimeAnimatorController = Character.animator;

        currentSkin = Character.skins[configuration.skinIndex];

        //Body
        sprBody.material.SetTexture("_MainText", currentSkin.mainTexture);

        sprBody.material.SetColor("_Color1", currentSkin.color1);
        sprBody.material.SetColor("_NewColor1", currentSkin.newColor1);

        sprBody.material.SetColor("_Color2", currentSkin.color2);
        sprBody.material.SetColor("_NewColor2", currentSkin.newColor2);

        sprBody.material.SetColor("_Color3", currentSkin.color3);
        sprBody.material.SetColor("_NewColor3", currentSkin.newColor3);


        //Legs
        sprLegs.material.SetTexture("_MainText", currentSkin.mainTexture);

        sprLegs.material.SetColor("_Color1", currentSkin.color1);
        sprLegs.material.SetColor("_NewColor1", currentSkin.newColor1);

        sprLegs.material.SetColor("_Color2", currentSkin.color2);
        sprLegs.material.SetColor("_NewColor2", currentSkin.newColor2);

        sprLegs.material.SetColor("_Color3", currentSkin.color3);
        sprLegs.material.SetColor("_NewColor3", currentSkin.newColor3);

    }

    void Clash()
    {
        direction = new Vector2(face, 0);
        clashPosition = new Vector2(co.bounds.extents.x + 0.215f, 0) * face;

        RaycastHit2D hitClash = Physics2D.Raycast(transform.position + clashPosition, direction, clashDistance, whatIsBody);

        Debug.DrawRay(transform.position + clashPosition, direction * clashDistance, Color.red);

        //Deetcta la distancia para empezar el CLASH
        if (hitClash)
        {
            Player rival = hitClash.collider.transform.root.GetComponent<Player>();

            //Acciones de los jugadores que te impiden el CLASH
            if (
                (action != Action.ATTACK && rival.action != Action.ATTACK)
                &&
                (action != Action.STOP && rival.action != Action.STOP)
                &&
                (action != Action.HURT || rival.action != Action.HURT)
                &&
                (action != Action.PUSH && rival.action != Action.PUSH)
            )
            {
                //Empieza el CLASH si no lo han hecho
                if (action != Action.CLASHSTAY && action != Action.CLASHENTER)
                {
                    action = Action.CLASHENTER;

                    rb.velocity = new Vector2(face * clashStartSpeed, 0);

                    anim.SetTrigger("ClashEnter");
                }
            }
        }
        else
        {
            //Volver a MOVE después de dejar el CLASH
            if (action == Action.CLASHSTAY)
            {
                action = Action.MOVE;
            }
        }
    }

    void FaceToOpponent()
    {
        face = transform.localScale.x;

        isFacingRight = transform.position.x < opponent.transform.position.x;

        Vector2 scale = transform.localScale;
        if (isFacingRight)
            scale.x = 1;
        else
            scale.x = -1;
        transform.localScale = scale;
    }

    void CheckBackWall()
    {
        Vector2 dir = new Vector2(-face, 0);
        wallPosition = new Vector2(co.bounds.extents.x - 0.215f, 0) * -face;

        RaycastHit2D hitWall = Physics2D.Raycast(transform.position + wallPosition, dir, wallDistance, whatIsWall);

        Debug.DrawRay(transform.position + wallPosition, dir * wallDistance, Color.yellow);

        if (hitWall)
        {
            IsWalled = true;
            Debug.Log("Detecta muro");
        }
        else
            IsWalled = false;
    }

    #region Collisions
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && action == Action.CLASHENTER)
        {
            Debug.Log("Colisión entre jugadores");

            MatchManager.instance.ClashStop();

            Vector3 clashPos = direction;
            clashPos.y = 1;
            ParticleManager.instance.Play("Clash", transform.position + clashPos);

            AudioManagerBrackeys.instance.Play(character.clashEnter);

            AudioManagerBrackeys.instance.Play(character.clashStay);


            action = Action.CLASHSTAY;

            rb.velocity = Vector2.zero;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("OnCollisionExit2D");

            AudioManagerBrackeys.instance.StopPlaying("ClashStay");

            if ((action == Action.CLASHSTAY || action == Action.PUSH))
            {
                action = Action.MOVE;
            }
        }
    }

    #endregion

    #region Animation Events
    public void Anim()
    {
        anim.SetFloat("SpeedH", rb.velocity.x);
        anim.SetBool("isFacingRight", isFacingRight);
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("ClashStay", action == Action.CLASHSTAY);
        anim.SetBool("Hurt", action == Action.HURT);
    }

    public void ChangeAction(Action a)
    {
        Debug.Log(name + " cambia " + a.ToString());
        action = a;
    }

    public void PlaySound(string sound)
    {
        AudioManagerBrackeys.instance.Play(sound);
    }

    #endregion

    #region Combat
    public void Hurt(Character character)
    {
        Debug.Log("Hurt de " + name);
        AudioManagerBrackeys.instance.Play(character.slash);
        MatchManager.instance.TakeDamage(character.atk, nPlayer);
        ParticleManager.instance.Play("Hit", transform.position);
        if (currentCo != null)
        {
            StopCoroutine(currentCo);
            sprBody.color = new Color(255, 255, 255, 255);
            sprLegs.color = new Color(255, 255, 255, 255);
            anim.speed = 1;
        }
        StartCoroutine(TimeInHurt(character.hitStun));
        HitStopOnHit();
    }

    public void InBlockStun(Character character)
    {
        Debug.Log("BlockStun de " + name);

        AudioManagerBrackeys.instance.Play(character.clashEnter);
        StartCoroutine(TimeInBlockStun(character.blockStun));
        HitStopOnBlock();

    }

    public void Parried(Character character)
    {
        AudioManagerBrackeys.instance.Play(Character.parry);
        HitStopOnParry();
        currentCo = StartCoroutine(TimeParried(character.parryTime));
        Debug.Log(name + "ha sido parreado");
    }

    public void KnockBack()
    {
        Debug.Log("KnockBack de " + name);
        StartCoroutine(Dash(-face, pushForce, pushTime));
        StartCoroutine(TimeNoMove(pushTime));
    }
    #endregion

    #region Animation
    public void Intro()
    {
        anim.SetTrigger("Intro");
    }

    public void Sheathe()
    {
        anim.SetTrigger("Sheathe");
        action = Action.STOP;
        rb.velocity = Vector2.zero;
    }

    public void Victory()
    {
        anim.SetTrigger("Victory");
    }

    public void Die()
    {
        AudioManagerBrackeys.instance.Play(Character.death);
        ParticleManager.instance.Play("Hit", transform.position);
        anim.SetTrigger("Lose");
        Debug.Log(name + " muere");
    }

    #endregion

    #region Hitstop

    void HitStopOnBlock()
    {
        MatchManager.instance.HitStop(0.1f, 0.2f);
    }
    void HitStopOnHit()
    {
        MatchManager.instance.HitStop(0.2f, 0.3f);
    }
    void HitStopOnParry()
    {
        MatchManager.instance.HitStop(0.2f, 0.2f);
    }
    #endregion

    #region Coroutines

    protected IEnumerator Dash(float direction, float speed, float time)
    {
        Debug.Log("Dash de " + name);

        //Inicio
        isDashing = true;

        rb.velocity = new Vector2(direction * speed, 0f);

        //Tiempo
        yield return new WaitForSeconds(time);

        AudioManagerBrackeys.instance.Play("Stomp");

        //Fin
        rb.velocity = Vector2.zero;
        isDashing = false;
    }

    IEnumerator TimeNoMove(float time)
    {
        Debug.Log("TimeNoMove de " + name);
        action = Action.STOP;
        yield return new WaitForSeconds(time);
        action = Action.MOVE;
    }

    IEnumerator TimeInBlockStun(float time)
    {
        AudioManagerBrackeys.instance.Play("ClashSwords");
        Debug.Log("TimeInBlockStun de " + name);
        action = Action.STOP;
        yield return new WaitForSeconds(time);
        StartCoroutine(DropBlock());
    }

    IEnumerator TimeParried(float time)
    {
        Debug.Log("TimeParried de " + name);
        action = Action.STOP;
        anim.speed = 0;

        yield return new WaitForSeconds(time);

        action = Action.MOVE;
        anim.speed = 1;
    }

    IEnumerator TimeInHurt(float time)
    {
        anim.speed = 1;
        action = Action.HURT;
        yield return new WaitForSecondsRealtime(time);
        StartCoroutine(DropBlock());
    }

    IEnumerator DropBlock()
    {
        yield return null;
        //blockHold = false;
        anim.SetBool("BlockV", false);
        anim.SetBool("BlockH", false);
        action = Action.MOVE;
    }
    #endregion

}