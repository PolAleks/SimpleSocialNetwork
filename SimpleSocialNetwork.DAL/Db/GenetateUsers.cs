using SimpleSocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSocialNetwork.DAL.Db
{
    public class GenetateUsers
    {
        private readonly string[] maleNames = new string[] { "Александр", "Алексей", "Андрей", "Антон", "Вадим", "Василий", "Виктор", "Виталий", "Георгий", "Глеб", "Давид", "Дамир", "Даниил", "Кирилл", "Константин", "Максим", "Марк", "Матвей", "Петр" };
        private readonly string[] femaleNames = new string[] { "Аглая", "Ангелина", "Анисия", "Анфиса", "Ариадна", "Арина", "Василиса", "Варвара", "Глафира", "Илона", "Кира", "Моника", "Элина", "Владислава", "Дарина", "Есения", "Забава", "Злата", "Иванна" };
        private readonly string[] lastNames = new string[] { "Иванов", "Кузнецов", "Попов", "Соколов", "Лебедев", "Козлов", "Новиков", "Морозов", "Петров", "Волков", "Соловьёв", "Васильев", "Зайцев", "Павлов", "Семёнов", "Голубев", "Виноградов", "Богданов", "Воробьёв" };

        private Random random = new Random();
        public IEnumerable<User> Populate(int count)
        {
            var users = new List<User>();
            for (int i = 1; i < count; i++)
            {
                string firstName;

                var male = random.Next(0, 2) == 1;

                var lastName = lastNames[random.Next(0, lastNames.Length - 1)];
                if (male)
                {
                    firstName = maleNames[random.Next(0, maleNames.Length - 1)];
                }
                else
                {
                    lastName = lastName + "a";
                    firstName = femaleNames[random.Next(0, femaleNames.Length - 1)];
                }

                var item = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = DateTime.Now.AddDays(-random.Next(1, (DateTime.Now - DateTime.Now.AddYears(-25)).Days)),
                    Email = "test" + random.Next(0, 10000) + "@test.com",
                };

                item.UserName = item.Email;
                item.Image = "https://thispersondoesnotexist.com/";

                users.Add(item);
            }

            return users;
        }
    }
}
