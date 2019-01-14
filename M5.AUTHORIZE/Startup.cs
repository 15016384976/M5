using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using DnsClient;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Wrap;

namespace M5.AUTHORIZE
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                    .AddExtensionGrantValidator<SMSCodeValidator>()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryApiResources(new List<ApiResource> {
                        new ApiResource("gateway", "GATEWAY"),
                        new ApiResource("api", "API"),
                    })
                    .AddInMemoryClients(new List<Client> {
                        new Client
                        {
                            ClientId = "client",
                            ClientSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                            RefreshTokenExpiration = TokenExpiration.Sliding,
                            AllowOfflineAccess = true,
                            RequireClientSecret = false,
                            AllowedGrantTypes = new List<string> { "sms_code" },
                            AlwaysIncludeUserClaimsInIdToken = true,
                            AllowedScopes = new List<string> {
                                "gateway",
                                "api",
                                IdentityServerConstants.StandardScopes.OfflineAccess,
                                IdentityServerConstants.StandardScopes.OpenId,
                                IdentityServerConstants.StandardScopes.Profile
                            }
                        }
                    })
                    .AddInMemoryIdentityResources(new List<IdentityResource> {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile()
                    });

            services.AddScoped<ICodeService, SMSCodeService>()
                    .AddScoped<IUserService, SMSUserService>();

            services.AddTransient<IProfileService, ProfileService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<ResilientHttpClient>>();
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

                var retryCount = 6;
                var exceptionsAllowedBeforeBreaking = 5;
                return new ResilientHttpClientFactory(logger, httpContextAccessor, exceptionsAllowedBeforeBreaking, retryCount);
            });
            services.AddSingleton<IHttpClient, ResilientHttpClient>(serviceProvider => serviceProvider.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }
    }

    public interface ICodeService
    {
        Task<bool> ValidateAsync(string phone, string code);
    }

    public class SMSCodeService : ICodeService
    {
        public async Task<bool> ValidateAsync(string phone, string code)
        {
            return await Task.FromResult(true);
        }
    }

    public interface IUserService
    {
        Task<MIdentity> ValidateAsync(string phone);
    }

    public class SMSUserService : IUserService
    {
        private readonly IHttpClient _httpClient;

        public SMSUserService(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MIdentity> ValidateAsync(string phone)
        {
            var dnsAddress = "127.0.0.1";
            var dnsPort = 8600;
            var baseDomain = "service.consul";
            var serviceName = "m5_api_user";

            var lookupClient = new LookupClient(IPAddress.Parse(dnsAddress), dnsPort);
            var serviceHostEntry = await lookupClient.ResolveServiceAsync(baseDomain, serviceName);
            var addressList = serviceHostEntry.First().AddressList;
            var address = addressList.Any() ? addressList.First().ToString() : serviceHostEntry.First().HostName;
            var port = serviceHostEntry.First().Port;

            var requestUri = $"http://{address}:{port}/api/user/signin";

            var response = await _httpClient.PostAsync(requestUri, new { Phone = phone });// services between

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MIdentity>(JObject.Parse(result)["content"].ToString());
            }

            return null;
        }
    }

    public class SMSCodeValidator : IExtensionGrantValidator
    {
        private readonly ICodeService _codeService;
        private readonly IUserService _userService;

        public SMSCodeValidator(ICodeService codeService, IUserService userService)
        {
            _codeService = codeService;
            _userService = userService;
        }

        public string GrantType => "sms_code";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw["phone"];
            var code = context.Request.Raw["code"];

            var result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);

            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(code))
            {
                context.Result = result;
                return;
            }

            if (!await _codeService.ValidateAsync(phone, code))
            {
                context.Result = result;
                return;
            }

            var identity = await _userService.ValidateAsync(phone);
            if (identity == null)
            {
                context.Result = result;
                return;
            }

            var claims = new Claim[]
            {
                new Claim("phone", identity.Phone),
                new Claim("name", identity.Name)
            };

            context.Result = new GrantValidationResult(identity.Id.ToString(), GrantType, claims);
        }
    }

    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentException(nameof(context.Subject));
            var subjectId = subject.Claims.Where(v => v.Type == "sub").FirstOrDefault().Value;
            if (!Guid.TryParse(subjectId, out Guid id))
                throw new ArgumentException("Invalid subject identifier");
            context.IssuedClaims = context.Subject.Claims.ToList();
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentException(nameof(context.Subject));
            var subjectId = subject.Claims.Where(v => v.Type == "sub").FirstOrDefault().Value;
            context.IsActive = Guid.TryParse(subjectId, out Guid id);
            return Task.CompletedTask;
        }
    }

    public interface IHttpClient
    {
        Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
    }

    public class ResilientHttpClient : IHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ResilientHttpClient> _logger;
        private readonly Func<string, IEnumerable<Policy>> _policyCreator;
        private ConcurrentDictionary<string, PolicyWrap> _policyWrappers;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResilientHttpClient(Func<string, IEnumerable<Policy>> policyCreator, ILogger<ResilientHttpClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _logger = logger;
            _policyCreator = policyCreator;
            _policyWrappers = new ConcurrentDictionary<string, PolicyWrap>();
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoPostPutAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoPostPutAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

                SetAuthorizationHeader(requestMessage);

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }

                return await _client.SendAsync(requestMessage);
            });
        }

        public Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                SetAuthorizationHeader(requestMessage);

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                var response = await _client.SendAsync(requestMessage);

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadAsStringAsync();
            });
        }

        private Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(method, uri);

                SetAuthorizationHeader(requestMessage);

                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }

                var response = await _client.SendAsync(requestMessage);

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

                return response;
            });
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);

            if (!_policyWrappers.TryGetValue(normalizedOrigin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.WrapAsync(_policyCreator(normalizedOrigin).ToArray());
                _policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            return await policyWrap.ExecuteAsync(action, new Context(normalizedOrigin));
        }

        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);

            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";

            return origin;
        }

        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }
        }
    }

    public interface IResilientHttpClientFactory
    {
        ResilientHttpClient CreateResilientHttpClient();
    }

    public class ResilientHttpClientFactory : IResilientHttpClientFactory
    {
        private readonly ILogger<ResilientHttpClient> _logger;
        private readonly int _retryCount;
        private readonly int _exceptionsAllowedBeforeBreaking;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResilientHttpClientFactory(ILogger<ResilientHttpClient> logger, IHttpContextAccessor httpContextAccessor, int exceptionsAllowedBeforeBreaking = 5, int retryCount = 6)
        {
            _logger = logger;
            _exceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
            _retryCount = retryCount;
            _httpContextAccessor = httpContextAccessor;
        }

        public ResilientHttpClient CreateResilientHttpClient()
        {
            return new ResilientHttpClient((origin) => CreatePolicies(), _logger, _httpContextAccessor);
        }

        private Policy[] CreatePolicies()
        {
            return new Policy[]
            {
                Policy.Handle<HttpRequestException>()
                    .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                                $"of {context.PolicyKey} " +
                                $"at {context.ExecutionKey}, " +
                                $"due to: {exception}.";
                            _logger.LogWarning(msg);
                            _logger.LogDebug(msg);
                        }),
                Policy.Handle<HttpRequestException>()
                    .CircuitBreakerAsync(_exceptionsAllowedBeforeBreaking, TimeSpan.FromMinutes(1),
                        (exception, duration) =>
                        {
                            _logger.LogTrace("Circuit breaker opened");
                        },
                        () =>
                        {
                            _logger.LogTrace("Circuit breaker reset");
                        })
            };
        }
    }

    public class MIdentity
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
    }
}

// Install-Package IdentityServer4
// Install-Package Polly -Version 5.8.0
// Install-Package Consul
// Install-Package DnsClient

/* 
POST -> 127.0.0.1:5001/connect/token
body -> form-data

grant_type -> sms_code
client_id -> client
client_secret -> secret
phone -> 13800000000
code -> 123456
scope -> 
*/
