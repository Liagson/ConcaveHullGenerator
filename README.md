# Concave hull generator
Unite all dots under the smallest possible area
![IMG1](https://github.com/Liagson/ConcaveHullGenerator/blob/master/Pics/Concavity.png)
## How it works
Inspired by *[Implementation of a fast and efficient concave hull algorith](http://www.it.uu.se/edu/course/homepage/projektTDB/ht13/project10/Project-10-report.pdf)*, the concave hull is reached through the finding of middlepoints within the edges of the convex hull. If the cosine between the middlepoint and the edge nodes is greater than the `concavity`variable the segment will be partitioned in two new ones using the new point. The search for middlepoints starts from the longest edge to the shortest. If no middlepoints are found the program will end.
![IMG2](https://raw.githubusercontent.com/Liagson/ConcaveHullGenerator/master/Pics/Steps.png)
Middlepoints are searched in an area around the edge that can be set using the `scaleFactor` variable. The higher the area the slower the program will be.
