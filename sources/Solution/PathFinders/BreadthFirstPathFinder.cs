using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

namespace Saxion.CMGT.Algorithms.sources.Solution.PathFinders;

internal class BreadthFirstPathFinder : PathFinder
{
	public BreadthFirstPathFinder(NodeGraph nodeGraph, Dungeon pDungeon, bool debugging) : base(nodeGraph, pDungeon, debugging) {}

	protected override List<Node> Generate(Node pFrom, Node pTo)
	{
		if (pFrom == pTo) return null;
		if (pFrom.connections.Contains(pTo)) return new List<Node> {pTo, pFrom};

		List<Node> shortestPath = null;
		Queue<Node> nodesToCheck = new Queue<Node>();
		List<Node> checkedNodes = new List<Node>();
		Dictionary<Node,Node> childParents = new Dictionary<Node,Node>();
		
		nodesToCheck.Enqueue(pFrom);
		
		while (nodesToCheck.Count > 0)
		{
			Node node = nodesToCheck.Dequeue();
			checkedNodes.Add(node);

			if (debugMode) Console.WriteLine($"Checking Node: {node.id}");

			if (AlgorithmsAssignment.DO_WONKY_STEP_BY_STEP)
			{
				DrawNode(node,Brushes.Red);
				Thread.Sleep(10);
			}

			foreach (Node connection in node.connections)
			{
				if (checkedNodes.Contains(connection) || nodesToCheck.Contains(connection)) continue;
				
				if (connection == pTo)
				{
					if (childParents.ContainsKey(pTo)) childParents[pTo] = node;
					else childParents.Add(pTo, node);


					if (debugMode)
					{
						Console.WriteLine($"-----");
						Console.WriteLine($"Making path...");
						Console.WriteLine($"-----");
					}

					if (AlgorithmsAssignment.DO_WONKY_STEP_BY_STEP)
					{
						DrawNode(node,Brushes.Red);
						Thread.Sleep(10);
					}
					
					List<Node> path = new List<Node> {pTo};
					GetParent(connection);

					void GetParent(Node child)
					{
						if (childParents.ContainsKey(child))
						{
							path.Add(child);
							if (debugMode) Console.WriteLine($"Added {child} to path");
							GetParent(childParents[child]);
						}
						else
						{
							path.Add(pFrom);
							if (debugMode) Console.WriteLine($"Added start: {pFrom}, path finished.");
						}
					}
					shortestPath = path;
				}

				nodesToCheck.Enqueue(connection);

				if (childParents.ContainsKey(connection)) childParents[connection] = node;
				else childParents.Add(connection, node);
			}
		}
		if (debugMode && shortestPath != null) Console.WriteLine($"Path length: {shortestPath.Count}");
		return shortestPath;
	}
}