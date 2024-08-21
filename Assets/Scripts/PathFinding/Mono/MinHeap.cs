using System.Collections.Generic;

namespace PathFinding.Mono
{
    public class MinHeap
    {
        private List<PathPoint> _nodes;

        public MinHeap()
        {
            _nodes = new List<PathPoint>();
        }

        public void Init()
        {
            _nodes.Clear();
        }

        public PathPoint Pop()
        {
            var res = _nodes[0];

            _nodes.RemoveAt(0);

            if (_nodes.Count > 1)
            {
                _nodes.Insert(0, _nodes[^1]);
                _nodes.RemoveAt(_nodes.Count - 1);

                var leftIndex = 1;
                var rightIndex = 2;

                var currentIndex = 0;

                do
                {
                    var leftNode = _nodes[leftIndex];
                    if (rightIndex < _nodes.Count)
                    {
                        var rightNode = _nodes[rightIndex];

                        if (leftNode.DistanceToEndPoint < rightNode.DistanceToEndPoint &&
                            leftNode.DistanceToEndPoint < _nodes[currentIndex].DistanceToEndPoint)
                        {
                            Swap(leftIndex, currentIndex);
                            currentIndex = leftIndex;
                        }
                        else if (rightNode.DistanceToEndPoint < leftNode.DistanceToEndPoint &&
                                 rightNode.DistanceToEndPoint < _nodes[currentIndex].DistanceToEndPoint)
                        {
                            Swap(rightIndex, currentIndex);
                            currentIndex = rightIndex;
                        }
                        else
                            break;
                    }
                    else if (leftNode.DistanceToEndPoint < _nodes[currentIndex].DistanceToEndPoint)
                    {
                        Swap(leftIndex, currentIndex);
                        currentIndex = leftIndex;
                    }
                    else
                    {
                        break;
                    }

                    leftIndex = currentIndex * 2 + 1;
                    rightIndex = currentIndex * 2 + 2;
                } while (leftIndex < _nodes.Count);
            }

            return res;
        }

        public void AddNode(PathPoint node)
        {
            var currentIndex = _nodes.Count;
            var parentIndex = (currentIndex - 1) / 2;

            _nodes.Add(node);

            do
            {
                if (_nodes[currentIndex].DistanceToEndPoint < _nodes[parentIndex].DistanceToEndPoint)
                    Swap(currentIndex, parentIndex);

                currentIndex = parentIndex;
                parentIndex = (currentIndex - 1) / 2;

                if (currentIndex == 0)
                    break;
            } while (parentIndex >= 0);
        }

        private void Swap(int a, int b)
        {
            (_nodes[a], _nodes[b]) = (_nodes[b], _nodes[a]);
        }
    }
}