using Amazon.DynamoDBv2.DocumentModel;
using DynamoDbMapper.Sdk.Entities;

namespace DynamoDbMapper.Sdk.Extensions;

public static class DynamoDbConditionExtensions
{
    public static QueryFilter ToQueryFilter(this IList<DynamoDbCondition> conditions)
    {
        var queryFilter = new QueryFilter();
        foreach (var condition in conditions)
            queryFilter.AddCondition(condition.PropertyName, condition.Operator.ToQueryOperator(), condition.Value);

        return queryFilter;
    }
    public static ScanFilter ToScanFilter(this IList<DynamoDbCondition> conditions)
    {
        var queryFilter = new ScanFilter();
        foreach (var condition in conditions)
            queryFilter.AddCondition(condition.PropertyName, condition.Operator.ToScanOperator(), condition.Value);

        return queryFilter;
    }
    
    public static QueryOperator ToQueryOperator(this DynamoDbOperator @operator)
    {
        switch (@operator)
        {
            case DynamoDbOperator.Equal:
                return QueryOperator.Equal;
            case DynamoDbOperator.LessThanOrEqual:
                return QueryOperator.LessThanOrEqual;
            case DynamoDbOperator.LessThan:
                return QueryOperator.LessThan;
            case DynamoDbOperator.GreaterThanOrEqual:
                return QueryOperator.GreaterThanOrEqual;
            case DynamoDbOperator.GreaterThan:
                return QueryOperator.GreaterThan;
            case DynamoDbOperator.BeginsWith:
                return QueryOperator.BeginsWith;
            case DynamoDbOperator.Between:
                return QueryOperator.Between;
            default:
                throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null);
        }
    }
    
    public static ScanOperator ToScanOperator(this DynamoDbOperator @operator)
    {
        switch (@operator)
        {
            case DynamoDbOperator.Equal:
                return ScanOperator.Equal;
            case DynamoDbOperator.LessThanOrEqual:
                return ScanOperator.LessThanOrEqual;
            case DynamoDbOperator.LessThan:
                return ScanOperator.LessThan;
            case DynamoDbOperator.GreaterThanOrEqual:
                return ScanOperator.GreaterThanOrEqual;
            case DynamoDbOperator.GreaterThan:
                return ScanOperator.GreaterThan;
            case DynamoDbOperator.BeginsWith:
                return ScanOperator.BeginsWith;
            case DynamoDbOperator.Between:
                return ScanOperator.Between;
            default:
                throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null);
        }
    }
}