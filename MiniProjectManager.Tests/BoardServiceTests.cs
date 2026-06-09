using Microsoft.EntityFrameworkCore;
using MiniProjectManager.API.Data;
using MiniProjectManager.API.Services;
using Xunit;

namespace MiniProjectManager.Tests
{
    public class BoardServiceTests
    {
        // Helper: Membuat database bayangan di RAM (InMemory) untuk pengujian
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                // Menggunakan GUID agar setiap pengujian punya database yang benar-benar baru & kosong
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            return new AppDbContext(options);
        }

        // [Fact] adalah penanda dari xUnit bahwa metode ini adalah sebuah skenario test
        [Fact]
        public async Task CreateBoardAsync_ShouldReturnNewBoard_WhenDataIsValid()
        {
            // ==========================================
            // 1. ARRANGE (Persiapan)
            // ==========================================
            var context = GetInMemoryDbContext();
            var service = new BoardService(context); // Service disuntikkan dengan database bayangan
            
            string expectedTitle = "Papan Kanban Baru";
            int expectedWorkspaceId = 1;

            // ==========================================
            // 2. ACT (Tindakan)
            // ==========================================
            var result = await service.CreateBoard(expectedTitle, expectedWorkspaceId);

            // ==========================================
            // 3. ASSERT (Pembuktian)
            // ==========================================
            Assert.NotNull(result); // Memastikan kembaliannya tidak null
            Assert.Equal(expectedTitle, result.Title); // Memastikan judulnya sesuai
            Assert.Equal(expectedWorkspaceId, result.WorkspaceId); // Memastikan ID Workspace sesuai
            Assert.True(result.Id > 0); // Memastikan Entity Framework membuatkan ID (Auto Increment)
        }
    }
}