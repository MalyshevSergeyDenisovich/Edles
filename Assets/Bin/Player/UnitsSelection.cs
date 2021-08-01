using System.Collections.Generic;
using Bin.Global;
using UnityEngine;


namespace Bin.Player
{
    public class UnitsSelection : MonoBehaviour
    {
        private bool _isDraggingMouseBox = false;
        private Vector3 _dragStartPosition;
        private Ray _ray;
        private RaycastHit _raycastHit;
        private Camera _camera;
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isDraggingMouseBox = true;
                _dragStartPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isDraggingMouseBox = false;
            }

            if (_isDraggingMouseBox && _dragStartPosition != Input.mousePosition)
            {
                _SelectUnitsInDraggingBox();
            }

            if (Globals.SelectedUnits.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    _DeselectAllUnits();
                if (Input.GetMouseButtonDown(0))
                {
                    _ray = _camera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(
                        _ray,
                        out _raycastHit,
                        1000f
                    ))
                    {
                        if (_raycastHit.transform.CompareTag(Globals.TerrainTag))
                            _DeselectAllUnits();
                    }
                }
            }
        }

        private static void _DeselectAllUnits()
        {
            var selectedUnits = new List<UnitManager>(Globals.SelectedUnits);
            foreach (var um in selectedUnits)
                um.Deselect();
        }
        
        private void OnGUI()
        {
            DrawScreenRect();
        }

        
        
        private void DrawScreenRect()
        {
            if (!_isDraggingMouseBox) return;
            var rect = Utils.GetScreenRect(_dragStartPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, Globals.ScreenRectColor);
            Utils.DrawScreenRectBorder(rect, 1, Globals.ScreenRectBorderColor);
        }

        private void _SelectUnitsInDraggingBox()
        {
            var selectionBounds = Utils.GetViewportBounds(
                _camera,
                _dragStartPosition,
                Input.mousePosition
            );
            foreach (var unit in Globals.SelectableUnits)
            {
                var inBounds = selectionBounds.Contains(
                    _camera.WorldToViewportPoint(unit.transform.position)
                );
                if (inBounds)
                    unit.GetComponent<UnitManager>().Select();
                else
                    unit.GetComponent<UnitManager>().Deselect();
            }
        }
    }
}