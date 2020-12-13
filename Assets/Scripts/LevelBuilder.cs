using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{

    public GameObject GeneralTilePrefab;
    public GameObject BelowTop;
    private float spacingMultiple = 0.18f;
    private int currentTile; 
    private int firstRandomSeed = 3;
    public int[] heightArray; 
    

    
    
    
    private float incrementingVariable; 
    // Start is called before the first frame update
    void Start()
    {
        currentTile = 0; 
        for(int currVal = 0; currVal < 5; currVal++) // Always flat for the first five tiles 
        {
            float temp = currVal * spacingMultiple;
            currentTile++; 
            Instantiate(GeneralTilePrefab, new Vector3(temp, 0, 0), Quaternion.identity);
        }
        CalculatingHeights(); 

    }
    private void CalculatingHeights()
    {
         
        if (currentTile <= heightArray.Length) { 
            Chances();
            Debug.Log(1);
        }   
    }
    private void Chances()
    {
        int tempRand = Random.Range(1, 4); 
        if(tempRand == firstRandomSeed)
        {
            heightArray[currentTile] = 1;
            float tempY = heightArray[currentTile] * spacingMultiple;
            float tempX = currentTile * spacingMultiple;
            Instantiate(GeneralTilePrefab, new Vector3(tempX, tempY, 0), Quaternion.identity);
            fillBottomTiles(heightArray[currentTile],tempX); 
            Debug.Log(2); 
        }
        else
        {
            float tempX = currentTile * spacingMultiple;
            Instantiate(GeneralTilePrefab, new Vector3(tempX, 0, 0), Quaternion.identity);
            Debug.Log(3); 
        }
        currentTile++;
        CalculatingHeights(); 
    }

    private void fillBottomTiles(int val, float posX)
    {
        for(int i = 0; i < val; i++)
        {
            Instantiate(BelowTop, new Vector3(posX, (spacingMultiple * i),0), Quaternion.identity);
        }
    }
}
