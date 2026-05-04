namespace Aogiri.Helpers;

/// <summary>
/// Описание динамического поля объявления для конкретной категории.
/// </summary>
public class CategoryField
{
    public string Key        { get; set; } = string.Empty;
    public string Label      { get; set; } = string.Empty;
    public string Type       { get; set; } = "text";        // text | number | select
    public bool   Required   { get; set; } = false;
    public string? Unit      { get; set; }                  // "км", "м²", "л.с." и т.д.
    public List<string> Options { get; set; } = new();      // для select-полей
}

/// <summary>
/// Статический справочник полей по категориям и подкатегориям.
/// </summary>
public static class CategoryFields
{
    // Ключ: CategoryID (основная категория)
    // Дополнительный ключ: SubcategoryID (или 0 = для всей категории)
    private static readonly Dictionary<int, List<CategoryField>> _byCategory = new()
    {
        // ── 1. Транспорт ─────────────────────────────────────────────────────
        [1] = new()
        {
            new() { Key = "brand",        Label = "Марка",              Type = "text",   Required = true  },
            new() { Key = "model",        Label = "Модель",             Type = "text",   Required = true  },
            new() { Key = "year",         Label = "Год выпуска",        Type = "number", Required = true  },
            new() { Key = "mileage",      Label = "Пробег",             Type = "number", Required = false, Unit = "км" },
            new() { Key = "engine_vol",   Label = "Объём двигателя",    Type = "number", Required = false, Unit = "л" },
            new() { Key = "fuel",         Label = "Тип топлива",        Type = "select", Required = false,
                    Options = new() { "Бензин", "Дизель", "Гибрид", "Электро", "Газ" } },
            new() { Key = "gearbox",      Label = "Коробка передач",    Type = "select", Required = false,
                    Options = new() { "Механика", "Автомат", "Робот", "Вариатор" } },
            new() { Key = "drive",        Label = "Привод",             Type = "select", Required = false,
                    Options = new() { "Передний", "Задний", "Полный" } },
            new() { Key = "body",         Label = "Кузов",              Type = "select", Required = false,
                    Options = new() { "Седан", "Хэтчбек", "Универсал", "Внедорожник", "Кроссовер", "Купе", "Минивэн", "Пикап", "Фургон" } },
            new() { Key = "color",        Label = "Цвет",               Type = "text",   Required = false },
        },

        // ── 2. Недвижимость ───────────────────────────────────────────────────
        [2] = new()
        {
            new() { Key = "rooms",        Label = "Количество комнат",  Type = "number", Required = true  },
            new() { Key = "area_total",   Label = "Общая площадь",      Type = "number", Required = true,  Unit = "м²" },
            new() { Key = "area_living",  Label = "Жилая площадь",      Type = "number", Required = false, Unit = "м²" },
            new() { Key = "area_kitchen", Label = "Площадь кухни",      Type = "number", Required = false, Unit = "м²" },
            new() { Key = "floor",        Label = "Этаж",               Type = "number", Required = false },
            new() { Key = "floors_total", Label = "Этажей в доме",      Type = "number", Required = false },
            new() { Key = "building_type",Label = "Тип дома",           Type = "select", Required = false,
                    Options = new() { "Кирпичный", "Панельный", "Монолитный", "Деревянный", "Блочный" } },
            new() { Key = "renovation",   Label = "Ремонт",             Type = "select", Required = false,
                    Options = new() { "Косметический", "Евроремонт", "Дизайнерский", "Без ремонта", "Чистовая отделка" } },
            new() { Key = "balcony",      Label = "Балкон/Лоджия",      Type = "select", Required = false,
                    Options = new() { "Есть", "Нет", "Застеклённый" } },
        },

        // ── 3. Электроника ───────────────────────────────────────────────────
        [3] = new()
        {
            new() { Key = "brand",        Label = "Производитель",      Type = "text",   Required = false },
            new() { Key = "model",        Label = "Модель",             Type = "text",   Required = false },
            new() { Key = "memory",       Label = "Память/Объём",       Type = "text",   Required = false },
            new() { Key = "color",        Label = "Цвет",               Type = "text",   Required = false },
            new() { Key = "warranty",     Label = "Гарантия",           Type = "select", Required = false,
                    Options = new() { "Есть", "Нет", "Заводская" } },
        },

        // ── 4. Одежда и обувь ────────────────────────────────────────────────
        [4] = new()
        {
            new() { Key = "gender",       Label = "Пол",                Type = "select", Required = false,
                    Options = new() { "Мужской", "Женский", "Унисекс", "Детский" } },
            new() { Key = "size",         Label = "Размер",             Type = "text",   Required = false },
            new() { Key = "brand",        Label = "Бренд",              Type = "text",   Required = false },
            new() { Key = "color",        Label = "Цвет",               Type = "text",   Required = false },
            new() { Key = "material",     Label = "Материал",           Type = "text",   Required = false },
        },

        // ── 5. Работа ────────────────────────────────────────────────────────
        [5] = new()
        {
            new() { Key = "employment",   Label = "Тип занятости",      Type = "select", Required = false,
                    Options = new() { "Полная занятость", "Частичная занятость", "Удалённо", "Вахта", "Подработка", "Стажировка" } },
            new() { Key = "experience",   Label = "Опыт работы",        Type = "select", Required = false,
                    Options = new() { "Без опыта", "1–3 года", "3–5 лет", "Более 5 лет" } },
            new() { Key = "schedule",     Label = "График работы",      Type = "select", Required = false,
                    Options = new() { "5/2", "2/2", "Свободный", "Сменный" } },
        },

        // ── 6. Услуги ────────────────────────────────────────────────────────
        [6] = new()
        {
            new() { Key = "payment_type", Label = "Оплата",             Type = "select", Required = false,
                    Options = new() { "Почасовая", "За объём работ", "Договорная" } },
            new() { Key = "experience",   Label = "Опыт специалиста",   Type = "text",   Required = false },
        },

        // ── 7. Животные ──────────────────────────────────────────────────────
        [7] = new()
        {
            new() { Key = "animal_type",  Label = "Вид животного",      Type = "text",   Required = false },
            new() { Key = "breed",        Label = "Порода",             Type = "text",   Required = false },
            new() { Key = "age",          Label = "Возраст",            Type = "text",   Required = false },
            new() { Key = "gender",       Label = "Пол",                Type = "select", Required = false,
                    Options = new() { "Кобель/Кот", "Сука/Кошка", "Не указан" } },
            new() { Key = "vaccinated",   Label = "Вакцинация",         Type = "select", Required = false,
                    Options = new() { "Есть", "Нет" } },
            new() { Key = "pedigree",     Label = "Документы/Родословная",Type = "select",Required = false,
                    Options = new() { "Есть", "Нет" } },
        },

        // ── 8. Хобби и спорт ─────────────────────────────────────────────────
        [8] = new()
        {
            new() { Key = "brand",        Label = "Производитель",      Type = "text",   Required = false },
            new() { Key = "sport_type",   Label = "Вид спорта",         Type = "text",   Required = false },
        },

        // ── 9. Мебель ────────────────────────────────────────────────────────
        [9] = new()
        {
            new() { Key = "material",     Label = "Материал",           Type = "text",   Required = false },
            new() { Key = "color",        Label = "Цвет",               Type = "text",   Required = false },
            new() { Key = "dimensions",   Label = "Размеры (Ш×Г×В)",   Type = "text",   Required = false },
            new() { Key = "brand",        Label = "Производитель",      Type = "text",   Required = false },
        },
    };

    /// <summary>
    /// Возвращает список полей для указанной категории.
    /// </summary>
    public static List<CategoryField> GetFields(int categoryId)
    {
        return _byCategory.TryGetValue(categoryId, out var fields) ? fields : new();
    }

    /// <summary>
    /// Возвращает JSON-строку с описанием полей для всех категорий,
    /// пригодную для использования в JavaScript.
    /// </summary>
    public static string ToJson()
    {
        var result = new System.Text.StringBuilder("{");
        foreach (var kv in _byCategory)
        {
            result.Append($"\"{kv.Key}\":[");
            foreach (var f in kv.Value)
            {
                result.Append("{");
                result.Append($"\"key\":\"{f.Key}\",");
                result.Append($"\"label\":\"{f.Label}\",");
                result.Append($"\"type\":\"{f.Type}\",");
                result.Append($"\"required\":{(f.Required ? "true" : "false")},");
                if (f.Unit != null)
                    result.Append($"\"unit\":\"{f.Unit}\",");
                if (f.Options.Count > 0)
                    result.Append($"\"options\":[{string.Join(",", f.Options.Select(o => $"\"{o}\""))}],");
                result.Append("},");
            }
            result.Append("],");
        }
        result.Append("}");
        return result.ToString();
    }
}
