using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GermanWhistWebPage.Models;
using GermanWhistWebPage.Services;

namespace GermanWhistWebPage.Controllers
{
    [Route("api/games/[controller]")]
    [ApiController]
    public class GermanWhistController : ControllerBase
    {
        private readonly GameContext _context;
        private readonly PlayerService  _playerService;
        private readonly GameService _gameService;

        public GermanWhistController(GameContext context, PlayerService playerService, GameService gameService)
        {
            _context = context;
            _playerService = playerService;
            _gameService = gameService;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameInfoDTO>>> GetGames()
        {
          if (_context.Games == null)
          {
              return NotFound();
          }
        var games = await _context.Games.ToListAsync();
        if (games == null)
        {
            return NotFound();
        }
            return games.Select(game => new GameInfoDTO(game)).ToList();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameInfoDTO>> GetGame(int id)
        {
          if (_context.Games == null)
          {
              return NotFound();
          }
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return new GameInfoDTO(game);
        }

        [HttpGet("{id}/game-state")]
        public async Task<ActionResult<PlayerViewOfGameStateDTO>> GetGameState(int id)
        {
            if (_context.Games == null)
            {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            Player player = _playerService.getUserPlayer();

            return new PlayerViewOfGameStateDTO(game, player);
        }

        [HttpPut("{id}/game-state")]
        public async Task<ActionResult<PlayerViewOfGameStateDTO>> MakeAMove(int id, int cardId)
        {
            if (_context.Games == null)
            {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            Player player = _playerService.getUserPlayer();

            if (! _gameService.isValidMove(game, player, cardId))
            {
                return Forbid();
            }
            _gameService.makeMove(game, player, cardId);    
            return new PlayerViewOfGameStateDTO(game, player);
        }



        //// PUT: api/Games/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutGame(int id, Game game)
        //{
        //    if (id != game.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(game).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!GameExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(int player1Id, int player2Id)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'GameContext.Games'  is null.");
            }

            Game game = _gameService.createGame(player1Id, player2Id);


            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, game);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            if (_context.Games == null)
            {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameExists(int id)
        {
            return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
