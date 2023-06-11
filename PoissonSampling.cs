using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Threading;
using System;
using Random = UnityEngine.Random;

namespace Algorithms.Poisson
{
    public class PoissonDiskSampling
    {

        private float cellSize;
        private int cols;
        private int rows;
        private List<Vector2> grid;
        private List<Vector2> active;
        private float width;
        private float height;
        private float timeOut;
        private float minDist;


        private void createGrid()
        {
            grid = new List<Vector2>(); // initialize 1D List that stores point data for an N dimensional grid of points
            active = new List<Vector2>(); // initialize List of valid points that can be used to generate new points
            cellSize = minDist / Mathf.Sqrt(2); // Determine size of individual grid cells in 2D space
            cols = Mathf.FloorToInt(width / cellSize); // determine amount of columns in the grid
            rows = Mathf.FloorToInt(height / cellSize); // determine the amount of rows in the grid

            for (int i = 0; i < cols * rows; i++)
            {
                grid.Insert(i, new Vector2(-1, -1));  // default value of each cell
            }
        } // functions as intended

        private void generateSeed()
        {
            float seedXPos = Random.Range(0, width-1);
            float seedYPos = Random.Range(0, height-1);
            // Converts X and Y coordinate values into an index
            int rowIndex = Mathf.FloorToInt(seedXPos / cellSize);
            int colIndex = Mathf.FloorToInt(seedYPos / cellSize);
            Vector2 point = new Vector2(seedXPos, seedYPos); // Vector2 object that represents the point in 2D space based on the initially generated coordinates
            grid[rowIndex + (colIndex * cols)] = point;  // saves seed point to dataset based on values generated when converting coordinates into intengers
            active.Add(point); // Save generated seed point to list of currently used points.
            Debug.Log("SEED POINT GENERATED AT: (" + active[0].x + ", " + active[0].y + ") LOCATED AT INDEX " + (rowIndex + (colIndex * cols)));
        }

        private bool checkAdjacentCells(Vector2 newPoint)
        {
            int pointColIndex = Mathf.FloorToInt(newPoint.x / cellSize);
            int pointRowIndex = Mathf.FloorToInt(newPoint.y / cellSize);

            Debug.Log("CHECKING ADJACENT POINTS...");
            // are adjacent points valid?
            for (int col = -1; col <= 1; col++)
            {
                for (int row = -1; row <= 1; row++)
                {
                    
                    int adjIndex = (pointColIndex + col) + ((pointRowIndex + row) * cols);
                    Debug.Log("ATTEMPTING TO INDEX CELL AT: " + adjIndex);
                    if (adjIndex > grid.Count - 1 || adjIndex < 0)
                    {
                        Debug.LogWarning("Adjacent cell " + adjIndex + " does not exist. checking next cell");
                        continue;
                    }

                    Debug.Log("VALID CELL DETECTED AT INDEX " + adjIndex +". CHECKING FOR INITIALIZATION...");
                    Vector2 adjPoint = grid[adjIndex]; // Retreive point adjacent to newly created point

                    if (adjPoint == new Vector2(-1, -1)) 
                    {
                        Debug.LogWarning("Adjacent cell it uninitialized. checking next cell");
                        continue;
                    }
                    if (adjPoint == newPoint) 
                    {
                        Debug.LogWarning("Adjacent point and new point are the same. checking next cell");
                        continue;
                    }

                    Debug.Log("DETECTED INITALIZED ADJACENT CELL. CELL CONTAINS POINT LOCATED AT (" + adjPoint.x + ", " + adjPoint.y + "). VERIFYING DISTANCE...");
                    // Checks distance from the newly created point and the point adjacent to it and checks if the new point is within the minimum distance required between points
                    //float distance = Mathf.Sqrt(Mathf.Pow(adjPoint.x - newPoint.x, 2) + Mathf.Pow(adjPoint.y - newPoint.y, 2)); // Pythagorean theorem ftw
                    float distance = Vector2.Distance(adjPoint, newPoint);
                    Debug.Log("DISTANCE BETWEEN POINTS IS: " + distance + ". A MINIMUM OF " + minDist + " IS EXPECTED");
                    if (distance < minDist)
                    {
                        return false; // invalid point
                    }

                    Debug.Log("ADJACENT POINT IS IS AT A VALID DISTANCE. CHECKING OTHER ADJACENT CELLS");
                }
            }
            // adjacent points are valid
            return true;
        }

