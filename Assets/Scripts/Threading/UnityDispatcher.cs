using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NineEightOhThree.Threading
{
    public class UnityDispatcher : MonoBehaviour
    {
        public static UnityDispatcher Instance { get; set; }
        
        private TaskScheduler scheduler;

        private void Awake()
        {
            Instance ??= this;
            if (Instance != this) Destroy(gameObject);
            
            scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        public async Task<T> Execute<T>(Func<T> func)
        {
            return await Task<T>.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, scheduler)
                .ContinueWith(t =>
            {
                if (t.IsFaulted) Logger.LogError(t.Exception);
                return t.Result;
            }, scheduler);
        }
        
        public async Task Execute(Action action)
        {
            await Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, scheduler)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted) Logger.LogError(t.Exception?.ToString());
                }, scheduler);
        }

        public void Run(Func<Task> action)
        {
            Task.Factory.StartNew(async () => await action(), CancellationToken.None, TaskCreationOptions.None, scheduler)
                .ContinueWith(t =>
            {
                if (t.IsFaulted) Logger.LogError(t.Exception?.ToString());
            });
        }
    }
}