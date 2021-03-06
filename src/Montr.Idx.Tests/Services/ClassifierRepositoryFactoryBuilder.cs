﻿using System;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Impl.Services;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Moq;

namespace Montr.Idx.Tests.Services
{
	public class ClassifierRepositoryFactoryBuilder
	{
		private readonly IDbContextFactory _dbContextFactory;

		public string RoleTypeCode { get; set; } = "role_for_test";

		public string UserTypeCode { get; set; } = "user_for_test";

		public ClassifierRepositoryFactoryBuilder(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public INamedServiceFactory<IClassifierRepository> Build()
		{
			var classifierTypeRepository = new DbClassifierTypeRepository(_dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(_dbContextFactory, classifierTypeRepository);

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(TextField));
			var dbFieldDataRepository = new DbFieldDataRepository(_dbContextFactory, fieldProviderRegistry);

			var metadataServiceMock = new Mock<IClassifierTypeMetadataService>();
			metadataServiceMock
				.Setup(x => x.GetMetadata(It.IsAny<ClassifierType>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => new FieldMetadata[]
				{
					/*new TextField { Key = "test1", Active = true, System = false },
					new TextField { Key = "test2", Active = true, System = false },
					new TextField { Key = "test3", Active = true, System = false }*/
				});

			var identityServiceFactory = new IdentityServiceFactory();

			var roleRepository = new DbRoleRepository(new NullLogger<DbRoleRepository>(), _dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null, identityServiceFactory.RoleManager);

			var userRepository = new DbUserRepository(new NullLogger<DbUserRepository>(), _dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null, identityServiceFactory.UserManager);

			var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == RoleTypeCode)))
				.Returns(() => roleRepository);

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == UserTypeCode)))
				.Returns(() => userRepository);

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name != RoleTypeCode && name != UserTypeCode )))
				.Throws<InvalidOperationException>();

			return classifierRepositoryFactoryMock.Object;
		}
	}
}
