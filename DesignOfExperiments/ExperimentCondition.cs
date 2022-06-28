using System.Collections.ObjectModel;
using ReliaCoat.Common;

namespace DesignOfExperiments;

public class ExperimentCondition
{
    #region Properties
    public uint conditionNumber { get; set; }
    public virtual IDictionary<string, object> parameterConditions { get; }
    #endregion

    #region Constructor
    public ExperimentCondition(uint conditionNumber = 0)
    {
        this.conditionNumber = conditionNumber;
        this.parameterConditions = new Dictionary<string, object>();
    }
    #endregion

    #region Methods
    public ExperimentCondition duplicate()
    {
        var experimentCondition = new ExperimentCondition(conditionNumber);

        foreach (var value in parameterConditions)
        {
            experimentCondition.parameterConditions.Add(value.Key, value.Value);
        }

        return experimentCondition;
    }

    public override string ToString()
    {
        return $"{conditionNumber}: {string.Join(", ", parameterConditions.Values.Select(x => x.ToString()))}";
    }
    #endregion
}

public class ExperimentCondition<T> : ExperimentCondition
{
    #region Properties
    public IDictionary<string, T> typedParameterConditions { get; }
    public override IDictionary<string, object> parameterConditions => getParameterConditions();
    #endregion

    #region Constructor
    public ExperimentCondition(uint conditionNumber = 0) : base(conditionNumber)
    {
        typedParameterConditions = new Dictionary<string, T>();
    }
    #endregion

    #region Methods
    public new ExperimentCondition<T> duplicate()
    {
        var experimentCondition = new ExperimentCondition<T>(conditionNumber);

        foreach (var value in typedParameterConditions)
        {
            experimentCondition.typedParameterConditions.Add(value.Key, value.Value);
        }

        return experimentCondition;
    }

    private ReadOnlyDictionary<string, object> getParameterConditions()
    {
        return new ReadOnlyDictionary<string, object>(typedParameterConditions
            .ToDictionary(x => x.Key, x => (object)x.Value));
    }
    #endregion
}