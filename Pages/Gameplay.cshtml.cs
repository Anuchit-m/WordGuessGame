using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WordGuessGame.Models;
using WordGuessGame.Services;

namespace WordGuessGame.Pages
{
    public class GameplayModel : PageModel
    {
        private readonly GameService _gameService;

        public GameplayModel(GameService gameService)
        {
            _gameService = gameService;
        }

        // รับค่าที่ผู้ใช้เดามาจากฟอร์ม
        [BindProperty]
        public string UserGuess { get; set; }

        // รับเวลาปัจจุบันจาก client
        [BindProperty]
        public int CurrentTimeLeft { get; set; }

        // รับสถานะว่าหมดเวลาหรือไม่
        [BindProperty]
        public bool TimeUp { get; set; }

        // เก็บสถานะเกมปัจจุบันเพื่อแสดงผลบนหน้าเว็บ
        public GameState CurrentGameState { get; set; }

        [TempData]
        public string Message { get; set; }

        // เมธอดนี้จะทำงานเมื่อผู้ใช้เลือกหมวดหมู่
        public async Task<IActionResult> OnGetAsync(int categoryId)
        {
            // บันทึกคะแนนของเกมเก่าก่อน (ถ้ามี)
            var existingGameState = _gameService.GetGameState();
            if (existingGameState != null && existingGameState.Score > 0 && User.Identity.IsAuthenticated)
            {
                var (wasSaved, previousBest) = await _gameService.SaveScoreAsync();
                
            }

            // ล้างข้อความเก่า
            Message = null;
            TempData.Remove("Message");

            CurrentGameState = await _gameService.StartNewGameAsync(categoryId);
            if (CurrentGameState == null)
            {
                // กรณีไม่พบคำศัพท์ในหมวดหมู่นั้นๆ
                TempData["ErrorMessage"] = "ไม่พบคำศัพท์ในหมวดหมู่นี้";
                return RedirectToPage("/Category");
            }
            return Page();
        }

        // เมธอดนี้จะทำงานเมื่อผู้ใช้กดปุ่ม "เดา"
        public async Task<IActionResult> OnPostAsync()
        {
            // อัพเดตเวลาจาก client ก่อนทำการเดา
            await _gameService.UpdateTimeLeftAsync(CurrentTimeLeft);
            
            // ตรวจสอบว่าหมดเวลาหรือยัง
            if (CurrentTimeLeft <= 0 || TimeUp)
            {
                var gameState = _gameService.GetGameState();
                if (gameState != null)
                {
                    gameState.IsGameOver = true;
                    _gameService.SetGameState(gameState);
                    
                    if (User.Identity.IsAuthenticated)
                    {
                        var (wasSaved, previousBest) = await _gameService.SaveScoreAsync();
                        if (wasSaved)
                        {
                            if (previousBest.HasValue)
                            {
                                TempData["Message"] = $"หมดเวลา! คุณได้ {gameState.Score} คะแนน (สถิติใหม่! เดิม: {previousBest.Value} คะแนน)";
                            }
                            else
                            {
                                TempData["Message"] = $"หมดเวลา! คุณได้ {gameState.Score} คะแนน (สถิติใหม่!)";
                            }
                        }
                        else
                        {
                            TempData["Message"] = $"หมดเวลา! คุณได้ {gameState.Score} คะแนน (สถิติเดิม: {previousBest} คะแนน)";
                        }
                    }
                    else
                    {
                        TempData["Message"] = $"หมดเวลา! คุณได้ {gameState.Score} คะแนน";
                    }
                    return RedirectToPage("/Leaderboard");
                }
            }

            var (isCorrect, newState) = await _gameService.MakeGuessAsync(UserGuess);
            CurrentGameState = newState;

            if (CurrentGameState.IsGameOver)
            {
                // ถ้าเกมจบแล้ว ให้บันทึกคะแนน (ถ้าล็อกอินแล้ว) และไปหน้า Leaderboard
                if (User.Identity.IsAuthenticated)
                {
                    var (wasSaved, previousBest) = await _gameService.SaveScoreAsync();
                    if (wasSaved)
                    {
                        if (previousBest.HasValue)
                        {
                            TempData["Message"] = $"เกมจบแล้ว! คุณได้ {CurrentGameState.Score} คะแนน (สถิติใหม่! เดิม: {previousBest.Value} คะแนน)";
                        }
                        else
                        {
                            TempData["Message"] = $"เกมจบแล้ว! คุณได้ {CurrentGameState.Score} คะแนน (สถิติใหม่!)";
                        }
                    }
                    else
                    {
                        TempData["Message"] = $"เกมจบแล้ว! คุณได้ {CurrentGameState.Score} คะแนน (สถิติเดิม: {previousBest} คะแนน)";
                    }
                }
                else
                {
                    TempData["Message"] = $"เกมจบแล้ว! คุณได้ {CurrentGameState.Score} คะแนน";
                }
                return RedirectToPage("/Leaderboard");
            }

            if (isCorrect)
            {
                Message = "ถูกต้อง!";
            }
            else
            {
                Message = "ผิด ลองใหม่นะ!";
            }

            // ล้าง input field หลังจากทาย
            UserGuess = string.Empty;
            
            return Page();
        }
    }
}
