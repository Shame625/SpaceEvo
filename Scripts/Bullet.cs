using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    float Speed = 10;
    public float Dmg = 0;

    Vector2 startedAt;

    public Vector2 offset_destruction;
    float moduleLevel = 1;

    void Start()
    {
        startedAt = transform.position;
    }

	void Update () 
    {
        if (!Builder.isBuilding)
        {
            transform.Translate(Vector2.up * Speed * Time.deltaTime);

            if (transform.position.x >= startedAt.x + offset_destruction.x * moduleLevel || transform.position.x <= startedAt.x - offset_destruction.x * moduleLevel ||
                transform.position.y >= startedAt.y + offset_destruction.y * moduleLevel || transform.position.y <= startedAt.y - offset_destruction.y * moduleLevel)
            {
                ObjDelete();
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Modules" && other.name != "ImportantCollider" && other.transform.parent.tag !="Player")
        {
            other.gameObject.GetComponent<Module>().TakeDamage(Dmg);
        }
        if (other.tag == "Modules")
        {
            other.gameObject.GetComponent<Module>().TakeDamage(Dmg);
        }
    }

    void ObjDelete()
    {
       /* //Deatach trail renderer
        if(transform.childCount > 0)
        {
        transform.GetChild(0).gameObject.GetComponent<DestroyTrail>().DestructionSequence();
        transform.GetChild(0).transform.SetParent(GameObject.FindGameObjectWithTag("GameManager").transform);

        }*/
        Destroy(gameObject);
    }

    public void SetProperties(Color myColor, float speed, float dmg, int level)
    {
        GetComponent<SpriteRenderer>().color = myColor;
        if (transform.childCount > 0)
        {
            transform.GetChild(0).GetComponent<TrailRenderer>().material.color = myColor;
            transform.GetChild(0).GetComponent<TrailRenderer>().time = (float)level / 20;
            transform.GetChild(0).GetComponent<TrailRenderer>().sortingOrder = 4;
        }
        Speed = speed;
        Dmg = dmg;
        moduleLevel = (level+1)/2f;
        
    }

}
