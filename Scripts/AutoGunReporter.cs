using UnityEngine;
using System.Collections;

public class AutoGunReporter : MonoBehaviour {

    AutoGun autoGun;




    void Awake()
    {
        autoGun = transform.root.GetComponent<AutoGun>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
            if (other.tag == "Enemy")
            {
                autoGun.AddPossibleTarget(other.gameObject);
            }
    }


    void OnTriggerExit2D(Collider2D other)
    {
            if (other.tag == "Enemy")
            {
                autoGun.RemovePossibleTarget(other.gameObject);
                if (other.gameObject == autoGun.currentTarget)
                {
                    autoGun.currentTarget = null;
                    autoGun.CalculateNewTarget();
                }
            }
    }
}
