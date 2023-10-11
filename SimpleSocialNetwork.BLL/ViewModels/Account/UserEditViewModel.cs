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
        public string UserId { get; set; }

        [Required(ErrorMessage = "Это обязательное поле")]
        [Display(Name = "Имя", Prompt = "Введите ваше имя")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Display(Name = "Отчество", Prompt = "Введите ваше отчество")]
        [DataType(DataType.Text)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Это обязательное поле")]
        [Display(Name = "Фамилия", Prompt = "Введите вашу фамилию")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Это обязательное поле")]
        [Display(Name = "День рождения")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Это обязательное поле")]
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
        public string UserName => Email;
    }
}
