﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Yaginx.DomainModels;

#nullable disable

namespace Yaginx.DataStore.PostgreSQLStore.Migrations
{
    [DbContext(typeof(CenterDbContext))]
    [Migration("20240302112525_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Yaginx.DataStore.PostgreSQLStore.Abstracted.WebPage", b =>
                {
                    b.Property<long>("PageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("page_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("PageId"));

                    b.Property<string>("Content")
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.HasKey("PageId")
                        .HasName("pk_web_page");

                    b.ToTable("web_page", (string)null);
                });

            modelBuilder.Entity("Yaginx.DataStore.PostgreSQLStore.Entities.AccountEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PasswordSalt")
                        .HasColumnType("text")
                        .HasColumnName("password_salt");

                    b.HasKey("Id")
                        .HasName("pk_account");

                    b.ToTable("account", (string)null);
                });

            modelBuilder.Entity("Yaginx.DataStore.PostgreSQLStore.Entities.HostTrafficEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("HostName")
                        .HasColumnType("text")
                        .HasColumnName("host_name");

                    b.Property<long>("InboundBytes")
                        .HasColumnType("bigint")
                        .HasColumnName("inbound_bytes");

                    b.Property<long>("OutboundBytes")
                        .HasColumnType("bigint")
                        .HasColumnName("outbound_bytes");

                    b.Property<long>("Period")
                        .HasColumnType("bigint")
                        .HasColumnName("period");

                    b.Property<long>("RequestCounts")
                        .HasColumnType("bigint")
                        .HasColumnName("request_counts");

                    b.HasKey("Id")
                        .HasName("pk_host_traffic");

                    b.ToTable("host_traffic", (string)null);
                });

            modelBuilder.Entity("Yaginx.DataStore.PostgreSQLStore.Entities.WebDomainEntity", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long?>("Id"));

                    b.Property<string>("FreeCertMessage")
                        .HasColumnType("text")
                        .HasColumnName("free_cert_message");

                    b.Property<bool>("IsUseFreeCert")
                        .HasColumnType("boolean")
                        .HasColumnName("is_use_free_cert");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean")
                        .HasColumnName("is_verified");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_web_domain");

                    b.ToTable("web_domain", (string)null);
                });

            modelBuilder.Entity("Yaginx.DataStore.PostgreSQLStore.Entities.WebsiteEntity", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long?>("Id"));

                    b.Property<DateTimeOffset>("CreateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<string[]>("Hosts")
                        .HasColumnType("jsonb")
                        .HasColumnName("hosts");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<List<WebsiteProxyRuleItem>>("ProxyRules")
                        .HasColumnType("jsonb")
                        .HasColumnName("proxy_rules");

                    b.Property<Dictionary<string, string>>("ProxyTransforms")
                        .HasColumnType("jsonb")
                        .HasColumnName("proxy_transforms");

                    b.Property<SimpleResponseItem[]>("SimpleResponses")
                        .HasColumnType("jsonb")
                        .HasColumnName("simple_responses");

                    b.Property<WebsiteSpecifications>("Specifications")
                        .HasColumnType("jsonb")
                        .HasColumnName("specifications");

                    b.Property<DateTimeOffset>("UpdateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_time");

                    b.HasKey("Id")
                        .HasName("pk_website");

                    b.ToTable("website", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}