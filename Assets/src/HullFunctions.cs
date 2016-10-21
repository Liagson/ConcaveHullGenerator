using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcaveHull {
    public class HullFunctions {

        public static bool verticalIntersection(Line lineA, Line lineB) {
            /* lineA is vertical */
            double y_intersection;
            if ((lineB.nodes[0].x > lineA.nodes[0].x) && (lineA.nodes[0].x > lineB.nodes[1].x) ||
                    ((lineB.nodes[1].x > lineA.nodes[0].x) && (lineA.nodes[0].x > lineB.nodes[0].x))) {
                y_intersection = (((lineB.nodes[1].y - lineB.nodes[0].y) * (lineA.nodes[0].x - lineB.nodes[0].x)) / (lineB.nodes[1].x - lineB.nodes[0].x)) + lineB.nodes[0].y;
                return ((lineA.nodes[0].y > y_intersection) && (y_intersection > lineA.nodes[1].y))
                    || ((lineA.nodes[1].y > y_intersection) && (y_intersection > lineA.nodes[0].y));
            } else {
                return false;
            }
        }

        public static bool intersection(Line lineA, Line lineB) {
            /* Returns true if segments collide
             * If they have in common a segment edge returns false
             * Algorithm obtained from: 
             * http://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect
             * Thanks OMG_peanuts !
             * */
            double dif;
            double A1, A2;
            double b1, b2;
            decimal X;
            
            if (Math.Max(lineA.nodes[0].x, lineA.nodes[1].x) < Math.Min(lineB.nodes[0].x, lineB.nodes[1].x)) {
                return false; //Not a chance of intersection
            }

            dif = lineA.nodes[0].x - lineA.nodes[1].x;
            if (dif != 0) { //Avoids dividing by 0
                A1 = (lineA.nodes[0].y - lineA.nodes[1].y) / dif;
            } else {
                //Segment is vertical
                A1 = 9999999;
            }

            dif = lineB.nodes[0].x - lineB.nodes[1].x;
            if (dif != 0) { //Avoids dividing by 0
                A2 = (lineB.nodes[0].y - lineB.nodes[1].y) / dif;
            } else {
                //Segment is vertical
                A2 = 9999999;
            }

            if (A1 == A2) {
                return false; //Parallel
            }else if(A1 == 9999999) {
                return verticalIntersection(lineA, lineB);
            } else if(A2 == 9999999) {
                return verticalIntersection(lineB, lineA);
            }

            b1 = lineA.nodes[0].y - (A1 * lineA.nodes[0].x);
            b2 = lineB.nodes[0].y - (A2 * lineB.nodes[0].x);
            X = Math.Round(System.Convert.ToDecimal((b2 - b1) / (A1 - A2)), 4);
            if ((X <= System.Convert.ToDecimal(Math.Max(Math.Min(lineA.nodes[0].x, lineA.nodes[1].x), Math.Min(lineB.nodes[0].x, lineB.nodes[1].x)))) ||
                (X >= System.Convert.ToDecimal(Math.Min(Math.Max(lineA.nodes[0].x, lineA.nodes[1].x), Math.Max(lineB.nodes[0].x, lineB.nodes[1].x))))) {
                return false; //Out of bound
            } else {
                return true;
            }
        }
                
        public static List<Line> setConcave(Line line, List<Node> nearbyPoints, List<Line> concave_hull, decimal concavity, bool isSquareGrid) {
            /* Adds a middlepoint to a line (if there can be one) to make it concave */
            List<Line> concave = new List<Line>();
            decimal cos1, cos2;
            decimal sumCos = -2;            
            Node middle_point = null;
            bool edgeIntersects;
            int count = 0;
            int count_line = 0;
            
            while (count < nearbyPoints.Count) {
                edgeIntersects = false;
                cos1 = getCos(nearbyPoints[count], line.nodes[0], line.nodes[1]);
                cos2 = getCos(nearbyPoints[count], line.nodes[1], line.nodes[0]);
                if (cos1 + cos2 >= sumCos && (cos1 > concavity && cos2 > concavity)) {
                    count_line = 0;
                    while (!edgeIntersects && count_line < concave_hull.Count) {
                        edgeIntersects = (intersection(concave_hull[count_line], new Line(nearbyPoints[count], line.nodes[0]))
                            || (intersection(concave_hull[count_line], new Line(nearbyPoints[count], line.nodes[1]))));
                        count_line++;
                    }
                    if (!edgeIntersects) {
                        // Prevents from getting sharp angles between middlepoints
                        Node[] nearNodes = getHullNearbyNodes(line, concave_hull);
                        if ((getCos(nearbyPoints[count], nearNodes[0], line.nodes[0]) < -concavity) &&
                            (getCos(nearbyPoints[count], nearNodes[1], line.nodes[1]) < -concavity)) {
                            // Prevents inner tangent lines to the concave hull
                            if (!(tangentToHull(line, nearbyPoints[count], cos1, cos2, concave_hull) && isSquareGrid)) {
                                sumCos = cos1 + cos2;
                                middle_point = nearbyPoints[count];
                            }
                        }
                    }
                }
                count++;
            }
            if (middle_point == null) {
                concave.Add(line);
            } else {
                concave.Add(new Line(middle_point, line.nodes[0]));
                concave.Add(new Line(middle_point, line.nodes[1]));
            }
            return concave;
        }

        public static bool tangentToHull(Line line_treated, Node node, decimal cos1, decimal cos2, List<Line> concave_hull) {
            /* A new middlepoint could (rarely) make a segment that's tangent to the hull.
             * This method detects these situations
             * I suggest turning this method of if you are not using square grids or if you have a high dot density
             * */
            bool isTangent = false;
            decimal current_cos1;
            decimal current_cos2;
            double edge_length;
            List<int> nodes_searched = new List<int>();
            Line line;
            Node node_in_hull;
            int count_line = 0;
            int count_node = 0;

            edge_length = Line.getLength(node, line_treated.nodes[0]) + Line.getLength(node, line_treated.nodes[1]);


            while (!isTangent && count_line < concave_hull.Count) {
                line = concave_hull[count_line];
                while (!isTangent && count_node < 2) {
                    node_in_hull = line.nodes[count_node];
                    if (!nodes_searched.Contains(node_in_hull.id)) {
                        if (node_in_hull.id != line_treated.nodes[0].id && node_in_hull.id != line_treated.nodes[1].id) {
                            current_cos1 = getCos(node_in_hull, line_treated.nodes[0], line_treated.nodes[1]);
                            current_cos2 = getCos(node_in_hull, line_treated.nodes[1], line_treated.nodes[0]);
                            if (current_cos1 == cos1 || current_cos2 == cos2) {
                                isTangent = (Line.getLength(node_in_hull, line_treated.nodes[0]) + Line.getLength(node_in_hull, line_treated.nodes[1]) < edge_length);
                            }
                        }
                    }
                    nodes_searched.Add(node_in_hull.id);
                    count_node++;
                }
                count_node = 0;
                count_line++;
            }
            return isTangent;
        }

        public static decimal getCos(Node a, Node b, Node o) {
            /* Law of cosines */
            double aPow2 = Math.Pow(a.x - o.x, 2) + Math.Pow(a.y - o.y, 2);
            double bPow2 = Math.Pow(b.x - o.x, 2) + Math.Pow(b.y - o.y, 2);
            double cPow2 = Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2);
            double cos = (aPow2 + bPow2 - cPow2) / (2 * Math.Sqrt(aPow2 * bPow2));
            return Math.Round(System.Convert.ToDecimal(cos), 4);
        }
        
        public static int[] getBoundary(Line line, int scaleFactor) {
            /* Giving a scaleFactor it returns an area around the line 
             * where we will search for nearby points 
             * */
            int[] boundary = new int[4];
            Node aNode = line.nodes[0];
            Node bNode = line.nodes[1];
            int min_x_position = (int)Math.Floor(Math.Min(aNode.x, bNode.x) / scaleFactor);
            int min_y_position = (int)Math.Floor(Math.Min(aNode.y, bNode.y) / scaleFactor);
            int max_x_position = (int)Math.Floor(Math.Max(aNode.x, bNode.x) / scaleFactor);
            int max_y_position = (int)Math.Floor(Math.Max(aNode.y, bNode.y) / scaleFactor);

            boundary[0] = min_x_position;
            boundary[1] = min_y_position;
            boundary[2] = max_x_position;
            boundary[3] = max_y_position;

            return boundary;
        }

        public static List<Node> getNearbyPoints(Line line, List<Node> nodeList, int scaleFactor) {
            /* The bigger the scaleFactor the more points it will return
             * Inspired by this precious algorithm:
             * http://www.it.uu.se/edu/course/homepage/projektTDB/ht13/project10/Project-10-report.pdf
             * Be carefull: if it's too small it will return very little points (or non!), 
             * if it's too big it will add points that will not be used and will consume time
             * */
            List<Node> nearbyPoints = new List<Node>();
            int[] boundary;
            int tries = 0;
            int node_x_rel_pos;
            int node_y_rel_pos;

            while (tries < 2 && nearbyPoints.Count == 0) {
                boundary = getBoundary(line, scaleFactor);
                foreach (Node node in nodeList) {
                    //Not part of the line
                    if (!(node.x == line.nodes[0].x && node.y == line.nodes[0].y ||
                        node.x == line.nodes[1].x && node.y == line.nodes[1].y)) {
                        node_x_rel_pos = (int)Math.Floor(node.x / scaleFactor);
                        node_y_rel_pos = (int)Math.Floor(node.y / scaleFactor);
                        //Inside the boundary
                        if (node_x_rel_pos >= boundary[0] && node_x_rel_pos <= boundary[2] &&
                            node_y_rel_pos >= boundary[1] && node_y_rel_pos <= boundary[3]) {
                            nearbyPoints.Add(node);
                        }
                    }
                }
                //if no points are found we increase the area
                scaleFactor = scaleFactor * 4 / 3;
                tries++;
            }
            return nearbyPoints;
        }

        public static Node[] getHullNearbyNodes(Line line, List<Line> concave_hull) {
            /* Return previous and next nodes to a line in the hull */
            Node[] nearbyHullNodes = new Node[2];
            int leftNodeID = line.nodes[0].id;
            int rightNodeID = line.nodes[1].id;
            int currentID;
            int nodesFound = 0;
            int line_count = 0;
            int position = 0;
            int opposite_position = 1;

            while(nodesFound < 2) {
                position = 0;
                opposite_position = 1;
                while (position < 2) {
                    currentID = concave_hull[line_count].nodes[position].id;
                    if(currentID == leftNodeID &&
                        concave_hull[line_count].nodes[opposite_position].id != rightNodeID) {
                        nearbyHullNodes[0] = concave_hull[line_count].nodes[opposite_position];
                        nodesFound++;
                    }else if (currentID == rightNodeID && 
                        concave_hull[line_count].nodes[opposite_position].id != leftNodeID) {
                        nearbyHullNodes[1] = concave_hull[line_count].nodes[opposite_position];
                        nodesFound++;
                    }
                    position++;
                    opposite_position--;
                }
                line_count++;
            }
            return nearbyHullNodes;
        }
    }
}