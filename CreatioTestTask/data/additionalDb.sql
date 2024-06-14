--
-- PostgreSQL database dump
--

-- Dumped from database version 14.2
-- Dumped by pg_dump version 14.11

-- Started on 2024-06-14 11:56:14 UTC

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP DATABASE IF EXISTS postgres;
--
-- TOC entry 3323 (class 1262 OID 13756)
-- Name: postgres; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE postgres WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'en_US.utf8';


ALTER DATABASE postgres OWNER TO postgres;

\connect postgres

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 3324 (class 0 OID 0)
-- Dependencies: 3323
-- Name: DATABASE postgres; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON DATABASE postgres IS 'default administrative connection database';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 211 (class 1259 OID 73309)
-- Name: Address; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Address" (
    "Id" uuid NOT NULL,
    "PhoneNumber" text,
    "Email" text,
    "City" text NOT NULL,
    "Home" text,
    "IsActive" boolean NOT NULL,
    "AddresType" boolean NOT NULL
);


ALTER TABLE public."Address" OWNER TO postgres;

--
-- TOC entry 209 (class 1259 OID 73295)
-- Name: Contacts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Contacts" (
    "Id" uuid NOT NULL,
    "PhoneNumber" text,
    "Email" text,
    "City" text,
    "Home" text,
    "IsActive" boolean NOT NULL,
    "DateOfBirth" date NOT NULL,
    "FullName" text NOT NULL,
    "Gender" boolean NOT NULL
);


ALTER TABLE public."Contacts" OWNER TO postgres;

--
-- TOC entry 210 (class 1259 OID 73302)
-- Name: Payments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Payments" (
    "Id" uuid NOT NULL,
    "PhoneNumber" text,
    "Email" text,
    "Ammount" money NOT NULL,
    "PaymentDetails" text,
    "PaymentDateTime" timestamp without time zone NOT NULL
);


ALTER TABLE public."Payments" OWNER TO postgres;

--
-- TOC entry 3317 (class 0 OID 73309)
-- Dependencies: 211
-- Data for Name: Address; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."Address" VALUES ('b1eaf6d4-2bd7-4d1d-b230-0b88377fa694', NULL, 'RileyLeach@gmail.com', 'Sydney', '20 The Strand', true, true) ON CONFLICT DO NOTHING;
INSERT INTO public."Address" VALUES ('040a80de-db51-47ba-8a36-b2f999619fa8', '+932168349', 'RileyLeach@gmail.com', 'Moscow', '126 Boulevard Saint-Raymond', false, true) ON CONFLICT DO NOTHING;
INSERT INTO public."Address" VALUES ('3006c975-798f-4226-989c-d573d6111b38', '+932168349', NULL, 'Kyiv', '5160 Explorer Dr #36', true, true) ON CONFLICT DO NOTHING;
INSERT INTO public."Address" VALUES ('c75ebc14-f4ad-4a39-8278-f2e0dca17c1e', NULL, 'ChristinaFrank@gmail.com', 'Port Moresby', '324 Portland Rd', false, false) ON CONFLICT DO NOTHING;
INSERT INTO public."Address" VALUES ('f9cc78df-6fd2-4e1a-a186-47c27c7d525d', '+304957283', NULL, 'Dhaka', 'Rheinstrasse 19', true, false) ON CONFLICT DO NOTHING;


--
-- TOC entry 3315 (class 0 OID 73295)
-- Dependencies: 209
-- Data for Name: Contacts; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."Contacts" VALUES ('5ba22bff-014f-4cd9-b836-8d36952c6a38', '+932168349', 'RileyLeach@gmail.com', 'Harare', '5010 Monique Falls, Christiansenmouth, KY 13066-8897', true, '1991-04-02', 'Riley Leach', false) ON CONFLICT DO NOTHING;
INSERT INTO public."Contacts" VALUES ('30e3c477-4ea2-4111-a86c-2098c915e519', NULL, 'FinleyMartin@gmail.com', 'Melbourne', '3497 Stacey Throughway, West Jon, IA 65004-8104', false, '1979-07-02', 'Finley Martin', true) ON CONFLICT DO NOTHING;
INSERT INTO public."Contacts" VALUES ('27a183fe-30cc-4537-be43-62d8518bc066', '+719573085', NULL, 'Calgary', 'Barranco Lucia Munguía 75, Guadalajara, Leo 44591', true, '1980-05-15', 'Emerson Brady', true) ON CONFLICT DO NOTHING;
INSERT INTO public."Contacts" VALUES ('5dd2948e-0d49-499f-87dd-6dc2e97942cf', NULL, 'ChristinaFrank@gmail.com', 'Asmara', '46 Graham Avenue, Broxbourne,EN10 7DS', false, '2000-12-21', 'Christina Frank', false) ON CONFLICT DO NOTHING;
INSERT INTO public."Contacts" VALUES ('be59460b-012f-4368-bab6-3621f7064056', '+304957283', NULL, 'Bobruisk', '2 OG Hans-von-Dohnanyi-Str. 40, West Victorialand, HB 18526', true, '1990-01-23', 'Kayla Pierce', false) ON CONFLICT DO NOTHING;


--
-- TOC entry 3316 (class 0 OID 73302)
-- Dependencies: 210
-- Data for Name: Payments; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."Payments" VALUES ('0ab6f94c-9c6d-4838-ae0b-15ae72d60eaa', NULL, 'ChristinaFrank@gmail.com', '$3.67', 'Wendys set', '2024-07-26 16:46:08') ON CONFLICT DO NOTHING;
INSERT INTO public."Payments" VALUES ('e38e557d-7330-4c0c-83dd-e5f7c8095b77', NULL, 'ChristinaFrank@gmail.com', '$80.35', 'Games in steam', '2025-05-07 02:22:04') ON CONFLICT DO NOTHING;
INSERT INTO public."Payments" VALUES ('e94a4a8c-e355-4983-9cda-a834bd678d6b', NULL, 'RileyLeach@gmail.com', '$1.42', 'Cheesburger', '2024-10-13 00:46:24') ON CONFLICT DO NOTHING;
INSERT INTO public."Payments" VALUES ('38a45a00-50cd-4064-b770-5847c5e80d8c', '+932168349', 'RileyLeach@gmail.com', '$500.32', 'Table with wireless chrager', '2024-09-29 02:56:48') ON CONFLICT DO NOTHING;
INSERT INTO public."Payments" VALUES ('a4b484ea-fa2e-40fc-bd71-12552dc4c881', '+932168349', NULL, '$1,000.00', 'Netflix services for 5 years', '2024-07-06 02:57:04') ON CONFLICT DO NOTHING;


--
-- TOC entry 3175 (class 2606 OID 73315)
-- Name: Address Address_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Address"
    ADD CONSTRAINT "Address_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 3171 (class 2606 OID 73301)
-- Name: Contacts Contacts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Contacts"
    ADD CONSTRAINT "Contacts_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 3173 (class 2606 OID 73308)
-- Name: Payments Payments_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Payments"
    ADD CONSTRAINT "Payments_pkey" PRIMARY KEY ("Id");


-- Completed on 2024-06-14 11:56:14 UTC

--
-- PostgreSQL database dump complete
--

