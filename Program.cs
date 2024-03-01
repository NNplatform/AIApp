using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var keyOpenAI = app.Configuration["OpenAI:Key"]!;
var modelGptOpenAI = "gpt-4-0125-preview";
var settings = new OpenAIPromptExecutionSettings(){
MaxTokens = 100,//control billing ofia
Temperature = 0.7
};

app.MapPost("/chat_completion", async(string promp) => {
   var kernel = Kernel.CreateBuilder()
   .AddOpenAIChatCompletion(modelId: modelGptOpenAI, apiKey: keyOpenAI)
   .Build();

   var kernelFuntion = kernel.CreateFunctionFromPrompt(promp, settings);

   var result  = await kernel.InvokeAsync(kernelFuntion);//async 

   return result.GetValue<string>();
});

app.MapPost("/kernel_plugin", async(string firstName, string lastName) => {
     var kernel = Kernel.CreateBuilder()
   .AddOpenAIChatCompletion(modelId: modelGptOpenAI, apiKey: keyOpenAI)
   .Build();

});


app.MapPost("/promp_definition", async() => {
    var kernel = Kernel.CreateBuilder()
   .AddOpenAIChatCompletion(modelId: modelGptOpenAI, apiKey: keyOpenAI)
   .Build();

   kernel.ImportPluginFromType<TimePlugin>("time");
   var promptDefinition = @"
   IS it morning? (morning/afternoon/evening) ?
   Is it weekend? (weekend/not weekend)?
   
   ##
   Today is {{time.Date}}
   Current time is {{timr.Time}}

   ";

//    var promptTemplateFactory =  new KernelPromptTemplateFactory();
//    var prompTemplate =  promptTemplateFactory.Create(new PromptTemplateConfig(promptDefinition));

//    var prompt = await promptTemplate //not finish

// var kernelFunct = kernel.CreateFunctionFromPrompt
// var result = 
// return result.GetValue<string>();

});

app.MapPost("/variable", async (string input) => {

});



// app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
