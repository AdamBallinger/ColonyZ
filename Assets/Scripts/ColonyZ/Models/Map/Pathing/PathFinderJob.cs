using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ColonyZ.Models.Map.Pathing
{
    [BurstCompile]
    public struct PathFinderJob : IJob
    {
        [ReadOnly]
        public int startID;

        [ReadOnly]
        public int endID;

        [ReadOnly]
        public int2 gridSize;
        
        /// <summary>
        ///     Native array containing the data for each node.
        /// </summary>
        public NativeArray<NodeData> graph;
        
        public NativeList<int> openSet;
        public NativeList<int> closedSet;

        /// <summary>
        ///     Native array containing the IDs for the nodes that makeup the resulting path.
        /// </summary>
        public NativeList<int> path;

        /// <summary>
        ///     Flag determining if a valid path was created.
        /// </summary>
        public NativeArray<bool> valid;

        public void Execute()
        {
            var startNode = graph[startID];
            var endNode = graph[endID];
            
            startNode.HCost = Heuristic(startNode, endNode);
            graph[GetIndex(startNode)] = startNode;
            openSet.Add(startNode.ID);

            var neighbourOffsets = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsets[0] = new int2(-1, 0);
            neighbourOffsets[1] = new int2(-1, 1);
            neighbourOffsets[2] = new int2(0, 1);
            neighbourOffsets[3] = new int2(1, 1);
            neighbourOffsets[4] = new int2(1, 0);
            neighbourOffsets[5] = new int2(1, -1);
            neighbourOffsets[6] = new int2(0, -1);
            neighbourOffsets[7] = new int2(-1, -1);

            while (openSet.Length > 0)
            {
                var currentNode = GetCheapestNode();

                closedSet.Add(currentNode.ID);

                if (currentNode.ID == endID)
                {
                    valid[0] = true;
                    RetracePath(currentNode);
                    neighbourOffsets.Dispose();
                    return;
                }

                foreach (var offset in neighbourOffsets)
                {
                    if (!CanMoveInto(currentNode, offset)) continue;
                    var neighbour = GetNode(currentNode.X + offset.x, currentNode.Y + offset.y);
                    if (!neighbour.Pathable || closedSet.Contains(neighbour.ID)) continue;
                    var costToNeighbour = currentNode.GCost + Heuristic(currentNode, neighbour); // TODO: Add movement cost of current node here
                    if (costToNeighbour < neighbour.GCost || !openSet.Contains(neighbour.ID))
                    {
                        neighbour.GCost = costToNeighbour;
                        neighbour.HCost = Heuristic(neighbour, endNode);
                        neighbour.Parent = currentNode.ID;
                        graph[GetIndex(neighbour)] = neighbour;

                        if (!openSet.Contains(neighbour.ID))
                        {
                            openSet.Add(neighbour.ID);
                        }
                    }
                }
            }

            valid[0] = false;
            neighbourOffsets.Dispose();
        }

        private bool CanMoveInto(NodeData _start, int2 _offset)
        {
            if (_offset.x == -1 && _offset.y == 1) // Move to North West
            {
                return GetNode(_start.X - 1, _start.Y).Pathable && GetNode(_start.X, _start.Y + 1).Pathable;
            }
            
            if (_offset.x == 1 && _offset.y == 1) // Move to North East
            {
                return GetNode(_start.X, _start.Y + 1).Pathable && GetNode(_start.X + 1, _start.Y).Pathable;
            }
            
            if (_offset.x == 1 && _offset.y == -1) // Move to South East
            {
                return GetNode(_start.X, _start.Y - 1).Pathable && GetNode(_start.X + 1, _start.Y).Pathable;
            }
            
            if (_offset.x == -1 && _offset.y == -1) // Move to South West
            {
                return GetNode(_start.X, _start.Y - 1).Pathable && GetNode(_start.X - 1, _start.Y).Pathable;
            }
            
            return GetNode(_start.X + _offset.x, _start.Y + _offset.y).Pathable;
        }

        private NodeData GetNode(int _x, int _y)
        {
            if (_x < 0 || _x > gridSize.x) return new NodeData(-1, 0, 0, false);
            if (_y < 0 || _y > gridSize.y) return new NodeData(-1, 0, 0, false);
            return graph[_x * gridSize.x + _y];
        }

        private int GetIndex(NodeData _data)
        {
            return _data.X * gridSize.x + _data.Y;
        }

        private NodeData GetCheapestNode()
        {
            var currentCheapest = graph[openSet[0]];
            var index = 0;

            for (var i = 0; i < openSet.Length; i++)
            {
                var node = graph[openSet[i]];
                if (node.FCost < currentCheapest.FCost)
                {
                    currentCheapest = node;
                    index = i;
                }
            }

            openSet.RemoveAt(index);
            return currentCheapest;
        }

        private void RetracePath(NodeData _currentNode)
        {
            path.Add(_currentNode.ID);
            while (_currentNode.Parent != -1)
            {
                path.Add(_currentNode.Parent);
                _currentNode = graph[_currentNode.Parent];
            }
        }

        private int Heuristic(NodeData _node, NodeData _end)
        {
            var dx = math.abs(_node.X - _end.X);
            var dy = math.abs(_node.Y - _end.Y);
            var remainder = math.abs(dx - dy);

            return 14 * math.min(dx, dy) + 10 * remainder;
        }
    }
}