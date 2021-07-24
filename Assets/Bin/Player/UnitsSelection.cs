using System;
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
                _isDraggingMouseBox = false;
            
            if (_isDraggingMouseBox && _dragStartPosition != Input.mousePosition)
                _SelectUnitsInDraggingBox();
            
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
            if (_isDraggingMouseBox)
            {
                // Create a rect from both mouse positions
                var rect = Utils.GetScreenRect(_dragStartPosition, Input.mousePosition);
                Utils.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
                Utils.DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f));
            }
        }

        private void _SelectUnitsInDraggingBox()
        {
            var selectionBounds = Utils.GetViewportBounds(
                _camera,
                _dragStartPosition,
                Input.mousePosition
            );
            var selectableUnits = GameObject.FindGameObjectsWithTag(Globals.UnitTag);
            foreach (var unit in selectableUnits)
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