using System.Collections.Generic;
using UnityEngine;

namespace Bin.Global
{
    public sealed class UnitManager : MonoBehaviour
    {
        public GameObject selectionCircle;

        private bool _hovered;

        private void OnMouseEnter()
        {
            _hovered = true;
        }

        private void OnMouseExit()
        {
            _hovered = false;
        }

        private void Update()
        {
            if (_hovered && Input.GetMouseButtonDown(0) && IsActive())
                Select(true);
        }

        public void Select(bool clearSelection = false)
        {
            if (Globals.SelectedUnits.Contains(this)) return;
            if (clearSelection)
            {
                var selectedUnits = new List<UnitManager>(Global.Globals.SelectedUnits);
                foreach (var um in selectedUnits)
                    um.Deselect();
            }
            Global.Globals.SelectedUnits.Add(this);
            selectionCircle.SetActive(true);
        }

        public void Deselect()
        {
            if (!Globals.SelectedUnits.Contains(this)) return;
            Globals.SelectedUnits.Remove(this);
            selectionCircle.SetActive(false);
        }

        private static bool IsActive()
        {
            return true;
        }
    }
}