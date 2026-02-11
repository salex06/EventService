using System.ComponentModel;

namespace MS_Lab.enums
{
    public enum EventType
    {
        [Description("Конференция")]
        Conference = 0,
        [Description("Фестиваль")]
        Festival,
        [Description("Спортивное мероприятие")]
        Sport,
        [Description("Мастер-класс")]
        Workshop,
        [Description("Концерт")]
        Concert
    }
}
