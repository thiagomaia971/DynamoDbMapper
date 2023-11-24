resource "aws_dynamodb_table" "dynamodb_mapper_example" {
    name = "DynamoDbMapperExample"
    billing_mode = "PROVISIONED"
    read_capacity = 5
    write_capacity = 5
    hash_key = "Id"
    range_key = "CreatedAt"

    attribute {
        name = "Id"
        type = "S"
    }

    attribute {
        name = "CreatedAt"
        type = "S"
    }

    attribute {
        name = "ForeingKey"
        type = "S"
    }

    attribute {
        name = "EntityType"
        type = "S"
    }

    attribute {
        name = "InheritedType"
        type = "S"
    }

    attribute {
        name = "UserId"
        type = "S"
    }

    attribute {
        name = "GSI-PrimaryKey"
        type = "S"
    }

    attribute {
        name = "GSI-PrimaryForeingKey"
        type = "S"
    }

    global_secondary_index {
        hash_key = "CreatedAt"
        name = "GSI-CreatedAt"
        projection_type = "ALL"
        read_capacity = 100
        write_capacity = 100
    }

    global_secondary_index {
        hash_key = "ForeingKey"
        name = "GSI-ForeingKey"
        projection_type = "ALL"
        read_capacity = 100
        write_capacity = 100
    }

    global_secondary_index {
        hash_key = "EntityType" 
        name = "GSI-EntityType"
        projection_type = "ALL"
        read_capacity = 100
        write_capacity = 100
    }

    global_secondary_index {
        hash_key = "InheritedType" 
        name = "GSI-InheritedType"
        projection_type = "ALL"
        read_capacity = 100
        write_capacity = 100
    }

    global_secondary_index {
        hash_key = "UserId"
        name = "GSI-UserId"
        projection_type = "ALL"
        read_capacity = 100
        write_capacity = 100
    }

    global_secondary_index {
        hash_key = "GSI-PrimaryKey"
        name = "GSI-PrimaryKey"
        projection_type = "ALL"
        read_capacity = 100
        write_capacity = 100
    }

    global_secondary_index {
        hash_key = "GSI-PrimaryForeingKey"
        name = "GSI-PrimaryForeingKey"
        projection_type = "ALL"
        read_capacity = 100
        write_capacity = 100
    }
}