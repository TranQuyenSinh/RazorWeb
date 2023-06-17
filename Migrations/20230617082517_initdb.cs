using System;
using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;
using Models;

#nullable disable

namespace razorweb.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.Id);
                });
            // Insert Data
            migrationBuilder.InsertData(
                table: "articles",
                columns: new[] { "Title", "Content", "Created" },
                values: new object[] {
                    "Bai viet 1",
                    "Noi dung 1",
                    new DateTime(2023, 7, 17)
                }
            );

            // Fake Data: Bogus
            Randomizer.Seed = new Random(8675309);
            var fakerArticle = new Faker<Article>();
            // phát sinh Title của article là câu văn với tối thiểu 5 từ và thêm bớt 5 từ nữa
            fakerArticle.RuleFor(x => x.Title, f => f.Lorem.Sentence(5, 5));
            // 1 đến 4 đoạn văn, cách nhau bằng \n
            fakerArticle.RuleFor(x => x.Content, f => f.Lorem.Paragraphs(1, 4));
            fakerArticle.RuleFor(x => x.Created, f => f.Date.Between(new DateTime(2021, 1, 1), DateTime.Now));

            for (int i = 0; i < 150; i++)
            {
                Article article = fakerArticle.Generate();
                migrationBuilder.InsertData(
                table: "articles",
                columns: new[] { "Title", "Content", "Created" },
                values: new object[] {
                        article.Title,
                        article.Content,
                        article.Created
                }
            );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles");
        }
    }
}
