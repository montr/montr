using Microsoft.Extensions.Logging.Abstractions;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.Docs.Services.Implementations;
using Montr.Idx.Impl.Services;
using Montr.Idx.Tests.Services;
using Montr.Kompany.Impl.Services;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Moq;

namespace Montr.Kompany.Tests.Services
{
	public class CompanyMockHelper
	{
		public static INamedServiceFactory<IClassifierRepository> CreateClassifierRepositoryFactory(IDbContextFactory dbContextFactory)
    	{
    		var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
    		var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

    		var fieldProviderRegistry = new DefaultFieldProviderRegistry();
    		fieldProviderRegistry.AddFieldType(typeof(TextField));
    		fieldProviderRegistry.AddFieldType(typeof(TextAreaField));
    		var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, fieldProviderRegistry);

    		var metadataService = new ClassifierTypeMetadataService(new DbFieldMetadataRepository(
    			dbContextFactory, fieldProviderRegistry, new NewtonsoftJsonSerializer()));

    		var classifierRepository = new DbClassifierRepository<Classifier>(dbContextFactory,
    			classifierTypeService, null, metadataService, dbFieldDataRepository, null);

    		var numeratorRepository = new DbNumeratorRepository(dbContextFactory,
    			classifierTypeService, null, metadataService, dbFieldDataRepository, null);

    		var documentTypeRepository = new DbDocumentTypeRepository(dbContextFactory,
    			classifierTypeService, null, metadataService, dbFieldDataRepository, null);

    		var companyRepository = new DbCompanyRepository(dbContextFactory,
    			classifierTypeService, null, metadataService, dbFieldDataRepository, null);

    		var userRepository = new DbUserRepository(NullLogger<DbUserRepository>.Instance,  dbContextFactory,
    			classifierTypeService, null, metadataService, dbFieldDataRepository, null,  new IdentityServiceFactory().UserManager);

    		var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();

    		classifierRepositoryFactoryMock
    			.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == MasterData.ClassifierTypeCode.Numerator)))
    			.Returns(() => numeratorRepository);
    		classifierRepositoryFactoryMock
    			.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == Docs.ClassifierTypeCode.DocumentType)))
    			.Returns(() => documentTypeRepository);
    		classifierRepositoryFactoryMock
    			.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == Idx.ClassifierTypeCode.User)))
    			.Returns(() => userRepository);
    		classifierRepositoryFactoryMock
    			.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == ClassifierTypeCode.Company)))
    			.Returns(() => companyRepository);
    		classifierRepositoryFactoryMock
    			.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name =>
    				name != MasterData.ClassifierTypeCode.Numerator &&
    				name != Docs.ClassifierTypeCode.DocumentType &&
    				name != Idx.ClassifierTypeCode.User &&
    				name != ClassifierTypeCode.Company)))
    			.Returns(() => classifierRepository);

    		return classifierRepositoryFactoryMock.Object;
    	}
	}
}
