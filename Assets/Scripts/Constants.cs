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
* Heading: This script is for each of the constants that are used in the game 
* production
* Author: Joy Ajayi
* Online Repository: https://github.com/AjayiJoy/Ultra-class-Match-3-
* File format: Assembly-CSharp (.cs)
 */
using UnityEngine;

public static class Constants 
{
    public static readonly int numberOfRows = 4;
    public static readonly int numberOfColumns = 5;
    public static readonly int minimumMatch = 3;
    public static readonly float tileDistance = 1.5f;
    public static Color tileSelected = new Color(0, 0, 0, 1.0f);
    public static Color tileDeselected = Color.white;
    public static readonly int scoreFor3 = 10;
    public static readonly int scoreFor4 = 20;

  public static bool IsObjSharingRowAndColumn(GameObject s1, GameObject s2)
  {
        return (s1.GetComponent<TileScript>().Column == s2.GetComponent<TileScript>().Column || s1.GetComponent<TileScript>().Row == s2.GetComponent<TileScript>().Row)
              && Mathf.Abs(s1.GetComponent<TileScript>().Column - s2.GetComponent<TileScript>().Column) <= 1
              && Mathf.Abs(s1.GetComponent<TileScript>().Row - s2.GetComponent<TileScript>().Row) <= 1;
  }

   public static bool IsObjTheSame(GameObject s1, GameObject s2)
    {
        return string.Compare(s1.tag, s2.tag) == 0; 
    }

    public enum GameState
    {
        None,
        SelectionStarted,
        Swapping
    }
}
