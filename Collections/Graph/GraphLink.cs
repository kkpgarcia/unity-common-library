using System.Text;

namespace Common.Collections
{
    public class GraphLink
    {
        public string ID { get; set; }
        public GraphNode From { get; set; }
        public GraphNode To { get; set; }

        public GraphLink(GraphNode from, GraphNode to)
        {
            this.From = from;
            this.To = to;
        }

        public override string ToString()
        {
            return new StringBuilder().Append("Edge:")
                .AppendLine("From: " + (this.From != null ? this.From.ToString() : "N/A"))
                .AppendLine("To: " + (this.To != null ? this.To.ToString() : "N/A")).ToString();
        }
    }
}