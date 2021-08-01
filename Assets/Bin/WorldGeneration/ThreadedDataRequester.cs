using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Bin.WorldGeneration
{
	public class ThreadedDataRequester : MonoBehaviour
	{
		private static ThreadedDataRequester _instance;
		private Queue<ThreadInfo> _dataQueue = new Queue<ThreadInfo>();

		private void Awake()
		{
			_instance = FindObjectOfType<ThreadedDataRequester>();
		}

		public static void RequestData(Func<object> generateData, Action<object> callback)
		{
			ThreadStart threadStart = delegate
			{
				_instance.DataThread(generateData, callback);
			};

			new Thread(threadStart).Start();
		}

		private void DataThread(Func<object> generateData, Action<object> callback)
		{
			var data = generateData();
			lock (_dataQueue)
			{
				_dataQueue.Enqueue(new ThreadInfo(callback, data));
			}
		}


		private void Update() 
		{
			if (_dataQueue.Count > 0) {
				for (int i = 0; i < _dataQueue.Count; i++)
				{
					ThreadInfo threadInfo = _dataQueue.Dequeue();
					threadInfo.callback(threadInfo.parameter);
				}
			}
		}

		struct ThreadInfo
		{
			public readonly Action<object> callback;
			public readonly object parameter;

			public ThreadInfo (Action<object> callback, object parameter)
			{
				this.callback = callback;
				this.parameter = parameter;
			}

		}
	}
}
