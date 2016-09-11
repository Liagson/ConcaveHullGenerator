namespace ConcaveHull {
    public class Node {
        public int id;
        public double x;
        public double y;
        public Node(double x, double y) {
            this.x = x;
            this.y = y;
        }
        public Node(double x, double y, int id) {
            this.x = x;
            this.y = y;
            this.id = id;
        }
    }
}