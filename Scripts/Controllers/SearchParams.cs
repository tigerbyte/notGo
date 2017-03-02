using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class searchParams
{


    public Tile StartLocation { get; set; }
    public Tile EndLocaton { get; set; }


    public bool[,] Map { get; set; }
    public searchParams(Tile startLocation, Tile endLocation, bool[,] map)
    {
        this.StartLocation = startLocation;
        this.EndLocaton = endLocation;
        this.Map = map;
    }

}