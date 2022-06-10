using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Assignment.Agent;

/**
 * Very simple example of a nodegraphagent that walks directly to the node you clicked on,
 * ignoring walls, connections etc.
 */
internal sealed class SampleNodeGraphAgent : NodeGraphAgent
{
	//Current target to move towards
	private Node target;

	public SampleNodeGraphAgent(NodeGraph.NodeGraph pNodeGraph) : base(pNodeGraph)
	{
		SetOrigin(width / 2.0f, height / 2.0f);

		//position ourselves on a random node
		if (pNodeGraph.nodes.Count > 0)
		{
			JumpToNode(pNodeGraph.nodes[Utils.Random(0, pNodeGraph.nodes.Count)]);
		}

		//listen to node clicks
		pNodeGraph.onNodeLeftClicked += OnNodeClickHandler;
	}

	private void OnNodeClickHandler(Node pNode)
	{
		target = pNode;
		
		
	}

	protected override void Update()
	{
		//no target? Don't walk
		if (target == null) return;

		//Move towards the target node, if we reached it, clear the target
		if (MoveTowardsNode(target))
		{
			target = null;
		}
	}
}