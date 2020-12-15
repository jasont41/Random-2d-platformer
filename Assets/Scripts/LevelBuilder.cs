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

    //      Random ranges
    Vector2Int gapChance = new Vector2Int(0, 10);
    Vector2Int continueHigherChance = new Vector2Int(0, 2);
    Vector2Int ledgeWithGapBelowChance = new Vector2Int(0, 2);
    Vector2Int heightRandomChance = new Vector2Int(0, 5);
    Vector2Int repeatHeightChance = new Vector2Int(0, 3);
    Vector2Int twoGapChance = new Vector2Int(0, 2); 

    Vector2Int[] originalValues = new Vector2Int[6];


    // bool controls below 
    private bool LastTileWasGap; //if true last tile was a gap or double gap so the next tile cannot be a gap
    private bool LastWasHigher;
    private bool ledgeWithGapFunction; 

    private float incrementingVariable;

    //      Saving original values so that we can decrease/increase the bounds
    //      as needed to increase difficultly 

    private void Awake()
    {
        originalValues[0] = gapChance;
        originalValues[1] = continueHigherChance;
        originalValues[2] = ledgeWithGapBelowChance;
        originalValues[3] = heightRandomChance;
        originalValues[4] = repeatHeightChance;
        originalValues[5] = twoGapChance; 

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
            Instantiate(GeneralTilePrefab, new Vector3(temp, 0, 0), Quaternion.identity); // makes sure first 5 tiles are general tiles 
        }
        CalculatingHeights(); 

    }
    private void CalculatingHeights()
    {
         
        if (currentTile <= heightArray.Length) { //prevents an infinite recurisve loop 
            Chances();
        }   
    }
    private void Chances()
    {
        choiceMade = false;
        int tempRand = randomNumber(heightRandomChance); 
        if (randomNumber(heightRandomChance) == 0) 
        {
            LastTileWasGap = false;
            if (SearchHeightArray(1))
            {
                tempRand = randomNumber(repeatHeightChance);
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
        if (randomNumber(continueHigherChance) == 1 && LastWasHigher == true) // 50% chance that the area continues to get taller, may need to decrease this chance 
        {
            ContinueHigher();
            choiceMade = true;
            LastWasHigher = true;
            currentTile++;
        }

        tempRand = randomNumber(gapChance); 
        if (tempRand == 0 && !LastTileWasGap)
            //This will run a 10% chance of no tile at all, and a further 50% chance of a two tile gap
        {
            LastWasHigher = false; 
            choiceMade = true;
            tempRand = randomNumber(twoGapChance);
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

    //      Makes a decision if the tiles will continue to get higher
    //      Need to change to add a chance of it staying the same height
    //      or reduce height without a sudden drop 

    private void ContinueHigher()
    {
        heightArray[currentTile] = heightArray[currentTile - 1] + 1;
        float tempY = heightArray[currentTile] * spacingMultiple;
        float tempX = currentTile * spacingMultiple;
        Instantiate(GeneralTilePrefab, new Vector3(tempX, tempY, 0), Quaternion.identity);
        fillBottomTiles(heightArray[currentTile], tempX);
    }

    //      To fill in ground tiles below top tiles 

    private void fillBottomTiles(int val, float posX)
    {
        for(int i = 0; i < val; i++)
        {
            Instantiate(BelowTop, new Vector3(posX, (spacingMultiple * i),0), Quaternion.identity);
        }
    }

    //      To make sure one of the last two tiles is at least 3 tiles high

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

    //      This is to search the array to see if there is a tile of a certain
    //      height within N tiles. Will be useful when having a ledge AND
    //      tiles below to make sure they are access able by the player

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
    //      Same as above, but more generic. It just tells if you if there is
    //      a tile of a certain height within 5 tiles. This is to reduce redunancy
    //      while the above is to make sure certain conditions exist.

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


    //      Using this because it will clean up main function, and make it more
    //      clear what values we are taking the random of 
    private int randomNumber(Vector2Int val) 
    {
        return Random.Range(val.x, val.y); 
    }
}
