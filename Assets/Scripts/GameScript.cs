/*
 * Copyright (c) 2020 Joy Ajayi
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
* 
* Heading: This script is written to the game board which instantiates the array
* of gameobjects for selecting, swapping, deleting and replacing
* Author: Joy Ajayi
* Online Repository: https://github.com/AjayiJoy/U
* File format: Assembly-CSharp (.cs)
 */



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public Transform startPoint; //start point of the game board
    public GameObject[] gameTiles; //the colors to be used will be stored
    public GameObject[,] gameBoard; //this is the array thart spawns in the scene
    public GameObject firstHit = null;
    public GameObject secondHit = null;
    public Constants.GameState boardState = Constants.GameState.None;
    public int noOfMoves = 10;
    public int score = 0;
    public Text movesText;
    public Text scoreText;
    public Transform scoreEffect;
    public GameObject panel;


    void CreateBoard()
    {
        gameBoard = new GameObject[Constants.numberOfRows, Constants.numberOfColumns];

        for (int row = 0; row < Constants.numberOfRows; row++)
        {
            for (int column = 0; column < Constants.numberOfColumns; column++)
            {
                GameObject randomTile = gameTiles[Random.Range(0, gameTiles.Length)];

                GameObject tileInstantiation = Instantiate(randomTile, new Vector2(startPoint.position.x + column * Constants.tileDistance, startPoint.position.y + row * Constants.tileDistance), Quaternion.identity);

                gameBoard[row, column] = tileInstantiation;

                tileInstantiation.GetComponent<TileScript>().Assign(tileInstantiation, row, column); //assigns this values to the individual objects


            }
        }
    }

    void Start()
    {
        CreateBoard();
        movesText.text = "Moves: " + noOfMoves;
        scoreText.text = "Score: " + score;
        MatchOnStart();
    }

    void Update()
    {
        movesText.text = "Moves: " + noOfMoves;
        scoreText.text = "Score: " + score;

        if (noOfMoves == 0) 
        { 
            panel.SetActive(true);
            score = 0;
            noOfMoves = 10;
        }

        if (boardState == Constants.GameState.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mouseHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (mouseHit.collider != null)
                {
                    firstHit = mouseHit.collider.gameObject;
                    firstHit.GetComponent<SpriteRenderer>().color = Constants.tileSelected;
                    boardState = Constants.GameState.SelectionStarted;
                }
            }
        }

        else if (boardState == Constants.GameState.SelectionStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mouseHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (firstHit != null && mouseHit.collider != null)
                {
                    secondHit = mouseHit.collider.gameObject;
                    secondHit.GetComponent<SpriteRenderer>().color = Constants.tileSelected;
                    boardState = Constants.GameState.Swapping;
                }

            }
        }

        else if (boardState == Constants.GameState.Swapping)
        {

            SwapAndMatch(firstHit, secondHit);
            firstHit.GetComponent<SpriteRenderer>().color = Constants.tileDeselected;
            secondHit.GetComponent<SpriteRenderer>().color = Constants.tileDeselected;

            firstHit = null;
            secondHit = null;
            boardState = Constants.GameState.None;
        }

        MatchOnStart();
    }


    void MatchOnStart()
    {
        List<GameObject> totalMatches = new List<GameObject>();

        for(int row = 0; row <Constants.numberOfRows; row++)
        {
            for(int column = 0; column < Constants.numberOfColumns; column++)
            {
                var firstHMatch = HorizontalMatches(gameBoard[row,column]);
                totalMatches.AddRange(firstHMatch);
                var firstVMatch = VerticalMatches(gameBoard[row, column]);
                totalMatches.AddRange(firstVMatch);
            }
        }

        if (totalMatches.Count >= Constants.minimumMatch)
        {
             IncreaseScore((totalMatches.Count - 2) * Constants.scoreFor3);

            foreach (var item in totalMatches)
            {
                RemoveFromBoard(item);
                Instantiate(scoreEffect, item.transform.position, Quaternion.identity);
            }

            StartCoroutine(CollapseAndReplace());
        }

        totalMatches.Clear();
    }
    void SwapAndMatch(GameObject firstSelect, GameObject secondSelect)
    {
        List<GameObject> totalMatches = new List<GameObject>();
        if (Constants.IsObjSharingRowAndColumn(firstSelect, secondSelect) == true)
        {
            Swap(firstSelect, secondSelect);
        }
        else
            return;

        var firstHMatch = HorizontalMatches(firstSelect);
        totalMatches.AddRange(firstHMatch);
        var firstVMatch = VerticalMatches(firstSelect);
        totalMatches.AddRange(firstVMatch);
        var secondHMatch = HorizontalMatches(secondSelect);
        totalMatches.AddRange(secondHMatch);
        var secondVMatch = VerticalMatches(secondSelect);
        totalMatches.AddRange(secondVMatch);

        if(totalMatches.Count < Constants.minimumMatch)
        {
            Swap(secondSelect, firstSelect);
            return;
        }

        if(totalMatches.Count >= Constants.minimumMatch)
        {
            noOfMoves--;

            if (totalMatches.Count > Constants.minimumMatch)
                IncreaseScore((totalMatches.Count - 2 )* Constants.scoreFor4);
            else 
                IncreaseScore((totalMatches.Count - 2)* Constants.scoreFor3);

            foreach(var item in totalMatches)
            {
                RemoveFromBoard(item);
                Instantiate(scoreEffect,item.transform.position, Quaternion.identity);
            }


            StartCoroutine(CollapseAndReplace());
        }

        totalMatches.Clear();
    }

    public int IncreaseScore(int newScore)
    {
        return score += newScore;
    }

    IEnumerator CollapseAndReplace(float shiftDelay = 0.05f)
    {
        for(int column = 0; column < Constants.numberOfColumns; column++)
        {
            //first row
            for(int firstRow = 0; firstRow < Constants.numberOfRows - 1; firstRow++)
            {
                if(gameBoard[firstRow, column].GetComponent<SpriteRenderer>().sprite == null)
                {
                    for(int nextRow = firstRow + 1; nextRow < Constants.numberOfRows; nextRow++)
                    {
                        if(gameBoard[nextRow, column].GetComponent<SpriteRenderer>().sprite != null)
                        {
                            gameBoard[firstRow, column].GetComponent<SpriteRenderer>().sprite = gameBoard[nextRow, column].GetComponent<SpriteRenderer>().sprite; //Changing sprite
                            gameBoard[firstRow, column].tag = gameBoard[nextRow, column].tag;
                            gameBoard[firstRow, column].name = gameBoard[nextRow, column].name;

                            gameBoard[nextRow, column].GetComponent<SpriteRenderer>().sprite = null;

                            gameBoard[firstRow, column].GetComponent<TileScript>().Assign(gameBoard[firstRow, column], firstRow, column); //Reassigning

                            break;
                        }
                    }
                }
            }


            yield return new WaitForSeconds(shiftDelay);


            //Replacing empty spaces
            for (int emptyRow = 0; emptyRow < Constants.numberOfRows; emptyRow++)
            {
                if (gameBoard[emptyRow, column].GetComponent<SpriteRenderer>().sprite == null)
                {
                    Destroy(gameBoard[emptyRow, column]);

                    GameObject randomTile = gameTiles[Random.Range(0, gameTiles.Length)];

                    GameObject newInstant = Instantiate(randomTile, new Vector2(gameBoard[emptyRow, column].transform.position.x, gameBoard[emptyRow, column].transform.position.y), Quaternion.identity);

                    gameBoard[emptyRow, column] = newInstant;

                    newInstant.GetComponent<TileScript>().Assign(newInstant, emptyRow, column); 
                }
            }
        }
    }

    void RemoveFromBoard(GameObject tile)
    {
        tile.GetComponent<SpriteRenderer>().sprite = null;
    }

    void Swap(GameObject firstSelected, GameObject secondSelected)
    {
        //Swap the object in array
        var tempObj = gameBoard[firstSelected.GetComponent<TileScript>().Row, firstSelected.GetComponent<TileScript>().Column];
        gameBoard[firstSelected.GetComponent<TileScript>().Row, firstSelected.GetComponent<TileScript>().Column] = gameBoard[secondSelected.GetComponent<TileScript>().Row, secondSelected.GetComponent<TileScript>().Column];
        gameBoard[secondSelected.GetComponent<TileScript>().Row, secondSelected.GetComponent<TileScript>().Column] = tempObj;

        //Swap the position on the board
        var tempPosition = new Vector2(firstSelected.transform.position.x, firstSelected.transform.position.y);
        firstSelected.transform.position = new Vector2(secondSelected.transform.position.x, secondSelected.transform.position.y);
        secondSelected.transform.position = tempPosition;

        //swap the position of the row
        var tempRow = firstSelected.GetComponent<TileScript>().Row;
        firstSelected.GetComponent<TileScript>().Row = secondSelected.GetComponent<TileScript>().Row;
        secondSelected.GetComponent<TileScript>().Row = tempRow;

        //swap the position of the column
        var tempColumn = firstSelected.GetComponent<TileScript>().Column;
        firstSelected.GetComponent<TileScript>().Column = secondSelected.GetComponent<TileScript>().Column;
        secondSelected.GetComponent<TileScript>().Column = tempColumn;

    }

    List<GameObject> HorizontalMatches(GameObject tile)
    {
        List<GameObject> availableMatches = new List<GameObject>();
        availableMatches.Add(tile);

        //Left
        if(tile.GetComponent<TileScript>().Column != 0)
        {
            for(int column = tile.GetComponent<TileScript>().Column  - 1; column >= 0; column--)
            {
                if (Constants.IsObjTheSame(tile, gameBoard[tile.GetComponent<TileScript>().Row, column]))
                {
                    availableMatches.Add(gameBoard[tile.GetComponent<TileScript>().Row, column]);

                }
                else
                    break;
            }
        }

        //Right
        if (tile.GetComponent<TileScript>().Column != Constants.numberOfColumns - 1)
        {
            for (int column = tile.GetComponent<TileScript>().Column + 1; column < Constants.numberOfColumns; column++)
            {
                if (Constants.IsObjTheSame(tile, gameBoard[tile.GetComponent<TileScript>().Row, column]))
                {
                    availableMatches.Add(gameBoard[tile.GetComponent<TileScript>().Row, column]);

                }
                else
                    break;
            }
        }

        if(availableMatches.Count < Constants.minimumMatch)
        {
            availableMatches.Clear();
        }

        return availableMatches;
    }

    List<GameObject> VerticalMatches(GameObject tile)
    {
        List<GameObject> availableMatches = new List<GameObject>();
        availableMatches.Add(tile);

        //Down
        if (tile.GetComponent<TileScript>().Row != 0)
        {
            for (int row = tile.GetComponent<TileScript>().Row - 1; row >= 0; row--)
            {
                if (Constants.IsObjTheSame(tile, gameBoard[row, tile.GetComponent<TileScript>().Column]))
                {
                    availableMatches.Add(gameBoard[row, tile.GetComponent<TileScript>().Column]);

                }
                else
                    break;
            }
        }

        //Up
        if (tile.GetComponent<TileScript>().Row != Constants.numberOfRows - 1)
        {
            for (int row = tile.GetComponent<TileScript>().Row + 1; row < Constants.numberOfRows; row++)
            {
                if (Constants.IsObjTheSame(tile, gameBoard[row, tile.GetComponent<TileScript>().Column]))
                {
                    availableMatches.Add(gameBoard[row, tile.GetComponent<TileScript>().Column]);

                }
                else
                    break;
            }
        }

        if (availableMatches.Count < Constants.minimumMatch)
        {
            availableMatches.Clear();
        }

        return availableMatches;
    }
}
