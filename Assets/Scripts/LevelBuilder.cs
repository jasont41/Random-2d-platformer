using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{

    public GameObject GeneralTilePrefab;
    public GameObject BelowTop;
    private float spacingMultiple = 0.18f;
    private int currentTile;
    private int firstRandomSeed = 5;
    private int LedgeWithGapHeight;
    public int[] heightArray;
    public bool choiceMade; //true == something other than a general block 2 y=0 happens 
    Vector2Int gapChance = new Vector2Int(0, 10);
    Vector2Int continueHigherChance = new Vector2Int(0, 4);
    Vector2Int ledgeWithGapBelowChance = new Vector2Int(0, 4);

    Vector2Int[] originalValues = new Vector2Int[3];


    // bool controls below 
    private bool LastTileWasGap; //if true last tile was a gap or double gap so the next tile cannot be a gap
    private bool LastWasHigher;
    private bool ledgeWithGapFunction; 

    private float incrementingVariable;

    private void Awake()
    {
        originalValues[0] = gapChance;
        originalValues[1] = continueHigherChance;
        originalValues[2] = ledgeWithGapBelowChance; 
    }
    // Start is called before the first frame update
    void Start()
    {
        LastTileWasGap = false; 
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
        }   
    }
    private void Chances()
    {
        choiceMade = false;
        int tempRand = Random.Range(1, 6);
        if(tempRand == firstRandomSeed)
        {
            LastTileWasGap = false;
            if (SearchHeightArray(1))
            {
                tempRand = Random.Range(0, 3);
            }
            if (!LastWasHigher && tempRand != 0)
            {
                heightArray[currentTile] = 1;
                float tempY = heightArray[currentTile] * spacingMultiple;
                float tempX = currentTile * spacingMultiple;
                Instantiate(GeneralTilePrefab, new Vector3(tempX, tempY, 0), Quaternion.identity);
                fillBottomTiles(heightArray[currentTile], tempX);
                choiceMade = true;
                LastWasHigher = true;
                currentTile++; 
            }
        }
        tempRand = Random.Range(continueHigherChance.x,continueHigherChance.y);
        if (tempRand == 1 && LastWasHigher == true) // 50% chance that the area continues to get taller, may need to decrease this chance 
        {
            ContinueHigher();
            choiceMade = true;
            LastWasHigher = true;
            currentTile++;
        }
        
        tempRand = Random.Range(gapChance.x, gapChance.y);
        if (tempRand == 9 && !LastTileWasGap)
            //This will run a 10% chance of no tile at all, and a further 50% chance of a two tile gap
        {
            LastWasHigher = false; 
            choiceMade = true;
            tempRand = Random.Range(0, 2);
            if(tempRand == 1)
            {
                currentTile++; 
            }
        }
        else if (LastOneOfTwoWasHigher())
        {
            //      Will change, just want it to work with the ledge of 3 wide first
            //      Gap first
            currentTile++; 
            for(int i = 0; i < 3; i++)
            {
                heightArray[currentTile] = LedgeWithGapHeight;
                float tempY = heightArray[currentTile] * spacingMultiple;
                float tempX = currentTile * spacingMultiple;
                Instantiate(GeneralTilePrefab, new Vector3(tempX, tempY, 0), Quaternion.identity);
                Instantiate(GeneralTilePrefab, new Vector3(tempX, tempY, 0), Quaternion.identity);
                currentTile++;
            }
            choiceMade = true;
            LastWasHigher = false; 
        }
        else if (choiceMade == false)
        {
            LastWasHigher = false; 
            LastTileWasGap = false; 
            float tempX = currentTile * spacingMultiple;
            Instantiate(GeneralTilePrefab, new Vector3(tempX, 0, 0), Quaternion.identity);
            currentTile++;
        }
        
        CalculatingHeights(); 
    }

    private void ContinueHigher()
    {
        heightArray[currentTile] = heightArray[currentTile - 1] + 1;
        float tempY = heightArray[currentTile] * spacingMultiple;
        float tempX = currentTile * spacingMultiple;
        Instantiate(GeneralTilePrefab, new Vector3(tempX, tempY, 0), Quaternion.identity);
        fillBottomTiles(heightArray[currentTile], tempX);
    }
    private void fillBottomTiles(int val, float posX)
    {
        for(int i = 0; i < val; i++)
        {
            Instantiate(BelowTop, new Vector3(posX, (spacingMultiple * i),0), Quaternion.identity);
        }
    }
    private bool LastOneOfTwoWasHigher()
    {
        LedgeWithGapHeight = 0; // clearing the variable
        int tempRand = Random.Range(ledgeWithGapBelowChance.x, ledgeWithGapBelowChance.y);
        if(tempRand != 0) // Fails chance to get a ledge with a gap below so returns 
        {
            return false; 
        }
        else if(heightArray[currentTile - 1] >= 3)
        {
            LedgeWithGapHeight = heightArray[currentTile - 1]; 
            return true;
        }
        else if(heightArray[currentTile - 2] >= 3)
        {
            LedgeWithGapHeight = heightArray[currentTile - 2]; 
            return true; 
        }
        else
        {
            return false; 
        }
    }
    private bool SearchHeightArray(int HeightNeeded, int HistoryLength)
    {
        for(int index = currentTile; index < currentTile - HistoryLength ; index--)
        {
            if(heightArray[index] == HeightNeeded)
            {
                return true; 
            }
        }
        return false; 
    }
    private bool SearchHeightArray(int HeightNeeded)
    {
        for(int index = currentTile; index < currentTile - 5; index--)
        {
            if(heightArray[index] == HeightNeeded)
            {
                return true; 
            }
        }
        return false; 
    }
}
