using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Bin.Pathfindings
{
    public class PathRequestManager : MonoBehaviour
    {
        private Queue<PathResult> results = new Queue<PathResult>();
        
        private static PathRequestManager _instance;
        private Pathfinding _pathfinding;
        
        private void Awake()
        {
            _instance = this;
            _pathfinding = GetComponent<Pathfinding>();
        }

        private void Update()
        {
            if (results.Count > 0)
            {
                var itemsInQueue = results.Count;
                lock (results)
                {
                    for (var i = 0; i < itemsInQueue; i++)
                    {
                        var result = results.Dequeue();
                        result.callback(result.path, result.success);
                    }
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

        public void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                results.Enqueue(result);   
            }
        }
        

    }
    public struct PathResult
    {
        public Vector3[] path;
        public bool success;
        public Action<Vector3[], bool> callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            this.path = path;
            this.success = success;
            this.callback = callback;
        }
    }
    
    public struct PathRequest
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