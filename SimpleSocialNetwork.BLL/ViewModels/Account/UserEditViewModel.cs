using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSocialNetwork.BLL.ViewModels.Account
{
    public class UserEditViewModel
    {
        [Required]
        [Display(Name = "Идентификатор пользователя")]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Имя")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Display(Name = "Отчество")]
        [DataType(DataType.Text)]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "День рождения")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required]
        [Display(Name = "Почта")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Ваше фото")]
        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }

        [Display(Name = "Мой статус")]
        [DataType(DataType.Text)]
        public string Status { get; set; }

        [Display(Name = "Коротко о себе")]
        [DataType(DataType.Text)]
        public string About { get; set; }
    }
}
