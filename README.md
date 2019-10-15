# Deprecated documentation: Incoming updates coming soon!

# Concave hull generator
Unite all dots under the smallest possible area. It comes with a simple demo for unity using just gizmos.

## How it works
The program has only two easy steps:
* Set a convex hull from a set of points (In the program they are the `Node` objects). It is very important that they all have an unique `id` value. The hull should be a list of `Line` objects (each line is defined by two `Node` objects). If you already have a set of lines you can skip the step (They don't need to form a convex hull)
* Set a concave hull from a list of `Line` objects.

You only need to set two constants before running the program:
* **concavity** : This sets how sharp you want the concave angles to be. It goes from `-1` (not concave at all. in fact, the hull will be left convex) up to `+1` (**very** sharp angles can occur. Setting concavity to `+1` might result in 0º angles!) `concavity` is defined as the cosine of the concave angles.
* **scaleFactor** : This sets how big is the area where concavities are going to be searched. The bigger, the more sharp the angles can be. Setting it to too high value might affect the performance of the program. This value should be relative to how far away the points to be conected are.

For more info you can check the `Init.cs` file and follow the demo :)

![IMG1](https://github.com/Liagson/ConcaveHullGenerator/blob/master/Pics/Concavity.png)
## Algorithm
Inspired by *[Implementation of a fast and efficient concave hull algorith](http://www.it.uu.se/edu/course/homepage/projektTDB/ht13/project10/Project-10-report.pdf)*, the concave hull is reached through the finding of middlepoints within the edges of the convex hull. If the cosine between the middlepoint and the edge nodes is greater than the `concavity`variable the segment will be partitioned in two new ones using the new point. The search for middlepoints starts from the longest edge to the shortest. If no middlepoints are found the program will end.

![IMG2](https://raw.githubusercontent.com/Liagson/ConcaveHullGenerator/master/Pics/Steps.png)
Middlepoints are searched in an area around the edge that can be set using the `scaleFactor` variable. The higher the area the slower the program will be.
