--Enabling implicit type conversions

DROP CAST IF EXISTS (varchar AS integer);
CREATE CAST (varchar AS integer) WITH INOUT AS IMPLICIT;

DROP CAST IF EXISTS (varchar AS uuid);
CREATE CAST (varchar AS uuid) WITH INOUT AS IMPLICIT;

DROP CAST IF EXISTS (text AS integer);
CREATE CAST (text AS integer) WITH INOUT AS IMPLICIT;

DROP CAST IF EXISTS (uuid AS text);
CREATE CAST (uuid AS text) WITH INOUT AS IMPLICIT;

DROP CAST IF EXISTS (text AS boolean);
CREATE CAST (text AS boolean) WITH INOUT AS IMPLICIT;

DROP CAST IF EXISTS (text AS numeric);
CREATE CAST (text AS numeric) WITH INOUT AS IMPLICIT;

DROP CAST IF EXISTS (text AS uuid);
CREATE CAST (text AS uuid) WITH INOUT AS IMPLICIT;

DROP FUNCTION IF EXISTS public."fn_CastTimeToDateTime" CASCADE;
CREATE FUNCTION public."fn_CastTimeToDateTime"(sourceValue TIME WITHOUT TIME ZONE)
RETURNS TIMESTAMP WITHOUT TIME ZONE
AS $BODY$
BEGIN
	RETURN (MAKE_DATE(1900, 01, 01) + sourceValue)::TIMESTAMP WITHOUT TIME ZONE;
END;
$BODY$
LANGUAGE PLPGSQL;

DROP CAST IF EXISTS (TIME WITHOUT TIME ZONE AS TIMESTAMP WITHOUT TIME ZONE);
CREATE CAST (TIME WITHOUT TIME ZONE AS TIMESTAMP WITHOUT TIME ZONE) 
	WITH FUNCTION public."fn_CastTimeToDateTime"(TIME WITHOUT TIME ZONE) AS IMPLICIT;