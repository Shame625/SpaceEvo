using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UniverseGenerator : MonoBehaviour 
{
    //sector offsets
    public float offset_x;
    public float offset_y;

    HashSet<Vector2> visibleUniverse = new HashSet<Vector2>();
    public GameObject sectorPrefab;
    Transform allSectors;
    public Vector2 Limits;
    public GameObject hazardPrefab;


    public GameManager GM;


    void Awake()
    {
        allSectors = transform.GetChild(0);
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        InitializeUniverse();
    }

    public void SectorEnter(Vector2 position)
    {
        visibleUniverse.Add(position);
        SectorSpawn(position);
    }


    void InitializeUniverse()
    {
        GameObject go = (GameObject)Instantiate(sectorPrefab, Vector2.zero, Quaternion.identity);
        go.transform.SetParent(allSectors);
        go.GetComponent<Sector>().SetParent(gameObject);
    }

    void SectorSpawn(Vector2 position)
    {
        Vector2[] locations = {
                              new Vector2(position.x-offset_x, position.y+offset_y), new Vector2(position.x, position.y+offset_y), new Vector2(position.x + offset_x, position.y+offset_y),
                              new Vector2(position.x-offset_x, position.y), new Vector2(position.x + offset_x, position.y), 
                              new Vector2(position.x-offset_x, position.y-offset_y), new Vector2(position.x, position.y-offset_y), new Vector2(position.x + offset_x, position.y-offset_y)
                              };
        foreach (Vector2 vec in locations)
        {
            if (!visibleUniverse.Contains(vec))
            {
                visibleUniverse.Add(vec);
                GameObject go = (GameObject)Instantiate(sectorPrefab, vec, Quaternion.identity);
                go.transform.SetParent(allSectors);
                go.GetComponent<Sector>().SetParent(gameObject);

            }
        }

    }
}
