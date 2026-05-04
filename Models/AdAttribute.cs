namespace Aogiri.Models;

/// <summary>
/// Дополнительные атрибуты объявления, зависящие от категории.
/// Например: для Транспорта — год выпуска, пробег; для Недвижимости — площадь, этаж.
/// </summary>
public class AdAttribute
{
    public int AdAttributeID { get; set; }

    /// <summary>Название атрибута, например "Год выпуска"</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Значение атрибута, например "2019"</summary>
    public string Value { get; set; } = string.Empty;

    public int AdID { get; set; }
    public Advertisement Advertisement { get; set; } = null!;
}
