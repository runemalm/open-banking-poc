// using System.Text.Json;
// using Demo.Domain.Model;
// using Demo.Domain.Model.BankAccounts;
//
// namespace Demo.Infrastructure.Serialization
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
