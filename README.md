# Poisson-disc-unity
## Description
This is an implementation of the Poisson-disc sampling algorithm written within the Unity game engine in the C# language. This algorithm produces a List of Vector2 objects each of which indicate an individual point randomly placed within 2D space while also having the requirement that each point must be at least a minimum distance away from every other point. This algorithm efficiently generates each point by separating the 2D plane into a grid of cells and only checks cells adjacent to the one in which a point is currently being generated, therefore maximizing the speed of generation. 

### How to use
This algorithm is contained in a single class called ***PoissonDiskSampling*** within the ***Algorithms.Poisson*** namespace. To access the class, all you have to do is import ***PoissonSampling.cs*** into your unity project, and in any file where you want to access the class, add "***using Algorithms.Poisson;***" at the top of the file in which you are trying to access the class in. 

The class only has a single public function in order to make it very easy to use. In order to return the List of points, all you must do is call the ***Samples()*** function while passing the width, height, and minimum distance between points in that order to the function. The function will then return a List object of Vector2 objects each of which indicate a single point located in 2D space starting from (0, 0) and ending at the width and height provided.

**NOTE:** The algorithm works in such a way that the grid of points are broken up into cells, and each cell contains a single point. Every cell is initialized with a default Vector2(-1, -1) object, which indicates a cell that does not have a valid point. Not every cell will contain a valid point due to the minimum distance requirement, so when using the points in this function for any purpose, *make sure to verify the point you are attempting to use is not a Vector(-1, -1) object or you will be attempting to use an invalid point*

## Purpose
The primary purpose of this algorithm when designing it is as a prerequisite step for the generation of voronoi diagrams or barycentric dual meshes via Delaunay triangulation, however any project that requires randomly generated points can benefit from this algorithm as it is a fast algorithm that produces points in such a way that is space efficient and does not result in overlapping points. 

## Examples
![example1](https://github.com/imnota4/Poisson-disc-unity/assets/4397050/6c181836-4894-47d9-b16f-5a5109dfb5ce)
![example2](https://github.com/imnota4/Poisson-disc-unity/assets/4397050/260c51e8-7440-4740-a0c1-e2efb20f69fe)

$~~~~~~~~~~~~~~~~~~~~~~~~~$*A 10x10 grid with 2 unit spacing* $~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~$*A 10x10 grid with 1 unit spacing*
