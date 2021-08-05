using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bin.Pathfindings
{
    public class PathRequestManager : MonoBehaviour
    {
        private readonly Queue<PathRequest> _pathRequestsQueue = new Queue<PathRequest>();
        private PathRequest _currentPathRequest;

        private static PathRequestManager _instance;
        private Pathfinding _pathfinding;
        
        private bool _isProcessingPath;
        
        private void Awake()
        {
            _instance = this;
            _pathfinding = GetComponent<Pathfinding>();
        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            var newRequest = new PathRequest(pathStart, pathEnd, callback);
            _instance._pathRequestsQueue.Enqueue(newRequest);
            _instance.TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (!_isProcessingPath && _pathRequestsQueue.Count > 0)
            {
                _currentPathRequest = _pathRequestsQueue.Dequeue();
                _isProcessingPath = true;
                _pathfinding.StartFindPath(_currentPathRequest.pathStart, _currentPathRequest.pathEnd);
            }
        }

        public void FinishedProcessingPath(Vector3[] path, bool success)
        {
            _currentPathRequest.callback(path, success);
            _isProcessingPath = false;
            TryProcessNext();
        }

        private struct PathRequest
        {
            public Vector3 pathStart;
            public Vector3 pathEnd;
            public Action<Vector3[], bool> callback;

            public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
            {
                this.pathStart = pathStart;
                this.pathEnd = pathEnd;
                this.callback = callback;
            }
        }
    }
}