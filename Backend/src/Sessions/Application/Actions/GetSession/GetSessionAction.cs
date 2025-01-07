﻿using DDD.Application;
using Demo.Domain.Model;

namespace Demo.Application.Actions.GetSession
{
    public class GetSessionAction : IAction<GetSessionCommand, Session>
    {
        private readonly ISessionRepository _sessionRepository;

        public GetSessionAction(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<Session> ExecuteAsync(GetSessionCommand command, CancellationToken ct)
        {
            var session = await _sessionRepository.GetAsync(command.SessionId);
            return session;
        }
    }
}
