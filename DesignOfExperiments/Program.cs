using DesignOfExperiments;

var exposureTimes = new DesignParameter<double>("Exposure Time (µs)", 50, 100, 150);
var gains = new DesignParameter<double>("Gain", 1, 5, 10);
var injection = new DesignParameter<double>("Injection", 2, 3, 5);

var experiments = FullFactorialDesigner.designExperiment(true, exposureTimes, gains, injection)
    .ToList();

Console.WriteLine($"Number of Experiments: {experiments.Count}");

foreach (var experiment in experiments)
{
    Console.WriteLine(experiment.ToString());
}

Console.ReadKey();