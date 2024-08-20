using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PathFinding.Mono
{
    [RequireComponent(typeof(GetTwoRandomTiles))]
    public class AStarPathFinding : MonoBehaviour
    {
        public TileBase selectTile;
        public TileBase barrierTile;
        public TileBase normalTile;

        private Tilemap _tilemap;
        private GetTwoRandomTiles _getTwoRandomTiles;

        private Vector3Int _startPoint;
        private Vector3Int _endPoint;

        private List<PathPoint> _openList = new();
        private List<Vector3Int> _closeList = new();

        private OpenPointComparer _comparer;

        private void Awake()
        {
            _tilemap = GetComponentInChildren<Tilemap>();
            _getTwoRandomTiles = GetComponentInChildren<GetTwoRandomTiles>();
        }

        public void GetPath()
        {
            Init();
            
            (_startPoint, _endPoint) = _getTwoRandomTiles.GetRandomTiles();

            var startPoint = new PathPoint()
            {
                Position = _startPoint,
                DistanceToEndPoint = Math.Abs(_endPoint.x - _startPoint.x) + Math.Abs(_endPoint.y - _startPoint.y)
            };
            
            while (true)
            {
                // 左侧点
                var leftPoint = startPoint.Position + new Vector3Int(-1, 0, 0);
                if (!_closeList.Contains(leftPoint) && _tilemap.cellBounds.Contains(leftPoint) &&
                    _tilemap.GetTile(leftPoint).name != barrierTile.name)
                    _openList.Add(new PathPoint()
                    {
                        Position = leftPoint,
                        DistanceToEndPoint = Math.Abs(_endPoint.x - leftPoint.x) + Math.Abs(_endPoint.y - leftPoint.y),
                    });

                // 上侧点
                var upPoint = startPoint.Position + new Vector3Int(0, 1, 0);
                if (!_closeList.Contains(upPoint) && _tilemap.cellBounds.Contains(upPoint) &&
                    _tilemap.GetTile(leftPoint).name != barrierTile.name)
                    _openList.Add(new PathPoint()
                    {
                        Position = upPoint,
                        DistanceToEndPoint = Math.Abs(_endPoint.x - upPoint.x) + Math.Abs(_endPoint.y - upPoint.y),
                    });

                // 右侧点
                var rightPoint = startPoint.Position + new Vector3Int(1, 0, 0);
                if (!_closeList.Contains(rightPoint) && _tilemap.cellBounds.Contains(rightPoint) &&
                    _tilemap.GetTile(leftPoint).name != barrierTile.name)
                    _openList.Add(new PathPoint()
                    {
                        Position = rightPoint,
                        DistanceToEndPoint =
                            Math.Abs(_endPoint.x - rightPoint.x) + Math.Abs(_endPoint.y - rightPoint.y),
                    });

                // 下侧点
                var bottomPoint = startPoint.Position + new Vector3Int(0, -1, 0);
                if (!_closeList.Contains(bottomPoint) && _tilemap.cellBounds.Contains(bottomPoint) &&
                    _tilemap.GetTile(leftPoint).name != barrierTile.name)
                    _openList.Add(new PathPoint()
                    {
                        Position = bottomPoint,
                        DistanceToEndPoint =
                            Math.Abs(_endPoint.x - bottomPoint.x) + Math.Abs(_endPoint.y - bottomPoint.y),
                    });

                _openList.Sort(_comparer);

                startPoint = _openList[0];
                _openList.RemoveAt(0);
                _closeList.Add(startPoint.Position);

                if (startPoint.Position != _endPoint)
                    _tilemap.SetTile(startPoint.Position, selectTile);
                else
                    break;
            }
        }

        private void Init()
        {
            foreach (var closePoint in _closeList)
                _tilemap.SetTile(closePoint, normalTile);
            
            _openList.Clear();
            _closeList.Clear();
        }
        
        private struct PathPoint
        {
            public Vector3Int Position;
            public int DistanceToEndPoint;

            public PathPoint(Vector3Int position, int distanceToEndPoint)
            {
                Position = position;
                DistanceToEndPoint = distanceToEndPoint;
            }
        }

        private struct OpenPointComparer : IComparer<PathPoint>
        {
            public int Compare(PathPoint x, PathPoint y)
            {
                return x.DistanceToEndPoint - y.DistanceToEndPoint;
            }
        }
    }
}