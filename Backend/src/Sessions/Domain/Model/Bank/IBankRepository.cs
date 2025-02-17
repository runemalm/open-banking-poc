﻿using OpenDDD.Domain.Model;

namespace Sessions.Domain.Model.Bank
{
	public interface IBankRepository : IRepository<Bank, Guid>
	{
		Task<Bank> GetByNameAsync(string name, CancellationToken ct);
	}
}
