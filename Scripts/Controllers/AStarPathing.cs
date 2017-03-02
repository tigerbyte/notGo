using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathing {

    public class Node {
        private Node parentNode; // parent of current node
        public Tile location { get; private set; } // location of current node in tile form
        public bool isOwned { get; private set; } // flag for if tile is owned
        public float g { get; private set; } // code from start of path to this point 
		public float h { get; private set; } // estimated cost from this point to end ( draws a straight line)
        public enum state {  // state of Node used when checking path to make sure that you do not redo nodes
            untested,
            open,
            closed
       }

        public state State { get; set; }
        public float f { // estimated full cost
            get { return this.g + this.h; }
        }

        public Node ParentNode {  
            get { return this.parentNode; }
            set{
                this.parentNode = value;
				this.g = this.parentNode.g + GetCost(this.location, parentNode.location);  // also calculate G (cost from start of path to here)
            }
        }
        
        //instantiate all values
        public Node(int x, int y, bool isOwned, Tile endLocation) {
            this.location = new Tile(x, y);
            this.isOwned = isOwned;
            this.h = GetCost(this.location, endLocation);
            this.g = 0; // G value is updated when parent node is set
        }
		// gets the cost distance between two points
        internal static float GetCost(Tile location,Tile secondLocation) {
            float deltaX = secondLocation.X - location.X;
            float deltaY = secondLocation.Y - location.Y;
            return (float)Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }

    private int width; //width of map
    private int height; //height of map
    private Node[,] nodes; 
    private Node startNode; // node to start pathfinding from
    private Node endNode; // node to find where the end of path resides
	private searchParams sParam; // parameters of search ( start node, end node and map)
	 
	//instantiate values
    public AStarPathing(searchParams sParam) {
        this.sParam = sParam;
        initNodes(sParam.Map);
        this.startNode = this.nodes[sParam.StartLocation.X, sParam.StartLocation.Y];
        this.startNode.State = Node.state.open;
        this.endNode = this.nodes[sParam.EndLocaton.X, sParam.EndLocaton.Y];
    }


	// calls the search function, if it finds a path to the end node it returns a list of all nodes to get to path. it needs to be reversed at the end because path is 
	// inverted
    public List<Tile> FindPath() {
        List<Tile> path = new List<Tile>();
        bool success = Search(startNode);  // returns true if path is found
        if (success) {
            Node node = this.endNode; //adds all nodes in the successfull path to the list
            while (node.ParentNode != null) {
                path.Add(node.location);
                node = node.ParentNode;
            }
            //reverse list so it is in correct order
            path.Reverse();
        }
        return path; //returns empty list if no path is available
    }

    private void initNodes(bool[,] map){
        this.width = map.GetLength(0); // gets size of the boolean map in the x axis
        this.height = map.GetLength(1); // gets size of the boolean map in the y axis
        this.nodes = new Node[this.width, this.height];

        // initializes the nodes 
        for (int y = 0; y < this.width; y++) {
            for (int x = 0; x < this.height; x++) {
                this.nodes[x, y] = new Node(x, y, map[x, y],this.sParam.EndLocaton);
            }
        }
    }

	// finds all nodes that are adjacent to this node that are valid
    private List<Node> getAdjacentValidNodes(Node fromNode) {

        List<Node> validNodes = new List<Node>();
        IEnumerable<Tile> nextLocations = GetAdjacentLocations(fromNode.location.X, fromNode.location.Y); // finds the locations of the up/down/left/right nodes and places them into array

        foreach (var location in nextLocations) {
            int x = location.X;
            int y = location.Y;
			if (x < 0 || x >= this.width || y < 0 || y >= this.height) { // check if the node is within dimensions boundary 
                continue;// keyword similar to break skips current check unlike break it doesnt exit loop
            }

            Node node = this.nodes[x, y];
            if (!node.isOwned) // if the node is not owned then skip over this node
                continue;

			if (node.State == Node.state.closed) // if node previously visited and deemed not viable ignore node 
				continue;

            // if the node is already opened then it is accessed via another route
            // it is only added if it is a lower cost via this route
            if (node.State == Node.state.open) {
                float cost = Node.GetCost(node.location, node.ParentNode.location);
                float gTemp = fromNode.g + cost;
                if (gTemp < node.g) {
                    node.ParentNode = fromNode;
                    validNodes.Add(node);
				} else {
	            // if it is untested then iterate and flag it as open
	                node.ParentNode = fromNode;
	                node.State = Node.state.open;
	                validNodes.Add(node);
	            }
	        }
   		}
		return validNodes;
	}

    private static IEnumerable<Tile> GetAdjacentLocations(int x, int y) {
        //returns an IEnumerable of all adjacent tiles , checks up down left right does not check diagonals
        return new Tile[] {
                new Tile(x-1,y),
                new Tile(x,y-1),
                new Tile(x,y+1),
                new Tile(x+1,y)
            };
    }

    private bool Search(Node currentNode) {
        currentNode.State = Node.state.closed;  // set node to closed so you do not go over this path again
        List<Node> nextNodes = getAdjacentValidNodes(currentNode); // find neighboring nodes

        nextNodes.Sort((node1,node2) => node1.f.CompareTo(node2.f)); // sorts nodes according to their F value lowest first

        foreach (var nextNode in nextNodes) {
			// check if goal reached
            if (nextNode.location == this.endNode.location) {
                return true;
            }
            else {
				// if not reached check next set of nodes
                if (Search(nextNode))   
                    return true;
       		}
        }
		// if no path is found return false
        return false;
    }
}
