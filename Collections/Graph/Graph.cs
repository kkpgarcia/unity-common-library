using System.Collections.Generic;

namespace Common.Collections
{
    public class Graph
    {
        public List<GraphLink> Links { get; private set; }
        public List<GraphNode> Nodes { get; set; }

        public Graph(List<GraphLink> edges, List<GraphNode> nodes)
        {
            this.Links = edges;
            this.Nodes = nodes;

            foreach (GraphNode node in nodes)
            {
                node.Graph = this;
            }

        }

        public GraphNode GetRoot()
        {
            return this.Nodes[0];
        }

        public void AddEdge(GraphLink edge)
        {
            this.Links.Add(edge);
        }

        public void AddEdge(GraphNode from, GraphNode to)
        {
            this.Links.Add(new GraphLink(from, to));
        }

        public void AddNode(GraphNode node)
        {
            this.Nodes.Add(node);
            node.Graph = this;
        }

        public void RemoveEdge(GraphLink edge)
        {
            this.Links.Remove(edge);
        }

        public void RemoveNode(GraphNode node)
        {
            this.Links.RemoveAll(e => e.From == node || e.To == node);
            this.Nodes.Remove(node);
        }
    }
}