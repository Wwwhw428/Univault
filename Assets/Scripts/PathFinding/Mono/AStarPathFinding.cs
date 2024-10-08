﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding.Mono
{
    [RequireComponent(typeof(GetTwoRandomTiles))]
    public class AStarPathFinding : BasicAStarPathFinding
    {
        private List<PathPoint> _openList = new();

        private OpenPointComparer _comparer;

        protected override void Init()
        {
            base.Init();
            
            _openList.Clear();
        }

        protected override void OneStep()
        {
            // 左侧点
            var leftPoint = new ClosePoint()
            {
                Position = CurrentPoint.Position + new Vector3Int(-1, 0, 0),
                ParentPosition = CurrentPoint.Position
            };
            if (!ClosePointList.Exists(p => p.Position == leftPoint.Position) &&
                Tilemap.cellBounds.Contains(leftPoint.Position) &&
                Tilemap.GetTile(leftPoint.Position) != barrierTile)
                _openList.Add(new PathPoint()
                {
                    Position = leftPoint.Position,
                    DistanceToEndPoint = Math.Abs(EndPoint.x - leftPoint.Position.x) +
                                         Math.Abs(EndPoint.y - leftPoint.Position.y),
                    ParentPosition = leftPoint.ParentPosition
                });

            // 上侧点
            var upPoint = new ClosePoint()
            {
                Position = CurrentPoint.Position + new Vector3Int(0, 1, 0),
                ParentPosition = CurrentPoint.Position
            };
            if (!ClosePointList.Exists(p => p.Position == upPoint.Position) &&
                Tilemap.cellBounds.Contains(upPoint.Position) &&
                Tilemap.GetTile(upPoint.Position) != barrierTile)
                _openList.Add(new PathPoint()
                {
                    Position = upPoint.Position,
                    DistanceToEndPoint = Math.Abs(EndPoint.x - upPoint.Position.x) +
                                         Math.Abs(EndPoint.y - upPoint.Position.y),
                    ParentPosition = leftPoint.ParentPosition
                });

            // 右侧点
            var rightPoint = new ClosePoint()
            {
                Position = CurrentPoint.Position + new Vector3Int(1, 0, 0),
                ParentPosition = CurrentPoint.Position
            };
            if (!ClosePointList.Exists(p => p.Position == rightPoint.Position) &&
                Tilemap.cellBounds.Contains(rightPoint.Position) &&
                Tilemap.GetTile(rightPoint.Position) != barrierTile)
                _openList.Add(new PathPoint()
                {
                    Position = rightPoint.Position,
                    DistanceToEndPoint =
                        Math.Abs(EndPoint.x - rightPoint.Position.x) + Math.Abs(EndPoint.y - rightPoint.Position.y),
                    ParentPosition = leftPoint.ParentPosition
                });

            // 下侧点
            var bottomPoint = new ClosePoint()
            {
                Position = CurrentPoint.Position + new Vector3Int(0, -1, 0),
                ParentPosition = CurrentPoint.Position
            };
            if (!ClosePointList.Exists(p => p.Position == bottomPoint.Position) &&
                Tilemap.cellBounds.Contains(bottomPoint.Position) &&
                Tilemap.GetTile(bottomPoint.Position) != barrierTile)
                _openList.Add(new PathPoint()
                {
                    Position = bottomPoint.Position,
                    DistanceToEndPoint =
                        Math.Abs(EndPoint.x - bottomPoint.Position.x) + Math.Abs(EndPoint.y - bottomPoint.Position.y),
                    ParentPosition = leftPoint.ParentPosition
                });

            _openList.Sort(_comparer);

            CurrentPoint = _openList[0];
            _openList.RemoveAt(0);
            ClosePointList.Add(new ClosePoint()
            {
                Position = CurrentPoint.Position,
                ParentPosition = CurrentPoint.ParentPosition,
            });
        }

        private struct OpenPointComparer : IComparer<PathPoint>
        {
            public int Compare(PathPoint x, PathPoint y)
            {
                return x.DistanceToEndPoint - y.DistanceToEndPoint;
            }
        }
    }

    public struct PathPoint
    {
        public Vector3Int Position;
        public int DistanceToEndPoint;
        public Vector3Int ParentPosition;
    }

    public struct ClosePoint
    {
        public Vector3Int Position;
        public Vector3Int ParentPosition;
    }
}