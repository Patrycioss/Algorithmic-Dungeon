using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Agent;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Solution.GraphAgents;

internal sealed class SufficientNodeGraphAgent : NodeGraphAgent
{
	private Node currentTarget;
	private Node currentNode;
	private List<Node> queue;

	public SufficientNodeGraphAgent(NodeGraph pNodeGraph) : base(pNodeGraph)
	{
		SetOrigin(width / 2.0f, height / 2.0f);

		if (pNodeGraph.nodes.Count > 0)
		{
			currentNode = pNodeGraph.nodes[Utils.Random(0, pNodeGraph.nodes.Count)];
			JumpToNode(currentNode);
		}
		
		pNodeGraph.onNodeLeftClicked += OnNodeClickHandler;
		queue = new List<Node>();
	}

	private void OnNodeClickHandler(Node pNode)
	{
		queue.Add(pNode);
	}

	protected override void Update()
	{
		if (currentTarget == null)
		{
			if (currentNode.connections.Count == 0) return;
			
			if (queue.Count > 0)
			{
				if (queue[0].connections.Contains(currentNode))
				{
					currentTarget = queue[0];
					queue.Remove(queue[0]);
				}
				else
				{
					queue.Remove(queue[0]);
				}
			} 	
		}
		else
		{
			MoveTowardsNode(currentTarget, 0.5f);
			
			float distance = DistanceFromPointToNode(new Point((int)x,(int)y),currentTarget);
		
			if (distance < 1)
			{
				currentNode = currentTarget;
				currentTarget = null;
			}
		}
	}

	private static float DistanceFromPointToNode(Point point, Node node)
	{
		float a = node.location.X - point.X;
		float b = node.location.Y - point.Y;
		return Mathf.Sqrt(a * a + b * b);	
	}
}