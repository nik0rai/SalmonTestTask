UPDATE "SysSettingsValue" SET "TextValue" = 'nikorai' WHERE "SysSettingsId" = (SELECT "Id" FROM "SysSettings" WHERE "Code" ='CustomerId' LIMIT 1);

UPDATE "SysSettingsValue" SET "TextValue" = 'NIKORAI' WHERE "SysSettingsId" = (SELECT "Id" FROM "SysSettings" WHERE "Code" ='Maintainer' LIMIT 1);

UPDATE "SysSettingsValue" SET "TextValue" = 'NIKO' WHERE "SysSettingsId" = (SELECT "Id" FROM "SysSettings" WHERE "Code" ='SchemaNamePrefix' LIMIT 1);
UPDATE "SysSettingsValue" SET "TextValue" = '' WHERE "SysSettingsId" = (SELECT "Id" FROM "SysSettings" WHERE "Code" ='GlobalSearchUrl' LIMIT 1);
UPDATE "SysSettingsValue" SET "TextValue" = '' WHERE "SysSettingsId" = (SELECT "Id" FROM "SysSettings" WHERE "Code" ='GlobalSearchConfigServiceUrl' LIMIT 1);
UPDATE "SysSettingsValue" SET "TextValue" = '' WHERE "SysSettingsId" = (SELECT "Id" FROM "SysSettings" WHERE "Code" ='GlobalSearchIndexingApiUrl' LIMIT 1);
UPDATE "SysSettingsValue" SET "TextValue" = '' WHERE "SysSettingsId" = (SELECT "Id" FROM "SysSettings" WHERE "Code" ='GlobalSearchIndexingApiUrl' LIMIT 1);
DO $$

DECLARE 
    GlobalSearchFeature VARCHAR(50) := 'GlobalSearch';
    GlobalSearchFeatureId uuid;
    GlobalSearchV2Feature VARCHAR(50) := 'GlobalSearch_V2';
    GlobalSearchV2FeatureId uuid;
    GS_RelatedEntityIndexingFeature VARCHAR(50) :=   'GlobalSearchRelatedEntityIndexing';
    GS_RelatedEntityIndexingFeatureId uuid;
    allEmployeesId uuid := 'A29A3BA5-4B0D-DE11-9A51-005056C00008';

BEGIN

   SELECT "Id" INTO GlobalSearchFeatureId FROM "Feature"
   WHERE "Code" = GlobalSearchFeature
   LIMIT 1;
   IF (GlobalSearchFeatureId IS NOT NULL)
      THEN
          IF EXISTS (SELECT * FROM "AdminUnitFeatureState" WHERE "FeatureId" = GlobalSearchFeatureId) THEN
             UPDATE "AdminUnitFeatureState" SET "FeatureState" = 0 WHERE "FeatureId" = GlobalSearchFeatureId;
          ELSE
              INSERT INTO "AdminUnitFeatureState" ("SysAdminUnitId", "FeatureState", "FeatureId") VALUES (allEmployeesId, '1', GlobalSearchFeatureId);
          END IF;
   ELSE
       GlobalSearchFeatureId := uuid_generate_v4();
       INSERT INTO "Feature" ("Id", "Name", "Code") VALUES (GlobalSearchFeatureId, GlobalSearchFeature, GlobalSearchFeature);
       INSERT INTO "AdminUnitFeatureState" ("SysAdminUnitId", "FeatureState", "FeatureId") VALUES (allEmployeesId, '1', GlobalSearchFeatureId);
   END IF;

   SELECT "Id" INTO GlobalSearchV2FeatureId FROM "Feature"
   WHERE "Code" = GlobalSearchV2Feature
   LIMIT 1;
   IF (GlobalSearchV2FeatureId IS NOT NULL)
    THEN
        IF EXISTS (SELECT * FROM "AdminUnitFeatureState" WHERE "FeatureId" = GlobalSearchV2FeatureId) THEN
            UPDATE "AdminUnitFeatureState" SET "FeatureState" = 0 WHERE "FeatureId" = GlobalSearchV2FeatureId;
        ELSE
           INSERT INTO "AdminUnitFeatureState" ("SysAdminUnitId", "FeatureState", "FeatureId") VALUES (allEmployeesId, '0', GlobalSearchV2FeatureId);
        END IF;
   ELSE
       GlobalSearchV2FeatureId := uuid_generate_v4();
       INSERT INTO "Feature" ("Id", "Name", "Code") VALUES (GlobalSearchV2FeatureId, GlobalSearchV2Feature, GlobalSearchV2Feature);
       INSERT INTO "AdminUnitFeatureState" ("SysAdminUnitId", "FeatureState", "FeatureId") VALUES (allEmployeesId, '0', GlobalSearchV2FeatureId);
   END IF;

  SELECT "Id" INTO GS_RelatedEntityIndexingFeatureId FROM "Feature" WHERE "Code" =GS_RelatedEntityIndexingFeature LIMIT 1;
  IF (GS_RelatedEntityIndexingFeatureId IS NOT NULL)
 THEN
    IF EXISTS (SELECT * FROM "AdminUnitFeatureState" WHERE "FeatureId" = GS_RelatedEntityIndexingFeatureId) THEN
UPDATE "AdminUnitFeatureState" SET "FeatureState" = 0 WHERE "FeatureId" = GS_RelatedEntityIndexingFeatureId;
  ELSE
  INSERT INTO "AdminUnitFeatureState" ("SysAdminUnitId", "FeatureState","FeatureId") VALUES (allEmployeesId, '0', GS_RelatedEntityIndexingFeatureId);
  END IF;
  ELSE
  GS_RelatedEntityIndexingFeatureId := uuid_generate_v4();
  INSERT INTO "Feature" ("Id", "Name", "Code") VALUES (GS_RelatedEntityIndexingFeatureId, GS_RelatedEntityIndexingFeature, GS_RelatedEntityIndexingFeature);
  INSERT INTO "AdminUnitFeatureState" ("SysAdminUnitId", "FeatureState","FeatureId") VALUES (allEmployeesId, '0', GS_RelatedEntityIndexingFeatureId);
  END IF;
END $$;