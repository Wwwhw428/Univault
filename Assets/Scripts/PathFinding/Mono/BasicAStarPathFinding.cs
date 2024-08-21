using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PathFinding.Mono
{
    public class BasicAStarPathFinding : MonoBehaviour
    {
        public TileBase selectTile;
        public TileBase pathTile;
        public TileBase barrierTile;
        public TileBase normalTile;

        protected Tilemap Tilemap;
        private GetTwoRandomTiles _getTwoRandomTiles;

        private Vector3Int _startPoint;
        protected Vector3Int EndPoint;
        protected PathPoint CurrentPoint;
        private bool _end = false;

        protected List<ClosePoint> ClosePointList = new();

        private void Awake()
        {
            Tilemap = GetComponentInChildren<Tilemap>();
            _getTwoRandomTiles = GetComponentInChildren<GetTwoRandomTiles>();
        }

        protected virtual void Init()
        {
            foreach (var closePoint in ClosePointList)
                Tilemap.SetTile(closePoint.Position, normalTile);
            
            ClosePointList.Clear();

            _end = false;
        }

        public virtual void StartPathFinding()
        {
            Init();

            (_startPoint, EndPoint) = _getTwoRandomTiles.GetRandomTiles();

            CurrentPoint = new PathPoint()
            {
                Position = _startPoint,
                DistanceToEndPoint = Math.Abs(EndPoint.x - _startPoint.x) + Math.Abs(EndPoint.y - _startPoint.y),
            };

            ClosePointList.Add(new ClosePoint()
                { Position = CurrentPoint.Position, ParentPosition = CurrentPoint.Position });
        }

        public virtual void GetWholePath()
        {
            if (CurrentPoint.Position == EndPoint)
            {
                Debug.Log("到达终点");
                return;
            }

            while (true)
            {
                OneStep();

                if (CurrentPoint.Position != EndPoint)
                    Tilemap.SetTile(CurrentPoint.Position, selectTile);
                else
                {
                    BackTrack();
                    _end = true;
                    break;
                }
            }
        }

        public virtual void NextStep()
        {
            if (CurrentPoint.Position == EndPoint)
            {
                if (!_end)
                {
                    BackTrack();
                    _end = true;
                }
                else
                    Debug.Log("到达终点");

                return;
            }

            OneStep();

            if (CurrentPoint.Position != EndPoint)
                Tilemap.SetTile(CurrentPoint.Position, selectTile);
        }

        protected virtual void OneStep()
        {
        }

        protected void BackTrack()
        {
            ClosePointList.RemoveAt(ClosePointList.Count - 1);
            var tempPoint = ClosePointList[^1];

            do
            {
                Tilemap.SetTile(tempPoint.Position, pathTile);
                tempPoint = ClosePointList.Find(p => p.Position == tempPoint.ParentPosition);
            } while (tempPoint.Position != _startPoint);
        }
    }
}