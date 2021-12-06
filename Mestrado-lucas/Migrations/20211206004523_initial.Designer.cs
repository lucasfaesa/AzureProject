﻿// <auto-generated />
using System;
using Mestrado_lucas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mestrado_lucas.Migrations
{
    [DbContext(typeof(Mestrado_lucasContext))]
    [Migration("20211206004523_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.21");

            modelBuilder.Entity("Mestrado_lucas.Aluno", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DtCriacao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DtUltimoLogin")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginNome")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nome")
                        .HasColumnType("TEXT");

                    b.Property<string>("Senha")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Aluno");
                });

            modelBuilder.Entity("Mestrado_lucas.Models.Fase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Descricao")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nome")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Fase");
                });

            modelBuilder.Entity("Mestrado_lucas.Models.Sessao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AlunoId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Concluida")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DtConclusao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DtUltimaJogada")
                        .HasColumnType("TEXT");

                    b.Property<int?>("FaseId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Pontuacao")
                        .HasColumnType("INTEGER");

                    b.Property<float?>("TempoDecorrido")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("AlunoId");

                    b.HasIndex("FaseId");

                    b.ToTable("Sessao");
                });

            modelBuilder.Entity("Mestrado_lucas.Models.Sessao", b =>
                {
                    b.HasOne("Mestrado_lucas.Aluno", "Aluno")
                        .WithMany()
                        .HasForeignKey("AlunoId");

                    b.HasOne("Mestrado_lucas.Models.Fase", "Fase")
                        .WithMany()
                        .HasForeignKey("FaseId");
                });
#pragma warning restore 612, 618
        }
    }
}
