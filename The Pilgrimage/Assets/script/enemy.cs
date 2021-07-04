using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    //�������������inspector������
    public float hp;
    public float velocity;
    public float hittedVelocity;
    public float attackArea;//����ɫ�͹������С��������������￪ʼ������

    //һ��Ҫ��inspector�а�player�ϵ�������������
    public player player;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    //�е�ͬѧ���ϵ��زĿ��������ˣ�����֮��Ķ���������������ӽ�ȥ���㲻��������
    //����ֻ����idle��hurted�͹���
    void FixedUpdate()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) < attackArea)
            anim.SetBool("fire", true);
        else
            anim.SetBool("fire",false);

        if (anim.GetBool("hurted") && rb.velocity.x < 1)
            anim.SetBool("hurted",false);

        BeDead();

        if(!anim.GetBool("hurted"))
            AutomaticallyMove();
    }
    //������ai���������ݵ�ͼ������Ƹ����ӵ�ai����������򵥲�������׷������ܵ�ai��ͬ־�ǽ����ο�
    private void AutomaticallyMove()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) > 2) 
            //����ڹ�����Χһ����Χ�ھͲ��ƶ��ˣ�����Զ��׷
        {
            if (player.transform.position.x > transform.position.x)
            {
                rb.velocity = new Vector2(velocity * Time.deltaTime, 0);
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                rb.velocity = new Vector2(-velocity * Time.deltaTime, 0);
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

    }
   //�����ж�
    private void BeDead()
    {
        if (hp <= 0)
        {
            Destroy(GetComponent<Collider2D>().gameObject);
        }
    }
    //�������ĺ���
    //player����object ��������������Ĵ�����ײ�������������������޸���С��
    public void BeHit()
    {
        anim.SetBool("hurted",true);
        hp -= 20;//�۶���Ѫ������Ҳ�����Լ�����
        //����Ч��
        if (player.transform.position.x > transform.position.x)
            rb.velocity = new Vector2(-hittedVelocity * Time.deltaTime, 0);
        else
            rb.velocity = new Vector2(+hittedVelocity * Time.deltaTime, 0);
    }
}
