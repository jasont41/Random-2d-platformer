using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{

    public GameObject GeneralTilePrefab;

    public float CurrentTileLength; // must be even number 
    private float incrementingVariable; 
    // Start is called before the first frame update
    void Start()
    {
        incrementingVariable = CurrentTileLength / 5; 
        //Instantiate(GeneralTilePrefab, new Vector3 (0, 0, 0), Quaternion.identity); 
        for(float i = 0f; i < CurrentTileLength; i += 0.18f)
        {
            Instantiate(GeneralTilePrefab, new Vector3(i, 0, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
