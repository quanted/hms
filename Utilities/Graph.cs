using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{

    public class Node
    {
        public int id;                  // Stream Network: Hydroseq
        public string name;             // Stream Network: ComID

        public List<int> upEdge;        // Stream Network: UpHydroseq
        public List<int> dwnEdge;       // Stream Network: DwHydroseq

        public bool leaf;               // Stream Network: Headwater, if upEdge.length == 0
        public bool inGraphNode;        // Stream Network: Out of network source segment, set if Node doesn't already exist and is an upEdge node.
        private bool mainNode;          // Stream Network: Segment is in the mainstem, hydroseq=dwnhydroseq of the uphydroseq node

        public List<Node> upNode;       // Stream Network: UpHydroseq Node
        public List<Node> dwnNode;      // Stream Network: DwHydroseq Node

        public string altId;            // Stream Network: WB ComID, if NWM=1

        public Dictionary<string, object> attributes;       // Stream Network: Segment attribute data to be provided in output
        public Dictionary<string, object> altAttributes;    // Stream Network: Waterbody attributes if altId is not null
        public bool traversed;

        public Node(int id, string name, Dictionary<string, object> attributes, bool leaf=true, bool inGraphNode=true, bool mainNode=true, string altId=null)
        {
            this.id = id;
            this.name = name;
            this.upEdge = new List<int>();
            this.dwnEdge = new List<int>();
            this.leaf = leaf;
            this.inGraphNode = inGraphNode;
            this.mainNode = mainNode;
            this.upNode = new List<Node>();
            this.dwnNode = new List<Node>();
            this.altId = altId;
            this.attributes = attributes;
            this.traversed = false;
        }

        public void AddEdge(Node node, bool upNode=true)
        {
            int id = node.id;
            if (upNode)
            {
                if (!this.upEdge.Contains(id)) 
                {
                    this.upEdge.Add(id);
                    this.upNode.Add(node);
                    this.leaf = false;
                }
            }
            else
            {
                if (!this.dwnEdge.Contains(id))
                {
                    this.dwnEdge.Add(id);
                    this.dwnNode.Add(node);
                }
            }
        }
    }

    public class Graph
    {
        public Node prime;                              // Stream Network: pourpoint segment
        public Dictionary<int, Node> nodeDict;          // Stream Network: Dictionary of nodes in network by id
        private List<Node> nodes;                       // Stream Network: List of nodes in network
        private List<Node> edgeNodes;                   // Stream Network: list of edge nodes in the network
        public List<List<string>> order;                // Stream Network: breadth-first search ordering by node name (ComID)
        public List<string> edges;                      // Stream Network: headwaters
        public List<string> outNodes;                   // Stream Network: List of out of network nodes

        public Graph()
        {
            this.prime = null;
            this.nodes = new List<Node>();
            this.nodeDict = new Dictionary<int, Node>();
            this.edgeNodes = new List<Node>();
            this.order = new List<List<string>>();
            this.edges = new List<string>();
            this.outNodes = new List<string>();
        }

        public void AddNode(Node node, bool primeNode = false)
        {
            if (node == null)
            {
                throw new ArgumentNullException();
            }
            if (!this.nodeDict.ContainsKey(node.id))
            {
                if (primeNode && this.prime == null) 
                {
                    this.prime = node;
                }
                this.nodes.Add(node);
            }
            if (!node.inGraphNode)
            {
                this.outNodes.Add(node.name);
            }
            this.nodeDict.Add(node.id, node);
        }

        public void AddEdge(int edgeId, int nodeId, bool upNode = true)
        {
            if (this.nodeDict.ContainsKey(edgeId) && this.nodeDict.ContainsKey(nodeId))
            {
                this.nodeDict[nodeId].AddEdge(this.nodeDict[edgeId], upNode);
            }
        }

        public List<string> GetEdges()
        {
            List<string> edges = new List<string>();
            for(int i = 0; i < this.nodes.Count; i++)
            {
                if (this.nodes[i].leaf)
                {
                    edges.Add(this.nodes[i].name);
                }
            }
            return edges;
        }

        private void CheckEdges(bool reset = false)
        {
            this.edgeNodes = new List<Node>();
            foreach (Node node in this.nodes)
            {
                if (node.upNode.Count == 0 && node.inGraphNode)
                {
                    node.leaf = true;
                    this.edgeNodes.Add(node);
                    this.edges.Add(node.name);
                }
                else if(node.inGraphNode)
                {
                    node.leaf = true;
                    for (int i = 0; i < node.upNode.Count; i++)
                    {
                        if (node.upNode[i].inGraphNode)
                        {
                            node.leaf = false;
                        }
                    }
                    if (node.leaf)
                    {
                        this.edgeNodes.Add(node);
                        this.edges.Add(node.name);
                    }
                }
                if (reset)
                {
                    node.traversed = false;
                }
            }
        }

        public void Traverse()
        {
            this.CheckEdges(true);

            List<Node> iEdges = new List<Node>(){ this.prime };
            List<Node> jEdges;
            int traversed = 0;
            while (traversed < this.nodes.Count - this.outNodes.Count)
            {
                jEdges = new List<Node>();
                List<string> level = new List<string>();
                for(int i = 0; i < iEdges.Count; i++)
                {
                    for(int j = 0; j < iEdges[i].upNode.Count; j++)
                    {
                        if (iEdges[i].upNode[j].inGraphNode)
                        {
                            jEdges.Add(iEdges[i].upNode[j]);
                        }
                    }
                    if (!level.Contains(iEdges[i].name))
                    {
                        level.Add(iEdges[i].name);
                        traversed += 1;
                    }
                }
                this.order.Add(level);
                iEdges = new List<Node>(jEdges);
            }
            this.order.Reverse();
        }

        public Dictionary<string, List<string>> GetSources()
        {
            Dictionary<string, List<string>> sources = new Dictionary<string, List<string>>();
            for(int i = 0; i < this.nodes.Count; i++)
            {
                if (this.nodes[i].inGraphNode)
                {
                    List<string> iSources = new List<string>();
                    for(int j = 0; j < this.nodes[i].upNode.Count; j++)
                    {
                        iSources.Add(this.nodes[i].upNode[j].name);
                    }
                    sources.Add(this.nodes[i].name, iSources);
                }
            }
            return sources;
        }
    }
}
