using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Threading;
using System;
using Random = UnityEngine.Random;
using Geometry;

namespace Algorithms.Poisson
{
    public class PoissonDiskSampling
    {

        private float cellSize;
        private int cols;
        private int rows;
        private List<Point> grid;
        private List<Point> active;
        private float width;
        private float height;
        private float timeOut;
        private float minDist;


        private void createGrid()
        {
            grid = new List<Point>(); // initialize 1D List that stores point data for an N dimensional grid of points
            active = new List<Point>(); // initialize List of valid points that can be used to generate new points
            cellSize = minDist / Mathf.Sqrt(2); // Determine size of individual grid cells in 2D space
            cols = Mathf.FloorToInt(width / cellSize); // determine amount of columns in the grid
            rows = Mathf.FloorToInt(height / cellSize); // determine the amount of rows in the grid

            for (int i = 0; i < cols * rows; i++)
            {
                grid.Insert(i, new Point(-1, -1, 0));  // default value of each cell
            }
        } // functions as intended

        private void generateSeed()
        {
            float seedXPos = 0;
            float seedYPos = 0;
            int rowIndex = 0;
            int colIndex = 0;

            do
            {
                seedXPos = Random.Range(1, width - 1);
                seedYPos = Random.Range(1, height - 1);
                // Converts X and Y coordinate values into an index
                rowIndex = Mathf.FloorToInt(seedXPos / cellSize);
                colIndex = Mathf.FloorToInt(seedYPos / cellSize);
            } while (rowIndex + (colIndex * cols) >= grid.Count);

            Point point = new Point(seedXPos, seedYPos); // Vector2 object that represents the point in 2D space based on the initially generated coordinates
            grid[rowIndex + (colIndex * cols)] = point;  // saves seed point to dataset based on values generated when converting coordinates into intengers
            active.Add(point); // Save generated seed point to list of currently used points.
        }

        private bool checkAdjacentCells(Point newPoint)
        {
            int pointColIndex = Mathf.FloorToInt(newPoint.x / cellSize);
            int pointRowIndex = Mathf.FloorToInt(newPoint.y / cellSize);

            // are adjacent points valid?
            for (int col = -1; col <= 1; col++)
            {
                for (int row = -1; row <= 1; row++)
                {
                    
                    int adjIndex = (pointColIndex + col) + ((pointRowIndex + row) * cols);
                    if (adjIndex > grid.Count - 1 || adjIndex < 0)
                    {
                        continue;
                    }

                    Point adjPoint = grid[adjIndex]; // Retreive point adjacent to newly created point

                    if (adjPoint.toVector() == new Vector3(-1, -1, 0)) 
                    {
                        continue;
                    }
                    if (adjPoint == newPoint) 
                    {
                        continue;
                    }

                    // Checks distance from the newly created point and the point adjacent to it and checks if the new point is within the minimum distance required between points
                    //float distance = Mathf.Sqrt(Mathf.Pow(adjPoint.x - newPoint.x, 2) + Mathf.Pow(adjPoint.y - newPoint.y, 2)); // Pythagorean theorem ftw
                    float distance = Vector2.Distance(adjPoint.toVector(), newPoint.toVector());
                    if (distance < minDist)
                    {
                        return false; // invalid point
                    }

                }
            }
            // adjacent points are valid
            return true;
        }

        private bool generatePoint(Point activePoint)
        {
            for (int i = 0; i < timeOut; i++)
            {

                // create a new random point within a circular region around the selected active point
                float angle = Random.Range(-2 * Mathf.PI, 2 * Mathf.PI);
                Point newPoint = new Point(activePoint.x + minDist*Mathf.Sin(angle), activePoint.y + minDist*Mathf.Cos(angle));

                if (newPoint.x < 1 || newPoint.y < 1 || newPoint.x > width-1 || newPoint.y > height-1)
                {
                    continue; 
                }

                bool valid = checkAdjacentCells(newPoint); 

                // if newly generated point is at least at the minimum required distance from all adjacent points
                if (valid && (Mathf.FloorToInt(newPoint.x / cellSize) + (Mathf.FloorToInt(newPoint.y / cellSize) * cols) < grid.Count))
                {
                    int pointColIndex = Mathf.FloorToInt(newPoint.x / cellSize);
                    int pointRowIndex = Mathf.FloorToInt(newPoint.y / cellSize);
                    grid[pointColIndex + (pointRowIndex * cols)] = newPoint; // save newly generated point to samples List
                    active.Add(newPoint); // add newly generated point to list of active points
                    return true;
                }
                
            }
            return false;
        }
        
        private List<Point> generatePoints()
        {

            while (active.Count > 0)
            {
                int randIndex = Mathf.FloorToInt(Random.Range(0, active.Count)); // generate a random index from the list of active points and save that point to "pos"
                Point activePoint = active[randIndex];
                bool stillActive = false;
                stillActive = generatePoint(activePoint);
                
                // if a valid point could not be generated from the currently selected active point then remove that active point
                if (!stillActive)
                {
                    active.RemoveAt(randIndex);
                }
            }
            return grid;
        }

        public List<Point> Samples(float width, float height, float minDist, float timeOut = 30f) 
        {
            this.width = width;
            this.height = height;
            this.timeOut = timeOut;
            this.minDist = minDist;
            // Step 1: Produce a 1D list representing a grid of arbitrary dimensions. "cols" and "rows" are integer values representing the amount of cells in each
            createGrid();

            // Step 2: Create an initial "seed" point in the grid to start the algorithm with
            generateSeed();


            return generatePoints();
            
        }
    }
}