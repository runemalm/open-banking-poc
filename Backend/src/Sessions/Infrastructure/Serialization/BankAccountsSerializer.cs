// using System.Text.Json;
// using Sessions.Domain.Model;
// using Sessions.Domain.Model.BankAccounts;
//
// namespace Sessions.Infrastructure.Serialization
// {
//     public static class BankAccountsSerializer
//     {
//         public static string Serialize(BankAccounts bankAccounts)
//         {
//             return JsonSerializer.Serialize(bankAccounts.Accounts, new JsonSerializerOptions
//             {
//                 WriteIndented = false
//             });
//         }
//
//         public static BankAccounts Deserialize(string json)
//         {
//             var accounts = JsonSerializer.Deserialize<List<BankAccount>>(json, new JsonSerializerOptions());
//             return BankAccounts.Create(accounts ?? new List<BankAccount>());
//         }
//     }
// }
