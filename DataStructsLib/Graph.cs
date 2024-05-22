using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataStructsLib
{
    public class Graph<TNode, TEdge> : IStringableAndCloneable<TNode>
    {
        protected Table<GraphEdge<TEdge>> adjacencyMatrix;
        protected List<TNode> nodes;
        public Graph()
        {
            adjacencyMatrix = new Table<GraphEdge<TEdge>>();
            nodes = new List<TNode>();
        }

        public void AddNode(TNode node)
        {
            if (nodes.Contains(node))
            {
                return;
            }
            nodes.Add(node);
            adjacencyMatrix.AddRowColumn(new GraphEdge<TEdge>()); //expand table
        }

        public void RemoveNode(TNode node)
        {
            int index = IndexOf(node);
            if (index == -1)
            {
                throw new ArgumentException($"Node {nameof(node)} was not in the graph");
            }
            adjacencyMatrix.RemoveRowColumn(index);
            nodes.RemoveAt(index);
        }

        

        public void Connect(TNode startNode, TNode endNode, TEdge weight)
        {
            TableCoord coord = TopRightCoordOf(startNode, endNode);
            adjacencyMatrix[coord] = new GraphEdge<TEdge>(weight);
        }
        public void Disconnect(TNode startNode, TNode endNode)
        {
            TableCoord coord = TopRightCoordOf(startNode, endNode);
            adjacencyMatrix[coord] = new GraphEdge<TEdge>();
        }
        public bool SafeTryDisconnect(TNode startNode, TNode endNode)
        {
            TableCoord coord = TopRightCoordOf(startNode, endNode);
            if (coord.Validate())
            {
                adjacencyMatrix[coord] = new GraphEdge<TEdge>();
                return true;
            }
            return false;
            
        }

        public TNode[] ListNodes()
        {
            return nodes.ToArray();
        }
        public Link<TNode, TEdge>[] ListEdges()
        {
            List<Link<TNode, TEdge>> weights = new List<Link<TNode, TEdge>>();
            for (int i = 0; i < adjacencyMatrix.Length1D; i++)
            {
                for (int j = 0; j < adjacencyMatrix.Length1D; j++)
                {
                    if (adjacencyMatrix[i,j].IsConnected)
                    {
                        weights.Add(new Link<TNode, TEdge>(nodes[i], nodes[j], adjacencyMatrix[i, j].Weight));
                    }
                }
            }
            return weights.ToArray();
        }
        public bool NodeQuery(TNode node)
        {
            return nodes.IndexOf(node) > -1;
        }
        public bool DoesEdgeExist(TNode node1, TNode node2)
        {
            return adjacencyMatrix[TopRightCoordOf(node1, node2)].IsConnected;
        }
        public TEdge? GetEdge(TNode node1, TNode node2)
        {
            GraphEdge<TEdge> edge = adjacencyMatrix[TopRightCoordOf(node1, node2)];
            if (edge.IsConnected)
            {
                return edge.Weight;
            }
            return default;
        }
        public TNode[] Neighbours(TNode node)
        {
            int index = nodes.IndexOf(node);
            List<TNode> neighbours = new List<TNode>();
            for (int i = 0; i < adjacencyMatrix.Length1D; i++)
            {
                if (index != i)
                {
                    if (adjacencyMatrix[ToTopRight(i, index)].IsConnected) //check row and column
                    {
                        neighbours.Add(nodes[i]);
                    }
                    
                }
            }
            return neighbours.ToArray();
        }
        protected int IndexOf(TNode node)
        {
            return nodes.IndexOf(node);
        }
        protected TableCoord TopRightCoordOf(TNode node1, TNode node2)
        {
            int index1 = IndexOf(node1);
            int index2 = IndexOf(node2);
            return ToTopRight(index1, index2);
        }
        protected TableCoord ToTopRight(int index1, int index2)
        {
            return new TableCoord(Math.Min(index1, index2), Math.Max(index1, index2));
        }

        public string Stringify(Func<TNode, string> transform)
        {
            return $"{nodes.Stringify(transform)}{adjacencyMatrix.Stringify(StringifyEdgeTransform)}";
        }
        public string Stringify(bool withNewLines, Func<TNode, string> transform)
        {
            if (!withNewLines)
            {
                return this.Stringify(transform);
            }
            
            return $"{nodes.Stringify(transform)}\n\n{adjacencyMatrix.Stringify(true, StringifyEdgeTransform)}";
        }
        private string StringifyEdgeTransform(GraphEdge<TEdge> edge)
        {
            return edge.IsConnected ? edge.Weight.ToString() ?? "" : "-";
        }
        public override string ToString()
        {
            return this.Stringify(true);
        }
        public object Clone()
        {
            Graph<TNode, TEdge> copy = new Graph<TNode, TEdge>();
            copy.adjacencyMatrix = (Table<GraphEdge<TEdge>>)this.adjacencyMatrix.Clone();
            copy.nodes = (List<TNode>)this.nodes.Clone();
            return copy;
        }

        protected struct GraphEdge<T>
        {
            private T? weight;
            public T Weight
            {
                get
                {
                    if (!IsConnected)
                    {
                        throw new Exception("There was no connection");
                    }
                    return weight;
                }
            }
            public bool IsConnected { get; private set; }
            public GraphEdge()
            {
                weight = default;
                IsConnected = false;
            }
            public GraphEdge(T weight)
            {
                this.weight = weight;
                IsConnected = true;
            }
        }
    }

    public struct Link<TNode, TEdge>
    {
        public TNode Start { get; private set; }
        public TNode End { get; private set; }
        public TEdge Weight { get; private set; }
        public Link(TNode start, TNode end, TEdge weight)
        {
            this.Start = start;
            this.End = end;
            this.Weight = weight;
        }
    }
    
}
