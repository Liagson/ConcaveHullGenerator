using ConcaveHull;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.src
{
    public static class HullFunctions
    { 
        public static List<Line> getDividedLine(Line line, List<Node> nearbyPoints, List<Line> concave_hull, double concavity) {
            // returns two lines if a valid middlePoint is found
            // returns empty list if the line can't be divided
            List<Line> dividedLine = new List<Line>();
            List<Node> okMiddlePoints = new List<Node>();
            foreach(Node middlePoint in nearbyPoints) {
                double _cos = getCos(line.nodes[0], line.nodes[1], middlePoint);
                if (_cos < concavity ) {
                    Line newLineA = new Line(line.nodes[0], middlePoint);
                    Line newLineB = new Line(middlePoint, line.nodes[1]);
                    if (!lineCollidesWithHull(newLineA, concave_hull) && !lineCollidesWithHull(newLineB, concave_hull)) {
                        middlePoint.cos = _cos;
                        okMiddlePoints.Add(middlePoint);
                    }
                }
            }
            if (okMiddlePoints.Count > 0) {
                // We want the middlepoint to be the one with the widest angle (smallest cosine)
                okMiddlePoints = okMiddlePoints.OrderBy(p => p.cos).ToList();
                dividedLine.Add(new Line(line.nodes[0], okMiddlePoints[0]));
                dividedLine.Add(new Line(okMiddlePoints[0], line.nodes[1]));
            }
            return dividedLine;
        }

        public static bool lineCollidesWithHull(Line line, List<Line> concave_hull) {
            foreach(Line hullLine in concave_hull) {
                // We don't want to check a collision with this point that forms the hull AND the line
                if (line.nodes[0].id != hullLine.nodes[0].id && line.nodes[0].id != hullLine.nodes[1].id
                    && line.nodes[1].id != hullLine.nodes[0].id && line.nodes[1].id != hullLine.nodes[1].id) {
                    // Avoid line interesections with the rest of the hull
                    if (LineIntersectionFunctions.doIntersect(line.nodes[0], line.nodes[1], hullLine.nodes[0], hullLine.nodes[1]))
                        return true;
                }  
            }
            return false;
        }

        private static double getCos(Node A, Node B, Node O) {
            /* Law of cosines */
            double aPow2 = Math.Pow(A.x - O.x, 2) + Math.Pow(A.y - O.y, 2);
            double bPow2 = Math.Pow(B.x - O.x, 2) + Math.Pow(B.y - O.y, 2);
            double cPow2 = Math.Pow(A.x - B.x, 2) + Math.Pow(A.y - B.y, 2);
            double cos = (aPow2 + bPow2 - cPow2) / (2 * Math.Sqrt(aPow2 * bPow2));
            return Math.Round(cos, 4);
        }

        public static List<Node> getNearbyPoints(Line line, List<Node> nodeList, double scaleFactor) {
            /* The bigger the scaleFactor the more points it will return
             * We calculate an ellipse arround the nodes that define the line (the focus points of said ellipse)
             * We will add all nodes contained within the base ellipse scaled to the scaleFactor
             * Be carefull: if it's too small it will return very little points (or non!), 
             * if it's too big it will add points that will not be used and will consume time
             * */
            List<Node> nearbyPoints = new List<Node>();
            double lineLength = line.getLength();
            double baseEllipseFocusSum = 2 * lineLength / Math.Sqrt(2);
            double scaledBaseEllipseFocusSum = baseEllipseFocusSum * scaleFactor;

            foreach(Node node in nodeList)
            {
                double distanceToFocusA = Math.Sqrt(Math.Pow(line.nodes[0].x - node.x, 2) + Math.Pow(line.nodes[0].y - node.y, 2));
                double distanceToFocusB = Math.Sqrt(Math.Pow(line.nodes[1].x - node.x, 2) + Math.Pow(line.nodes[1].y - node.y, 2));
                double ellipseFocusSum = distanceToFocusA + distanceToFocusB;
                if(ellipseFocusSum <= scaledBaseEllipseFocusSum)
                {
                    nearbyPoints.Add(node);
                }
            }
            
            return nearbyPoints;
        }
    }
}

