// using System.Text.Json;
// using Sessions.Domain.Model.User;
//
// namespace Sessions.Infrastructure.Serialization
// {
//     public static class UserSerializer
//     {
//         private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
//         {
//             PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//             WriteIndented = true
//         };
//
//         public static string Serialize(User user)
//         {
//             if (user == null) throw new ArgumentNullException(nameof(user));
//
//             return JsonSerializer.Serialize(new
//             {
//                 id = user.Id,
//                 nin = user.Nin,
//                 name = user.Name
//             }, _options);
//         }
//
//         public static User Deserialize(string json)
//         {
//             if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
//
//             var userDto = JsonSerializer.Deserialize<UserDto>(json, _options);
//             if (userDto == null)
//             {
//                 throw new InvalidOperationException("Invalid JSON: Unable to deserialize.");
//             }
//
//             return User.CreateFromSerializedData(Guid.Parse(userDto.Id), userDto.Nin, userDto.Name);
//         }
//         
//         private class UserDto
//         {
//             public string Id { get; set; }
//             public string Nin { get; set; }
//             public string Name { get; set; }
//         }
//     }
// }
