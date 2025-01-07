// using System.Text.Json;
// using System.Text.Json.Serialization;
// using Demo.Domain.Model;
//
// namespace Demo.Infrastructure.Serialization
// {
//     public static class TransactionSerializer
//     {
//         private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
//         {
//             PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//             WriteIndented = true,
//             Converters =
//             {
//                 new JsonStringEnumConverter()
//             }
//         };
//
//         public static string Serialize(Transaction transaction)
//         {
//             if (transaction == null) throw new ArgumentNullException(nameof(transaction));
//
//             return JsonSerializer.Serialize(new
//             {
//                 id = transaction.Id,
//                 amount = transaction.Amount.Amount,
//                 currency = transaction.Amount.Currency,
//                 date = transaction.Date,
//                 description = transaction.Description,
//                 type = transaction.Type
//             }, _options);
//         }
//
//         public static Transaction Deserialize(string json)
//         {
//             if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
//
//             var transactionDto = JsonSerializer.Deserialize<TransactionDto>(json, _options);
//             if (transactionDto == null)
//             {
//                 throw new InvalidOperationException("Invalid JSON: Unable to deserialize.");
//             }
//
//             var money = new Money(transactionDto.Amount, transactionDto.Currency);
//             return Transaction.CreateFromSerializedData(
//                 Guid.Parse(transactionDto.Id),
//                 money,
//                 transactionDto.Date,
//                 transactionDto.Description,
//                 transactionDto.Type
//             );
//         }
//
//         private class TransactionDto
//         {
//             public string Id { get; set; }
//             public decimal Amount { get; set; }
//             public CurrencyType Currency { get; set; }
//             public DateTime Date { get; set; }
//             public string Description { get; set; }
//             public TransactionType Type { get; set; }
//         }
//     }
// }
