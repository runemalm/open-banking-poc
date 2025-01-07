// using System.Text.Json;
// using Demo.Domain.Model;
//
// namespace Demo.Infrastructure.Serialization
// {
//     public static class TransactionHistorySerializer
//     {
//         private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
//         {
//             PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//             WriteIndented = true
//         };
//
//         public static string Serialize(TransactionHistory history)
//         {
//             if (history == null) throw new ArgumentNullException(nameof(history));
//
//             return JsonSerializer.Serialize(new
//             {
//                 transactions = history.Transactions.Select(TransactionSerializer.Serialize)
//             }, _options);
//         }
//
//         public static TransactionHistory Deserialize(string json)
//         {
//             if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
//
//             var historyDto = JsonSerializer.Deserialize<TransactionHistoryDto>(json, _options);
//             if (historyDto == null)
//             {
//                 throw new InvalidOperationException("Invalid JSON: Unable to deserialize.");
//             }
//
//             var transactions = historyDto.Transactions
//                 .Select(TransactionSerializer.Deserialize)
//                 .ToList();
//
//             return TransactionHistory.Create(
//                 transactions
//             );
//         }
//
//         private class TransactionHistoryDto
//         {
//             public string UserId { get; set; }
//             public List<string> Transactions { get; set; }
//         }
//     }
// }
