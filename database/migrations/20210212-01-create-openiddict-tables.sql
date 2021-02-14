CREATE TABLE "OpenIddictApplications" (
    "Id" text NOT NULL,
    "ClientId" character varying(100) NULL,
    "ClientSecret" text NULL,
    "ConcurrencyToken" character varying(50) NULL,
    "ConsentType" character varying(50) NULL,
    "DisplayName" text NULL,
    "DisplayNames" text NULL,
    "Permissions" text NULL,
    "PostLogoutRedirectUris" text NULL,
    "Properties" text NULL,
    "RedirectUris" text NULL,
    "Requirements" text NULL,
    "Type" character varying(50) NULL,
    CONSTRAINT "PK_OpenIddictApplications" PRIMARY KEY ("Id")
);


CREATE TABLE "OpenIddictScopes" (
    "Id" text NOT NULL,
    "ConcurrencyToken" character varying(50) NULL,
    "Description" text NULL,
    "Descriptions" text NULL,
    "DisplayName" text NULL,
    "DisplayNames" text NULL,
    "Name" character varying(200) NULL,
    "Properties" text NULL,
    "Resources" text NULL,
    CONSTRAINT "PK_OpenIddictScopes" PRIMARY KEY ("Id")
);


CREATE TABLE "OpenIddictAuthorizations" (
    "Id" text NOT NULL,
    "ApplicationId" text NULL,
    "ConcurrencyToken" character varying(50) NULL,
    "CreationDate" timestamp without time zone NULL,
    "Properties" text NULL,
    "Scopes" text NULL,
    "Status" character varying(50) NULL,
    "Subject" character varying(400) NULL,
    "Type" character varying(50) NULL,
    CONSTRAINT "PK_OpenIddictAuthorizations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~" FOREIGN KEY ("ApplicationId") REFERENCES "OpenIddictApplications" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "OpenIddictTokens" (
    "Id" text NOT NULL,
    "ApplicationId" text NULL,
    "AuthorizationId" text NULL,
    "ConcurrencyToken" character varying(50) NULL,
    "CreationDate" timestamp without time zone NULL,
    "ExpirationDate" timestamp without time zone NULL,
    "Payload" text NULL,
    "Properties" text NULL,
    "RedemptionDate" timestamp without time zone NULL,
    "ReferenceId" character varying(100) NULL,
    "Status" character varying(50) NULL,
    "Subject" character varying(400) NULL,
    "Type" character varying(50) NULL,
    CONSTRAINT "PK_OpenIddictTokens" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId" FOREIGN KEY ("ApplicationId") REFERENCES "OpenIddictApplications" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId" FOREIGN KEY ("AuthorizationId") REFERENCES "OpenIddictAuthorizations" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX "IX_OpenIddictApplications_ClientId" ON "OpenIddictApplications" ("ClientId");
CREATE INDEX "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type" ON "OpenIddictAuthorizations" ("ApplicationId", "Status", "Subject", "Type");
CREATE UNIQUE INDEX "IX_OpenIddictScopes_Name" ON "OpenIddictScopes" ("Name");
CREATE INDEX "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type" ON "OpenIddictTokens" ("ApplicationId", "Status", "Subject", "Type");
CREATE INDEX "IX_OpenIddictTokens_AuthorizationId" ON "OpenIddictTokens" ("AuthorizationId");
CREATE UNIQUE INDEX "IX_OpenIddictTokens_ReferenceId" ON "OpenIddictTokens" ("ReferenceId");
