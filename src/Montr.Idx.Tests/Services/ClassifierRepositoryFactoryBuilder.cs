using System;
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
		public static readonly string RoleTypeCode = "role_for_test";
		public static readonly string UserTypeCode = "user_for_test";

		private readonly IDbContextFactory _dbContextFactory;

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

			var roleManager = new DefaultRoleManager(new NullLogger<DefaultRoleManager>(), identityServiceFactory.RoleManager);
			var userManager = new DefaultUserManager(new NullLogger<DefaultUserManager>(), identityServiceFactory.UserManager);

			var roleRepository = new DbRoleRepository(_dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null, roleManager);

			var userRepository = new DbUserRepository(_dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null, userManager);

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
