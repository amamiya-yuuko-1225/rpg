using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    //怪物参数，请在inspector中适配
    public float hp;
    public float velocity;
    public float hittedVelocity;
    public float attackArea;//当角色和怪物相距小于这个参数，怪物开始攻击！

    //一定要在inspector中把player拖到这个参数这里！！
    public player player;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    //有的同学手上的素材可能有受伤，死亡之类的动画，可以自行添加进去，搞不定来找我
    //这里只考虑idle，hurted和攻击
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
    //这里是ai，可以依据地图地形设计更复杂的ai，这里是最简单不能跳的追着玩家跑的ai，同志们仅供参考
    private void AutomaticallyMove()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) > 2) 
            //玩家在怪物周围一定范围内就不移动了，距离远才追
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
   //死亡判断
    private void BeDead()
    {
        if (hp <= 0)
        {
            Destroy(GetComponent<Collider2D>().gameObject);
        }
    }
    //被攻击的函数
    //player的子object ：“攻击”里面的触发碰撞体调用这个函数，如若修改请小心
    public void BeHit()
    {
        anim.SetBool("hurted",true);
        hp -= 20;//扣多少血，这里也可以自己调整
        //击退效果
        if (player.transform.position.x > transform.position.x)
            rb.velocity = new Vector2(-hittedVelocity * Time.deltaTime, 0);
        else
            rb.velocity = new Vector2(+hittedVelocity * Time.deltaTime, 0);
    }
}
