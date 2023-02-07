using Fresh_Farm_Market.Model;
using Fresh_Farm_Market.Models;

namespace Fresh_Farm_Market.Service
{
    public class PasswordHistoryService
    {
        private readonly UserDbContext userDbContext;
        public PasswordHistoryService(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }
        public List<PasswordHistory> GetPasswordHistoriesbyId(string id)
        {
            
            return userDbContext.passwordHistories.Where(x => x.UserId == id).ToList();
        }

        public void AddPassowrdHistory(PasswordHistory passwordHistory)
        {
            userDbContext.passwordHistories.Add(passwordHistory);
            userDbContext.SaveChanges();
        }
        public void removehistory(User user)
        {
            PasswordHistory? passwordHistory = userDbContext.passwordHistories.FirstOrDefault(x => x.UserId == user.Id);
            userDbContext.passwordHistories.Remove(passwordHistory);
            userDbContext.SaveChanges();
        }
    }
}
