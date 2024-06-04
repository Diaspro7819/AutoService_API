using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using WebApplication3;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TradeContext>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.MapGet("/getstring", () =>
{
    return "jjjjjjjjj";
})
.WithName("GetString")
.WithOpenApi();

app.MapPost("/authenticate", async (HttpContext context, TradeContext dbContext) =>
{
    try
    {
        //string username = context.Request.Form["username"];
        string password = context.Request.Form["password"];
        string login = context.Request.Form["login"];

        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            //var dbContext = serviceProvider.GetRequiredService<TradeContext>();

            bool isAuthenticated = await AuthenticateUserAsync(dbContext, login, password);

            if (isAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }
    }
    catch (Exception ex)
    {
        // Логируйте или возвращайте ошибку обратно клиенту
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync($"Internal Server Error: {ex.Message}");
    }
})
.WithName("PostAuthenticate")
.WithOpenApi();

app.MapPost("/addProduct", async (HttpContext context, TradeContext dbContext) =>
{
    try
    {
    string productArticle = context.Request.Form["productArticle"];
        string productName = context.Request.Form["productName"];
        string productDescription = context.Request.Form["productDescription"];
        string productCategory = context.Request.Form["productCategory"];
        string productManufacturer = context.Request.Form["productManufacturer"];
        decimal productCost = Decimal.Parse(context.Request.Form["productCost"]);
        int productDiscountAmount = Int32.Parse(context.Request.Form["productDiscountAmount"]);
        int productQuantityInStock = Int32.Parse(context.Request.Form["productQuantityInStock"]);
        string productStatus = context.Request.Form["productStatus"];
        string unit = context.Request.Form["unit"];
        string maxDiscountAmount = context.Request.Form["maxDiscountAmount"];
        string supplier = context.Request.Form["supplier"];
        string countInPack = context.Request.Form["countInPack"];
        string minCount = context.Request.Form["minCount"];


        var newProduct = new Product
        {
            ProductArticleNumber = productArticle,
            ProductName = productName,
            ProductDescription = productDescription,
            ProductCategory = productCategory,
            ProductManufacturer = productManufacturer,
            ProductCost = productCost,
            ProductDiscountAmount = productDiscountAmount,
            ProductQuantityInStock = productQuantityInStock,
            ProductStatus = productStatus,
            Unit = unit,
            MaxDiscountAmount = maxDiscountAmount,
            Supplier = supplier,
            CountInPack = countInPack,
            MinCount = minCount
        };

        dbContext.Products.Add(newProduct);
        await dbContext.SaveChangesAsync();

        context.Response.StatusCode = StatusCodes.Status201Created;
        await context.Response.WriteAsync("Product added successfully");
    }
    catch (Exception ex)
    {
        // Log the error or return an error message
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync($"Internal Server Error: {ex.Message}");
    }
})
.WithName("PostAddProduct")
.WithOpenApi();

app.MapPost("/addUser", async (HttpContext context, TradeContext dbContext) =>
{
     try
    {
        // Extracting user details from the form data
        string userSurname = context.Request.Form["UserSurname"];
        string userName = context.Request.Form["UserName"];
        string userPatronymic = context.Request.Form["UserPatronymic"];
        string userLogin = context.Request.Form["UserLogin"];
        string userPassword = context.Request.Form["UserPassword"];
        int userRole = Int32.Parse(context.Request.Form["UserRole"]);

        // Хэширование пароля
        string hashedPassword = HashPassword(userPassword);

        // Creating a new user instance
        var newUser = new User
        {
            UserSurname = userSurname,
            UserName = userName,
            UserPatronymic = userPatronymic,
            UserLogin = userLogin,
            UserPassword = hashedPassword, // Сохраняем хэшированный пароль в базе данных
            UserRole = userRole
        };

        // Adding the user to the database
        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();

        // If user is successfully added
        context.Response.StatusCode = StatusCodes.Status201Created;
        await context.Response.WriteAsync("User created successfully");
    }
catch (Exception ex)
{
    // Log the error or return an error message
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsync($"Internal Server Error: {ex.Message}");
}
})
.WithName("AddUser")
.WithOpenApi();
// Метод для хэширования пароля
string HashPassword(string password)
{
    using (MD5 md5 = MD5.Create())
    {
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = md5.ComputeHash(bytes);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("x2"));
        }
        return sb.ToString();
    }
}

app.MapPost("/api/orders", async (HttpContext context, TradeContext dbContext) =>
{
    try
    {  
        using (var reader = new StreamReader(context.Request.Body))
        {
           
            var body = await reader.ReadToEndAsync();
            var orderDetails = JsonConvert.DeserializeObject<dynamic>(body); // десериализация 

            var order = new Order
            {
                OrderStatus = orderDetails.OrderStatus,
                DateOrders = orderDetails.DateOrders,
                OrderPickupPoint = orderDetails.OrderPickupPoint,
                OrderDeliveryDate = orderDetails.OrderDeliveryDate,
                Code = orderDetails.Code,
                NameClient = orderDetails.NameClient
            };

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync(); 

            foreach (var product in orderDetails.Products)
            {
                var orderProduct = new OrderProduct
                {
                    OrderId = order.OrderId,
                    ProductArticleNumber = product.ProductArticleNumber,
                    Count = product.Count
                };
                dbContext.OrderProducts.Add(orderProduct);
            }

            await dbContext.SaveChangesAsync();
            context.Response.StatusCode = StatusCodes.Status201Created;
            await context.Response.WriteAsync("Order created successfully");
        }
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync($"Internal Server Error: {ex.Message}");
    }
})
.WithName("CreateOrder")
.WithOpenApi();

app.MapGet("/api/PickupPoint", async (HttpContext context, TradeContext dbContext) => {

        return dbContext.PickupPoints.ToList();

}).WithName("GetPickupPoint").WithOpenApi();

async Task<bool> AuthenticateUserAsync(TradeContext dbContext, string login, string password)
{
    User user = await dbContext.Users.FirstOrDefaultAsync(p => p.UserLogin == login && p.UserPassword == password);
    return user != null;
}

app.MapGet("/api/roles", async (HttpContext context, TradeContext dbContext) =>
{
    try
    {
        var roles = await dbContext.Roles.ToListAsync(); // Получаем список ролей из базы данных

        // Отправляем список ролей в формате JSON
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(roles));
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync($"Internal Server Error: {ex.Message}");
    }
})
.WithName("GetRoles")
.WithOpenApi();

app.MapGet("/api/pickuppoints", async (HttpContext context, TradeContext dbContext) =>
{
    try
    {
        var pickupPoints = await dbContext.PickupPoints.ToListAsync(); // Получаем список пунктов выдачи из базы данных

        // Отправляем список пунктов выдачи в формате JSON
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(pickupPoints));
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync($"Internal Server Error: {ex.Message}");
    }
});
app.MapGet("/api/products", async (HttpContext context, TradeContext dbContext) =>
{
    try
    {
        var products = await dbContext.Products.ToListAsync(); // Получаем список товаров из базы данных

        // Отправляем список товаров в формате JSON
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(products));
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync($"Internal Server Error: {ex.Message}");
    }
})
.WithName("GetProducts")
.WithOpenApi();


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

