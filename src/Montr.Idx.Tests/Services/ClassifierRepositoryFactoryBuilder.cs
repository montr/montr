using System;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Montr.Core.Services;
using Montr.Idx.Services.Implementations;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Services.Implementations;
using Montr.Metadata.Models;
using Montr.Metadata.Services.Implementations;
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
			var dbFieldDataRepository = new DbFieldDataRepository(NullLogger<DbFieldDataRepository>.Instance, _dbContextFactory, fieldProviderRegistry);

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

			var roleRepository = new DbRoleRepository(NullLogger<DbRoleRepository>.Instance, _dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null, identityServiceFactory.RoleManager);

			var userRepository = new DbUserRepository(NullLogger<DbUserRepository>.Instance, _dbContextFactory,
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
