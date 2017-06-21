using UnityEngine;
using System.Collections;

public class playerGun : MonoBehaviour {

    Player player;
    public Transform movablePart;
    public Transform shootStart;
    Animation anim;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>() ;
        movablePart = transform.GetChild(0);
        shootStart = movablePart.GetChild(0).GetChild(0);
        anim = movablePart.GetChild(0).GetComponent<Animation>();
    }

    void Update()
    {
        if (!Builder.isBuilding)
        {
        #if UNITY_STANDALONE_WIN
            LookAtMouse();
        #endif
        #if UNITY_EDITOR
            LookAtMouse();
        #endif
        }

    }

    public void Shoot()
    {
        LookAtMouse();
        anim.Play();
        player.Shoot(0, shootStart.position, movablePart.rotation);
    }

    public void LookAtMouse()
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        movablePart.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }


}
