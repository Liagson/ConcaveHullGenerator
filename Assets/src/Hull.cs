using Assets.src;
using System.Collections.Generic;
using System.Linq;

namespace ConcaveHull {
    public static class Hull {
        public static List<Node> unused_nodes = new List<Node>();
        public static List<Line> hull_edges = new List<Line>();
        public static List<Line> hull_concave_edges = new List<Line>();

        public static List<Line> getHull(List<Node> nodes) {
            List<Node> convexH = new List<Node>();
            List<Line> exitLines = new List<Line>();
            
            convexH = new List<Node>();
            convexH.AddRange(GrahamScan.convexHull(nodes));
            for (int i = 0; i < convexH.Count - 1; i++) {
                exitLines.Add(new Line(convexH[i], convexH[i + 1]));
            }
            exitLines.Add(new Line(convexH[0], convexH[convexH.Count - 1]));
            return exitLines;
        }

        public static void setConvexHull(List<Node> nodes) {
            unused_nodes.AddRange(nodes);
            hull_edges.AddRange(getHull(nodes));
            foreach (Line line in hull_edges) {
                foreach (Node node in line.nodes) {
                    unused_nodes.RemoveAll(a => a.id == node.id);
                }
            }
        }

        public static List<Line> setConcaveHull(double concavity, int scaleFactor) {
            /* Run setConvHull before! 
             * Concavity is a value used to restrict the concave angles 
             * It can go from -1 (no concavity) to 1 (extreme concavity) 
             * Avoid concavity == 1 if you don't want 0º angles
             * */
            bool aLineWasDividedInTheIteration;
            hull_concave_edges.AddRange(hull_edges);
            do {
                aLineWasDividedInTheIteration = false;
                for(int linePositionInHull = 0; linePositionInHull < hull_concave_edges.Count && !aLineWasDividedInTheIteration; linePositionInHull++) {
                    Line line = hull_concave_edges[linePositionInHull];
                    List<Node> nearbyPoints = HullFunctions.getNearbyPoints(line, unused_nodes, scaleFactor);
                    List<Line> dividedLine = HullFunctions.getDividedLine(line, nearbyPoints, hull_concave_edges, concavity);
                    if (dividedLine.Count > 0) { // Line divided!
                        aLineWasDividedInTheIteration = true;
                        unused_nodes.Remove(unused_nodes.Where(n => n.id == dividedLine[0].nodes[1].id).FirstOrDefault()); // Middlepoint no longer free
                        hull_concave_edges.AddRange(dividedLine);
                        hull_concave_edges.RemoveAt(linePositionInHull); // Divided line no longer exists
                    }
                }

                hull_concave_edges = hull_concave_edges.OrderByDescending(a => Line.getLength(a.nodes[0], a.nodes[1])).ToList();
            } while (aLineWasDividedInTheIteration);

            return hull_concave_edges;
        }
    }
}