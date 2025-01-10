// using Microsoft.EntityFrameworkCore;
// using Xunit;
//
// public class EfRepositoryBaseTests
// {
//     [Fact]
//     public async Task SaveAsync_AddsOrUpdatesEntity()
//     {
//         var options = new DbContextOptionsBuilder<SessionDbContext>()
//             .UseInMemoryDatabase(databaseName: "TestDatabase")
//             .Options;
//
//         var context = new SessionDbContext(options);
//         var repository = new SessionRepository(context); // Assuming SessionRepository extends EfRepositoryBase
//
//         // Add a new entity
//         var newSession = new Session(Guid.NewGuid())
//         {
//             Type = SessionType.Authenticate,
//             IsStarted = false
//         };
//
//         await repository.SaveAsync(newSession, CancellationToken.None);
//
//         // Verify entity is added
//         var savedSession = await repository.GetAsync(newSession.Id);
//         Assert.NotNull(savedSession);
//
//         // Update the entity
//         savedSession.IsStarted = true;
//         await repository.SaveAsync(savedSession, CancellationToken.None);
//
//         // Verify the update
//         var updatedSession = await repository.GetAsync(newSession.Id);
//         Assert.True(updatedSession.IsStarted);
//     }
// }
