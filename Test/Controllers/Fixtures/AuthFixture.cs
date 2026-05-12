using System;
using System.Collections.Generic;
using System.IO;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Test.Controllers.Fixtures
{
    public class AuthFixture
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private IConfiguration _configuration;

        public AppDbContext Context => _context;
        public IAuthService AuthService => _authService;
        public IConfiguration Configuration => _configuration;

        public static AuthFixture Build()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["Jwt:Key"] = "ThisIsASuperSecretKeyForJWT1234567890",
                        ["Jwt:Issuer"] = "https://localhost:5001",
                        ["Jwt:Audience"] = "https://localhost:5001",
                    }
                )
                .Build();

            var connectionString = "Data Source=:memory:";
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connectionString)
                .Options;

            var context = new AppDbContext(options);

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserResponseDto>();
            }).CreateMapper();

            var authService = new AuthService(mapper, configuration, context);

            return new AuthFixture(context, authService, mapper, configuration);
        }

        private AuthFixture(
            AppDbContext context,
            IAuthService authService,
            IMapper mapper,
            IConfiguration configuration
        )
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
            _configuration = configuration;
        }
    }
}
