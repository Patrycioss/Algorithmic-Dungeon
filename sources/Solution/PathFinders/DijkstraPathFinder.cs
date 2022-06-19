using System;
using System.Collections.Generic;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

namespace Saxion.CMGT.Algorithms.sources.Solution.PathFinders;

internal class DijkstraPathFinder : PathFinder
{
	public DijkstraPathFinder(NodeGraph nodeGraph, Dungeon pDungeon) : base(nodeGraph, pDungeon) {}

	protected override List<Node> Generate(Node pFrom, Node pTo)
	{
		List<Node> shortestPath = new List<Node>();
		
		
		if (pFrom.connections.Contains(pTo))
		{
			shortestPath = new List<Node> {pFrom, pTo};
		}
		else
		{
			Queue<Node> nodesToCheck = new Queue<Node>();
			List<Node> checkedNodes = new List<Node>();
			Dictionary<Node,Node> childParents = new Dictionary<Node,Node>();
			Dictionary<Node,float> nodeDistances = new Dictionary<Node,float>();

			
			//Set everything to max except start
			foreach (Node existingNode in nodeGraph.nodes) nodeDistances.Add(existingNode,float.MaxValue);
			nodeDistances[pFrom] = 0;
			checkedNodes.Add(pFrom);


			//Start with pFrom
			nodesToCheck.Enqueue(pFrom);
			
			
			
			while (nodesToCheck.Count > 0)
			{
				Node node = nodesToCheck.Dequeue();

				Console.WriteLine($"{node.id} has value: {nodeDistances[node]}");


				foreach (Node connection in node.connections)
				{
					if (excludedNodes.Contains(connection)) continue;
					
					float value = GetDistanceFromNodeToNode(node, connection) + nodeDistances[node];
					if (value < nodeDistances[connection]) nodeDistances[connection] = value;
					
					if (checkedNodes.Contains(connection)) continue;
					
					if (!childParents.ContainsKey(connection)) childParents.Add(connection,node);

					checkedNodes.Add(connection);

					if (connection == pTo)
					{
						GetParent(pTo);
					
						void GetParent(Node child)
						{
							Node bestConnection = null;

							Console.WriteLine($"Child: {child.id}, value: {nodeDistances[child]}");
							shortestPath.Add(child);

							foreach (Node newConnection in child.connections)
							{
								if (excludedNodes.Contains(node)) continue;
								if (shortestPath.Contains(newConnection)) continue;
							
								if (newConnection == pFrom)
								{
									shortestPath.Add(pFrom);
									return;
								}
								
								if (nodeDistances.ContainsKey(newConnection))
								{
									if (bestConnection == null || nodeDistances[newConnection] < nodeDistances[bestConnection])
									{
										bestConnection = newConnection;
									}
								}
							}

							if (bestConnection != null) GetParent(bestConnection);
						}
						
						nodesToCheck.Clear();
					}
					else
					{
						nodesToCheck.Enqueue(connection);
					}
				}
			}
		}

		Console.WriteLine($"PathLength: {shortestPath.Count}");
		return shortestPath;
	}

	private float GetDistanceFromNodeToNode(Node nodeA, Node nodeB)
	{
		float distance = Mathf.Sqrt(Mathf.Pow(nodeA.location.X - nodeB.location.X, 2) + Mathf.Pow(nodeA.location.Y - nodeB.location.Y, 2)); 

		return distance;
	}
}