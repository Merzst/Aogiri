using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Aogiri.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category_2",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category_2", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Location_3",
                columns: table => new
                {
                    LocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location_3", x => x.LocationID);
                });

            migrationBuilder.CreateTable(
                name: "ModerationRule_8",
                columns: table => new
                {
                    RuleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Phrase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationRule_8", x => x.RuleID);
                });

            migrationBuilder.CreateTable(
                name: "User_1",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Active"),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_1", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLog_7",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLog_7", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_ActivityLog_7_User_1_UserID",
                        column: x => x.UserID,
                        principalTable: "User_1",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Advertisement_4",
                columns: table => new
                {
                    AdID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "money", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Draft"),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    LocationID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisement_4", x => x.AdID);
                    table.ForeignKey(
                        name: "FK_Advertisement_4_Category_2_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Category_2",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advertisement_4_Location_3_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Location_3",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advertisement_4_User_1_UserID",
                        column: x => x.UserID,
                        principalTable: "User_1",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Favorite_6",
                columns: table => new
                {
                    FavoriteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    AdID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorite_6", x => x.FavoriteID);
                    table.ForeignKey(
                        name: "FK_Favorite_6_Advertisement_4_AdID",
                        column: x => x.AdID,
                        principalTable: "Advertisement_4",
                        principalColumn: "AdID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorite_6_User_1_UserID",
                        column: x => x.UserID,
                        principalTable: "User_1",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message_5",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SenderID = table.Column<int>(type: "int", nullable: false),
                    ReceiverID = table.Column<int>(type: "int", nullable: false),
                    AdID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message_5", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Message_5_Advertisement_4_AdID",
                        column: x => x.AdID,
                        principalTable: "Advertisement_4",
                        principalColumn: "AdID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Message_5_User_1_ReceiverID",
                        column: x => x.ReceiverID,
                        principalTable: "User_1",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Message_5_User_1_SenderID",
                        column: x => x.SenderID,
                        principalTable: "User_1",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Category_2",
                columns: new[] { "CategoryID", "IconClass", "Name" },
                values: new object[,]
                {
                    { 1, "bi-car-front", "Транспорт" },
                    { 2, "bi-house", "Недвижимость" },
                    { 3, "bi-laptop", "Электроника" },
                    { 4, "bi-bag", "Одежда и обувь" },
                    { 5, "bi-briefcase", "Работа" },
                    { 6, "bi-tools", "Услуги" },
                    { 7, "bi-heart", "Животные" },
                    { 8, "bi-bicycle", "Хобби и спорт" },
                    { 9, "bi-lamp", "Мебель" },
                    { 10, "bi-three-dots", "Другое" }
                });

            migrationBuilder.InsertData(
                table: "Location_3",
                columns: new[] { "LocationID", "City", "Country", "Region" },
                values: new object[,]
                {
                    { 1, "Полоцк", "Беларусь", "Витебская область" },
                    { 2, "Минск", "Беларусь", "Минская область" },
                    { 3, "Витебск", "Беларусь", "Витебская область" },
                    { 4, "Гродно", "Беларусь", "Гродненская область" },
                    { 5, "Брест", "Беларусь", "Брестская область" },
                    { 6, "Гомель", "Беларусь", "Гомельская область" },
                    { 7, "Могилев", "Беларусь", "Могилевская область" }
                });

            migrationBuilder.InsertData(
                table: "ModerationRule_8",
                columns: new[] { "RuleID", "CreatedAt", "Phrase" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "только пересылка" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "бесплатно отдам" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "казино" }
                });

            migrationBuilder.InsertData(
                table: "User_1",
                columns: new[] { "UserID", "AvatarUrl", "Email", "Login", "Name", "PasswordHash", "Phone", "RegDate", "Role", "Status" },
                values: new object[,]
                {
                    { 1, null, null, "admin", "Администратор", "$2a$11$nkwBOrzbgCICBe1dQSSioeq4KQ1tcpAaEnISfUmC0.O5pxjGWjJam", "+375291234567", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", "Active" },
                    { 2, null, null, "moder", "Модератор", "$2a$11$d23u.U1JVX8L/fntQ0ee5OYN2/e.vlXc3t3Euk1TpW/5DrC5gx59e", "+375297654321", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Moderator", "Active" },
                    { 3, null, null, "user1", "Иван Иванов", "$2a$11$coETBOh9tiXJBkJifV9WW.1x7Ix3gwmNMYZG3E/cZ5Yk3UhYS/EV2", "+375291111111", new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "User", "Active" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_7_UserID",
                table: "ActivityLog_7",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_4_CategoryID",
                table: "Advertisement_4",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_4_LocationID",
                table: "Advertisement_4",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_4_UserID",
                table: "Advertisement_4",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_6_AdID",
                table: "Favorite_6",
                column: "AdID");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_6_UserID",
                table: "Favorite_6",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Message_5_AdID",
                table: "Message_5",
                column: "AdID");

            migrationBuilder.CreateIndex(
                name: "IX_Message_5_ReceiverID",
                table: "Message_5",
                column: "ReceiverID");

            migrationBuilder.CreateIndex(
                name: "IX_Message_5_SenderID",
                table: "Message_5",
                column: "SenderID");

            migrationBuilder.CreateIndex(
                name: "IX_User_1_Login",
                table: "User_1",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLog_7");

            migrationBuilder.DropTable(
                name: "Favorite_6");

            migrationBuilder.DropTable(
                name: "Message_5");

            migrationBuilder.DropTable(
                name: "ModerationRule_8");

            migrationBuilder.DropTable(
                name: "Advertisement_4");

            migrationBuilder.DropTable(
                name: "Category_2");

            migrationBuilder.DropTable(
                name: "Location_3");

            migrationBuilder.DropTable(
                name: "User_1");
        }
    }
}
