﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ANDERSONDFM.Aplicacao.Interfaces.auth;
using ANDERSONDFM.Aplicacao.Servicos.auth;
using ANDERSONDFM.Aplicacao.ViewModels;
using ANDERSONDFM.Aplicacao.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ANDERSONDFM.API.Controllers.Security
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthAppService _authAppService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthAppService authAppService, 
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _authAppService = authAppService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model.Nome != null)
            {
                var user = await _userManager.FindByNameAsync(model.Nome);
                if (model.Password != null && user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    if (user.UserName != null)
                    {
                        var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };

                        foreach (var userRole in userRoles)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                        }

                        var token = GetToken(authClaims);

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            success = true
                        });
                    }
                }
            }

            return Unauthorized(new
            {
                Success = false,
                Message = "Usuário ou Senha Incorretos."
            });
        }

        [HttpPost]
        [Route("cadastrar-usuarios-comum")]
        public async Task<IActionResult> Usuarios([FromBody] UsuarioAuth usuarioAuth)
        {
            var result = await _authAppService.CadastrarUsuario(usuarioAuth);

            return Ok(result);
        }

        [HttpPost]
        [Route("cadastrar-gestor-estabelecimento")]
        public async Task<IActionResult> GestorEstabelecimento([FromBody] UsuarioAuth usuarioAuth)
        {
            var result = await _authAppService.CadastrarGestorEstabelecimento(usuarioAuth);

            return Ok(result);
        }

        [HttpPost]
        [Route("cadastrar-usuarios-estabelecimento")]
        public async Task<IActionResult> UsuariosEstabelecimento([FromBody] UsuarioAuth usuarioAuth)
        {
            var result = await _authAppService.CadastrarUsuariosEstabelecimento(usuarioAuth);

            return Ok(result);
        }

        [HttpPost]
        [Route("cadastrar-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] UsuarioAuth model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Nome);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Usuário Já Existe!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Nome
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Falha na criação do usuário! Verifique os detalhes do usuário e tente novamente." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = "Usuário criado com sucesso!" });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? string.Empty));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}
