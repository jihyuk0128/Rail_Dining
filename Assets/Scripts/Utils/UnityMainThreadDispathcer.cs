using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private static readonly Queue<Action> _executionQueue = new();

    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            // 이미 존재하면 바로 리턴
            if (_instance != null)
                return _instance;

            // 존재하지 않아도 GameObject를 "찾기"만 시도
            _instance = FindObjectOfType<UnityMainThreadDispatcher>();
            if (_instance != null)
                return _instance;

            // 서브스레드라면 GameObject 생성하지 않음
            if (!Application.isPlaying || !UnityMainThreadDispatcher.IsMainThread)
            {
                Debug.LogWarning("[Dispatcher] 생성 시도: 메인스레드가 아님!");
                return null;
            }

            // 메인스레드일 때만 GameObject 생성
            var obj = new GameObject("UnityMainThreadDispatcher");
            _instance = obj.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(obj);
            return _instance;
        }
    }

    private static int _mainThreadId;
    public static bool IsMainThread => _mainThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId;

    private void Awake()
    {
        _mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Enqueue(Action action)
    {
        if (action == null)
            return;

        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                var action = _executionQueue.Dequeue();
                action?.Invoke();
            }
        }
    }
}