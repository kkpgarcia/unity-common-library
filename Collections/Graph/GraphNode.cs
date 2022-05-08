using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Collections
{
    public abstract class GraphNode
    {
        public string ID { get; protected set; }
        public Graph Graph { get; internal set; }

        protected GraphNode()
        {
            this.ID = Guid.NewGuid().ToString();
        }

        public List<GraphLink> InboundEdges
        {
            get
            {
                UnityEngine.Assertions.Assert.IsNotNull(Graph);
                return Graph.Links.Where(e => e.To == this).ToList();
            }
        }

        public List<GraphLink> OutboundEdges
        {
            get
            {
                UnityEngine.Assertions.Assert.IsNotNull(Graph);
                return Graph.Links.Where(e => e.From == this).ToList();
            }
        }

        public virtual void Clean() {}
    }

    public class GraphNode<T> : GraphNode
    {
        public T Data { get; protected set; }

        public GraphNode(T data) : base()
        {
            this.Data = data;
        }

        protected void ChangeData(T data)
        {
            this.Data = data;
        }
    }
}