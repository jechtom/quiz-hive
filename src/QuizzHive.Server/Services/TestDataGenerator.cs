using QuizzHive.Server.DataLayer;
using QuizzHive.Server.State;
using System.Collections.Immutable;

namespace QuizzHive.Server.Services
{
    public class TestDataGenerator
    {
        private readonly IRepository<Session> sessionRepo;
        private readonly IRepository<ActiveSessions> actionSessionsRepo;

        public TestDataGenerator(IRepository<Session> sessionRepo, IRepository<ActiveSessions> actionSessionsRepo)
        {
            this.sessionRepo = sessionRepo;
            this.actionSessionsRepo = actionSessionsRepo;
        }

        private Session session = GenerateTestSession();

        private static Session GenerateTestSession()
        {
            IImmutableList<SessionSegment> segments = [
                        new SessionSegmentLobby(),
                new SessionSegmentQuiz()
                {
                    QuestionContent = new Content("Kolik je `10 + 20`?"),
                    AnswerOptions = [
                                new AnswerOption() { Content = new Content("10") },
                        new AnswerOption() { Content = new Content("20") },
                        new AnswerOption() { Content = new Content("30"), IsCorrect = true },
                        new AnswerOption() { Content = new Content("40") }
                            ]
                },
                new SessionSegmentQuiz()
                {
                    QuestionContent = new Content("What is the capital of France?"),
                    AnswerOptions = [
                                new AnswerOption() { Content = new Content("Madrid") },
                        new AnswerOption() { Content = new Content("Paris"), IsCorrect = true },
                        new AnswerOption() { Content = new Content("Berlin") },
                        new AnswerOption() { Content = new Content("London") }
                            ]
                },
                new SessionSegmentEnd()
            ];

            return new Session()
            {
                IsUnlocked = true,
                HasStarted = false,
                JoinCode = "123456", //sessionCodeGenerator.GenerateCode(),
                SessionId = "aaaaa",
                Segments = segments,
                CurrentSegment = new SessionSegmentProgress()
                {
                    Segment = segments[0],
                    SegmentId = 0
                }
            };
        }

        public async Task DeleteAsync()
        {
            foreach (var f in Directory.GetFiles("bin/", "tmp.data-*.json"))
            {
                File.Delete(f);
            }
        }

        public async Task GenerateAsync()
        {
            if (await sessionRepo.TryGetAsync(session.SessionId) is (true, _, _))
            {
                // already exists
                return;
            }

            (bool result, VersionKey version) = await sessionRepo.TrySetAsync(session.SessionId, session, VersionKey.NonExisting);
            if(!result)
            {
                throw new InvalidOperationException("Can't save.");
            }

            var activeSessions = ActiveSessions.Empty
                .WithAddSession(session.SessionId, session.JoinCode ?? throw new InvalidOperationException());

            (result, version) = await actionSessionsRepo.TrySetAsync(string.Empty, activeSessions, VersionKey.NonExisting);

            if (!result)
            {
                throw new InvalidOperationException("Can't save.");
            }
        }
    }
}
