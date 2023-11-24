#resource "aws_dynamodb_table_item" "user_data_1" {
#    table_name = "${aws_dynamodb_table.dynamodb_mapper_example.name}"
#    hash_key = "${aws_dynamodb_table.dynamodb_mapper_example.hash_key}"
#    range_key = "${aws_dynamodb_table.dynamodb_mapper_example.range_key}"
#    for_each = {
#        "02a01310-d8a0-48c0-a655-9755a91b4aff" = {
#            ForeingKey = "02a01310-d8a0-48c0-a655-9755a91b4aff",
#            EntityType = "User",
#            InheritedType = "User",
#            GSI-PrimaryKey = "admin@admin.com",
#            GSI-PrimaryForeingKey = "admin",
#            Nome = "Admin",
#            PasswordForeingKey = "AQAAAAEAACcQAAAAEEHP6dksHxraYptLNNadEse/60t177wcgZs6ST66LR9xBzx883uvUVu1DeDyaOExkA==",
#            Roles = "['ac337365-e690-4c75-9f05-e5ea75caa1e5']"
#        },
#        "c27b1bdc-9a32-4f77-a23f-62ba371c8741" = {
#            ForeingKey = "c27b1bdc-9a32-4f77-a23f-62ba371c8741",
#            EntityType = "User",
#            InheritedType = "User",
#            GSI-PrimaryKey = "another@user.com",
#            GSI-PrimaryForeingKey = "nayara",
#            Nome = "Another User",
#            PasswordForeingKey = "AQAAAAEAACcQAAAAEEHP6dksHxraYptLNNadEse/60t177wcgZs6ST66LR9xBzx883uvUVu1DeDyaOExkA==",
#            Roles = "['0439ac1d-c8aa-4d9b-b7b7-68d5c562eac7']"
#        }
#    }
#    item = <<ITEM
#    {
#        "Id": {"S": "${each.key}"},
#        "ForeingKey": {"S": "${each.value.ForeingKey}"},
#        "EntityType": {"S": "${each.value.EntityType}"},
#        "InheritedType": {"S": "${each.value.InheritedType}"},
#        "GSI-PrimaryKey": {"S": "${each.value.GSI-PrimaryKey}"},
#        "GSI-PrimaryForeingKey": {"S": "${each.value.GSI-PrimaryForeingKey}"},
#        "Nome": {"S": "${each.value.Nome}"},
#        "PasswordForeingKey": {"S": "${each.value.PasswordForeingKey}"},
#        "Roles": {"S": "${each.value.Roles}"}
#    }  
#    ITEM
#
#    depends_on = [
#        aws_dynamodb_table.dynamodb_mapper_example
#    ]
#}