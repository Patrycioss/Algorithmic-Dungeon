using System;
using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.sources.Assignment.Agent;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Solution.GraphAgents;


internal sealed class ExcellentNodeGraphAgent : NodeGraphAgent
{
	//Current target to move towards
	private Node target;

	private Node currentTarget;
	private Node goal;
	private bool canReceiveNewGoal;

	private Node currentNode;
	private Node previousNode;

	private Random random;

	public ExcellentNodeGraphAgent(NodeGraph pNodeGraph) : base(pNodeGraph)
	{
		random = new Random();
		
		SetOrigin(width / 2.0f, height / 2.0f);

		//position ourselves on a random node
		if (pNodeGraph.nodes.Count > 0)
		{
			currentNode = pNodeGraph.nodes[random.Next(0, pNodeGraph.nodes.Count)];
			JumpToNode(currentNode);
		}

		//listen to node clicks
		pNodeGraph.onNodeLeftClicked += OnNodeClickHandler;

		goal = null;

		canReceiveNewGoal = true;
	}
	

	private void OnNodeClickHandler(Node pNode)
	{
		if (canReceiveNewGoal)
		{
			goal = pNode;
			canReceiveNewGoal = false;
		}
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
		if (goal != null)
		{
			if (currentTarget == null)
			{
				if (currentNode.connections.Contains(goal)) currentTarget = goal;

				List<Node> connections = currentNode.connections;
				connections.Remove(previousNode);

				int i = random.Next(0, connections.Count);

				currentTarget = connections[i];
					
			}
			else
			{
				MoveTowardsNode(currentTarget, 0.5f);

				float distance = DistanceFromPointToNode(new Point((int)x,(int)y),currentTarget);

				if (distance < 1)
				{
					previousNode = currentNode;
					currentNode = currentTarget;
					currentTarget = null;

					if (currentNode == goal)
					{
						canReceiveNewGoal = true;
						goal = null;
					}
					
				}	
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