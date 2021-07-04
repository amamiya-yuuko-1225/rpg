using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    //player的刚体，动画控制器，碰撞体和地面相关
    private CapsuleCollider2D coll;
    private Rigidbody2D rb;
    private Animator anim;
    public LayerMask ground;//把地面的layer设置成ground

    //player的各种可调参数，请自己适配环境
    public float velocity;//速度，在inspector设置合适的
    public float jumpForce;//跳跃，在inspector设置合适的
    public float hitForce;//被击中后退，在inspector设置合适的
    public float attackStart;//攻击开始时间，一般设置为0
    public float attackEnd;//攻击结束时间，我这里设置为了0.04，动画的快慢和这个匹配
    private bool isAttacking = false;//是否在攻击的状态变量

    //攻击时的触发碰撞体
    private BoxCollider2D normalAttack;//普通攻击的触发碰撞体，是player子object的组件
    private CircleCollider2D jumpAttack;//下落攻击，同上

    //默认的角色参数
    private int hp = 100;//血量
    private int defense = 1;//防御力
    public int hp_ore;//血量石，请江同学在设计交易时用这些public变量
    public int df_ore;//防御石
    public int ch_ore;//暴击石
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
        normalAttack = GetComponentInChildren<BoxCollider2D>();
        jumpAttack = GetComponentInChildren<CircleCollider2D>();
    }
    
    void FixedUpdate()
    {
        Death();//死亡判断
        if(!anim.GetBool("hurted"))
            Movement();
        SwitchAnim();
        Attack();
    }
    //死亡判断
    void Death() 
    {
        if (hp <= 0)
        {
            anim.SetBool("death",true);
            //死亡后的跳转逻辑麻烦江同学了，跳转到存档处
        }
    }
    //移动相关
    void Movement()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float faceOrientation = Input.GetAxisRaw("Horizontal");
        if (horizontalMove != 0)
        {
            rb.velocity = new Vector2(horizontalMove * velocity * Time.deltaTime, rb.velocity.y);
            anim.SetFloat("running", Mathf.Abs(horizontalMove));
            anim.SetBool("idle",false);
        }else{
            anim.SetFloat("running",0);
        }
        if (faceOrientation != 0) 
        {
            transform.localScale = new Vector3(faceOrientation,1,1);
        }
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground) &&(anim.GetBool("idle") || anim.GetFloat("running") > 0.1))
        {
            rb.velocity = new Vector2(rb.velocity.x,jumpForce*Time.deltaTime);
            anim.SetBool("idle", false);
            anim.SetBool("jumping",true);
        }
    }
    //攻击相关
    void Attack()
    {
        if (!isAttacking && !anim.GetBool("running"))
        {
            if (Input.GetMouseButton(0))
            {
                anim.SetBool("attacking", true);
                isAttacking = true;
                StartCoroutine(PlayAttack());
            }
            else
            {
                anim.SetBool("attacking", false);
            }
        }
    }
    
    IEnumerator PlayAttack()
    {
        yield return new WaitForSeconds(attackStart);
        if (anim.GetBool("falling"))
        {
            jumpAttack.enabled = true;
        }
        else 
        {
            normalAttack.enabled = true;
        }
        yield return new WaitForSeconds(attackEnd);
        jumpAttack.enabled = false;
        normalAttack.enabled = false;
        isAttacking = false;
    }
    //部分动画切换
    void SwitchAnim()
    {
        if (anim.GetBool("hurted") && Mathf.Abs(rb.velocity.x) < 0.1f)
        {
            anim.SetBool("hurted",false);
        }
        if (anim.GetBool("jumping"))
        {
            if (rb.velocity.y < 0)
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", true);
            }
        }
        else if (coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            anim.SetBool("falling", false);
            anim.SetBool("idle", true);
        }
    }
    //与矿石交互相关，如有问题来找我
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("ore"))
        {
            Destroy(collision.gameObject);
            switch (collision.tag.Substring(tag.Length - 1, tag.Length))
            {
                case "1":hp_ore += 1;break;
                case "2":df_ore += 1;break;
                case "3":ch_ore += 1;break;
                default:break;
            }
        }
    }
    //受伤相关
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "monster")
        {
            hp -= 10;
            anim.SetBool("hurted", true);
            if (transform.position.x <= collision.gameObject.transform.position.x)
                rb.velocity = new Vector2(-hitForce * Time.deltaTime, rb.velocity.y);
            else
                rb.velocity = new Vector2(hitForce * Time.deltaTime, rb.velocity.y);
        }
    }
}
