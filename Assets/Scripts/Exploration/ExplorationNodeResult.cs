public class ExplorationNodeResult
{
    public MapNodeIntel Node;
    public MapNodeType ResultType;

    public ExplorationNodeResult(MapNodeIntel node)
    {
        Node = node;
        ResultType = node == null ? MapNodeType.Event : node.ActualNodeType;
    }
}
