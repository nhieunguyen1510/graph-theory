using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace GraphTheory
{
    class Edge
    {
        public int startVertex { get; set; }
        public int endVertex { get; set; }
        bool isDirected { get; set; }
    }

    class ColoredGraph
    {
        private List<List<int>> graph { get; set; }
        private OrderedDictionary orderedVerticesByDegree { get; set; }
        public int maxColor { get; set; }
        public int colorQuantity {
            get
            {
                return this.maxColor + 1;
            }
        }
        public Dictionary<int, int> coloredVertices {
            get
            {
                return this._coloredVertices;
            }
        }
        public Dictionary<int, List<int>> groupColor
        {
            get
            {
                return this._groupColor;
            }
        }
        private Dictionary<int, int> _coloredVertices { get; set; }
        private Dictionary<int, List<int>> _groupColor { get; set; }

        public ColoredGraph(List<List<int>> graph)
        {
            this.graph = graph;
            int size = graph.Count;
            this.orderedVerticesByDegree = new OrderedDictionary();
            SortedDictionary<int, int> sortedVerticesByDegree = new SortedDictionary<int, int>();
            for (int i = 0; i < size; i++)
            {
                int degree = this.graph[i].Count(isEdge =>
                {
                    return isEdge == 1;
                });
                sortedVerticesByDegree.Add(i, degree);
            }
            this._coloredVertices = new Dictionary<int, int>();
            foreach (var pair in sortedVerticesByDegree.OrderBy(p => 0 - p.Value))
            {
                //this.orderedVerticesByDegree.Add(pair.Key, pair.Value);
                int currentVertex = pair.Key;
                //this._coloredVertices.has
                int color = -1;
                if (!this._coloredVertices.ContainsKey(currentVertex))
                {
                    color = 0;
                    for (int i = 0; i < size; i++)
                    {
                        if (graph[currentVertex][i] == 1 && this._coloredVertices.ContainsKey(i) && this._coloredVertices[i] >= color)
                        {
                            color = this._coloredVertices[i] + 1;
                        }
                        //if (graph[currentVertex][i] == 1 && this._coloredVertices.ContainsKey(i) && this._coloredVertices[i] == 1)
                        //{
                        //    color = 0;
                        //    break;
                        //}
                    }
                    if (this.maxColor < color)
                    {
                        this.maxColor = color;
                    }
                    this._coloredVertices.Add(currentVertex, color);
                }
                DFSStack dfsStack = new DFSStack(graph, currentVertex);
                while (dfsStack.visited < dfsStack.size)
                {
                    int nextVertex = dfsStack.nextVertex();
                    // Check if this vertex is not colored
                    if (!this._coloredVertices.ContainsKey(nextVertex))
                    {
                        // Check if this vertex is not adjacent with current vertex
                        if (graph[nextVertex][currentVertex] == 0)
                        {
                            bool isSameColorWithOtherAdjacent = false;
                            for (int i = 0; i < size; i++)
                            {
                                if (graph[nextVertex][i] == 1 && this._coloredVertices.ContainsKey(i) && this._coloredVertices[i] == color)
                                {
                                    isSameColorWithOtherAdjacent = true;
                                    break;
                                }
                            }
                            if (isSameColorWithOtherAdjacent == false)
                            {
                                this._coloredVertices.Add(nextVertex, color);
                            }
                        }
                    }
                }
            }
            this._groupColor = new Dictionary<int, List<int>>();
            foreach (var pair in this._coloredVertices)
            {
                if (this._groupColor.ContainsKey(pair.Value))
                {
                    this._groupColor[pair.Value].Add(pair.Key);
                }
                else
                {
                    List<int> list = new List<int>();
                    list.Add(pair.Key);
                    this._groupColor.Add(pair.Value, list);
                }
            }
        }
    }

    class DFSStack
    {
        public Stack<int> dfsStack { get; set; }
        public List<int> visitedNodes { get; set; }
        public int size { get; set; }
        public int visited { get { return this._visited; } }
        private int _visited { get; set; }
        private int currentVetex = 0;
        private List<List<int>> graph;

        public DFSStack(List<List<int>> graph, int startVertex = 0)
        {
            this.dfsStack = new Stack<int>();
            this.graph = graph;
            this.size = graph.Count;
            this.visitedNodes = new List<int>(new int[this.size]);
            this.currentVetex = startVertex;
            this.visitedNodes[startVertex] = 1;
            this._visited = 1;
            this.dfsStack.Push(startVertex);
        }

        public int nextVertex()
        {
            int nextVertex = this.getNextVertex();
            if (nextVertex != -1)
            {
                this.currentVetex = nextVertex;
                this._visited++;
                this.dfsStack.Push(nextVertex);
                this.visitedNodes[nextVertex] = 1;
                return nextVertex;
            }
            while (nextVertex == -1 && this.dfsStack.Count > 0)
            {
                this.dfsStack.Pop();
                int lastVertex = this.dfsStack.First();
                this.currentVetex = lastVertex;
                nextVertex = this.getNextVertex();
            }
            if (nextVertex != -1)
            {
                this._visited++;
                this.currentVetex = nextVertex;
                this.dfsStack.Push(nextVertex);
                this.visitedNodes[nextVertex] = 1;
            }
            return nextVertex;
        }

        public int getNextVertex()
        {
            int nextVertex = this.currentVetex;
            for (int i = 0; i < size; i++)
            {
                if (graph[this.currentVetex][i] == 1 && this.visitedNodes[i] == 0)
                {
                    nextVertex = i;
                    break;
                }
            }
            if (this.currentVetex != nextVertex)
            {
                return nextVertex;
            }
            return -1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Program.checkGraph("input.txt");
        }

        static void checkGraph(string fileName = "input.txt")
        {
            try
            {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(fileName))
                {
                    // Read the stream to a string, and write the string to the console.
                    int size = Int32.Parse(sr.ReadLine());
                    List<List<int>> graph = new List<List<int>>();
                    for (int i = 0; i < size; i++)
                    {
                        List<int> row = new List<int>();
                        string line = sr.ReadLine();
                        string[] values = line.Split(' ');
                        Array.ForEach(values, v => row.Add(Int32.Parse(v)));
                        graph.Add(row);
                    }

                    ColoredGraph coloredGraph = new ColoredGraph(graph);
                    bool isBiGraph = coloredGraph.maxColor == 1;
                    bool isCompleteBiGraph = true;
                    if (isBiGraph)
                    {
                        foreach (int v1 in coloredGraph.groupColor[0])
                        {
                            foreach (int v2 in coloredGraph.groupColor[1])
                            {
                                if (graph[v1][v2] == 0)
                                {
                                    isCompleteBiGraph = false;
                                    break;
                                }
                            }
                            if (!isCompleteBiGraph)
                            {
                                break;
                            }
                        }
                    } else
                    {
                        isCompleteBiGraph = false;
                    }
                    bool isStarGraph = Program.checkStarGraph(graph);
                    bool isButterFlyGraph = Program.checkButterFlyGraph(graph);
                    int[] mindWillProps = Program.getWindMillGraph(graph);
                    bool isKParties = !isBiGraph && graph.Count != 0;
                    string kParties = "";
                    if (isKParties)
                    {
                        kParties = coloredGraph.maxColor + 1 + "-partie";
                        foreach (var colorGroup in coloredGraph.groupColor)
                        {
                            kParties += " {" + String.Join(", ", colorGroup.Value.ToArray()) + "}";
                        }
                    }
                    int wheel = Program.getWheelGraph(graph);
                    Console.WriteLine("Đồ thị lưỡng phân: {0}", isBiGraph ? "{" + String.Join(", ", coloredGraph.groupColor[0].ToArray()) + "} {" + String.Join(", ", coloredGraph.groupColor[1].ToArray()) + "}" : "Không");
                    Console.WriteLine("Đồ thị lưỡng phân đầy đủ: {0}", isCompleteBiGraph && graph.Count > 0 ? "Có" : "Không");
                    Console.WriteLine("Đồ thị rỗng: {0}", graph.Count == 0 ? "Có" : "Không");
                    Console.WriteLine("Đồ thị hình sao: {0}", isStarGraph ? "S" + graph.Count : "Không");
                    Console.WriteLine("Đồ thị bánh xe: {0}", wheel != -1 ? "W" + wheel : "Không");
                    Console.WriteLine("Đồ thị hình bướm: {0}", isButterFlyGraph ? "Có" : "Không");
                    Console.WriteLine("Đồ thị hình cối xay gió: {0}", mindWillProps != null ? "Wd(" + mindWillProps[0] + ", " + mindWillProps[1] + ")" : "Không");
                    Console.WriteLine("Đồ thị k-phân: {0}", isKParties ? kParties : "Không");
                    Console.WriteLine("");
                    //Environment.Exit(0);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        static bool checkButterFlyGraph(List<List<int>> graph)
        {
            int size = graph.Count;
            if (size == 5)
            {
                int centerNodeDegree = 4;
                int subNodeDegree = 2;
                bool isButterFlyGraph = true;
                for (int i = 0; i < size; i++)
                {
                    int degree = graph[i].Count(isEdge => isEdge == 1);
                    if (degree != centerNodeDegree && degree != subNodeDegree)
                    {
                        isButterFlyGraph = false;
                        break;
                    }
                }
                return isButterFlyGraph;
            }
            return false;
        }

        static int getWheelGraph(List<List<int>> graph)
        {
            int size = graph.Count;
            if (size < 4)
            {
                return -1;
            }
            int centerNodeDegree = size - 1;
            int subNodeDegree = 3;
            bool isWheelGraph = true;
            for (int i = 0; i < size; i++)
            {
                int degree = graph[i].Count(isEdge => isEdge == 1);
                if (degree != centerNodeDegree && degree != subNodeDegree)
                {
                    isWheelGraph = false;
                    break;
                }
            }
            if (isWheelGraph)
            {
                return size;
            }
            return -1;
        }

        static int[] getWindMillGraph(List<List<int>> graph)
        {
            int size = graph.Count;
            if(size < 5)
            {
                return null;
            }
            int centerNodeDegree = size - 1;
            int centerNode = -1;
            int completeGraphDegree = -1;
            bool isNotAllEqual = false;
            for (int i = 0; i < size; i++)
            {
                int degree = graph[i].Count(isEdge => isEdge == 1);
                if (degree == centerNodeDegree && centerNode == -1)
                {
                    centerNode = i;
                }
                else if (completeGraphDegree == -1)
                {
                    completeGraphDegree = degree;
                }
                else if(completeGraphDegree != degree)
                {
                    isNotAllEqual = true;
                    break;
                }
            }
            if (isNotAllEqual)
            {
                return null;
            }
            ColoredGraph coloredGraph = new ColoredGraph(graph);
            int n = coloredGraph.colorQuantity;
            int nodeEachSubGraph = n - 1;
            if (nodeEachSubGraph != completeGraphDegree || nodeEachSubGraph < 2)
            {
                return null;
            }
            int k = (size  - 1) / nodeEachSubGraph;
            int nodes = n * k - k + 1;
            if (size != nodes)
            {
                return null;
            }
            return new int[] { n, k };
        }

        static bool checkStarGraph(List<List<int>> graph)
        {
            int size = graph.Count;
            if (size == 0)
            {
                return false;
            }
            if (size == 1)
            {
                return true;
            }
            if (size == 2)
            {
                return graph[0][1] == 1;
            }
            bool isStar = true;
            for (int i = 0; i < size; i++)
            {
                int degree = graph[i].Count(isEdge => isEdge == 1);
                if (degree != 1 && degree != size -1)
                {
                    isStar = false;
                    break;
                }
            }
            return isStar;
        }

        //static bool isBipartieGraph(List<List<int>> graph, ColoredGraph coloredGraph)
        //{
        //    return coloredGraph.vertices0.All(v1 => {
        //        return !coloredGraph.vertices0.Any(v2 =>
        //        {
        //            return graph[v1][v2] == 1;
        //        });
        //    }) && coloredGraph.vertices1.All(v1 => {
        //        return !coloredGraph.vertices1.Any(v2 =>
        //        {
        //            return graph[v1][v2] == 1;
        //        });
        //    });
        //}
    }
}
