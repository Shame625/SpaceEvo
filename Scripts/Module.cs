using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Module : MonoBehaviour 
{
	public bool hasCore = false;
    public Sprite icon;

    public float maximumHp;
    public float currentHp;
    Player player;

    public bool alive = true;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
	public void GetNeighborsObjects()
	{
		hasCore = true;
		GetComponentInParent<Player> ().alreadyLooked.Add (gameObject);

		foreach (GameObject go in GetComponentInParent<Player>().playerModules) 
		{
			if (!GetComponentInParent<Player> ().alreadyLooked.Contains (go)) {

                if (go.transform.localPosition.x == transform.localPosition.x && go.transform.localPosition.y == transform.localPosition.y + 1) 
				{
					go.GetComponent<Module> ().GetNeighborsObjects ();
				}
                else if (go.transform.localPosition.x == transform.localPosition.x && go.transform.localPosition.y == transform.localPosition.y - 1) 
				{
					go.GetComponent<Module> ().GetNeighborsObjects ();
				}
                else if (go.transform.localPosition.x == transform.localPosition.x + GameManager.offsetX && go.transform.localPosition.y == transform.localPosition.y + GameManager.offsetY) 
				{
					go.GetComponent<Module> ().GetNeighborsObjects ();
				}
                else if (go.transform.localPosition.x == transform.localPosition.x - GameManager.offsetX && go.transform.localPosition.y == transform.localPosition.y + GameManager.offsetY) 
				{
					go.GetComponent<Module> ().GetNeighborsObjects ();
				}
                else if (go.transform.localPosition.x == transform.localPosition.x + GameManager.offsetX && go.transform.localPosition.y == transform.localPosition.y - GameManager.offsetY) 
				{
					go.GetComponent<Module> ().GetNeighborsObjects ();
				}
                else if (go.transform.localPosition.x == transform.localPosition.x - GameManager.offsetX && go.transform.localPosition.y == transform.localPosition.y - GameManager.offsetY) 
				{
					go.GetComponent<Module> ().GetNeighborsObjects ();
				}
			}
		}
	}

    public void SetColor(Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;

        //yey way of setting colors
        if (gameObject.name != "hpModule")
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<SpriteRenderer>() != null)
                    child.GetComponent<SpriteRenderer>().color = newColor;
                if (child.childCount > 0)
                {
                    foreach (Transform child_of_child in child.transform)
                    {
                        if (child_of_child.GetComponent<SpriteRenderer>() != null)
                            child_of_child.GetComponent<SpriteRenderer>().color = newColor;
                    }
                }
            }
        }
        else
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = newColor;
            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = newColor;
            Animation anim = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animation>();
            anim["moduleSpecific"].speed = player.GetComponent<PlayerStats>().hp_Yield / 25f;
        }

        //a bit shitty, but hey, it works ;)
        if (gameObject.name == "engineModule")
        {
            TrailRenderer trail = transform.GetChild(0).GetComponentInChildren<TrailRenderer>();
            trail.material.color = newColor;    
        }

        if (gameObject.name == "turningModule")
        {

            Animation anim = transform.GetChild(0).GetComponent<Animation>();
            anim["moduleSpecific"].speed = player.GetComponent<PlayerStats>().rotationSpeed / 25;
        }
        else if (gameObject.name == "engineModule")
        {
            
            Animation anim = transform.GetChild(0).GetComponent<Animation>();
            anim["moduleSpecific"].speed = player.GetComponent<PlayerStats>().maximumSpeed;
        }
        else if (gameObject.name == "coreModule")
        {
            Animation anim = transform.GetChild(0).GetComponent<Animation>();
            anim["moduleSpecific"].speed = player.GetComponent<PlayerStats>().coreModuleSize.x + 0.25f;    
        }
    }

    public void TakeDamage(float dmg)
    {
        if (gameObject.name != "grinderModule")
        {
            float temp = 0;
            if (player.currentShield > 0)
            {
                player.TakeDamageShield(dmg);
                if (player.currentShield < 0)
                {
                    temp = Mathf.Abs(player.currentShield);
                    player.currentShield = 0;
                }
            }
            if (temp != 0)
                dmg = temp;
            if (player.currentShield <= 0)
            {
                if (currentHp > 0)
                {
                    currentHp -= dmg;
                    if (currentHp < 0)
                        dmg = Mathf.Abs(currentHp);

                    
                    player.TakeDamageIndividual(dmg);
                }
                if (currentHp <= 0 && gameObject.activeSelf && alive)
                {
                    alive = false;
                    currentHp = 0;
                    player.playerModules.Remove(gameObject);
                    player.CheckIfConnectedToCoreAttacked();
                    PlayDead();
                    player.destroyedModules.Add(gameObject);
                    if (gameObject.name == "hpModule")
                        player.HpModuleDestroyed();
                    else if (gameObject.name == "shieldModule")
                        player.ShieldModuleDestroyed();
                    else if (gameObject.name == "turningModule" || gameObject.name == "engineModule")
                        player.CalculateMovement();
                    
                }
            }
        }
    }

    public void OverrideTakeDmg(float dmg)
    {
        TakeDamage(dmg);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    public void PlayDead()
    {
        GetComponent<Animation>().Play("objectDelete");
        Invoke("Hide", GetComponent<Animation>()["objectDelete"].length);
    }

	}
