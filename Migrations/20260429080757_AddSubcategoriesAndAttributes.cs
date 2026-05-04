using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Aogiri.Migrations
{
    /// <inheritdoc />
    public partial class AddSubcategoriesAndAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "Advertisement_4",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealType",
                table: "Advertisement_4",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubcategoryID",
                table: "Advertisement_4",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdAttribute_12",
                columns: table => new
                {
                    AdAttributeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdAttribute_12", x => x.AdAttributeID);
                    table.ForeignKey(
                        name: "FK_AdAttribute_12_Advertisement_4_AdID",
                        column: x => x.AdID,
                        principalTable: "Advertisement_4",
                        principalColumn: "AdID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdImage_10",
                columns: table => new
                {
                    AdImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    AdID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdImage_10", x => x.AdImageID);
                    table.ForeignKey(
                        name: "FK_AdImage_10_Advertisement_4_AdID",
                        column: x => x.AdID,
                        principalTable: "Advertisement_4",
                        principalColumn: "AdID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Report_9",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false),
                    AdID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_9", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_Report_9_Advertisement_4_AdID",
                        column: x => x.AdID,
                        principalTable: "Advertisement_4",
                        principalColumn: "AdID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Report_9_User_1_UserID",
                        column: x => x.UserID,
                        principalTable: "User_1",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subcategory_11",
                columns: table => new
                {
                    SubcategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategory_11", x => x.SubcategoryID);
                    table.ForeignKey(
                        name: "FK_Subcategory_11_Category_2_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Category_2",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Location_3",
                columns: new[] { "LocationID", "City", "Country", "Region" },
                values: new object[,]
                {
                    { 8, "Новополоцк", "Беларусь", "Витебская область" },
                    { 9, "Барановичи", "Беларусь", "Брестская область" },
                    { 10, "Бобруйск", "Беларусь", "Могилевская область" },
                    { 11, "Борисов", "Беларусь", "Минская область" },
                    { 12, "Пинск", "Беларусь", "Брестская область" },
                    { 13, "Орша", "Беларусь", "Витебская область" },
                    { 14, "Солигорск", "Беларусь", "Минская область" },
                    { 15, "Мозырь", "Беларусь", "Гомельская область" },
                    { 16, "Лида", "Беларусь", "Гродненская область" },
                    { 17, "Молодечно", "Беларусь", "Минская область" },
                    { 18, "Жодино", "Беларусь", "Минская область" },
                    { 19, "Слуцк", "Беларусь", "Минская область" },
                    { 20, "Несвиж", "Беларусь", "Минская область" },
                    { 21, "Жлобин", "Беларусь", "Гомельская область" },
                    { 22, "Светлогорск", "Беларусь", "Гомельская область" },
                    { 23, "Речица", "Беларусь", "Гомельская область" },
                    { 24, "Ошмяны", "Беларусь", "Гродненская область" },
                    { 25, "Волковыск", "Беларусь", "Гродненская область" },
                    { 26, "Слоним", "Беларусь", "Гродненская область" },
                    { 27, "Кобрин", "Беларусь", "Брестская область" },
                    { 28, "Пружаны", "Беларусь", "Брестская область" },
                    { 29, "Берёза", "Беларусь", "Брестская область" },
                    { 30, "Дзержинск", "Беларусь", "Минская область" }
                });

            migrationBuilder.InsertData(
                table: "Subcategory_11",
                columns: new[] { "SubcategoryID", "CategoryID", "IconClass", "Name" },
                values: new object[,]
                {
                    { 1, 1, "bi-car-front", "Легковые автомобили" },
                    { 2, 1, "bi-bicycle", "Мотоциклы" },
                    { 3, 1, "bi-truck", "Грузовики" },
                    { 4, 1, "bi-gear", "Запчасти" },
                    { 5, 1, "bi-bicycle", "Велосипеды" },
                    { 6, 2, "bi-building", "Квартиры" },
                    { 7, 2, "bi-house", "Дома и дачи" },
                    { 8, 2, "bi-door-open", "Комнаты" },
                    { 9, 2, "bi-geo", "Земельные участки" },
                    { 10, 2, "bi-shop", "Коммерческая" },
                    { 11, 3, "bi-phone", "Телефоны" },
                    { 12, 3, "bi-laptop", "Ноутбуки и ПК" },
                    { 13, 3, "bi-tablet", "Планшеты" },
                    { 14, 3, "bi-tv", "Телевизоры" },
                    { 15, 3, "bi-music-note", "Аудио и видео" },
                    { 16, 3, "bi-camera", "Фото и видео" },
                    { 17, 4, "bi-person", "Мужская одежда" },
                    { 18, 4, "bi-person", "Женская одежда" },
                    { 19, 4, "bi-star", "Детская одежда" },
                    { 20, 4, "bi-bag", "Обувь" },
                    { 21, 4, "bi-watch", "Аксессуары" },
                    { 22, 5, "bi-code-slash", "IT и интернет" },
                    { 23, 5, "bi-hammer", "Строительство" },
                    { 24, 5, "bi-cart", "Торговля" },
                    { 25, 5, "bi-heart-pulse", "Медицина" },
                    { 26, 5, "bi-book", "Образование" },
                    { 27, 6, "bi-tools", "Ремонт и строительство" },
                    { 28, 6, "bi-scissors", "Красота и здоровье" },
                    { 29, 6, "bi-book", "Репетиторство" },
                    { 30, 6, "bi-truck", "Перевозки" },
                    { 31, 7, "bi-heart", "Собаки" },
                    { 32, 7, "bi-heart", "Кошки" },
                    { 33, 7, "bi-feather", "Птицы" },
                    { 34, 7, "bi-droplet", "Рыбки и аквариумы" },
                    { 35, 8, "bi-trophy", "Спортивный инвентарь" },
                    { 36, 8, "bi-backpack", "Туризм и отдых" },
                    { 37, 8, "bi-book", "Книги и журналы" },
                    { 38, 8, "bi-music-note-beamed", "Музыкальные инструменты" },
                    { 39, 9, "bi-lamp", "Спальня" },
                    { 40, 9, "bi-tv", "Гостиная" },
                    { 41, 9, "bi-cup-hot", "Кухня" },
                    { 42, 9, "bi-briefcase", "Офисная мебель" }
                });

            migrationBuilder.UpdateData(
                table: "User_1",
                keyColumn: "UserID",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$NKjkwTwLu6KLCU0On5/y4um4rbvXPAZ9EfYel3pz.3QCUK0z5BnA6");

            migrationBuilder.UpdateData(
                table: "User_1",
                keyColumn: "UserID",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$47bl0ES41utvX0yTFdDNEe3E4Qj2XGrx1uiVR9F1MYqzoTkTesgvi");

            migrationBuilder.UpdateData(
                table: "User_1",
                keyColumn: "UserID",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$NEWfnId0jt.xm1mfW8b8QewkDrwbufrntAaaPL0NVN.xhq99Z35vq");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_4_SubcategoryID",
                table: "Advertisement_4",
                column: "SubcategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_AdAttribute_12_AdID",
                table: "AdAttribute_12",
                column: "AdID");

            migrationBuilder.CreateIndex(
                name: "IX_AdImage_10_AdID",
                table: "AdImage_10",
                column: "AdID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_9_AdID",
                table: "Report_9",
                column: "AdID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_9_UserID",
                table: "Report_9",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategory_11_CategoryID",
                table: "Subcategory_11",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Advertisement_4_Subcategory_11_SubcategoryID",
                table: "Advertisement_4",
                column: "SubcategoryID",
                principalTable: "Subcategory_11",
                principalColumn: "SubcategoryID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advertisement_4_Subcategory_11_SubcategoryID",
                table: "Advertisement_4");

            migrationBuilder.DropTable(
                name: "AdAttribute_12");

            migrationBuilder.DropTable(
                name: "AdImage_10");

            migrationBuilder.DropTable(
                name: "Report_9");

            migrationBuilder.DropTable(
                name: "Subcategory_11");

            migrationBuilder.DropIndex(
                name: "IX_Advertisement_4_SubcategoryID",
                table: "Advertisement_4");

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Location_3",
                keyColumn: "LocationID",
                keyValue: 30);

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Advertisement_4");

            migrationBuilder.DropColumn(
                name: "DealType",
                table: "Advertisement_4");

            migrationBuilder.DropColumn(
                name: "SubcategoryID",
                table: "Advertisement_4");

            migrationBuilder.UpdateData(
                table: "User_1",
                keyColumn: "UserID",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$nkwBOrzbgCICBe1dQSSioeq4KQ1tcpAaEnISfUmC0.O5pxjGWjJam");

            migrationBuilder.UpdateData(
                table: "User_1",
                keyColumn: "UserID",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$d23u.U1JVX8L/fntQ0ee5OYN2/e.vlXc3t3Euk1TpW/5DrC5gx59e");

            migrationBuilder.UpdateData(
                table: "User_1",
                keyColumn: "UserID",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$coETBOh9tiXJBkJifV9WW.1x7Ix3gwmNMYZG3E/cZ5Yk3UhYS/EV2");
        }
    }
}
