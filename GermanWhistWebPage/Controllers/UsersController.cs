using GermanWhistWebPage.Models;
using GermanWhistWebPage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Numerics;
using System.Security.Claims;

namespace GermanWhistWebPage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly GameContext _gameContext;

        public UsersController(
            UserManager<IdentityUser> userManager,
            JwtService jwtService,
            GameContext gameContext
        )
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _gameContext = gameContext;
        }


        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            IdentityUser identityUser = new IdentityUser() { UserName = user.UserName, Email = user.Email };
            var result = await _userManager.CreateAsync(
                identityUser,
                user.Password
            );

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            HumanPlayer player = new HumanPlayer()
            {
                IdentityUserId = identityUser.Id,
                IdentityUser = identityUser
            };
            _gameContext.HumanPlayers.Add(player);
            await _gameContext.SaveChangesAsync();

            user.Password = null;
            user.PlayerId = player.Id;
            return Created("", user);
        }

        // GET: api/Users/username
        [HttpGet("{username}")]
        public async Task<ActionResult<UserDTO>> GetUser(string username)
        {
            IdentityUser user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return new UserDTO
            {
                UserName = user.UserName,
                Email = user.Email
            };
        }

        // POST: api/Users/BearerToken
        [HttpPost("BearerToken")]
        public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad credentials");
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return BadRequest("Bad credentials");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }

            var token = _jwtService.CreateToken(user);

            return Ok(token);
        }

        // GET: api/Users/games/GermanWhist
        [Authorize]
        [HttpGet("games/GermanWhist")]
        public async Task<ActionResult<List<GameInfoDTO>>> GetGermanWhistGamesOfUser()
        {
            if (_gameContext.Games == null)
            {
                return NotFound();
            }
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Player? player = _gameContext.HumanPlayers.FirstOrDefault(p => p.IdentityUserId == userId);
            if (player == null)
                return Problem("User could not be connected to player");

            List< GameInfoDTO> gameDTOs = await _gameContext.Games
                .Where(g=> ((g.Player1Id == player.Id || g.Player2Id == player.Id) &&               // Is player playing
                !(g.TotalScorePlayer1 >= g.TargetScore || g.TotalScorePlayer2 >= g.TargetScore)))   // Has game not ended)
                .Select(game => new GameInfoDTO(game, player.Id))
                .ToListAsync();

            return gameDTOs;
        }
    }
}
