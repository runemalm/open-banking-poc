using DDD.Domain.Model;

namespace Demo.Domain.Model.User
{
    public class User : IValueObject
    {
        public string Nin { get; private set; }
        public string Name { get; private set; }
        
        private User() { }

        public static User Create(string nin, string name)
        {
            return new User()
            {
                Nin = nin ?? throw new ArgumentNullException(nameof(nin)),
                Name = name ?? throw new ArgumentNullException(nameof(name))
            };
        }
    }
}
