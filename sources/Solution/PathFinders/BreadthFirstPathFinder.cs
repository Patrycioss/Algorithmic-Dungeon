using System;
using System.Collections.Generic;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

namespace Saxion.CMGT.Algorithms.sources.Solution.PathFinders;

internal class BreadthFirstPathFinder : PathFinder
{
	public BreadthFirstPathFinder(NodeGraph nodeGraph) : base(nodeGraph) {}

	protected override List<Node> Generate(Node pFrom, Node pTo)
	{
		List<Node> shortestPath = new List<Node>();
		Queue<Node> nodesToCheck = new Queue<Node>();
		List<Node> checkedNodes = new List<Node>();
		Dictionary<Node,Node> childParents = new Dictionary<Node,Node>();


		nodesToCheck.Enqueue(pFrom);
		
		if (pFrom.connections.Contains(pTo))
		{
			shortestPath = new List<Node> {pFrom, pTo};
		}
		else
		{
			while (nodesToCheck.Count > 0)
			{
				Node node = nodesToCheck.Dequeue();
				checkedNodes.Add(node);

				Console.WriteLine($"Checking Node: {node.id}");
			
				foreach (Node connection in node.connections)
				{
					if (checkedNodes.Contains(connection)) continue;
					if (nodesToCheck.Contains(connection)) continue;
					
					if (connection == pTo)
					{
						childParents.Add(pTo,node);
						
						List<Node> path = new List<Node>{pTo};

						GetParent(connection);

						void GetParent(Node child)
						{
							if (childParents.ContainsKey(child))
							{
								path.Add(child);
								GetParent(childParents[child]);
							}
							else
							{
								path.Add(pFrom);
							}
						}
						shortestPath = path;
					}
					nodesToCheck.Enqueue(connection);
				
					if (childParents.ContainsKey(connection)) childParents[connection] = node;
					else childParents.Add(connection,node);
				}
			}
		}

		return shortestPath;
	}
}