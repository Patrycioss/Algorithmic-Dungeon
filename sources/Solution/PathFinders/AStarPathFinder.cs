using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

namespace Saxion.CMGT.Algorithms.sources.Solution.PathFinders;

internal class AStarPathFinder : PathFinder
{
	private List<Node> nodesToCheck;
	private Dictionary<Node, (float distanceToStart, float distanceToEnd, float fCost, bool visited, Node parent)> nodeInformation;
	
	public AStarPathFinder(NodeGraph pNodeGraph, Dungeon pDungeon, bool debugging) : base(pNodeGraph, pDungeon, debugging) { }


	protected override List<Node> Generate(Node pFrom, Node pTo)
	{
		if (pFrom == pTo) return null;
		if (pFrom.connections.Contains(pTo)) return new List<Node> {pFrom, pTo};
		
		List<Node> shortestPath = new List<Node>();
		nodesToCheck = new List<Node>();
		nodeInformation = new Dictionary<Node, (float distanceToStart, float distanceToEnd, float fCost, bool visited, Node parent)>();

		//Preset information to all nodes
		foreach (Node node in nodeGraph.nodes) nodeInformation.Add(node, (float.MaxValue, GetDistanceFromNodeToNode(node, pTo), float.MaxValue, false, null));

		float distanceFromStartToEnd = GetDistanceFromNodeToNode(pFrom, pTo);
		nodeInformation[pFrom] = (0,distanceFromStartToEnd,distanceFromStartToEnd,false,null);
		
		nodesToCheck.Add(pFrom);

		SortNodesToCheck();

		int nodesExpanded = 0;
		
		while (nodesToCheck.Count > 0)
		{
			Node node = nodesToCheck[0];
			nodesToCheck.RemoveAt(0);

			//Debug
			if (debugMode)
			{
				Console.WriteLine($"---------------");
				Console.WriteLine($"Checking Node: {node} with heuristic: {nodeInformation[node].fCost}");
			}
			
			if (AlgorithmsAssignment.DO_WONKY_STEP_BY_STEP)
			{
				DrawNode(node,Brushes.Red);
				Thread.Sleep(100);
			}

			
			//Update node information to say it's been visited
			(float distanceToStart, float distanceToEnd, float fCost, bool visited, Node parent) previousInformation = nodeInformation[node];
			previousInformation.visited = true;
			nodeInformation[node] = previousInformation;

			//Info
			nodesExpanded++;
			//

			//Make a path if it reaches the end
			if (node.connections.Contains(pTo))
			{
				DetermineNewNodeInformation(pTo,node);
				
				AddParent(pTo);
				
				void AddParent(Node child)
				{
					shortestPath.Add(child);
				
					Node tempParent = nodeInformation[child].parent;

					if (tempParent == pFrom) shortestPath.Add(pFrom);
					else AddParent(tempParent);
				}
				break;
			}
			
			
			foreach (Node connection in node.connections)
			{
				if (excludedNodes.Contains(connection)) continue;

				//Updates node information
				DetermineNewNodeInformation(connection,node);
				
				//Debug
				if (debugMode) Console.WriteLine($"Connection with id: {connection} now has heuristic {nodeInformation[connection].fCost}");

				if (!nodeInformation[connection].visited) nodesToCheck.Add(connection);
			}
			
			SortNodesToCheck();
		}
		

		//Debug
		if (debugMode)
		{
			Console.WriteLine($"PathLength: {shortestPath.Count}");
			Console.WriteLine($"NodesExpanded: {nodesExpanded}");
		}
		return shortestPath;
	}

	private static float GetDistanceFromNodeToNode(Node nodeA, Node nodeB) => Mathf.Sqrt(Mathf.Pow(nodeA.location.X - nodeB.location.X, 2) + Mathf.Pow(nodeA.location.Y - nodeB.location.Y, 2));

	private void DetermineNewNodeInformation(Node node, Node newParent)
	{
		(float distanceToStart, float distanceToEnd, float fCost, bool visited, Node parent) previousInformation = nodeInformation[node];
		
		float newDistanceToStart = nodeInformation[newParent].distanceToStart + GetDistanceFromNodeToNode(node, newParent);
		if (previousInformation.distanceToStart > newDistanceToStart) previousInformation.distanceToStart = newDistanceToStart;
				
		float fCost = previousInformation.distanceToStart + previousInformation.distanceToEnd;
		if (previousInformation.parent == null || Math.Abs(previousInformation.fCost - fCost) > 0.001f)
		{
			previousInformation.parent = newParent;
		}

		previousInformation.fCost = fCost;
				
		nodeInformation[node] = previousInformation;
	}
	
	private void SortNodesToCheck()
	{
		for (int i = nodesToCheck.Count - 1; i >= 0; i--)
		{
			if (nodeInformation[nodesToCheck[i]].visited)
			{
				nodesToCheck.Remove(nodesToCheck[i]);
				continue;
			}
			for (int j = i - 1; j >= 0; j--)
			{
				float iHeuristic = nodeInformation[nodesToCheck[i]].fCost;
				float jHeuristic = nodeInformation[nodesToCheck[j]].fCost;
				Node tempI = nodesToCheck[i];
			
				if (iHeuristic < jHeuristic)
				{
					nodesToCheck[i] = nodesToCheck[j];
					nodesToCheck[j] = tempI;
				}
				// else if (Math.Abs(iHeuristic - jHeuristic) < 0.001f)
				// {
				// 	float iDistanceToEnd = nodeInformation[nodesToCheck[i]].distanceToEnd;
				// 	float jDistanceToEnd = nodeInformation[nodesToCheck[j]].distanceToEnd;
				//
				// 	if (iDistanceToEnd < jDistanceToEnd)
				// 	{
				// 		nodesToCheck[i] = nodesToCheck[j];
				// 		nodesToCheck[j] = tempI;
				// 	}
				// }
			}
		}
		
		for (int i = nodesToCheck.Count - 1; i >= 0; i--)
		{
			for (int j = i - 1; j >= 0; j--)
			{
				float iHeuristic = nodeInformation[nodesToCheck[i]].fCost;
				float jHeuristic = nodeInformation[nodesToCheck[j]].fCost;
				Node tempI = nodesToCheck[i];
				
				if (Math.Abs(iHeuristic - jHeuristic) < 0.001f)
				{
					float iDistanceToEnd = nodeInformation[nodesToCheck[i]].distanceToEnd;
					float jDistanceToEnd = nodeInformation[nodesToCheck[j]].distanceToEnd;
				
					if (iDistanceToEnd < jDistanceToEnd)
					{
						nodesToCheck[i] = nodesToCheck[j];
						nodesToCheck[j] = tempI;
					}
				}
			}
		}
	}
}