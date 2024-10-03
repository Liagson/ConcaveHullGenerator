﻿using System;

namespace ConcaveHull {
    public class Line {
        public Node[] nodes = new Node[2];
        public Line(Node n1, Node n2) {
            nodes[0] = n1;
            nodes[1] = n2;
        }
        public double getLength()
        {
            double length = Math.Sqrt(Math.Pow(nodes[0].y - nodes[1].y, 2) + Math.Pow(nodes[0].x - nodes[1].x, 2));
            return length;
        }

        public static double getLength(Node node1, Node node2) {
            double length = Math.Sqrt(Math.Pow(node1.y - node2.y, 2) + Math.Pow(node1.x - node2.x, 2));
            return length;
        }
    }
}