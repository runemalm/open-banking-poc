﻿using OpenDDD.Domain.Service;
using Sessions.Domain.Model;
using Sessions.Domain.Model.Input;

namespace Sessions.Domain.Services
{
	public interface ISessionDomainService : IDomainService
	{
		Task<(Session, Input)> CreateSessionAsync(SessionType type, string? bankId, string? integrationId, CancellationToken ct);
		Task SelectBankAsync(Session session, string bankId, CancellationToken ct);
		Task SelectIntegrationAsync(Session session, string integrationId, CancellationToken ct);
		Task<Session> StartSessionAsync(Guid sessionId, CancellationToken ct);
		Task<Input> ProvideInputAsync(Session session, string value, CancellationToken ct);
		Task<(State, Input)> GetCurrentStateAndInputAsync(Guid sessionId, CancellationToken ct);
	}
}
