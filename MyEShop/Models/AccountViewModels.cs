using System.ComponentModel.DataAnnotations;

namespace MyEShop.Models
{
    public class RegisterViewModel
    {

        [Display(Name = "نام کاربری")]
        [MaxLength(300)] 
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")] 
        public string UserName { get; set; }



        [Display(Name = "رمز عبور")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Password { get; set; }



        [Display(Name = "تکرار رمز عبور  ")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string RePassword { get; set; }


    }


    public class LoginViewModel
    {
        [Display(Name = "نام کاربری")]
        [MaxLength(300)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string UserName { get; set; }



        [Display(Name = "رمز عبور")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]

        public string Password { get; set; }



        [Display(Name = "مرا به خاطر بسپار")]
        public  bool ReMemberMe { get; set; }



    }
}
