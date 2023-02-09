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

        public async Task<T> Execute<T>(Func<T> action)
        {
            return await Task<T>.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
        
        public async Task Execute(Action action)
        {
            await Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
    }
}