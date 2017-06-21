using UnityEngine;
using System.Collections;

public class Sector : MonoBehaviour {

    UniverseGenerator Universe;

    public bool nearLimit = false;
    public bool hazard = false;

    bool added = false;

    void Start()
    {
        hazard = CheckIfHazardous(transform.position);
        if (hazard)
        {
            SpawnHazardOverlay();
        }
        else if (!hazard)
        {
            nearLimit = CheckIfNearLimit(transform.position);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            if (!hazard && !nearLimit)
            {
                DestroyTrigger();
            }
            if (!added)
            {
                Universe.SectorEnter(transform.position);
                added = true;
            }
            if (hazard)
            {
                Universe.GM.PlayerInHazard(true);
            }
            if (nearLimit)
            {
                Universe.GM.PlayerInHazard(false);
            }
        }
    }

    bool CheckIfHazardous(Vector2 position)
    {
        if (transform.position.x >= Universe.Limits.x + Universe.offset_x || transform.position.x <= -Universe.Limits.x - Universe.offset_x
    || transform.position.y >= Universe.Limits.y + Universe.offset_y || transform.position.y <= -Universe.Limits.y - Universe.offset_y)
            return true;

        return false;
    }

    bool CheckIfNearLimit(Vector2 position)
    {
        if (transform.position.x >= Universe.Limits.x - Universe.offset_x || transform.position.x <= -Universe.Limits.x + Universe.offset_x
            || transform.position.y >= Universe.Limits.y - Universe.offset_y || transform.position.y <= -Universe.Limits.y + Universe.offset_y)
            return true;

        return false;
    }

    void DestroyTrigger()
    {
        Destroy(GetComponent<BoxCollider2D>());
    }


    public void SetParent(GameObject go)
    {
        Universe = go.GetComponent<UniverseGenerator>();
    }

    void SpawnHazardOverlay()
    {
        GameObject go = (GameObject)Instantiate(Universe.hazardPrefab, transform.position, Quaternion.identity);
        go.transform.SetParent(transform);
    }
}
