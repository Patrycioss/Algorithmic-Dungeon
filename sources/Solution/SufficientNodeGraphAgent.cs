using System;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine;
using GXPEngine.Core;
using Saxion.CMGT.Algorithms.sources.Assignment.Agent;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Solution;

/**
 * Very simple example of a nodegraphagent that walks directly to the node you clicked on,
 * ignoring walls, connections etc.
 */
internal sealed class SufficientNodeGraphAgent : NodeGraphAgent
{
	//Current target to move towards
	private Node target;

	private Node currentTarget;

	private NodeGraph nodeGraph;

	private Node currentNode;
	private Node previousNode;

	private List<Node> queue;

	public SufficientNodeGraphAgent(NodeGraph pNodeGraph) : base(pNodeGraph)
	{
		SetOrigin(width / 2.0f, height / 2.0f);

		//position ourselves on a random node
		if (pNodeGraph.nodes.Count > 0)
		{
			currentNode = pNodeGraph.nodes[Utils.Random(0, pNodeGraph.nodes.Count)];
			JumpToNode(currentNode);
		}

		//listen to node clicks
		pNodeGraph.onNodeLeftClicked += OnNodeClickHandler;

		nodeGraph = pNodeGraph;
		
		queue = new List<Node>();
	}

	private void OnNodeClickHandler(Node pNode)
	{
		queue.Add(pNode);
	}
	
	private Node GetClosestConnection()
	{
		(Node node, float distance) closest = (null,float.MaxValue);

		foreach (Node node in currentNode.connections)
		{
			if (node == previousNode) continue;
			
			float newDistance = DistanceFromNodeToNode(node,target);
			
			if (closest.node == null || newDistance < closest.distance)
			{
				closest = (node, newDistance);
			}
		}

		if (closest.node != null)
		{
			Console.WriteLine(closest.node.location);
		}
		
		return closest.node;
	}

	protected override void Update()
	{
		if (currentTarget == null)
		{
			if (queue.Count > 0)
			{
				Console.WriteLine("ja");
				Console.WriteLine(queue[0].connections.Count);
				
				if (queue[0].connections.Contains(currentNode))
				{
					currentTarget = queue[0];
					queue.Remove(queue[0]);
				}
				else
				{
					queue.Remove(queue[0]);
					Console.WriteLine("doesn't connect");
				}
			} 	
		}
		else
		{
			MoveTowardsNode(currentTarget, 0.5f);

			float distance = DistanceFromPointToNode(new Point((int)x,(int)y),currentTarget);
			Console.WriteLine(distance);

			Console.WriteLine(currentTarget.location);
			Console.WriteLine($"{x}, {y}");
			
			if (distance < 1)
			{
				previousNode = currentNode;
				currentNode = currentTarget;
				currentTarget = null;
				// currentTarget = GetClosestConnection();
			
				Console.WriteLine("ja");
			}
		}
	}

	private float DistanceFromPointToNode(Point point, Node node)
	{
		float a = node.location.X - point.X;
		float b = node.location.Y - point.Y;
		return Mathf.Sqrt(a * a + b * b);	
	}

	private float DistanceFromNodeToNode(Node nodeA, Node nodeB)
	{
		float a = nodeA.location.X - nodeB.location.X;
		float b = nodeA.location.Y - nodeB.location.Y;
		return Mathf.Sqrt(a * a + b * b);
	}
}