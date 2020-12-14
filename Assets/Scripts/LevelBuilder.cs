﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{

    public GameObject GeneralTilePrefab;
    public GameObject BelowTop;
    private float spacingMultiple = 0.18f;
    private int currentTile; 
    private int firstRandomSeed = 5;
    public int[] heightArray;
    public bool choiceMade; //true == something other than a general block 2 y=0 happens 
    Vector2Int gapChance = new Vector2Int(0, 10);
    Vector2Int continueHigherChance = new Vector2Int(0, 4);


    // bool controls below 
    private bool LastTileWasGap; //if true last tile was a gap or double gap so the next tile cannot be a gap
    private bool LastWasHigher; 

    private float incrementingVariable; 
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
            if (!LastWasHigher)
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
}
