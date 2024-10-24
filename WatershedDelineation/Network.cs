﻿using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using Serilog;
using Serilog.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace WatershedDelineation
{
    public class Network
    {
        public List<List<object>> networkTable;
        private List<string> comids;
        private Dictionary<string, int> comidHydro;
        private Graph networkGraph;
        private List<string> divergences;
        private Dictionary<string, List<string>> divergentPaths;
        private List<object> waterbodyTable;
        private Dictionary<string, string> comidToWB;

        private string comidStr;
        private string hydroStr;
        private string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");


        public Network(List<List<object>> table, string pourpoint)
        {
            this.networkTable = table;
            this.comids = new List<string>();
            this.comidHydro = new Dictionary<string, int>();
            this.networkGraph = new Graph();
            this.divergences = new List<string>();
            this.divergentPaths = new Dictionary<string, List<string>>();
            this.waterbodyTable = new List<object>();
            this.comidToWB = new Dictionary<string, string>();

            StringBuilder comidSB = new StringBuilder();
            StringBuilder hydroSB = new StringBuilder();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Add all in-network nodes
            int i = 0;
            for(i = 1; i < table.Count; i++)
            {
                string comid = table[i][0].ToString();
                string strHydro = table[i][1].ToString();
                int hydroseq = Int32.Parse(strHydro);
                this.comids.Add(comid);
                this.comidHydro.Add(comid, hydroseq);
                Dictionary<string, object> attributes = new Dictionary<string, object>();
                for (int j = 0; j < table[0].Count; j++)
                {
                    attributes.Add(table[0][j].ToString(), table[i][j]);
                }
                Node node = new Node(hydroseq, comid, attributes);
                this.networkGraph.AddNode(node, pourpoint.Equals(comid));

                comidSB.Append(comid);
                hydroSB.Append(hydroseq);
                if (i < table.Count - 1)
                {
                    comidSB.Append(",");
                    hydroSB.Append(",");
                }
            }
            stopwatch.Stop();
            Log.Information("Stream Network - Add in-table nodes Runtime: " + stopwatch.Elapsed.TotalSeconds.ToString() + " sec");
            stopwatch.Restart();

            this.comidStr = comidSB.ToString();
            this.hydroStr = hydroSB.ToString();

            // Load out of network nodes
            //string outSources = "SELECT ComID, Hydroseq, DnHydroseq FROM PlusFlowlineVAA WHERE DnHydroseq IN (" + this.hydroStr + ") AND ComID NOT IN (" + this.comidStr + ")";
            string outSources = "SELECT ComID, Hydroseq, DnHydroseq FROM PlusFlowlineVAA WHERE DnHydroseq IN (" + this.hydroStr + ")";
            Dictionary<string, string> outResults = Utilities.SQLite.GetData(this.dbPath, outSources) as Dictionary<string, string>;
            if (outResults.Count > 0)
            {
                List<string> outComids = outResults["ComID"].Split(",").ToList();
                List<string> outHydro = outResults["Hydroseq"].Split(",").ToList();
                List<string> outDnHydro = outResults["DnHydroseq"].Split(",").ToList();
                for (i = 0; i < outComids.Count; i++)
                {
                    if (!this.comids.Contains(outComids[i]))
                    {
                        Node node = new Node(Int32.Parse(outHydro[i]), outComids[i], null, false, false, false);
                        this.networkGraph.AddNode(node);
                    }
                }
                stopwatch.Stop();
                Log.Information("Stream Network - Add out-of-table nodes Runtime: " + stopwatch.Elapsed.TotalSeconds.ToString() + " sec");
                stopwatch.Restart();
            }

            string altUpHydroQuery = "SELECT Hydroseq, DnHydroseq FROM PlusFlowlineVAA WHERE DnHydroseq IN (" + this.hydroStr + ")";
            Dictionary<string, string> altHydroEdges = Utilities.SQLite.GetData(this.dbPath, altUpHydroQuery);
            List<string> hydroList = new List<string>();
            List<string> dnHydroList = new List<string>();
            Dictionary<int, List<int>> upHydroMapping = new Dictionary<int, List<int>>();
            if (altHydroEdges.Count > 0)
            {
                hydroList = altHydroEdges["Hydroseq"].Split(",").ToList();
                dnHydroList = altHydroEdges["DnHydroseq"].Split(",").ToList();
                for(int j = 0; j < hydroList.Count; j++)
                {
                    int hydro = Int32.Parse(hydroList[j]);
                    int dnHydro = Int32.Parse(dnHydroList[j]);
                    if (upHydroMapping.ContainsKey(dnHydro))
                    {
                        upHydroMapping[dnHydro].Add(hydro);
                    }
                    else
                    {
                        upHydroMapping.Add(dnHydro, new List<int>() { hydro });
                    }
                }
            }

            // Add all edges from table
            i = 0;
            Parallel.For(1, table.Count, i =>
            {
                string comid = table[i][0].ToString();
                int hydroseq = Int32.Parse(table[i][1].ToString());
                int uphydroseq = Int32.Parse(table[i][2].ToString());
                int dnhydroseq = Int32.Parse(table[i][3].ToString());
                if (!comid.Equals(pourpoint))
                {
                    this.networkGraph.AddEdge(dnhydroseq, hydroseq, false);
                }
                if (uphydroseq != 0)
                {
                    this.networkGraph.AddEdge(uphydroseq, hydroseq, true);
                    if (upHydroMapping.ContainsKey(hydroseq))
                    {
                        if (upHydroMapping[hydroseq].Count > 1)
                        {
                            for (int j = 0; j < upHydroMapping[hydroseq].Count; j++)
                            {
                                int altupHydro = upHydroMapping[hydroseq][j];
                                if (uphydroseq != altupHydro)
                                {
                                    this.networkGraph.AddEdge(altupHydro, hydroseq, true);
                                }
                            }
                        }
                    }
                }

            });
            stopwatch.Stop();
            Log.Information("Stream Network - Add graph edges Runtime: " + stopwatch.Elapsed.TotalSeconds.ToString() + " sec");
            stopwatch.Restart();

            this.networkGraph.Traverse();

            stopwatch.Stop();
            Log.Information("Stream Network - Graph Traversal Runtime: " + stopwatch.Elapsed.TotalSeconds.ToString() + " sec");
            stopwatch.Restart();

        }

        public void LoadData()
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Load list of divergent segments
            string divergentNodesQuery = "SELECT ComID From PlusFlowlineVAA WHERE Comid IN (" + this.comidStr + ") AND Divergence>1";
            Dictionary<string, string> divergentNodeResults = Utilities.SQLite.GetData(dbPath, divergentNodesQuery) as Dictionary<string, string>;
            if (divergentNodeResults.Count > 0)
            {
                this.divergences = divergentNodeResults["ComID"].ToString().Split(",").ToList();
            }
            this.divergentPaths = new Dictionary<string, List<string>>();
            for (int i = 0; i < this.divergences.Count; i++)
            {
                this.divergentPaths.Add(this.divergences[i], new List<string>());
                List<Node> sourceHydro = this.networkGraph.nodeDict[this.comidHydro[this.divergences[i]]].upNode;
                for (int j = 0; j < sourceHydro.Count; j++)
                {
                    for (int k = 0; k < sourceHydro[j].dwnNode.Count; k++)
                    {
                        string dnComID = sourceHydro[j].dwnNode[k].name;
                        if (dnComID != this.divergences[i])
                        {
                            this.divergentPaths[this.divergences[i]].Add(sourceHydro[j].dwnNode[k].name);
                        }
                    }
                }
            }
            Dictionary<string, List<string>> allDivergentPaths = new Dictionary<string, List<string>>();
            foreach(KeyValuePair<string, List<string>> kv in this.divergentPaths)
            {
                allDivergentPaths.Add(kv.Key, kv.Value);
                for(int i = 0; i < kv.Value.Count; i++)
                {
                    List<string> altPaths = new List<string>(kv.Value);
                    altPaths.RemoveAt(i);
                    altPaths.Add(kv.Key);
                    allDivergentPaths.Add(kv.Value[i], altPaths);
                }
            }
            this.divergentPaths = allDivergentPaths;
            stopwatch.Stop();
            Log.Information("Stream Network - Loaded Divergent Paths Runtime: " + stopwatch.Elapsed.TotalSeconds.ToString() + " sec");
            stopwatch.Restart();

            // Load waterbodies
            List<string> wbTableLabels = new List<string>
            {
                "COMID",
                "GNIS_NAME",
                "AREASQKM",
                "ELEVATION",
                "REACHCODE",
                "FTYPE",
                "FCODE",
                "MeanDepth",
                "LakeVolume"
            };
            this.waterbodyTable.Add(wbTableLabels);

            string waterbodyQuery = "SELECT ComID, WBAREACOMI FROM PlusFlowlineVAA WHERE Comid IN (" + comidStr + ") AND WBAREACOMI IS NOT null";
            Dictionary<string, string> waterbodyResults = Utilities.SQLite.GetData(dbPath, waterbodyQuery) as Dictionary<string, string>;
            List<string> catComIDs = new List<string>();
            List<string> wbComIDs = new List<string>();
            if (waterbodyResults.Count > 0)
            {
                catComIDs = waterbodyResults["ComID"].Split(",").ToList();
                wbComIDs = waterbodyResults["WBAREACOMI"].Split(",").ToList();
                string wbDataQuery = "SELECT COMID, GNIS_NAME, AREASQKM, ELEVATION, REACHCODE, FTYPE, FCODE, MeanDepth, LakeVolume FROM Waterbodies WHERE COMID IN (" + waterbodyResults["WBAREACOMI"] + ") AND NWM=1";
                Dictionary<string, string> wbResults = Utilities.SQLite.GetData(dbPath, wbDataQuery) as Dictionary<string, string>;

                //Dictionary<string, string> wbMapping = new Dictionary<string, string>();
                if (wbResults.Count > 0)
                {
                    for (int i = 0; i < catComIDs.Count; i++)
                    {
                        string comid = catComIDs[i];
                        string wbComID = wbComIDs[i];
                        this.comidToWB.Add(comid, wbComID);
                    }
                    Dictionary<string, List<string>> wbData = new Dictionary<string, List<string>>();
                    for (int i = 0; i < wbTableLabels.Count; i++)
                    {
                        wbData.Add(wbTableLabels[i], wbResults[wbTableLabels[i]].Split(",").ToList());
                    }
                    for (int i = 0; i < wbData[wbTableLabels[0]].Count; i++)
                    {
                        string comid = wbData["COMID"][i];

                        string gnisName = wbData["GNIS_NAME"][i].ToString();
                        long reachcode = Int64.Parse(wbData["REACHCODE"][i].ToString());
                        int fcode = Int32.Parse(wbData["FCODE"][i].ToString());
                        string ftype = wbData["FTYPE"][i].ToString();

                        double area = -9999;
                        Double.TryParse(wbData["AREASQKM"][i].ToString(), out area);
                        double elevation = -9999;
                        Double.TryParse(wbData["ELEVATION"][i].ToString(), out elevation);
                        double mdepth = -9999;
                        Double.TryParse(wbData["MeanDepth"][i].ToString(), out mdepth);
                        double vol = -9999;
                        Double.TryParse(wbData["LakeVolume"][i].ToString(), out vol);


                        List<object> data = new List<object>()
                {
                    comid,
                    gnisName,
                    area,
                    elevation,
                    reachcode,
                    ftype,
                    fcode,
                    mdepth,
                    vol
                };
                        this.waterbodyTable.Add(data);
                    }
                }
            }

            stopwatch.Stop();
            Log.Information("Stream Network - Loaded Waterbodies Runtime: " + stopwatch.Elapsed.TotalSeconds.ToString() + " sec");
        }

        public Dictionary<string, object> ReturnData()
        {
            return new Dictionary<string, object>()
            {
                { "sources", this.networkGraph.GetSources() },
                { "order", this.networkGraph.order },
                { "boundary", new Dictionary<string, object>()
                    {
                        { "headwater", this.networkGraph.GetEdges() },
                        { "out-of-network", this.networkGraph.outNodes },
                        { "divergences", this.divergences },
                    }
                },
                { "divergent-paths", this.divergentPaths },
                { "network", this.networkTable },
                { "waterbodies", new Dictionary<string, object>()
                    {
                        { "comid-wbcomid", this.comidToWB },
                        { "waterbody-table", this.waterbodyTable }
                    } 
                }
            };
        }
    }
}
