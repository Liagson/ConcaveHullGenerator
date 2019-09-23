using System.Collections.Generic;
using System.Linq;

namespace ConcaveHull {
    public static class Hull {
        public static int scaleFactor;
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

        public static void setConvHull(List<Node> nodes) {
            unused_nodes.AddRange(nodes);
            hull_edges.AddRange(getHull(nodes));
            foreach (Line line in hull_edges) {
                foreach (Node node in line.nodes) {
                    if (unused_nodes.Find(a => a.id == node.id) != null) {
                        unused_nodes.Remove(unused_nodes.Where(a => a.id == node.id).First());
                    }
                }
            }
        }

        public static void setConcaveHull(double concavity, int scaleFactor, bool isSquareGrid) {
            /* Run setConvHull before! 
             * Concavity is a value used to restrict the concave angles 
             * it can go from -1 to 1 (it wont crash if you go further)
             * */
            Hull.scaleFactor = scaleFactor;
            hull_concave_edges = new List<Line>(hull_edges.OrderByDescending(a => Line.getLength(a.nodes[0], a.nodes[1])).ToList());
            Line selected_edge;
            List<Line> aux = new List<Line>(); ;
            int list_original_size;
            int count = 0;
            bool listIsModified = false;
            do {
                listIsModified = false;
                count = 0;
                list_original_size = hull_concave_edges.Count;
                while (count < list_original_size) {
                    selected_edge = hull_concave_edges[0];
                    hull_concave_edges.RemoveAt(0);
                    aux = new List<Line>();
                    if (!selected_edge.isChecked) {
                        List<Node> nearby_points = HullFunctions.getNearbyPoints(selected_edge, unused_nodes, Hull.scaleFactor);
                        aux.AddRange(HullFunctions.setConcave(selected_edge, nearby_points, hull_concave_edges, concavity, isSquareGrid));
                        listIsModified = listIsModified || (aux.Count > 1);

                        if (aux.Count > 1) {
                            foreach (Node node in aux[0].nodes) {
                                if (unused_nodes.Find(a => a.id == node.id) != null) {
                                    unused_nodes.Remove(unused_nodes.Where(a => a.id == node.id).First());
                                }
                            }
                        }else {
                            aux[0].isChecked = true;
                        }
                    } else {
                        aux.Add(selected_edge);
                    }
                    hull_concave_edges.AddRange(aux);
                    count++;
                }
                hull_concave_edges = hull_concave_edges.OrderByDescending(a => Line.getLength(a.nodes[0], a.nodes[1])).ToList();
                list_original_size = hull_concave_edges.Count;
            } while (listIsModified);
        }
    }
}