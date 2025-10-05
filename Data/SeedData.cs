using Microsoft.EntityFrameworkCore;
using WordGuessGame.Models;

namespace WordGuessGame.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // ตรวจสอบว่ามีข้อมูลหมวดหมู่ในระบบแล้วหรือยัง
                if (context.Categories.Any())
                {
                    return;   // ถ้ามีข้อมูลแล้ว ก็ไม่ต้องทำอะไรเพิ่ม
                }

                // --- สร้างข้อมูลหมวดหมู่ ---
                var animalsCategory = new Category { Name = "สัตว์" };
                var foodCategory = new Category { Name = "อาหาร" };
                var sportsCategory = new Category { Name = "กีฬา" };
                var movieCategory = new Category { Name = "ภาพยนตร์" };
                var musicCategory = new Category { Name = "เพลง" };
                var travelCategory = new Category { Name = "สถานที่ท่องเที่ยว" };

                context.Categories.AddRange(
                        animalsCategory,
                        foodCategory,
                        sportsCategory,
                        movieCategory,
                        musicCategory,
                        travelCategory
                );
                context.SaveChanges(); // บันทึกหมวดหมู่ลง DB ก่อน เพื่อให้ได้ ID มาใช้

                // --- สร้างข้อมูลคำศัพท์ ---
                context.Words.AddRange(
                    // สัตว์
                    new Word { Text = "ช้าง", CategoryId = animalsCategory.Id },
                    new Word { Text = "สิงโต", CategoryId = animalsCategory.Id },
                    new Word { Text = "ยีราฟ", CategoryId = animalsCategory.Id },
                    new Word { Text = "หมี", CategoryId = animalsCategory.Id },
                    new Word { Text = "เสือ", CategoryId = animalsCategory.Id },
                    new Word { Text = "หมาป่า", CategoryId = animalsCategory.Id },
                    new Word { Text = "แมว", CategoryId = animalsCategory.Id },
                    new Word { Text = "หมา", CategoryId = animalsCategory.Id },

                    // อาหาร
                    new Word { Text = "ผัดไทย", CategoryId = foodCategory.Id },
                    new Word { Text = "ต้มยำกุ้ง", CategoryId = foodCategory.Id },
                    new Word { Text = "ส้มตำ", CategoryId = foodCategory.Id },
                    new Word { Text = "แกงเขียวหวาน", CategoryId = foodCategory.Id },
                    new Word { Text = "ข้าวผัด", CategoryId = foodCategory.Id },
                    new Word { Text = "มาม่า", CategoryId = foodCategory.Id },
                    new Word { Text = "ไก่ทอด", CategoryId = foodCategory.Id },
                    new Word { Text = "ข้าวเหนียวมะม่วง", CategoryId = foodCategory.Id },

                    // กีฬา
                    new Word { Text = "ฟุตบอล", CategoryId = sportsCategory.Id },
                    new Word { Text = "บาสเกตบอล", CategoryId = sportsCategory.Id },
                    new Word { Text = "เทนนิส", CategoryId = sportsCategory.Id },
                    new Word { Text = "แบดมินตัน", CategoryId = sportsCategory.Id },
                    new Word { Text = "มวยไทย", CategoryId = sportsCategory.Id },
                    new Word { Text = "วอลเลย์บอล", CategoryId = sportsCategory.Id },
                    new Word { Text = "ว่ายน้ำ", CategoryId = sportsCategory.Id },
                    new Word { Text = "เทเบิลเทนนิส", CategoryId = sportsCategory.Id },

                    // ภาพยนตร์
                    new Word { Text = "อเวนเจอร์ส", CategoryId = movieCategory.Id },
                    new Word { Text = "ไททานิค", CategoryId = movieCategory.Id },
                    new Word { Text = "สไปเดอร์แมน", CategoryId = movieCategory.Id },
                    new Word { Text = "แฮร์รี่พอตเตอร์", CategoryId = movieCategory.Id },
                    new Word { Text = "โฟรเซน", CategoryId = movieCategory.Id },
                    new Word { Text = "บ้านผีปอบ", CategoryId = movieCategory.Id },
                    new Word { Text = "ชัตเตอร์", CategoryId = movieCategory.Id },
                    new Word { Text = "พี่มากพระโขนง", CategoryId = movieCategory.Id },

                    // เพลง
                    new Word { Text = "สายตาหลอกกันไม่ได้", CategoryId = musicCategory.Id },
                    new Word { Text = "ถ้าเขาจะรัก", CategoryId = musicCategory.Id },
                    new Word { Text = "ลาลาลอย", CategoryId = musicCategory.Id },
                    new Word { Text = "เพื่อนเล่น ไม่เล่นเพื่อน", CategoryId = musicCategory.Id },
                    new Word { Text = "วาฬเกยตื้น", CategoryId = musicCategory.Id },
                    new Word { Text = "ทิ้งไว้กลางทาง", CategoryId = musicCategory.Id },
                    new Word { Text = "ได้แค่ไหน เอาแค่นั้น", CategoryId = musicCategory.Id },
                    new Word { Text = "ไม่บอกเธอ", CategoryId = musicCategory.Id },
                    

                    // สถานที่ท่องเที่ยว
                    new Word { Text = "ภูเก็ต", CategoryId = travelCategory.Id },
                    new Word { Text = "เชียงใหม่", CategoryId = travelCategory.Id },
                    new Word { Text = "พัทยา", CategoryId = travelCategory.Id },
                    new Word { Text = "กรุงเทพ", CategoryId = travelCategory.Id },
                    new Word { Text = "หัวหิน", CategoryId = travelCategory.Id },
                    new Word { Text = "เกาะสมุย", CategoryId = travelCategory.Id },
                    new Word { Text = "อยุธยา", CategoryId = travelCategory.Id },
                    new Word { Text = "สุโขทัย", CategoryId = travelCategory.Id }
                );

                // บันทึกข้อมูลทั้งหมดลงฐานข้อมูล
                context.SaveChanges();
            }
        }
    }
}
