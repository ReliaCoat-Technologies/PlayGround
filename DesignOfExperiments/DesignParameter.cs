namespace DesignOfExperiments;

public abstract class DesignParameter
{
    public string parameterName { get; }
    public abstract object[] values { get; }

    protected DesignParameter(string parameterName)
    {
        this.parameterName = parameterName;
    }
}

public class DesignParameter<T> : DesignParameter
{
    #region Fields
    public override object[] values => typedValues?.Cast<object>().ToArray();
    public T[] typedValues { get; }
    #endregion

    #region Constructor
    public DesignParameter(string parameterName, params T[] valuesInput) : base(parameterName)
    {
        typedValues = valuesInput;
    }
    #endregion
}