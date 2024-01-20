using TodoItems.Configurations.Options;

namespace TodoItems.Configurations.Extensions
{
    public static class WebApplicationBuilderExtension
    {

        public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddOptions<CorsConfiguration>()
                    .Bind(builder.Configuration.GetSection("Cors"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policyBuilder =>
                {
                    var origins = new List<string>();

                    builder.Configuration.Bind("Cors:Origins", origins);

                    policyBuilder
                        .WithOrigins(origins.ToArray())
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            return builder;
        }
    }
}
