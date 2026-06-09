using Microsoft.AspNetCore.Mvc;
using Moq;
using MiniProjectManager.API.Controllers;
using MiniProjectManager.API.Entities;
using MiniProjectManager.API.Services;
using MiniProjectManager.API.DTOs;
using Xunit;

namespace MiniProjectManager.Tests
{
    public class TaskItemsControllerTests
    {
        [Fact]
        public async Task GetByBoardId_ShouldReturnOk_WhenTasksExist()
        {
            // ==========================================
            // 1. ARRANGE (Persiapan)
            // ==========================================
            // Membuat Service Palsu (Mock)
            var mockService = new Mock<ITaskItemService>();

            // Menyiapkan data palsu
            var fakeTasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Tugas A", BoardId = 1 },
                new TaskItem { Id = 2, Title = "Tugas B", BoardId = 1 }
            };

            // Melatih Mock: "Jika metode GetTasksByBoardIdAsync(1) dipanggil, tolong kembalikan fakeTasks"
            mockService.Setup(s => s.GetTasksByBoardIdAsync(1))
                       .ReturnsAsync(fakeTasks);

            // Menyuntikkan Service Palsu tersebut ke dalam Controller sungguhan
            var controller = new TaskItemController(mockService.Object);

            // ==========================================
            // 2. ACT (Tindakan)
            // ==========================================
            // Controller tidak tahu bahwa ia sedang memakai service palsu
            var result = await controller.GetByBoardId(1);

            // ==========================================
            // 3. ASSERT (Pembuktian)
            // ==========================================
            // Verifikasi 1: Pastikan kembaliannya adalah HTTP 200 (OkObjectResult)
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            // Verifikasi 2: Pastikan isi datanya adalah daftar TaskItemResponseDto
            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskItemResponseDto>>(okResult.Value);
            
            // Verifikasi 3: Pastikan jumlah tugasnya sesuai dengan data palsu kita (2 tugas)
            Assert.Equal(2, returnedTasks.Count());
        }
    }
}