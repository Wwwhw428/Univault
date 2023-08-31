using UnityEngine;

namespace MeshCuttingModule
{
    public class MeshCutting : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private bool _startDraw = false;
        private Vector3 _startPos;
        private Vector3 _endPos;

        // Start is called before the first frame update
        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1)) 
            {
                _startDraw = false;
                CancelDrawLine();
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (!_startDraw)
                {
                    var mousePos = Input.mousePosition;
                    mousePos.z = Camera.main.nearClipPlane + 1;
                    _startPos = Camera.main.ScreenToWorldPoint(mousePos);

                    _startDraw = true;
                }
                else
                    _startDraw = false;
            }

            if (_startDraw)
            {
                var mousePos = Input.mousePosition;
                mousePos.z = Camera.main.nearClipPlane + 1;
                _endPos = Camera.main.ScreenToWorldPoint(mousePos);
                DrawTemporaryLine();
            }
        }

        private void DrawTemporaryLine()
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, _startPos);
            _lineRenderer.SetPosition(1, _endPos);
        }

        private void CancelDrawLine()
        {
            if (_lineRenderer.positionCount > 0)
                _lineRenderer.positionCount = 0;
        }
    }
}
