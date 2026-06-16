using Microsoft.EntityFrameworkCore;
using MyEShop.Models;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace MyEShop.Data
{
    public class MyShopContext:DbContext
    {
        public MyShopContext(DbContextOptions<MyShopContext>options):base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryToProducts> CategoryToProducts  { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Users> User { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Address> Addresses { get; set; }

       

         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {





            modelBuilder.Entity<Users>(c =>
            {

                c.HasKey(r => r.UserId);
                c.Property(r => r.UserName).HasMaxLength(200);
                c.Property(r => r.Password).HasMaxLength(50);
               


            });

            modelBuilder.Entity<CategoryToProducts>(c =>
            {
                c.HasKey(T => new
                {
                    T.CategoryId,
                    T.ProductId

                });
                c.HasOne(e => e.Category).WithMany(x => x.CategoryToProducts).HasForeignKey(v => v.CategoryId);
                c.HasOne(e => e.Product).WithMany(x => x.CategoryToProducts).HasForeignKey(v => v.ProductId);
             
            });
            modelBuilder.Entity<Product>(c =>
            {
               c.HasData(new Product()
               {
                   Id = 1,
                   ItemId = 1,
                   Name = "آموزش Asp.Net Core 3 پروژه محور",
                   Description =
                        "آموزش Asp.Net Core 3 پروژه محور ASP.NET Core بر پایه‌ی NET Core.استوار است و نگارشی از NET.محسوب می شود که مستقل از سیستم عامل و بدون واسط برنامه نویسی ویندوز عمل می کند.ویندوز هنوز هم سیستم عاملی برتر به حساب می آید ولی برنامه های وب نه تنها روز به روز از کاربرد و اهمیت بیشتری برخوردار می‌شوند بلکه باید بر روی سکوهای دیگری مانند فضای ابری(Cloud) هم بتوانند میزبانی(Host) شوند، مایکروسافت با معرفی ASP.NET Core گستره کارکرد NET.را افزایش داده است.به این معنی که می‌توان برنامه‌های کاربردی ASP.NET Core را بر روی بازه‌ی گسترده ای از محیط‌های مختلف میزبانی کرد هم‌اکنون می‌توانید پروژه های وب را برای Linux یا macOS هم تولید کنید."
               },
                new Product()
                {
                    Id = 2,
                    ItemId = 2,
                    Name = "آموزش Blazor از مقدماتی تا پیشرفته",
                    Description =
                        "در سال های گذشته ، کمپانی مایکروسافت با معرفی تکنولوژی های جدید و حرفه ای در زمینه های مختلف ، عرصه را برای سایر کمپانی ها تنگ تر کرده است. اخیرا این غول فناوری با معرفی فریم ورک های ASP.NET Core و همینطور Blazor قدرت خود در زمینه ی وب را به اثبات رسانده است."
                },
                new Product()
                {
                    Id = 3,
                    ItemId = 3,
                    Name = "آموزش اپلیکیشن های وب پیش رونده ( PWA )",
                    Description = "آموزش اپلیکیشن های وب پیش رونده ( PWA ) آموزش PWA از مقدماتی تا پیشرفته وب اپلیکیشن‌های پیش رونده(PWA) نسل جدید اپلیکیشن‌های تحت وب هستند که می‌توانند آینده‌ی اپلیکیشن‌های موبایل را متحول کنند.در این دوره به طور جامع به بررسی آن‌ها خواهیم پرداخت. مزایا و فاکتور هایی که یک وب اپلیکیشن دارا می باشد به صورت زیر می باشد : ریسپانسیو :  رکن اصلی سایت برای PWA ریسپانسیو بودن اپلیکیشن هستش که برای صفحه نمایش کاربران مختلف موبایل و تبلت خود را وفق دهند."
                });



                c.HasKey(x => x.Id);
           
                c.HasOne<Item>(x => x.Item).WithOne(x => x.Product).HasForeignKey<Item>(x => x.Id);
                c.Property(x => x.Name).HasMaxLength(100);
                c.Property(x => x.Description).HasMaxLength(1000);


            });
            modelBuilder.Entity<Item>(c =>
            {
                c.HasKey(x => x.Id);
                c.HasData(
                    new Item()
                    {
                        Id = 1,
                        Price = 854.0M,
                        QuantityInStock = 5
                    },
                    new Item()
                    {
                        Id = 2,
                        Price = 3302.0M,
                        QuantityInStock = 8
                    },
                    new Item()
                    {
                        Id = 3,
                        Price = 2500,
                        QuantityInStock = 3
                    });


            });
            modelBuilder.Entity<Category>(c =>
            {
                c.HasKey(x => x.Id);
                c.Property(x => x.Description).HasMaxLength(1000);


            });

            #region Seed Data Category

            modelBuilder.Entity<Category>().HasData(new Category()
                {
                    Id = 1,
                    Name = "کفش زنانه",
                    Description = "jlkfghhfgkdfgj"



                },

                new Category()
                {
                    Id = 2,
                    Name = "لباس ورزشی",
                    Description = "گروه لباس ورزشی"
                },
                new Category()
                {
                    Id = 3,
                    Name = "ساعت مچی",
                    Description = "ساعت مچی"
                },
                new Category()
                {
                    Id = 4,
                    Name = "لوازم منزل",
                    Description = "لوازم منزل"
                },

                new Category()
                {
                    Id = 5,
                    Name = "کفش مردانه",
                    Description = "jlkfghسیسیببسیسیبhfgkdfgj"



                });

            modelBuilder.Entity<CategoryToProducts>().HasData(
                new CategoryToProducts() { CategoryId = 1, ProductId = 1 },
                new CategoryToProducts() { CategoryId = 2, ProductId = 1 },
                new CategoryToProducts() { CategoryId = 3, ProductId = 1 },
                new CategoryToProducts() { CategoryId = 4, ProductId = 1 },
                new CategoryToProducts() { CategoryId = 1, ProductId = 2 },
                new CategoryToProducts() { CategoryId = 2, ProductId = 2 },
                new CategoryToProducts() { CategoryId = 3, ProductId = 2 },
                new CategoryToProducts() { CategoryId = 4, ProductId = 2 },
                new CategoryToProducts() { CategoryId = 1, ProductId = 3 },
                new CategoryToProducts() { CategoryId = 2, ProductId = 3 },
                new CategoryToProducts() { CategoryId = 3, ProductId = 3 },
                new CategoryToProducts() { CategoryId = 4, ProductId = 3 }
            );

            #endregion


            base.OnModelCreating(modelBuilder);
        }
    }
}
