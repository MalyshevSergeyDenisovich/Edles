using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Grid = Bin.Map.Grid;

namespace Bin.Pathfindings
{
    public class PathRequestManager : MonoBehaviour
    {
        private readonly Queue<PathResult> _results = new Queue<PathResult>();
        
        private static PathRequestManager _instance;
        private Pathfinding _pathfinding;
        
        private void Awake()
        {
            _instance = this;
            _pathfinding = new Pathfinding(GetComponent<Grid>());
        }

        private void Update()
        {
            DoOnUpdate();
        }

        private void DoOnUpdate()
        {
            if (_results.Count <= 0) return;
            var itemsInQueue = _results.Count;
            lock (_results)
            {
                for (var i = 0; i < itemsInQueue; i++)
                {
                    var result = _results.Dequeue();
                    result.Callback(result.Path, result.Success);
                }
            }
        }

        public static void RequestPath(PathRequest request)
        {
            ThreadStart threadStart = delegate
            {
                _instance._pathfinding.FindPath(request, _instance.FinishedProcessingPath);
            };
            threadStart.Invoke();
        }

        private void FinishedProcessingPath(PathResult result)
        {
            lock (_results)
            {
                _results.Enqueue(result);   
            }
        }
        

    }
    public readonly struct PathResult
    {
        public readonly Vector3[] Path;
        public readonly bool Success;
        public readonly Action<Vector3[], bool> Callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            Path = path;
            Success = success;
            Callback = callback;
        }
    }
    
    public struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public readonly Action<Vector3[], bool> Callback;

        public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathStart = pathStart;
            PathEnd = pathEnd;
            Callback = callback;
        }
    }
}