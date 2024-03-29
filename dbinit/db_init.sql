--
-- PostgreSQL database dump
--

-- Dumped from database version 13.3 (Debian 13.3-1.pgdg100+1)
-- Dumped by pg_dump version 13.3

-- Started on 2022-04-16 20:12:28 CST

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

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 203 (class 1259 OID 16459)
-- Name: CustomRanks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CustomRanks" (
    "SubjectId" integer NOT NULL,
    "SciRank" integer NOT NULL
);


ALTER TABLE public."CustomRanks" OWNER TO postgres;

--
-- TOC entry 205 (class 1259 OID 16471)
-- Name: SubjectEntities; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SubjectEntities" (
    "Id" integer NOT NULL,
    "SubjectId" integer NOT NULL,
    "Alias" text,
    "NormalizedAlias" text
);


ALTER TABLE public."SubjectEntities" OWNER TO postgres;

--
-- TOC entry 204 (class 1259 OID 16469)
-- Name: SubjectEntities_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."SubjectEntities" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."SubjectEntities_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 201 (class 1259 OID 16446)
-- Name: Subjects; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Subjects" (
    "Id" integer NOT NULL,
    "Name" character varying(80) NOT NULL,
    "NameCN" character varying(80),
    "Infobox" json,
    "Platform" integer NOT NULL,
    "Summary" text,
    "Rank" integer,
    "NSFW" boolean NOT NULL,
    "Type" text,
    "FavCount" integer NOT NULL,
    "RateCount" integer NOT NULL,
    "CollectCount" integer NOT NULL,
    "DoCount" integer NOT NULL,
    "DroppedCount" integer NOT NULL,
    "OnHoldCount" integer NOT NULL,
    "WishCount" integer NOT NULL
);


ALTER TABLE public."Subjects" OWNER TO postgres;

--
-- TOC entry 207 (class 1259 OID 16486)
-- Name: Tags; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Tags" (
    "Id" integer NOT NULL,
    "SubjectId" integer NOT NULL,
    "Content" text,
    "NormalizedContent" text,
    "UserCount" integer NOT NULL,
    "Confidence" double precision NOT NULL,
    "NormUserCount" integer DEFAULT 0 NOT NULL
);


ALTER TABLE public."Tags" OWNER TO postgres;

--
-- TOC entry 206 (class 1259 OID 16484)
-- Name: Tags_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Tags" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Tags_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 202 (class 1259 OID 16454)
-- Name: Timestamps; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Timestamps" (
    "Date" timestamp without time zone NOT NULL
);


ALTER TABLE public."Timestamps" OWNER TO postgres;

--
-- TOC entry 200 (class 1259 OID 16441)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- TOC entry 2836 (class 2606 OID 16463)
-- Name: CustomRanks PK_CustomRanks; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomRanks"
    ADD CONSTRAINT "PK_CustomRanks" PRIMARY KEY ("SubjectId");


--
-- TOC entry 2839 (class 2606 OID 16478)
-- Name: SubjectEntities PK_SubjectEntities; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SubjectEntities"
    ADD CONSTRAINT "PK_SubjectEntities" PRIMARY KEY ("Id");


--
-- TOC entry 2832 (class 2606 OID 16453)
-- Name: Subjects PK_Subjects; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Subjects"
    ADD CONSTRAINT "PK_Subjects" PRIMARY KEY ("Id");


--
-- TOC entry 2842 (class 2606 OID 16493)
-- Name: Tags PK_Tags; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Tags"
    ADD CONSTRAINT "PK_Tags" PRIMARY KEY ("Id");


--
-- TOC entry 2834 (class 2606 OID 16458)
-- Name: Timestamps PK_Timestamps; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Timestamps"
    ADD CONSTRAINT "PK_Timestamps" PRIMARY KEY ("Date");


--
-- TOC entry 2830 (class 2606 OID 16445)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 2837 (class 1259 OID 52544)
-- Name: IX_SubjectEntities_SubjectId_NormalizedAlias; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SubjectEntities_SubjectId_NormalizedAlias" ON public."SubjectEntities" USING btree ("SubjectId", "NormalizedAlias");


--
-- TOC entry 2840 (class 1259 OID 52543)
-- Name: IX_Tags_SubjectId_NormalizedContent; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Tags_SubjectId_NormalizedContent" ON public."Tags" USING btree ("SubjectId", "NormalizedContent");


--
-- TOC entry 2843 (class 2606 OID 16464)
-- Name: CustomRanks FK_CustomRanks_Subjects_SubjectId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomRanks"
    ADD CONSTRAINT "FK_CustomRanks_Subjects_SubjectId" FOREIGN KEY ("SubjectId") REFERENCES public."Subjects"("Id") ON DELETE CASCADE;


--
-- TOC entry 2844 (class 2606 OID 16479)
-- Name: SubjectEntities FK_SubjectEntities_Subjects_SubjectId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SubjectEntities"
    ADD CONSTRAINT "FK_SubjectEntities_Subjects_SubjectId" FOREIGN KEY ("SubjectId") REFERENCES public."Subjects"("Id") ON DELETE CASCADE;


--
-- TOC entry 2845 (class 2606 OID 16494)
-- Name: Tags FK_Tags_Subjects_SubjectId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Tags"
    ADD CONSTRAINT "FK_Tags_Subjects_SubjectId" FOREIGN KEY ("SubjectId") REFERENCES public."Subjects"("Id") ON DELETE CASCADE;


-- Completed on 2022-04-16 20:12:28 CST

--
-- PostgreSQL database dump complete
--

