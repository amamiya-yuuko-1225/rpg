using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpAttack : MonoBehaviour
{
    private Collider2D coll;
    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "monster")
        {
            enemy enemy = collision.gameObject.GetComponent<enemy>();
            if(enemy != null)
                enemy.BeHit();
        }
    }
}
