using System;
using System.Collections.Generic;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

namespace Saxion.CMGT.Algorithms.sources.Solution.PathFinders;

internal class BreadthFirstPathFinder : PathFinder
{
	public BreadthFirstPathFinder(NodeGraph nodeGraph, Dungeon pDungeon) : base(nodeGraph, pDungeon) {}

	protected override List<Node> Generate(Node pFrom, Node pTo)
	{
		if (pFrom == pTo) return null;
		if (pFrom.connections.Contains(pTo)) return new List<Node> {pTo};
		
		List<Node> shortestPath = new List<Node>();
		Queue<Node> nodesToCheck = new Queue<Node>();
		List<Node> checkedNodes = new List<Node>();
		Dictionary<Node,Node> childParents = new Dictionary<Node,Node>();
		
		nodesToCheck.Enqueue(pFrom);
		
		while (nodesToCheck.Count > 0)
		{
			Node node = nodesToCheck.Dequeue();
			checkedNodes.Add(node);

			Console.WriteLine($"Checking Node: {node.id}");

			foreach (Node connection in node.connections)
			{
				if (checkedNodes.Contains(connection)) continue;
				
				if (connection == pTo)
				{
					if (childParents.ContainsKey(pTo)) childParents[pTo] = node;
					else childParents.Add(pTo, node);

					List<Node> path = new List<Node> {pTo};
					GetParent(connection);

					void GetParent(Node child)
					{
						if (childParents.ContainsKey(child))
						{
							path.Add(child);
							GetParent(childParents[child]);
						}
						else path.Add(pFrom);
					}
					shortestPath = path;
				}

				nodesToCheck.Enqueue(connection);

				if (childParents.ContainsKey(connection)) childParents[connection] = node;
				else childParents.Add(connection, node);
			}
		}
		return shortestPath;
	}
}