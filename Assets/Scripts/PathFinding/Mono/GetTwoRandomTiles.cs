using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace PathFinding.Mono
{
    [RequireComponent(typeof(Grid))]
    public class GetTwoRandomTiles : MonoBehaviour
    {
        public TileBase startTile;
        public TileBase endTile;
        public TileBase barrierTile;
        public TileBase normalTile;

        private Tilemap _tilemap;
        private Vector3Int _startPoint;
        private Vector3Int _endPoint;

        public void Awake()
        {
            _tilemap = GetComponentInChildren<Tilemap>();
        }

        public (Vector3Int, Vector3Int) GetRandomTiles()
        {
            RevertRandomTiles();
            var bounds = _tilemap.cellBounds;

            var rand = new System.Random();

            do
            {
                _startPoint = new Vector3Int(
                    rand.Next(bounds.xMin, bounds.xMax),
                    rand.Next(bounds.yMin, bounds.yMax),
                    0);
            } while (_tilemap.GetTile(_startPoint).name == barrierTile.name);

            do
            {
                _endPoint = new Vector3Int(
                    rand.Next(bounds.xMin, bounds.xMax),
                    rand.Next(bounds.yMin, bounds.yMax),
                    0);
            } while (_startPoint == _endPoint && _tilemap.GetTile(_endPoint).name == barrierTile.name);

            _tilemap.SetTile(_startPoint, startTile);
            _tilemap.SetTile(_endPoint, endTile);

            return (_startPoint, _endPoint);
        }

        public void RevertRandomTiles()
        {
            _tilemap.SetTile(_startPoint, normalTile);
            _tilemap.SetTile(_endPoint, normalTile);
        }
    }
}