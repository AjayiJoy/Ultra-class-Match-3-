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
* Heading: This script assigns row, column and game object to each sprite in the 
* multidimensional array
* Author: Joy Ajayi
* Online Repository: https://github.com/AjayiJoy/Ultra-class-Match-3-
* File format: Assembly-CSharp (.cs)
 */
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public int Row { get; set; }
    public int Column { get; set; }
    public GameObject Tile { get; set;}

    public void Assign(GameObject tile, int row, int column)
    {
        Tile = tile;
        Row = row;
        Column = column;
    }
}
