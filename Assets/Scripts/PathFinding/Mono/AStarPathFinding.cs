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
        public TileBase pathTile;
        public TileBase barrierTile;
        public TileBase normalTile;

        private Tilemap _tilemap;
        private GetTwoRandomTiles _getTwoRandomTiles;

        private Vector3Int _startPoint;
        private Vector3Int _endPoint;
        private PathPoint _currentPoint;
        private bool _end = false;

        private List<PathPoint> _openList = new();
        private List<ClosePoint> _closePointList = new();

        private OpenPointComparer _comparer;

        private void Awake()
        {
            _tilemap = GetComponentInChildren<Tilemap>();
            _getTwoRandomTiles = GetComponentInChildren<GetTwoRandomTiles>();
        }

        public void GetWholePath()
        {
            if (_currentPoint.Position == _endPoint)
            {
                Debug.Log("到达终点");
                return;
            }

            while (true)
            {
                OneStep();

                if (_currentPoint.Position != _endPoint)
                    _tilemap.SetTile(_currentPoint.Position, selectTile);
                else
                {
                    BackTrack();
                    _end = true;
                    break;
                }
            }
        }

        public void StartPathFinding()
        {
            Init();

            (_startPoint, _endPoint) = _getTwoRandomTiles.GetRandomTiles();

            _currentPoint = new PathPoint()
            {
                Position = _startPoint,
                DistanceToEndPoint = Math.Abs(_endPoint.x - _startPoint.x) + Math.Abs(_endPoint.y - _startPoint.y),
            };

            _closePointList.Add(new ClosePoint()
                { Position = _currentPoint.Position, ParentPosition = _currentPoint.Position });
        }

        public void NextStep()
        {
            if (_currentPoint.Position == _endPoint && !_end)
            {
                BackTrack();
                _end = true;
                return;
            }
            else if (_currentPoint.Position == _endPoint)
            {
                Debug.Log("到达终点");
                return;
            }

            OneStep();

            if (_currentPoint.Position != _endPoint)
                _tilemap.SetTile(_currentPoint.Position, selectTile);
        }

        private void Init()
        {
            foreach (var closePoint in _closePointList)
                _tilemap.SetTile(closePoint.Position, normalTile);

            _openList.Clear();
            _closePointList.Clear();

            _end = false;
        }

        private void OneStep()
        {
            // 左侧点
            var leftPoint = new ClosePoint()
            {
                Position = _currentPoint.Position + new Vector3Int(-1, 0, 0),
                ParentPosition = _currentPoint.Position
            };
            if (!_closePointList.Exists(p => p.Position == leftPoint.Position) &&
                _tilemap.cellBounds.Contains(leftPoint.Position) &&
                _tilemap.GetTile(leftPoint.Position) != barrierTile)
                _openList.Add(new PathPoint()
                {
                    Position = leftPoint.Position,
                    DistanceToEndPoint = Math.Abs(_endPoint.x - leftPoint.Position.x) +
                                         Math.Abs(_endPoint.y - leftPoint.Position.y),
                    ParentPosition = leftPoint.ParentPosition
                });

            // 上侧点
            var upPoint = new ClosePoint()
            {
                Position = _currentPoint.Position + new Vector3Int(0, 1, 0),
                ParentPosition = _currentPoint.Position
            };
            if (!_closePointList.Exists(p => p.Position == upPoint.Position) &&
                _tilemap.cellBounds.Contains(upPoint.Position) &&
                _tilemap.GetTile(upPoint.Position) != barrierTile)
                _openList.Add(new PathPoint()
                {
                    Position = upPoint.Position,
                    DistanceToEndPoint = Math.Abs(_endPoint.x - upPoint.Position.x) +
                                         Math.Abs(_endPoint.y - upPoint.Position.y),
                    ParentPosition = leftPoint.ParentPosition
                });

            // 右侧点
            var rightPoint = new ClosePoint()
            {
                Position = _currentPoint.Position + new Vector3Int(1, 0, 0),
                ParentPosition = _currentPoint.Position
            };
            if (!_closePointList.Exists(p => p.Position == rightPoint.Position) &&
                _tilemap.cellBounds.Contains(rightPoint.Position) &&
                _tilemap.GetTile(rightPoint.Position) != barrierTile)
                _openList.Add(new PathPoint()
                {
                    Position = rightPoint.Position,
                    DistanceToEndPoint =
                        Math.Abs(_endPoint.x - rightPoint.Position.x) + Math.Abs(_endPoint.y - rightPoint.Position.y),
                    ParentPosition = leftPoint.ParentPosition
                });

            // 下侧点
            var bottomPoint = new ClosePoint()
            {
                Position = _currentPoint.Position + new Vector3Int(0, -1, 0),
                ParentPosition = _currentPoint.Position
            };
            if (!_closePointList.Exists(p => p.Position == bottomPoint.Position) &&
                _tilemap.cellBounds.Contains(bottomPoint.Position) &&
                _tilemap.GetTile(bottomPoint.Position) != barrierTile)
                _openList.Add(new PathPoint()
                {
                    Position = bottomPoint.Position,
                    DistanceToEndPoint =
                        Math.Abs(_endPoint.x - bottomPoint.Position.x) + Math.Abs(_endPoint.y - bottomPoint.Position.y),
                    ParentPosition = leftPoint.ParentPosition
                });

            _openList.Sort(_comparer);

            _currentPoint = _openList[0];
            _openList.RemoveAt(0);
            _closePointList.Add(new ClosePoint()
            {
                Position = _currentPoint.Position,
                ParentPosition = _currentPoint.ParentPosition,
            });
        }

        private void BackTrack()
        {
            _closePointList.RemoveAt(_closePointList.Count - 1);
            var tempPoint = _closePointList[^1];

            do
            {
                _tilemap.SetTile(tempPoint.Position, pathTile);
                tempPoint = _closePointList.Find(p => p.Position == tempPoint.ParentPosition);
            } while (tempPoint.Position != _startPoint);
        }

        private struct PathPoint
        {
            public Vector3Int Position;
            public int DistanceToEndPoint;
            public Vector3Int ParentPosition;
        }

        private struct ClosePoint
        {
            public Vector3Int Position;
            public Vector3Int ParentPosition;
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