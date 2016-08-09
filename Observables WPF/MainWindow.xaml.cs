using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;

namespace Observables_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            var query = Enumerable.Range(1, 25).Select(writeString);
            var observable = query.ToObservable(Scheduler.Default);
            observable.ObserveOnDispatcher().Subscribe(ItemList.AppendText, onError, onCompletion);
        }

        private string writeString(int input)
        {
            Thread.Sleep(250);
            return $"{input}, {Thread.CurrentThread.ManagedThreadId}\n";
        }

        private void onError(Exception error)
        {
            MessageBox.Show(error.Message);
        }

        private void onCompletion()
        {
            MessageBox.Show("Completed!");
        }
    }
}
