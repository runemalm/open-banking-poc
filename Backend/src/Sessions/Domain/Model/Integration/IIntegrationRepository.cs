﻿using DDD.Domain.Model;

namespace Sessions.Domain.Model.Integration
{
	public interface IIntegrationRepository : IRepository<Integration, Guid>
	{
		Task<Integration> GetByNameAsync(string name);
	}
}
