#resource "aws_dynamodb_table_item" "role_data" {
#    table_name = "${aws_dynamodb_table.dynamodb_mapper_example.name}"
#    hash_key = "${aws_dynamodb_table.dynamodb_mapper_example.hash_key}"
#    range_key = "${aws_dynamodb_table.dynamodb_mapper_example.range_key}"
#    for_each = {
#        "ac337365-e690-4c75-9f05-e5ea75caa1e5" = {
#            ForeingKey = "ac337365-e690-4c75-9f05-e5ea75caa1e5",
#            EntityType = "Role",
#            InheritedType = "Role",
#            Nome = "Admin"
#        },
#        "0439ac1d-c8aa-4d9b-b7b7-68d5c562eac7" = {
#            ForeingKey = "0439ac1d-c8aa-4d9b-b7b7-68d5c562eac7",
#            EntityType = "Role",
#            InheritedType = "Role",
#            Nome = "Generic"
#        }
#    }
#    item = <<ITEM
#    {
#        "Id": {"S": "${each.key}"},
#        "ForeingKey": {"S": "${each.value.ForeingKey}"},
#        "EntityType": {"S": "${each.value.EntityType}"},
#        "InheritedType": {"S": "${each.value.InheritedType}"},
#        "Nome": {"S": "${each.value.Nome}"}
#    }  
#    ITEM
#
#    depends_on = [
#        aws_dynamodb_table.dynamodb_mapper_example
#    ]
#}