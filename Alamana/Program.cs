using System.Text;
using Alamana.Data.Context;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Alamana.Repository.Repositories;
using Alamana.Service.Advertisment;
using Alamana.Service.Advertisment.Dtos;
using Alamana.Service.Authentication;
using Alamana.Service.CartItem;
using Alamana.Service.CartItem.Dtos;
using Alamana.Service.Carts;
using Alamana.Service.Carts.Dtos;
using Alamana.Service.Category;
using Alamana.Service.Category.Dtos;
using Alamana.Service.ConfirmationEmail;
using Alamana.Service.ContactUs;
using Alamana.Service.Email;
using Alamana.Service.Location;
using Alamana.Service.Location.Dtos;
using Alamana.Service.Orders;
using Alamana.Service.Payment;
using Alamana.Service.Payment.Dtos;
using Alamana.Service.Product;
using Alamana.Service.ProductFavourite;
using Alamana.Service.ProductFavourite.Dtos;
using Alamana.Service.SaveAndDeleteImage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


builder.Services.AddDbContext<AlamanaBbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<AlamanaBbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // ??????? ????????? ?? config
        ValidAudience = builder.Configuration["Jwt:Audience"], // ??????? ????????? ?? config
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});





builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 500_000_000; // 500MB ?????? ????? ???????
});


builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = 500_000_000; // ??? ????
});



builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartItemsService, CartItemsService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISaveAndDeleteImageService, SaveAndDeleteImageService>();
builder.Services.AddScoped<ILocationServices, LocationServices>();
builder.Services.AddScoped<IAdvertiseService, AdvertiseService>();
builder.Services.AddScoped<IFavouriteServices, FavouriteServices>();
builder.Services.AddScoped<IPaymentMethodsServices, PaymentMethodsServices>();
builder.Services.AddScoped<IContactUsServices, ContactUsServices>();



builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IConfirmationService, ConfirmationService>();








builder.Services.AddAutoMapper(typeof(CategoryProfile));
builder.Services.AddAutoMapper(typeof(ProductServices));
builder.Services.AddAutoMapper(typeof(CartProfile));
builder.Services.AddAutoMapper(typeof(CartItemProfile));
builder.Services.AddAutoMapper(typeof(LocationProfile));
builder.Services.AddAutoMapper(typeof(paymentMethodsProfile));
builder.Services.AddAutoMapper(typeof(AdvertiseProfile));
builder.Services.AddAutoMapper(typeof(FavouriteProfile));





// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", builder =>
//        builder.AllowAnyOrigin() // ?????? ??? ????
//               .AllowAnyHeader() // ?????? ??? ??????
//               .AllowAnyMethod()); // ?????? ??? ????? HTTP
//});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAlamanaOrigins", policy =>
        policy.WithOrigins(
                "http://localhost:4200",
                "https://alamanamarket.com",
                "https://www.alamanamarket.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());

});




var app = builder.Build();


app.UseCors("AllowAlamanaOrigins");



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
