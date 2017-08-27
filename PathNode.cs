using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : IComparable<PathNode> {

    bool explored;
    int distance;
    int x;
    int y;
    PathNode parent;

    // constructor
    public PathNode (int x, int y, int distance = 0, PathNode parent = null) {
        this.x = x;
        this.y = y;
        this.distance = distance;
        this.parent = parent;
        explored = false;
    }

    public bool Explored {
        get { return explored; }
        set { explored = value; }
    }

    public int Distance {
        get { return distance; }
        set { distance = value; }
    }

    public int Y {
        get { return y; }
        set { y = value; }
    }

    public int X {
        get { return x; }
        set { x = value; }
    }

    public PathNode Parent {
        get { return parent; }
        set { parent = value; }
    }

    public int CompareTo(PathNode compareNode)
    {
        if (compareNode == null) {
            return 1;
        } else {
            return Distance.CompareTo(compareNode.Distance);
        }
    }
}
