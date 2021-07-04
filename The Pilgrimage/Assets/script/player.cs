using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    //player�ĸ��壬��������������ײ��͵������
    private CapsuleCollider2D coll;
    private Rigidbody2D rb;
    private Animator anim;
    public LayerMask ground;//�ѵ����layer���ó�ground

    //player�ĸ��ֿɵ����������Լ����价��
    public float velocity;//�ٶȣ���inspector���ú��ʵ�
    public float jumpForce;//��Ծ����inspector���ú��ʵ�
    public float hitForce;//�����к��ˣ���inspector���ú��ʵ�
    public float attackStart;//������ʼʱ�䣬һ������Ϊ0
    public float attackEnd;//��������ʱ�䣬����������Ϊ��0.04�������Ŀ��������ƥ��
    private bool isAttacking = false;//�Ƿ��ڹ�����״̬����

    //����ʱ�Ĵ�����ײ��
    private BoxCollider2D normalAttack;//��ͨ�����Ĵ�����ײ�壬��player��object�����
    private CircleCollider2D jumpAttack;//���乥����ͬ��

    //Ĭ�ϵĽ�ɫ����
    private int hp = 100;//Ѫ��
    private int defense = 1;//������
    public int hp_ore;//Ѫ��ʯ���뽭ͬѧ����ƽ���ʱ����Щpublic����
    public int df_ore;//����ʯ
    public int ch_ore;//����ʯ
    
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
        Death();//�����ж�
        if(!anim.GetBool("hurted"))
            Movement();
        SwitchAnim();
        Attack();
    }
    //�����ж�
    void Death() 
    {
        if (hp <= 0)
        {
            anim.SetBool("death",true);
            //���������ת�߼��鷳��ͬѧ�ˣ���ת���浵��
        }
    }
    //�ƶ����
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
    //�������
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
    //���ֶ����л�
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
    //���ʯ������أ���������������
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
    //�������
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
