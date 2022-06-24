using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

namespace Saxion.CMGT.Algorithms.sources.Solution.PathFinders;

internal class DijkstraPathFinder : PathFinder
{
	private List<Node> nodesToCheck;
	private List<Node> checkedNodes;
	private Dictionary<Node, float> nodeDistances;


	
	public DijkstraPathFinder(NodeGraph nodeGraph, Dungeon pDungeon, bool debugging) : base(nodeGraph, pDungeon, debugging) {}

	protected override List<Node> Generate(Node pFrom, Node pTo)
	{
		if (pTo == pFrom) return null;
		if (pFrom.connections.Contains(pTo)) return new List<Node> {pTo, pFrom};

		nodesToCheck = new List<Node>();
		checkedNodes = new List<Node>();
		nodeDistances = new Dictionary<Node, float>();
		
		List<Node> shortestPath = null;
		
		int nodesExpanded = 0;
		
		//Set everything to max except start
		foreach (Node existingNode in nodeGraph.nodes) nodeDistances.Add(existingNode, float.MaxValue);
		nodeDistances[pFrom] = 0;
		checkedNodes.Add(pFrom);

		//Start with pFrom
		nodesToCheck.Add(pFrom);
		
		while (nodesToCheck.Count > 0)
		{
			Node node = nodesToCheck[0];
			nodesToCheck.RemoveAt(0);
			
			//Info
			nodesExpanded++;
			if (debugMode)
			{
				Console.WriteLine($"{node.id} has value: {nodeDistances[node]}");
				AlgorithmsAssignment.DrawCross(node.location.X,node.location.Y);
			}
			//

			if (AlgorithmsAssignment.DO_WONKY_STEP_BY_STEP)
			{
				DrawNode(node,Brushes.Red);
				Thread.Sleep(10);
			}
			
			foreach (Node connection in node.connections)
			{
				if (excludedNodes.Contains(connection)) continue;

				float value = GetDistanceFromNodeToNode(node, connection) + nodeDistances[node];
				if (value < nodeDistances[connection]) nodeDistances[connection] = value;

				if (checkedNodes.Contains(connection)) continue;
				if (!nodesToCheck.Contains(connection)) nodesToCheck.Add(connection);

				checkedNodes.Add(connection);

				if (connection == pTo)
				{
					shortestPath = new List<Node>();
					
					GetParent(pTo);

					void GetParent(Node child)
					{
						Node bestConnection = null;

						if (debugMode) Console.WriteLine($"Child: {child.id}, value: {nodeDistances[child]}");
						shortestPath.Add(child);
						
						foreach (Node newConnection in child.connections)
						{
							if (newConnection == pFrom)
							{
								shortestPath.Add(pFrom);
								return;
							}
							if (nodeDistances.ContainsKey(newConnection) && (bestConnection == null || nodeDistances[newConnection] < nodeDistances[bestConnection])) 
								bestConnection = newConnection;
						}
						if (bestConnection != null) GetParent(bestConnection);
					}
					nodesToCheck.Clear();
					break;
				}
				if (debugMode) Console.WriteLine(nodesToCheck.Count);
				SortNodesToCheck();
			}
		}

		if (debugMode)
		{
			if (shortestPath != null) Console.WriteLine($"PathLength: {shortestPath.Count}");
			Console.WriteLine($"NodesExpanded: {nodesExpanded}");
		}
		return shortestPath;
	}

	private static float GetDistanceFromNodeToNode(Node nodeA, Node nodeB) => 
		Mathf.Sqrt(Mathf.Pow(nodeA.location.X - nodeB.location.X, 2) + Mathf.Pow(nodeA.location.Y - nodeB.location.Y, 2));

	private void SortNodesToCheck()
	{
		for (int i = nodesToCheck.Count - 1; i >= 0; i--)
		{
			for (int j = i - 1; j >= 0; j--)
			{
				float iDistance = nodeDistances[nodesToCheck[i]];
				float jDistance = nodeDistances[nodesToCheck[j]];
				Node tempI = nodesToCheck[i];
			
				if (iDistance < jDistance)
				{
					nodesToCheck[i] = nodesToCheck[j];
					nodesToCheck[j] = tempI;
				}
			}
		}
	}
}