        private bool generatePoint(Vector2 activePoint)
        {
            for (int i = 0; i < timeOut; i++)
            {
                Debug.LogWarning("Iterations until timout: " + (timeOut - i));

                // create a new random point within a circular region around the selected active point
                float angle = Random.Range(-2 * Mathf.PI, 2 * Mathf.PI);
                Vector2 newPoint = new Vector2(activePoint.x + minDist*Mathf.Sin(angle), activePoint.y + minDist*Mathf.Cos(angle));
                Debug.Log("NEW POINT GENERATED AT (" + newPoint.x + ", " + newPoint.y + ")");

                if (newPoint.x < 0 || newPoint.y < 0 || newPoint.x > width-1 || newPoint.y > height-1)
                {
                    Debug.LogError("New point coordinates are out of bounds. Generating new point.");
                    continue; 
                }

                bool valid = checkAdjacentCells(newPoint); 

                // if newly generated point is at least at the minimum required distance from all adjacent points
                if (valid)
                {
                    int pointColIndex = Mathf.FloorToInt(newPoint.x / cellSize);
                    int pointRowIndex = Mathf.FloorToInt(newPoint.y / cellSize);
                    Debug.Log("(" + newPoint.x + ", " + newPoint.y + ") IS A VALID NEW POINT. ADDING IT TO LIST OF SAMPLE AND ACTIVE POINTS");
                    Debug.LogWarning("New list index: " + (pointColIndex + (pointRowIndex * cols)));
                    Debug.Log(pointColIndex);
                    Debug.Log(pointRowIndex);
                    grid[pointColIndex + (pointRowIndex * cols)] = newPoint; // save newly generated point to samples List
                    active.Add(newPoint); // add newly generated point to list of active points
                    return true;
                }
                Debug.Log("(" + newPoint.x + ", " + newPoint.y + ") IS An INVALID NEW POINT. GENERATING RANDOM NEW POINT...");
                
            }
            return false;
        }
        
        private List<Vector2> generatePoints()
        {

            while (active.Count > 0)
            {
                int randIndex = Mathf.FloorToInt(Random.Range(0, active.Count)); // generate a random index from the list of active points and save that point to "pos"
                Vector2 activePoint = active[randIndex];
                Debug.Log("RANDOM ACTIVE POINT SELECTED: (" + activePoint.x + ", " + activePoint.y + ") AT INDEX " + randIndex);
                bool stillActive = false;
                stillActive = generatePoint(activePoint);
                
                // if a valid point could not be generated from the currently selected active point then remove that active point
                if (!stillActive)
                {
                    Debug.LogWarning("Unable to generate a new point from selected active point. Removing it from list of active points");
                    active.RemoveAt(randIndex);
                }
            }
            return grid;
        }

        public List<Vector2> Samples(float width, float height, float minDist, float timeOut = 30f) 
        {
            this.width = width;
            this.height = height;
            this.timeOut = timeOut;
            this.minDist = minDist;
            // Step 1: Produce a 1D list representing a grid of arbitrary dimensions. "cols" and "rows" are integer values representing the amount of cells in each
            Debug.Log("STARTING STEP 1");
            createGrid();
            Debug.Log("INITIALIZED GRID OF SIZE: " + grid.Count);

            // Step 2: Create an initial "seed" point in the grid to start the algorithm with
            Debug.Log("STARTING STEP 2");
            Debug.Log("GENERATING SEED POINT");
            generateSeed();


            return generatePoints();
            
        }
    }
}