using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Context>(opotion =>
{
    opotion.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// É capaz de injetar a classe JWTService dentro dos controllers
builder.Services.AddScoped<JWTService>();

//Define nosso IdentityCore Service
builder.Services.AddIdentityCore<User>(options =>
{
    //configuração de senha
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    // para confirmação de email
    options.SignIn.RequireConfirmedEmail = true;

})
    .AddRoles<IdentityRole>() // Capaz de add roles
    .AddRoleManager<RoleManager<IdentityRole>>() // capaz de fazer uso do RoleManager
    .AddEntityFrameworkStores<Context>() // Fornece nosso context
    .AddSignInManager<SignInManager<User>>() // faz uso do Signi Manager
    .AddUserManager<UserManager<User>>()  //faz uso do UserManager para criar usuários
    .AddDefaultTokenProviders(); // capaz de criar tokens para confirmação de email

//Capaz de autenticar usuarios usando o JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //Valida o token baseado na chave que foi disponibilizada dentro do appsettings.development.jason JWT:Key
            ValidateIssuerSigningKey = true,
            // chave de assinatura do emissor baseado no JWT:Key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            // o emissor no qual aqui é a api project url que está sendo usada
            ValidIssuer = builder.Configuration["JWT:Issuerg"],
            // validar o emissor( Quem já está emitindo o JWT)
            ValidateIssuer = true,
            // não valida audience (lado do angular)
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Adicionando o UseAuthentication no pipeline e isso deve vir antes do UseAuthorization
//Authentication verifica a identidade do usuário ou seviço, e authorizaton determina os diretos de acesso deles.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
