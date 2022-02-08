using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveExtensionsPlayground
{
	class Program
	{
		static void Main(string[] args)
		{
			var cts = new CancellationTokenSource();

			var task1 = awaitTask(1, 100, cts.Token);
			var task2 = awaitTask(2, 500, cts.Token);
			var task3 = awaitTask(3, 300, cts.Token);

			var taskArray = new[] { task1, task2, task3 };

			var taskObservable = taskArray
				.ToObservable()
				.SelectMany(x => x);

			var firstCompleted = taskObservable
				.FirstOrDefaultAsync()
				.Subscribe(i => onFirstCompleted(i, cts), onException);

			var allCompleted = taskObservable
				.Subscribe(i => { }, onAllCompleted);

			Console.ReadLine();
		}

		private static Task<int> awaitTask(int taskNumber, int timeMilliseconds, CancellationToken token)
		{
			return Task.Run(() =>
			{
				Console.WriteLine($"Starting task number {taskNumber}: {timeMilliseconds} ms at {DateTime.Now:hh:mm:ss.fff}");

				var interval = 5;

				var numWaits = timeMilliseconds / interval;

				for (var i = 0; i < numWaits; i++)
				{
					Thread.Sleep(interval);

					if (!token.IsCancellationRequested)
					{
						continue;
					}

					Console.WriteLine($"Cancelled task number {taskNumber} at {i * interval} ms out of {timeMilliseconds}");

					return -1;
				}

				Console.WriteLine($"Completing task number {taskNumber}: {timeMilliseconds} ms");

				return taskNumber;
			}, token);
		}

		private static void onFirstCompleted(int taskNumber, CancellationTokenSource cts)
		{
			Console.WriteLine($"The winner of the race is Task {taskNumber}");

			cts.Cancel();
		}

		private static void onAllCompleted()
		{
			Console.WriteLine("All tasks are done!");
		}

		private static void onException(Exception exception)
		{
			Console.WriteLine($"Error -- {exception.Message}");
		}
	}
}
