using System.Collections.Generic;
using System.Linq;
using Bin.Global.ActionStates;
using UnityEngine;
using UnityEngine.Serialization;


namespace Bin.Global
{
    public sealed class UnitManager : MonoBehaviour, IActionStateSwitcher
    {
        #region UnitSelection
        
        private GameObject _selectionCircle;
        private bool _hovered;
        
        public void Select(bool clearSelection = false)
        {
            if (Globals.SelectedUnits.Contains(this)) return;
            if (clearSelection)
            {
                var selectedUnits = new List<UnitManager>(Globals.SelectedUnits);
                foreach (var um in selectedUnits)
                    um.Deselect();
            }
            Globals.SelectedUnits.Add(this);
            _selectionCircle.SetActive(true);
        }

        public void Deselect()
        {
            if (!Globals.SelectedUnits.Contains(this)) return;
            Globals.SelectedUnits.Remove(this);
            _selectionCircle.SetActive(false);
        }

        private static bool IsActive()
        {
            return true;
        }
        #endregion

        #region Unity

        private void Awake()
        {
            _animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
            
            DeclareActions();
            
            _selectionCircle = transform.GetChild(1).gameObject;


            targetPos = transform.position + new Vector3(1, 0, 1);
            _curState = _allActionStates[0];

            
            
            Globals.SelectableUnits.Add(this);
        }

        private void Start()
        {
            _curState.Start();
        }

        private void OnMouseEnter()
        {
            _hovered = true;
        }

        private void OnMouseExit()
        {
            _hovered = false;
        }

        private void FixedUpdate()
        {
            if (_hovered && Input.GetMouseButtonDown(0) && IsActive())
                Select(true);
            
            _curState.OnUpdateCheck();
        }

        #endregion
        

        #region ActionStates

        public Animator _animator;
        
        private ActionState _curState;
        public Vector3 targetPos;
        
        #endregion

        public void SetTargetPoint(Vector3 target)
        {
            targetPos += target;
        }

        private List<ActionState> _allActionStates;

        private void DeclareActions()
        {
            _allActionStates = new List<ActionState>()
            {
                new IdleState(this, _animator, this),
                new MoveState(this, _animator, this),
                new RandomMoveState(this, _animator, this)
            };
        }

        public void SwitchState<T>() where T : ActionState
        {
            var state = _allActionStates.FirstOrDefault(s => s is T);
            _curState.Stop();
            state.Start();
            _curState = state;
        }
    }
}