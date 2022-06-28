using ReliaCoat.Common;

namespace DesignOfExperiments;

public static class FullFactorialDesigner
{
    public static IEnumerable<ExperimentCondition> designExperiment(bool randomizeOrder, params DesignParameter[] designParameters)
    {
        var experimentConditionList = new List<ExperimentCondition>();

        foreach (var parameter in designParameters)
        {
            // Initialize experiment list
            if (!experimentConditionList.Any())
            {
                experimentConditionList.AddRange(parameter.values.Select(x => createNewExperiment(parameter, x)));
                
                continue;
            }

            var experimentsToAdd = experimentConditionList
                .SelectMany(x => parameter.values, (a, b) => addParameterToExperiment(a, parameter, b))
                .ToList();

            experimentConditionList = experimentsToAdd;
        }

        if (randomizeOrder)
        {
            experimentConditionList = experimentConditionList.shuffle().ToList();
        }

        for (var i = 0; i < experimentConditionList.Count; i++)
        {
            experimentConditionList[i].conditionNumber = (uint)i + 1;
        }

        return experimentConditionList;
    }
    
    public static IEnumerable<ExperimentCondition<T>>designExperiment<T>(bool randomizeOrder, params DesignParameter<T>[] designParameters)
    {
        var experimentConditionList = new List<ExperimentCondition<T>>();

        foreach (var parameter in designParameters)
        {
            // Initialize experiment list
            if (!experimentConditionList.Any())
            {
                experimentConditionList.AddRange(parameter.typedValues.Select(x => createNewExperiment<T>(parameter, x)));
                
                continue;
            }

            var experimentsToAdd = experimentConditionList
                .SelectMany(x => parameter.typedValues, (a, b) => addParameterToExperiment<T>(a, parameter, b))
                .ToList();

            experimentConditionList = experimentsToAdd;
        }

        if (randomizeOrder)
        {
            experimentConditionList = experimentConditionList.shuffle().ToList();
        }

        for (var i = 0; i < experimentConditionList.Count; i++)
        {
            experimentConditionList[i].conditionNumber = (uint)i + 1;
        }

        return experimentConditionList;
    }

    private static ExperimentCondition createNewExperiment(DesignParameter parameter, object itemToAdd)
    {
        return new ExperimentCondition
        {
            parameterConditions =
            {
                [parameter.parameterName] = itemToAdd
            }
        };
    }

    private static ExperimentCondition<T> createNewExperiment<T>(DesignParameter parameter, T itemToAdd)
    {
        return new ExperimentCondition<T>
        {
            typedParameterConditions =
            {
                [parameter.parameterName] = itemToAdd
            }
        };
    }

    private static ExperimentCondition addParameterToExperiment(ExperimentCondition experimentCondition, DesignParameter parameter, object itemToAdd)
    {
        var experimentToReturn = experimentCondition.duplicate();

        experimentToReturn.parameterConditions.Add(parameter.parameterName, itemToAdd);

        return experimentToReturn;
    }

    private static ExperimentCondition<T> addParameterToExperiment<T>(ExperimentCondition<T> experimentCondition, DesignParameter parameter, T itemToAdd)
    {
        var experimentToReturn = experimentCondition.duplicate();

        experimentToReturn.typedParameterConditions.Add(parameter.parameterName, itemToAdd);

        return experimentToReturn;
    }
}